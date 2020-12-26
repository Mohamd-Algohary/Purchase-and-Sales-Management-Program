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
    public partial class frm_StoreList : frm_master
    {
        public frm_StoreList()
        {
            InitializeComponent();
            but_save.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
        }
        public override void Refresh_Data()
        {
            var db = new dbDataContext();
            gridControl1.DataSource = db.Stores.Select( x=> new { x.ID, x.Name });
            base.Refresh_Data();
        }
        private void frm_StoreList_Load(object sender, EventArgs e)
        {

            Refresh_Data();

            gridView1.OptionsBehavior.Editable = false;
            gridView1.Columns["ID"].Visible = false;
            gridView1.Columns["Name"].Caption = "name";
        }

        private void gridView1_DoubleClick(object sender, EventArgs e)
        {
            int id = Convert.ToInt32(gridView1.GetFocusedRowCellValue("ID"));

            frm_stores st_frm = new frm_stores(id);
            frm_main.openForm(st_frm, true);   // stop until form is closed
            Refresh_Data();
        }
        public override void New()
        {
            frm_stores st1_frm = new frm_stores();
            frm_main.openForm(st1_frm, true);
            Refresh_Data();
            base.New();
        }
        public override void Delete()
        {
            if (XtraMessageBox.Show("Are you sure from delete this item?", "Delete message", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                int id = Convert.ToInt32(gridView1.GetFocusedRowCellValue("ID"));
                var db = new dbDataContext();
                DAL.Store st = db.Stores.Where(s => s.ID == id).First();
                db.Stores.DeleteOnSubmit(st);
                db.SubmitChanges();
                base.Delete();
            }
            Refresh_Data();
        }

    }
}
