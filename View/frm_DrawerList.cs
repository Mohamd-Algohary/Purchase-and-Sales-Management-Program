using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
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
    public partial class frm_DrawerList : frm_master
    {
        public frm_DrawerList()
        {
            InitializeComponent();
            Refresh_Data();
            but_save.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
        }
        public override void Refresh_Data()
        {
            var db = new dbDataContext();
            gridControl1.DataSource = db.Drawers;

            gridView1.OptionsBehavior.Editable = false;
            gridView1.Columns["ID"].Visible = false;
            base.Refresh_Data();
        }
        private void gridView1_DoubleClick(object sender, EventArgs e)
        {
            DXMouseEventArgs ea = e as DXMouseEventArgs;
            GridView view = sender as GridView;
            GridHitInfo info = view.CalcHitInfo(ea.Location);
            if(info.InRow || info.InRowCell)
            {
                int id = Convert.ToInt32(gridView1.GetFocusedRowCellValue("ID"));
                frm_Drawer dr_list = new frm_Drawer(id);
                frm_main.openForm(dr_list, true);
                Refresh_Data();
            }

        }
        public override void New()
        {
            frm_Drawer dr_list = new frm_Drawer();
            frm_main.openForm(dr_list, true);
            Refresh_Data();
            base.New();
        }
        public override void Delete()
        {
            if (XtraMessageBox.Show("Are you sure from delete this item?", "Delete message", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                int id = Convert.ToInt32(gridView1.GetFocusedRowCellValue("ID"));

                var db = new dbDataContext();
                DAL.Drawer dr = db.Drawers.Single(d => d.ID == id);
                DAL.Account acc = db.Accounts.Single(x => x.ID == dr.AccountID);
                db.Drawers.DeleteOnSubmit(dr);
                db.Accounts.DeleteOnSubmit(acc);
                db.SubmitChanges();
                base.Delete();
            }
            Refresh_Data();
        }
    }
}
