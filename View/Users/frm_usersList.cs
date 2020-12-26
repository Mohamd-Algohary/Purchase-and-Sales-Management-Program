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

namespace Selling.Forms
{
    public partial class frm_usersList : frm_master
    {
        public frm_usersList()
        {
            InitializeComponent();
            Refresh_Data();
            gridView1.DoubleClick += GridView1_DoubleClick;
            but_save.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
        }

        public override void Refresh_Data()
        {
            gridControl1.DataSource = null;
            using (var db = new DAL.dbDataContext())
            {
                gridControl1.DataSource = db.Users.Select(x => new { x.ID, x.Name, x.IsActive }).ToList();
            }
            gridView1.Columns["ID"].Visible = false;
            gridView1.Columns["Name"].OptionsColumn.AllowEdit = false;
            gridView1.Columns["IsActive"].OptionsColumn.AllowEdit = false;
            base.Refresh_Data();
        }
        public override void New()
        {
            var frm = new frm_User();
            frm_main.openForm(frm, true);
            Refresh_Data();
            base.New();
        }
        public override void Delete()
        {
            using (var db = new DAL.dbDataContext())
            {
                if (XtraMessageBox.Show("Are you sure from deleting this uers", "Delet Message", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    int id = Convert.ToInt32(gridView1.GetFocusedRowCellValue("ID"));
                    db.Users.DeleteAllOnSubmit(db.Users.Where(x => x.ID == id));
                    db.SubmitChanges();
                }
            }
            Refresh_Data();
            base.Delete();
        }
        private void GridView1_DoubleClick(object sender, EventArgs e)
        {
            var ea = e as DXMouseEventArgs;
            var view = sender as GridView;
            var info = view.CalcHitInfo(ea.Location);
            if (info.InRow)
            {
                var frm = new frm_User(Convert.ToInt32(gridView1.GetFocusedRowCellValue("ID")));
                frm_main.openForm(frm, true);
                Refresh_Data();
            }
        }
    }
}