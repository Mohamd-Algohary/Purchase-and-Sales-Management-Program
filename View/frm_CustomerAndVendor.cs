using DevExpress.XtraEditors;
using Selling.DAL;
using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace Selling.Forms
{
    public partial class frm_CustomerAndVendor : frm_master
    {
        bool IsCustomer;
        DAL.CustomerAndVendor CusVnd;
        DAL.Account acc;
        public frm_CustomerAndVendor(bool iscustomer)
        {
            InitializeComponent();
            this.IsCustomer = iscustomer;
            this.Text = (IsCustomer) ? "Customer" : "Vendor";
            this.Name = (IsCustomer) ? "frm_Customer" : "frm_vendor";
            New();
        }
        public frm_CustomerAndVendor(bool iscustome, int Id)
        {
            InitializeComponent();
            this.IsCustomer = iscustome;
            this.Text = (IsCustomer) ? "Customer" : "Vendor";
            this.Name = (IsCustomer) ? "frm_Customer" : "frm_vendor";
            var db = new DAL.dbDataContext();
            CusVnd = db.CustomerAndVendors.Single(x => x.ID == Id);
            Getdata();
        }
        
        public override void Save()
        {
            if (IsDataValid() == false) return;

            var db = new dbDataContext();
            if (CusVnd.ID == 0)
            {
                acc = new DAL.Account();
                db.CustomerAndVendors.InsertOnSubmit(CusVnd);
                db.Accounts.InsertOnSubmit(acc);
            }
            else
            {
                db.CustomerAndVendors.Attach(CusVnd);
                acc = db.Accounts.Single(s => s.ID == CusVnd.AccountID);
            }

            Setdata();
            acc.Name = CusVnd.Name;
            db.SubmitChanges();
            CusVnd.AccountID = acc.ID;
            db.SubmitChanges();
            base.Save();
        }
        public override void New()
        {
            CusVnd = new DAL.CustomerAndVendor();
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
                db.CustomerAndVendors.Attach(CusVnd);
                db.Accounts.Attach(acc);
                db.CustomerAndVendors.DeleteOnSubmit(CusVnd);
                db.Accounts.DeleteOnSubmit(acc);
                db.SubmitChanges();
                New();
            }
            base.Delete();
        }
        public override void Getdata()
        {
            txt_name.Text = CusVnd.Name;
            txt_phone.Text = CusVnd.Phone;
            txt_mobile.Text = CusVnd.Mobile;
            txt_address.Text = CusVnd.Address;
            txt_accountId.Text = CusVnd.AccountID.ToString();
            base.Getdata();
        }
        public override void Setdata()
        {
            CusVnd.Name = txt_name.Text;
            CusVnd.Phone = txt_phone.Text;
            CusVnd.Mobile = txt_mobile.Text;
            CusVnd.Address = txt_address.Text;
            CusVnd.IsCustomer = IsCustomer;
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
            if (db.CustomerAndVendors.Where(x => x.Name.Trim() == txt_name.Text.Trim() && x.IsCustomer == IsCustomer && x.ID != CusVnd.ID).Count() > 0)
            {
                txt_name.ErrorText = "This name is exist";
                return false;
            }
            return true;
        }
    }
}
