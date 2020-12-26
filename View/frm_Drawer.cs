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
    public partial class frm_Drawer : frm_master
    {
        DAL.Drawer dr;
        DAL.Account acc;
        public frm_Drawer()
        {
            InitializeComponent();
            New();
        }
        public frm_Drawer(int id)
        {
            InitializeComponent();
            var db = new dbDataContext();
            dr = db.Drawers.Single(d => d.ID == id);
            Getdata();
        }
        public override void New()
        {
            dr = new DAL.Drawer();
            base.New();
        }
        public override void Save()
        {
            if (txt_name.Text.Trim() == string.Empty)
            {
                txt_name.ErrorText = "Enter the Drawer Name";
                return;
            }
            var db = new DAL.dbDataContext();
            if (dr.ID == 0)
            {
                acc = new DAL.Account();

                db.Drawers.InsertOnSubmit(dr);
                db.Accounts.InsertOnSubmit(acc);
            }
            else
            {
                db.Drawers.Attach(dr);
                acc = db.Accounts.Single(s => s.ID == dr.AccountID);
            }
            Setdata();
            acc.Name = dr.Name;
            db.SubmitChanges();
            dr.AccountID = acc.ID;
            db.SubmitChanges();
            base.Save();
        }
        public override void Delete()
        {
            if (XtraMessageBox.Show("Are you sure from delete this item?", "Delete message", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                var db = new dbDataContext();
                db.Drawers.Attach(dr);
                db.Drawers.DeleteOnSubmit(dr);
                db.Accounts.Attach(acc);
                db.Accounts.DeleteOnSubmit(acc);
                db.SubmitChanges();
                base.Delete();
                New();
            }
        }
        public override void Getdata()
        {
            txt_name.Text = dr.Name;
            base.Getdata();
        }
        public override void Setdata()
        {
            dr.Name = txt_name.Text;
            base.Setdata();
        }
    }
}
