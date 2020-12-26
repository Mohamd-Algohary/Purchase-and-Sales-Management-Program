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
using DevExpress.Utils;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraBars;

namespace Selling.Forms
{
    public partial class frm_UserSettingProfileList : frm_master
    {
        public frm_UserSettingProfileList()
        {
            InitializeComponent();
            Refresh_Data();
            gridView1.OptionsBehavior.Editable = false;
            gridView1.Columns["ID"].Visible = false;
            gridView1.DoubleClick += GridView1_DoubleClick;
            but_save.Visibility = BarItemVisibility.Never;
        }
        public override void Refresh_Data()
        {
            gridControl1.DataSource = null;
            using (var db = new DAL.dbDataContext())
            {
                gridControl1.DataSource = db.UserSettingsProfiles.ToList();
            }
            base.Refresh_Data();
        }
        private void GridView1_DoubleClick(object sender, EventArgs e)
        {
            DXMouseEventArgs ea = e as DXMouseEventArgs;
            GridView view = sender as GridView;
            GridHitInfo info = view.CalcHitInfo(ea.Location);
            if (info.InRow )
            {
                var frm = new frm__UserSettingsProfile(Convert.ToInt32(view.GetFocusedRowCellValue("ID")));
                frm.ShowDialog();
                Refresh_Data();
            }

        }
        public override void New()
        {
            //frm_main.openbyname(nameof(frm__UserSettingsProfile));
            Form frm = new frm__UserSettingsProfile();
            frm.ShowDialog();
            Refresh_Data();
            base.New();
        }
        public override void Delete()
        {
            if (XtraMessageBox.Show("Are you sure from delete this profile?", "Delete message", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                int id = Convert.ToInt32(gridView1.GetFocusedRowCellValue("ID"));
                using (var db = new DAL.dbDataContext())
                {
                    DAL.UserSettingsProfile user = db.UserSettingsProfiles.Where(s => s.ID == id).First();
                    db.UserSettingsProfileProperties.DeleteAllOnSubmit(db.UserSettingsProfileProperties.Where(u => u.ProfileID == user.ID));
                    db.UserSettingsProfiles.DeleteOnSubmit(user);
                    db.SubmitChanges();
                }
            }
            Refresh_Data();
            base.Delete();
        }
    }

}