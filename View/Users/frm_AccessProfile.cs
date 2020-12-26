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
using DevExpress.DataProcessing;                                                              
using Selling.Classes;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.Controls;
using DevExpress.LookAndFeel;

namespace Selling.Forms
{
    public partial class frm_AccessProfile : frm_master
    {
        DAL.UserAccessProfile profile;
        Classes.ScreensAccessProfile ins = new ScreensAccessProfile("");
        public frm_AccessProfile()
        {
            InitializeComponent();
            New();
            Getdata();
        }
        public frm_AccessProfile(int id)
        {
            InitializeComponent();
            using(var db = new DAL.dbDataContext())
            {
                profile = db.UserAccessProfiles.SingleOrDefault(x => x.ID == id);
            }
            textEdit1.Text = profile.Name;
            Getdata();
        }
        RepositoryItemCheckEdit repoCheck;
        private void frm_AccessProfile_Load(object sender, EventArgs e)
        {
            textEdit1.Text = profile.Name;
            treeList1.CustomNodeCellEdit += TreeList1_CustomNodeCellEdit;
            treeList1.ParentFieldName = nameof(ins.ParantScreenID);
            treeList1.KeyFieldName = nameof(ins.ScreenID);
            treeList1.Columns[nameof(ins.ScreenName)].Visible = false;
            treeList1.Columns[nameof(ins.ScreenCaption)].Caption = "Screen Name";
            treeList1.Columns[nameof(ins.CanAdd)].Caption = "Add";
            treeList1.Columns[nameof(ins.CanDelete)].Caption = "Delete";
            treeList1.Columns[nameof(ins.CanEdit)].Caption = "Edit";
            treeList1.Columns[nameof(ins.CanOpen)].Caption = "Open";
            treeList1.Columns[nameof(ins.CanPrint)].Caption = "Print";
            treeList1.Columns[nameof(ins.CanShow)].Caption = "Show";
            treeList1.BestFitColumns();

            repoCheck = new RepositoryItemCheckEdit();
            repoCheck.CheckBoxOptions.Style = CheckBoxStyle.SvgRadio2;
            repoCheck.CheckBoxOptions.SvgColorChecked = DXSkinColors.ForeColors.Information;
            repoCheck.CheckBoxOptions.SvgColorUnchecked = DXSkinColors.ForeColors.DisabledText;

            treeList1.Columns[nameof(ins.CanAdd)].ColumnEdit =
            treeList1.Columns[nameof(ins.CanDelete)].ColumnEdit =
            treeList1.Columns[nameof(ins.CanEdit)].ColumnEdit =
            treeList1.Columns[nameof(ins.CanOpen)].ColumnEdit =
            treeList1.Columns[nameof(ins.CanPrint)].ColumnEdit =
            treeList1.Columns[nameof(ins.CanShow)].ColumnEdit = repoCheck;
        }
        public override void New()
        {
            profile = new DAL.UserAccessProfile();
        }
        private void TreeList1_CustomNodeCellEdit(object sender, DevExpress.XtraTreeList.GetCustomNodeCellEditEventArgs e)
        {
            if(e.Node.Id >= 0)
            {
                var row = treeList1.GetRow(e.Node.Id) as Classes.ScreensAccessProfile;
                if(row != null)
                {
                  if(e.Column.FieldName == nameof(ins.CanAdd) && row.Actions.Contains(master.Actions.Add) == false)
                  {
                        e.RepositoryItem = new RepositoryItem();
                  }
                  else if (e.Column.FieldName == nameof(ins.CanDelete) && row.Actions.Contains(master.Actions.Delete) == false)
                  {
                        e.RepositoryItem = new RepositoryItem();
                  }
                  else if (e.Column.FieldName == nameof(ins.CanEdit) && row.Actions.Contains(master.Actions.Edit) == false)
                  {
                        e.RepositoryItem = new RepositoryItem();
                  }
                  else if (e.Column.FieldName == nameof(ins.CanOpen) && row.Actions.Contains(master.Actions.Open) == false)
                  {
                        e.RepositoryItem = new RepositoryItem();
                  }
                  else if (e.Column.FieldName == nameof(ins.CanPrint) && row.Actions.Contains(master.Actions.Print) == false)
                  {
                        e.RepositoryItem = new RepositoryItem();
                  }
                  else if (e.Column.FieldName == nameof(ins.CanShow) && row.Actions.Contains(master.Actions.Show) == false)
                  {
                        e.RepositoryItem = new RepositoryItem();
                  }
                }
            }
        }

        public override void Getdata()
        {
            List<Classes.ScreensAccessProfile> data;
            using (var db = new DAL.dbDataContext())
            {
                data = (from s in Classes.Screens.GetScreens
                           from d in db.UserAccessProfileDetails.Where(x=> x.ProfileID == profile.ID && x.ScreenID == s.ScreenID).DefaultIfEmpty()
                           select new Classes.ScreensAccessProfile(s.ScreenName)
                           {
                              Actions = s.Actions,
                              ScreenName = s.ScreenName,
                              ScreenID = s.ScreenID,
                              ParantScreenID = s.ParantScreenID,
                              ScreenCaption = s.ScreenCaption,
                              CanAdd = (d==null)? true: d.CanAdd,
                              CanDelete = (d == null) ? true : d.CanDelete,
                              CanEdit = (d == null) ? true : d.CanEdit,
                              CanOpen = (d == null) ? true : d.CanOpen,
                              CanPrint = (d == null) ? true : d.CanPrint,
                              CanShow = (d == null) ? true : d.CanShow
                           }).ToList();
            }
            treeList1.DataSource = data;
            base.Getdata();
        }
        public override bool IsDataValid()
        {
            if(textEdit1.Text.Trim() == string.Empty)
            {
                textEdit1.ErrorText = "This Field is required";
                return false;
            }
            return base.IsDataValid();
        }
        public override void Save()
        {
            var db = new DAL.dbDataContext();
            if(profile.ID == 0)
            {
                db.UserAccessProfiles.InsertOnSubmit(profile);
            }
            else
            {
                db.UserAccessProfiles.Attach(profile);
            }
            profile.Name = textEdit1.Text;
            db.SubmitChanges();
            db.UserAccessProfileDetails.DeleteAllOnSubmit(db.UserAccessProfileDetails.Where(x => x.ProfileID == profile.ID));
            db.SubmitChanges();
            var data = treeList1.DataSource as List<Classes.ScreensAccessProfile>;
            var dbData = data.Select(s => new DAL.UserAccessProfileDetail
            {
                ProfileID = profile.ID,
                ScreenID = s.ScreenID,
                CanAdd = s.CanAdd,
                CanDelete = s.CanDelete,
                CanEdit = s.CanEdit,
                CanOpen = s.CanOpen,
                CanPrint = s.CanPrint,
                CanShow = s.CanShow
            }).ToList();
            db.UserAccessProfileDetails.InsertAllOnSubmit(dbData);
            db.SubmitChanges();
            base.Save();
        }
    }
}