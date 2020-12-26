using DevExpress.Utils.Taskbar;
using DevExpress.XtraEditors;
using Selling.DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Selling.Forms
{
    public partial class frm_Product_Category :frm_master
    {
        DAL.ProductCategory category;
        public frm_Product_Category()
        {
            InitializeComponent();
            New();
        }
        private void Product_Category_Load(object sender, EventArgs e)
        {
            Refresh_Data();
            LookUpEdit1.Properties.DisplayMember = "Name";
            LookUpEdit1.Properties.ValueMember = "ID";

            treeList1.ParentFieldName = nameof(category.ParantID);
            treeList1.KeyFieldName = nameof(category.ID);
            treeList1.OptionsBehavior.Editable = false;
            treeList1.Columns[nameof(category.Number)].Visible = false;
        }
        public override void Refresh_Data()
        {
            var db = new DAL.dbDataContext();
            LookUpEdit1.Properties.DataSource = db.ProductCategories;
            treeList1.DataSource = db.ProductCategories;
            base.Refresh();
        }
        public override void Save()
        {
            if (IsDataValid() == false) return;

            var db = new dbDataContext();
            if (category.ID == 0)
            {
                db.ProductCategories.InsertOnSubmit(category);
            }
            else
            {
                db.ProductCategories.Attach(category);
            }
            Setdata();
            db.SubmitChanges();
            base.Save();
           
        }
        public override void New()
        {
            category = new DAL.ProductCategory();
            base.New();
        }
        public override void Delete()
        {
            if (txt_name.Text.Trim() == string.Empty)
            {
                txt_name.ErrorText = "Enter the Name";
                return;
            }
            else if (XtraMessageBox.Show(text: "Are you sure from delete this item?", caption: "Delete Message", buttons: MessageBoxButtons.YesNo, icon: MessageBoxIcon.Question) == DialogResult.Yes)
            {
                var db = new DAL.dbDataContext();
                db.ProductCategories.Attach(category);
                db.ProductCategories.DeleteOnSubmit(category);
                db.SubmitChanges();
                New();
                base.Delete();
            }
        }
        public override void Getdata()
        {
            txt_name.Text = category.Name;
            LookUpEdit1.EditValue = category.ParantID;
            base.Getdata();
        }
        public override void Setdata()
        {
            category.Name = txt_name.Text;
            category.ParantID = (LookUpEdit1.EditValue as int?) ?? 0;  // if LookUpEdit1.EditValue will be int, Put it else put 0 
            category.Number = "0";
            base.Setdata();
        }
        public override bool IsDataValid()
        {
            if (txt_name.Text.Trim() == string.Empty)
            {
                txt_name.ErrorText = "Enter the Name";
                return false;
            }
            var db = new DAL.dbDataContext();
            if (db.ProductCategories.Where(x => x.Name.Trim() == txt_name.Text.Trim() && x.ID != category.ID).Count() > 0)
            {
                txt_name.ErrorText = "This name is exist";
                return false;
            }
            return true;
        }
        private void treeList1_FocusedNodeChanged(object sender, DevExpress.XtraTreeList.FocusedNodeChangedEventArgs e)
        {
            int id = 0;
            if (int.TryParse(e.Node.GetValue("ID").ToString(), out id))
            {
                var db = new DAL.dbDataContext();
                category = db.ProductCategories.Single(c => c.ID == id);
                Getdata();
            }
        }
    }
}
