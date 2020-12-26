using DevExpress.XtraGrid.Views.Grid;
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
using static Selling.Classes.master;

namespace Selling.Forms
{
    public partial class frm_ProductList : frm_master
    {
        public frm_ProductList()
        {
            InitializeComponent();
        }

        private void frm_ProductList_Load(object sender, EventArgs e)
        {
            Refresh_Data();
            gridView1.OptionsBehavior.Editable = false;
            but_save.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            but_delete.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            gridView1.CustomColumnDisplayText += GridView1_CustomColumnDisplayText;
            gridView1.DoubleClick += GridView1_DoubleClick;

            gridControl1.ViewRegistered += GridControl1_ViewRegistered;
            gridView1.OptionsDetail.ShowDetailTabs = false;
        }
        public override void New()
        {
            var frm = new frm_Products();
            frm_main.openForm(frm, true);
            Refresh_Data();
            base.New();
        }
        private void GridControl1_ViewRegistered(object sender, DevExpress.XtraGrid.ViewOperationEventArgs e)
        {
            if(e.View.LevelName == "UOM")
            {
                GridView view = e.View as GridView;
                view.OptionsView.ShowViewCaption = true;
                view.ViewCaption = "Unit Name";
            }
        }
        private void GridView1_DoubleClick(object sender, EventArgs e)
        {
            int id = 0;
            if(int.TryParse(gridView1.GetFocusedRowCellValue("ID").ToString(), out id) && id > 0)
            {
                var frm = new frm_Products(id);
                frm_main.openForm(frm, true);
                Refresh_Data();
            }
        }
        private void GridView1_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            if (e.Column.FieldName == "Type")
            {
                e.DisplayText = PruductTypeList.Single(x => x.ID == Convert.ToInt32(e.Value)).Name;
            }
        }
        public override void Refresh_Data()
        {
             gridControl1.DataSource = session.ProductsView;
             var ins = new session.ProductViewClass();
             gridView1.Columns[nameof(ins.ID)].Visible = false;
        }
    }
}
