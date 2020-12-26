using DevExpress.Utils;
using DevExpress.Utils.Animation;
using DevExpress.XtraBars.Navigation;
using DevExpress.XtraEditors;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;
using DevExpress.XtraTab;
using Selling.Classes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Selling.Forms
{
    public partial class frm__UserSettingsProfile : frm_master
    {
        DAL.UserSettingsProfile profile;
        List<BaseEdit> editors;
        public frm__UserSettingsProfile()
        {
            InitializeComponent();
            accordionControl1.ElementClick += AccordionControl1_ElementClick;
            New();
            Getdata();
        }
        public frm__UserSettingsProfile(int id)
        {
            InitializeComponent();
            accordionControl1.ElementClick += AccordionControl1_ElementClick;
            using (var db = new DAL.dbDataContext())
            {
               profile = db.UserSettingsProfiles.Single(x => x.ID == id);
            }
            textEdit1.Text = profile.Name;
            Getdata();
            
        }
        public override void Getdata()
        {
            editors = new List<BaseEdit>();
            UserSettingsTemplate settings = new UserSettingsTemplate(profile.ID);

            accordionControl1.Elements.Clear();
            xtraTabControl1.TabPages.Clear();

            var catlog = settings.GetType().GetProperties(); //Get Properties of each property in this class as arry such as (General - Invoices......).
            foreach (var item in catlog)
            {
                accordionControl1.Elements.Add(new AccordionControlElement
                {
                    Name = item.Name.ToString(),
                    Text = UserSettingsTemplate.GetPropCaption(item.Name.ToString()),
                    Style = ElementStyle.Item
                });

                var Pages = new XtraTabPage();
                xtraTabControl1.TabPages.Add(Pages);
                LayoutControl lc = new LayoutControl();
                lc.Dock = DockStyle.Fill;
                Pages.Controls.Add(lc);

                EmptySpaceItem spaceItem1 = new EmptySpaceItem();
                EmptySpaceItem spaceItem2 = new EmptySpaceItem();
                spaceItem1.SizeConstraintsType = SizeConstraintsType.Custom;
                spaceItem1.MaxSize = new Size(500, 22);
                spaceItem1.MinSize = new Size(250, 22);
                lc.AddItem(spaceItem1);
                lc.AddItem(spaceItem2, spaceItem1, InsertType.Left);

                var Props = item.GetValue(settings).GetType().GetProperties();   // Get Properties of each item such as (properties in General Class....).
                foreach (var prop in Props)
                {
                    BaseEdit edit = UserSettingsTemplate.GetProprtyControl(prop.Name, prop.GetValue(item.GetValue(settings)));
                    if (edit != null)
                    {
                        var layoutItems = lc.AddItem(" ", edit, spaceItem2, InsertType.Top);
                        layoutItems.TextVisible = true;
                        layoutItems.Text = UserSettingsTemplate.GetPropCaption(prop.Name);
                        editors.Add(edit);
                    }
                }
            }
            base.Getdata();
        }
        private void AccordionControl1_ElementClick(object sender, ElementClickEventArgs e)
        {
            xtraTabControl1.SelectedTabPageIndex = accordionControl1.Elements.IndexOf(e.Element);
        }
        public override void New()
        {
            profile = new DAL.UserSettingsProfile();
            textEdit1.Text = "";
        }
        public override bool IsDataValid()
        {
            if (textEdit1.Text.Trim() == string.Empty)
            {
                textEdit1.ErrorText = "Please enter your name";
                return false;
            }
            int no = 0;
            editors.ForEach(e =>
            {
                if (e.GetType() == typeof(LookUpEdit) && ((LookUpEdit)e).Properties.DataSource.GetType() != typeof(List<master.ValueAndID>))
                {
                    no += ((LookUpEdit)e).IsEditeValueValidAndNotZero() ? 0 : 1;
                }
            });
            return (no == 0);
        }
        public override void Save()
        {
            var db = new DAL.dbDataContext();
            if(profile.ID == 0)
            {
                db.UserSettingsProfiles.InsertOnSubmit(profile);
            }
            else
            {
                db.UserSettingsProfiles.Attach(profile);
            }
            profile.Name = textEdit1.Text;
            db.SubmitChanges();
            db.UserSettingsProfileProperties.DeleteAllOnSubmit(db.UserSettingsProfileProperties.Where(x => x.ProfileID == profile.ID));
            db.SubmitChanges();
            editors.ForEach(e =>
            {
                db.UserSettingsProfileProperties.InsertOnSubmit(new DAL.UserSettingsProfileProperty
                {
                    ProfileID = profile.ID,
                    PropertyName = e.Name,
                    PropertyValue = master.ToByteArray<object>(e.EditValue)
                });
            });
            db.SubmitChanges();
            base.Save();
        }
        private void frm__UserSettingsProfile_Load(object sender, EventArgs e)
        {
           
            accordionControl1.AnimationType = AnimationType.Simple;
            accordionControl1.AllowItemSelection = true;
            accordionControl1.Appearance.Item.Hovered.Font = new Font(accordionControl1.Appearance.Item.Hovered.Font, FontStyle.Bold);
            accordionControl1.Appearance.Item.Hovered.Options.UseFont = true;
            accordionControl1.Appearance.Item.Pressed.Font = new Font(accordionControl1.Appearance.Item.Pressed.Font, FontStyle.Bold);
            accordionControl1.Appearance.Item.Pressed.Options.UseFont = true;
            accordionControl1.ScrollBarMode = ScrollBarMode.Hidden;
            accordionControl1.OptionsMinimizing.AllowMinimizeMode = DefaultBoolean.False;
          
            xtraTabControl1.ShowTabHeader = DefaultBoolean.False;
            xtraTabControl1.Transition.AllowTransition = DefaultBoolean.True;
            //xtraTabControl1.Transition.EasingMode = EasingMode.EaseInOut;
            xtraTabControl1.SelectedPageChanging += XtraTabControl1_SelectedPageChanging;
        }
        private void XtraTabControl1_SelectedPageChanging(object sender, TabPageChangingEventArgs e)
        {
            SlideFadeTransition trans = new SlideFadeTransition();
            var currentPage = xtraTabControl1.TabPages.IndexOf(e.Page);
            var prevPage = xtraTabControl1.TabPages.IndexOf(e.PrevPage);
            if (currentPage > prevPage) trans.Parameters.EffectOptions = PushEffectOptions.FromBottom;
            else trans.Parameters.EffectOptions = PushEffectOptions.FromTop;
            xtraTabControl1.Transition.TransitionType = trans;
        }
    }
}