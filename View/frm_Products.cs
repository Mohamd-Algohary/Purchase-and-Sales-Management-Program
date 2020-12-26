using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using DevExpress.Utils.Filtering.Internal;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.Grid;
using Selling.DAL;
using Selling.Model;
using static Selling.Classes.master;

namespace Selling.Forms
{
    public partial class frm_Products : frm_master
    {
        DAL.Productss prod;
        DAL.dbDataContext sdb = new DAL.dbDataContext();
        DAL.ProductUnit ins = new DAL.ProductUnit();
        RepositoryItemLookUpEdit relookUpEdit = new RepositoryItemLookUpEdit();
        public frm_Products()
        {
            InitializeComponent();
            Refresh_Data();
            New();
        }
        public frm_Products(int Id)
        {
            InitializeComponent();
            loadProduct(Id);
            Refresh_Data();
        }
        public void loadProduct(int iD)
        {
            using (var db = new DAL.dbDataContext())
            {
                prod = db.Productsses.Single(x => x.ID == iD);
                this.Text = string.Format("Product: {0}", prod.Name);
            }
            Getdata();
        }
        public override void Refresh_Data()
        {
            using (var db = new dbDataContext())
            {
                lookUpEdit_category.Properties.DataSource = db.ProductCategories
                    .Where(x => db.ProductCategories.Where(y => y.ParantID == x.ID).Count() == 0).ToList();
                relookUpEdit.DataSource = db.UnitNames.ToList();
            }
            lookUpEdit_CCM.InitializeData(MasterInventory.CostCalculationMethodList);
            base.Refresh_Data();
        }
        private void frm_Products_Load(object sender, EventArgs e)
        {
            lookUpEdit_category.Properties.DisplayMember = "Name";
            lookUpEdit_category.Properties.ValueMember = "ID";
            lookUpEdit_category.ProcessNewValue += LookUpEdit_category_ProcessNewValue;
            lookUpEdit_category.Properties.TextEditStyle = TextEditStyles.Standard;
            lookUpEdit_type.Properties.DataSource = PruductTypeList;
            lookUpEdit_type.Properties.DisplayMember = "Name";
            lookUpEdit_type.Properties.ValueMember = "ID";

            gridView1.OptionsView.ShowGroupPanel = false;
            gridView1.OptionsView.NewItemRowPosition = NewItemRowPosition.Bottom;
            var ins = new DAL.ProductUnit();
            gridView1.Columns[nameof(ins.ID)].Visible = false;
            gridView1.Columns[nameof(ins.ProductID)].Visible = false;
            RepositoryItemCalcEdit calcEdit = new RepositoryItemCalcEdit();
            gridControl1.RepositoryItems.Add(calcEdit);
            gridControl1.RepositoryItems.Add(relookUpEdit);
            gridView1.Columns[nameof(ins.Factor)].ColumnEdit = calcEdit;
            gridView1.Columns[nameof(ins.BuyPrice)].ColumnEdit = calcEdit;
            gridView1.Columns[nameof(ins.SalePrice)].ColumnEdit = calcEdit;
            gridView1.Columns[nameof(ins.SaleDiscount)].ColumnEdit = calcEdit;
            gridView1.Columns[nameof(ins.UnitID)].ColumnEdit = relookUpEdit;
            var ins1 = new DAL.UnitName();
            relookUpEdit.DisplayMember = nameof(ins1.Name);
            relookUpEdit.ValueMember = nameof(ins1.ID);
            relookUpEdit.NullText = "";


            gridView1.ValidateRow += GridView1_ValidateRow;
            gridView1.InvalidRowException += GridView1_InvalidRowException;
            gridView1.FocusedRowChanged += GridView1_FocusedRowChanged;
            gridView1.CustomRowCellEditForEditing += GridView1_CustomRowCellEditForEditing;
        }
        private void GridView1_CustomRowCellEditForEditing(object sender, CustomRowCellEditEventArgs e)
        {
            if (e.Column.FieldName == nameof(ins.UnitID))
            {
                var ind = ((Collection<DAL.ProductUnit>)gridView1.DataSource).Select(x => x.UnitID).ToList();
                RepositoryItemLookUpEdit rep = new RepositoryItemLookUpEdit();
                using (var db = new dbDataContext())
                {
                    var CurrentID = (int?)e.CellValue;
                    ind.Remove(CurrentID ?? 0);
                    rep.DataSource = db.UnitNames.Where(x => ind.Contains(x.ID) == false).ToList();
                    rep.ValueMember = "ID";
                    rep.DisplayMember = "Name";
                    rep.PopulateColumns();
                    rep.Columns["ID"].Visible = false;
                    e.RepositoryItem = rep;
                    rep.NullText = "";
                    rep.TextEditStyle = TextEditStyles.Standard;
                    rep.ProcessNewValue += RelookUpEdit_ProcessNewValue;
                }
            }
        }
        private void GridView1_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            gridView1.Columns[nameof(ins.Factor)].OptionsColumn.AllowEdit = !(e.FocusedRowHandle == 0);  //can not change in first row
        }
        private void GridView1_InvalidRowException(object sender, DevExpress.XtraGrid.Views.Base.InvalidRowExceptionEventArgs e)
        {
            e.ExceptionMode = ExceptionMode.NoAction;
        }
        private void GridView1_ValidateRow(object sender, DevExpress.XtraGrid.Views.Base.ValidateRowEventArgs e)
        {
            var view = sender as GridView;
            var row = e.Row as DAL.ProductUnit;
            if (row == null) return;

            if (row.Factor <= 1 && e.RowHandle != 0)  //e.RowHandle = No. of row
            {
                e.Valid = false;
                view.SetColumnError(view.Columns[nameof(row.Factor)], "The entered value must be greater than 1");
            }
            if (row.UnitID <= 0)
            {
                e.Valid = false;
                view.SetColumnError(view.Columns[nameof(row.UnitID)], "This field is requird");
            }
            if (checkBarCodeExist(row.Barcode, proID: prod.ID))
            {
                e.Valid = false;
                view.SetColumnError(view.Columns[nameof(row.Barcode)], "The entered value is already exist");
            }
        }
        private void RelookUpEdit_ProcessNewValue(object sender, ProcessNewValueEventArgs e)
        {
            if (e.DisplayValue is string s && s.Trim() != string.Empty)
            {
                var obj = new DAL.UnitName();
                obj.Name = s;
                using (var db = new dbDataContext())
                {
                    db.UnitNames.InsertOnSubmit(obj);
                    db.SubmitChanges();
                }
                ((List<DAL.UnitName>)relookUpEdit.DataSource).Add(obj);
                ((List<DAL.UnitName>)(((LookUpEdit)sender).Properties.DataSource)).Add(obj);
                e.Handled = true;
            }
        }
        private void LookUpEdit_category_ProcessNewValue(object sender, ProcessNewValueEventArgs e)
        {
            if (e.DisplayValue is string st && st.Trim() != string.Empty)
            {
                var NewObject = new DAL.ProductCategory() { Name = st, ParantID = 0, Number = "0" };
                using (var db = new dbDataContext())
                {
                    db.ProductCategories.InsertOnSubmit(NewObject);
                    db.SubmitChanges();
                }
                ((List<DAL.ProductCategory>)lookUpEdit_category.Properties.DataSource).Add(NewObject);
                e.Handled = true;
            }
            // Refresh_Data();
        }
        public override void Save()
        {
            gridView1.UpdateCurrentRow();

            if (IsDataValid() == false) return;

            var db = new dbDataContext();
            if (prod.ID == 0) db.Productsses.InsertOnSubmit(prod);
            else db.Productsses.Attach(prod);
            Setdata();
            db.SubmitChanges();

            var data = gridView1.DataSource as BindingList<DAL.ProductUnit>;
            foreach (var item in data)
            {
                item.ProductID = prod.ID;
                if (string.IsNullOrEmpty(item.Barcode)) { item.Barcode = ""; }
            }
            sdb.SubmitChanges();
            base.Save();
            this.Text = string.Format("Product: {0}", prod.Name);
        }
        public override void New()
        {
            this.Text = string.Format("Product");
            prod = new Productss() { IsActive = true, Code = GetNewProductCode() };
            Getdata();
            var data = gridView1.DataSource as BindingList<DAL.ProductUnit>;
            var db = new dbDataContext();
            if (db.UnitNames.Count() == 0)
            {
                db.UnitNames.InsertOnSubmit(new UnitName() { Name = "Each" });
            }
            data.Add(new ProductUnit() { UnitID = db.UnitNames.First().ID, Factor = 1, Barcode = GetNewBarCode() });

            var catg = db.ProductCategories
                    .Where(x => db.ProductCategories.Where(y => y.ParantID == x.ID).Count() == 0).First();
            if (catg != null)
                prod.CategoryID = catg.ID;
            base.New();

        }
        public override void Delete()
        {
            if (IsDataValid() == false) return;
            if (XtraMessageBox.Show("Are you sure from delete this item?", "Delete message", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                var db = new dbDataContext();
                db.Productsses.Attach(prod);
                db.Productsses.DeleteOnSubmit(prod);
                db.SubmitChanges();
                base.Delete();
                New();
            }
        }
        public override void Getdata()
        {
            txt_code.Text = prod.Code;
            txt_name.Text = prod.Name;
            lookUpEdit_category.EditValue = prod.CategoryID;
            lookUpEdit_type.EditValue = prod.Type;
            lookUpEdit_CCM.EditValue = prod.CostCalculationMethod;
            checkEdit_active.Checked = prod.IsActive;
            if (prod.Image != null)
                pictureEdit1.Image = GetImageFromByt(prod.Image.ToArray());
            else pictureEdit1.Image = null;
            gridControl1.DataSource = sdb.ProductUnits.Where(x => x.ProductID == prod.ID);

            base.Getdata();
        }
        public override void Setdata()
        {
            prod.Code = txt_code.Text;
            prod.Name = txt_name.Text;
            prod.CategoryID = (lookUpEdit_category.EditValue as int?) ?? 0;
            prod.Type = Convert.ToByte(lookUpEdit_type.EditValue);
            prod.CostCalculationMethod = Convert.ToByte(lookUpEdit_CCM.EditValue);
            prod.IsActive = checkEdit_active.Checked;
            if (pictureEdit1.Image != null)
                prod.Image = GetBytFromImage(pictureEdit1.Image);
            base.Setdata();
        }
        public byte[] GetBytFromImage(Image image)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                try
                {
                    image.Save(stream, ImageFormat.Jpeg);
                    return stream.ToArray();
                }
                catch
                {
                    return stream.ToArray();
                }
            }
        }
        public Image GetImageFromByt(byte[] BytArray)
        {
            Image img;
            try
            {
                byte[] imgbyt = BytArray;
                MemoryStream stream = new MemoryStream(imgbyt, false);
                img = Image.FromStream(stream);
            }
            catch { img = null; }
            return img;
        }
        public override bool IsDataValid()
        {
            if (txt_code.Text.Trim() == string.Empty)
            {
                txt_code.ErrorText = "Enter the code";
                return false;
            }
            if (txt_name.Text.Trim() == string.Empty)
            {
                txt_name.ErrorText = "Enter the Name";
                return false;
            }
            if (lookUpEdit_type.EditValue is int == false)
            {
                lookUpEdit_type.ErrorText = "Enter an integer number";
                return false;
            }
            if (lookUpEdit_category.EditValue is int == false)
            {
                lookUpEdit_category.ErrorText = "Enter an integer number";
                return false;
            }
            var db = new DAL.dbDataContext();
            if (db.Productsses.Where(x => x.Name.Trim() == txt_name.Text.Trim() && x.ID != prod.ID).Count() > 0)
            {
                txt_name.ErrorText = "This name is exist";
                return false;
            }
            if (db.Productsses.Where(x => x.Name.Trim() == txt_code.Text.Trim() && x.ID != prod.ID).Count() > 0)
            {
                txt_name.ErrorText = "This code is exist";
                return false;
            }
            return true;
        }
        bool checkBarCodeExist(string barcode, int proID)
        {
            using (var db = new dbDataContext())
            {
                return db.ProductUnits.Where(x => x.Barcode == barcode && x.ProductID != proID).Count() > 0;
            }
        }
        private string GetNewBarCode()
        {
            string Maxcode;
            using (var db = new dbDataContext())
            {
                Maxcode = db.ProductUnits.Select(x => x.Barcode).Max();
            }
            return GetNextCodeInstring(Maxcode);
        }
        private string GetNewProductCode()
        {
            string Maxcode;
            using (var db = new dbDataContext())
            {
                Maxcode = db.Productsses.Select(x => x.Code).Max();
            }
            return GetNextCodeInstring(Maxcode);
        }
    }
}
