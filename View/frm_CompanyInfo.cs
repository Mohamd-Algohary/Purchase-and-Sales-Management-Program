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
    public partial class frm_CompanyInfo : frm_master
    {
        public frm_CompanyInfo()
        {
            InitializeComponent();
            this.Load += CompanyInfo_Load;
            but_add.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            but_delete.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
        }
        private void CompanyInfo_Load(object sender, EventArgs e)
        {
            Getdata();
        }
        public override void Save()
        {
            if (txt_name.Text.Trim() == string.Empty)
            {
                txt_name.ErrorText = "Enter the Company Name";
                return;
            }
            DAL.dbDataContext db = new DAL.dbDataContext();
            // -------Insert into DB----------
            /*DAL.CompanyInfo info = new DAL.CompanyInfo();
            info.CompanyName = txt_name.Text;
            info.Phone = txt_phone.Text;
            info.Mobile = txt_mobile.Text;
            info.Address = txt_address.Text;

            db.CompanyInfos.InsertOnSubmit(info);  
            db.SubmitChanges();*/

            // -------Change the fist row into DB----------
            DAL.CompanyInfo info = db.CompanyInfos.FirstOrDefault();
            if (info == null)
            {
                info = new DAL.CompanyInfo();
                db.CompanyInfos.InsertOnSubmit(info);
            }
            info.CompanyName = txt_name.Text;
            info.Phone = txt_phone.Text;
            info.Mobile = txt_mobile.Text;
            info.Address = txt_address.Text;
            db.SubmitChanges();
            base.Save();
        }
        public override void Getdata()
        {
            DAL.dbDataContext db = new DAL.dbDataContext();   // db is the connection between C# file and DB

            DAL.CompanyInfo info = db.CompanyInfos.FirstOrDefault();     // SELECT TOP (1) * FROM [SalesDB].[dbo].[CompanyInfo]
            if (info == null) return;
            txt_name.Text = info.CompanyName;
            txt_phone.Text = info.Phone;
            txt_mobile.Text = info.Mobile;
            txt_address.Text = info.Address;
            base.Getdata();
        }
        
    }
}