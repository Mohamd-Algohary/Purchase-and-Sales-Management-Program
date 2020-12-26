using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraBars;
using DevExpress.Utils;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.Charts.Native;

namespace Selling.Forms
{
    public partial class frm_AccessProfileList : frm_master
    {
        public frm_AccessProfileList()
        {
            InitializeComponent();
            Refresh_Data();
            gridView1.DoubleClick += GridView1_DoubleClick;
            but_save.Visibility = BarItemVisibility.Never;
        }

        private void GridView1_DoubleClick(object sender, EventArgs e)
        {
            var ea = e as DXMouseEventArgs;
            var view = sender as GridView;
            var info = view.CalcHitInfo(ea.Location);
            if (info.InRow)
            {
                int ID = Convert.ToInt32(gridView1.GetFocusedRowCellValue("ID"));
                var frm = new frm_AccessProfile(ID);
                frm_main.openForm(frm, true);
                Refresh_Data();
            }
        }

        public override void Refresh_Data()
        {
            gridControl1.DataSource = null;
            using (var db = new DAL.dbDataContext())
            {
               gridControl1.DataSource = db.UserAccessProfiles.ToList();
            }
            gridView1.Columns["ID"].Visible = false;
            gridView1.Columns["Name"].OptionsColumn.AllowEdit = false;
            base.Refresh_Data();
        }
        public override void New()
        {
            var frm = new frm_AccessProfile();
            frm_main.openForm(frm, true);
            Refresh_Data();
            base.New();
        }
        public override void Delete()
        {
            if (XtraMessageBox.Show("Are you sure from delete this profile?", "Delete message", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
               using(var db = new DAL.dbDataContext())
               {
                    int ID = Convert.ToInt32(gridView1.GetFocusedRowCellValue("ID"));
                    db.UserAccessProfileDetails.DeleteAllOnSubmit(db.UserAccessProfileDetails.Where(x => x.ProfileID == ID));
                    db.SubmitChanges();
                    db.UserAccessProfiles.DeleteOnSubmit(db.UserAccessProfiles.Single(x => x.ID == ID));
                    db.SubmitChanges();
               }
                Refresh_Data();
            }
                base.Delete();
        }
    }
}