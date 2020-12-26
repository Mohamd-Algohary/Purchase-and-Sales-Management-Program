using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using Selling.Classes;
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

    public partial class frm_CustomersAndVendorsList : frm_master
    {
        bool IsCustomer;
        public frm_CustomersAndVendorsList(bool iscustomer)
        {
            InitializeComponent();
            this.IsCustomer = iscustomer;
            but_save.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
        }
        private void frm_CustomersAndVendorsList_Load(object sender, EventArgs e)
        {
            session.Vendors.ListChanged += Vendors_ListChanged;
            session.Customers.ListChanged += Customers_ListChanged;
            this.Text = (IsCustomer) ? "Customers List" : "Vendors List";
            this.Name = (IsCustomer) ? "frm_CustomersList" : "frm_VendorList";
            Refresh_Data();
        }

        private void Customers_ListChanged(object sender, ListChangedEventArgs e)
        {
            Refresh_Data();
        }

        private void Vendors_ListChanged(object sender, ListChangedEventArgs e)
        {
            Refresh_Data();
        }

        public override void Refresh_Data()
        {
            var db = new DAL.dbDataContext();
           
            gridControl1.DataSource =(IsCustomer)? session.Customers.ToList() : session.Vendors.ToList();
            gridView1.OptionsBehavior.Editable = false;
            gridView1.Columns["ID"].Visible = false;
            gridView1.Columns["IsCustomer"].Visible = false;
            base.Refresh_Data();
        }
        public override void New()
        {
            frm_CustomerAndVendor frm = new frm_CustomerAndVendor(IsCustomer);
            frm_main.openForm(frm, true);
            Refresh_Data();
            base.New();

        }
        public override void Delete()
        {
            if (XtraMessageBox.Show(text: "Are you sure from delete this item?", caption: "Delete Message", buttons: MessageBoxButtons.YesNo, icon: MessageBoxIcon.Question) == DialogResult.Yes)
            { 
                int id = Convert.ToInt32(gridView1.GetFocusedRowCellValue("ID"));
                var db = new DAL.dbDataContext();
                DAL.CustomerAndVendor cusvd = db.CustomerAndVendors.Single(x => x.ID == id);
                DAL.Account acc = db.Accounts.Single(x => x.ID == cusvd.AccountID);
                db.CustomerAndVendors.DeleteOnSubmit(cusvd);
                db.Accounts.DeleteOnSubmit(acc);
                db.SubmitChanges();
                base.Delete();
            }
            Refresh_Data();

        }
        private void gridView1_DoubleClick(object sender, EventArgs e)
        {
            DXMouseEventArgs ea = e as DXMouseEventArgs;
            GridView view = sender as GridView;
            GridHitInfo info = view.CalcHitInfo(ea.Location);
            if (info.InRow || info.InRowCell)
            {
                int id = Convert.ToInt32(gridView1.GetFocusedRowCellValue("ID"));
                frm_CustomerAndVendor frm = new frm_CustomerAndVendor(IsCustomer, id);
                frm_main.openForm(frm);
                //frm.ShowDialog();
                //Refresh_Data();
            }
        }
    }

}
