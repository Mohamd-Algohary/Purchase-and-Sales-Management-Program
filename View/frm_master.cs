using DevExpress.XtraEditors;
using Selling.Classes;
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
    public partial class frm_master : XtraForm
    {
        bool IsNew = false;
        public frm_master()
        {
            InitializeComponent();
        }
        public virtual void Save()
        {
            XtraMessageBox.Show("Save is complete successful", "Save message", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Refresh_Data();
            IsNew = false;
        }

        public virtual void New()
        {
            IsNew = true;
            Getdata();
            but_delete.Enabled = false;
        }
        public virtual void Delete()
        {
            XtraMessageBox.Show(text: "Item Deleted successfuly", caption: "Delete message", buttons: MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        public virtual void Getdata()
        {

        }
        public virtual void Setdata()
        {

        }
        public virtual void Refresh_Data()
        {

        }
        public virtual bool IsDataValid()
        {
            return true;
        }
        private void but_save_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (CheckActionAuthrization(this.Name, IsNew? master.Actions.Add: master.Actions.Edit) && IsDataValid())
            {
                Save();
                but_delete.Enabled = true;
            }
        }

        private void but_add_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            New();
           
        }

        private void but_delete_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (CheckActionAuthrization(this.Name, master.Actions.Delete)) Delete();
        }
        public virtual void Print()
        {

        }
        private void but_print_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if(CheckActionAuthrization(this.Name, master.Actions.Print)) Print();
        }
        private void frm_master_KeyDown(object sender, KeyEventArgs e)
        {
            //if (e.KeyCode == Keys.Enter)
            //{
            //    //but_save_ItemClick(null, null);
            //    but_save.PerformClick();
            //}
            if (e.KeyCode == Keys.Add)
            {
                New();
            }
            //if (e.KeyCode == Keys.Delete)
            //{
            //    Delete();
            //}
        }
        //-----------------------------------------------------------------------------------------------------
        public static bool CheckActionAuthrization(string formName, master.Actions action, DAL.User user = null)
        {
            if (user == null) user = session.User;

            bool flag = true;
            if (user.UserType == (byte)master.UserType.Admin) return true;
            else
            {
                var screen = session.ScreensAccesses.SingleOrDefault(x => x.ScreenName == formName);
                if(screen != null)
                {
                    switch (action)
                    {
                        case master.Actions.Add:
                            flag = screen.CanAdd;
                            break;
                        case master.Actions.Edit:
                            flag = screen.CanEdit;
                            break;
                        case master.Actions.Delete:
                            flag = screen.CanDelete;
                            break;
                        case master.Actions.Print:
                            flag = screen.CanPrint;
                            break;
                        default:
                            break;
                    }

                }
            }
            if (!flag)
            {
                XtraMessageBox.Show("You don't have a permission reach for this", "Permission Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return flag;
        }
        
        
    }
}