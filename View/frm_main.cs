using DevExpress.XtraBars;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using Selling.Forms;
using Selling.Classes;
using Selling.Properties;
using DevExpress.LookAndFeel;
using DevExpress.XtraBars.Navigation;
using DevExpress.XtraEditors;

namespace Selling.Forms
{
    public partial class frm_main : DevExpress.XtraBars.FluentDesignSystem.FluentDesignForm
    {
        private static frm_main _instance;
        public static frm_main Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new frm_main();
                }
                return _instance;
            }
        }
        public frm_main()
        {
            InitializeComponent();
            accordionControl1.ElementClick += AccordionControl1_ElementClick;
        }

        private void AccordionControl1_ElementClick(object sender, DevExpress.XtraBars.Navigation.ElementClickEventArgs e)
        {
            var tag = e.Element.Tag as string;
            if (tag != string.Empty)
            {
                openbyname(tag);
            }

        }

        public static void openbyname(string name)
        {
            Form form = null;
            switch (name)
            {
                case "frm_Customer":
                    form = new frm_CustomerAndVendor(true);
                    break;
                case "frm_vendor":
                    form = new frm_CustomerAndVendor(false);
                    break;
                case "frm_CustomersList":
                    form = new frm_CustomersAndVendorsList(true);
                    break;
                case "frm_VendorList":
                    form = new frm_CustomersAndVendorsList(false);
                    break;
                case "frm_purchaseinvoice":
                    form = new frm_Invoice(master.InvoiceType.Purchase);
                    break;
                case "frm_Salesinvoice":
                    form = new frm_Invoice(master.InvoiceType.Sales);
                    break;
                default:
                    var ins = Assembly.GetExecutingAssembly().GetTypes().FirstOrDefault(x => x.Name == name);
                    if (ins != null)
                    {
                        form = Activator.CreateInstance(ins) as Form;
                        if (Application.OpenForms[form.Name] != null)
                        {
                            form = Application.OpenForms[form.Name];
                        }
                        else
                        {
                            // form.Show();
                        }
                        form.BringToFront();
                    }
                    break;
            }
            
            if (form != null)
            {
                form.Name = name;
                openForm(form);
            }
        }
        public static void openForm(Form frm, bool OpenInDialog = false)
        {
            if (session.User.UserType == (byte)master.UserType.Admin)
            {
                if (OpenInDialog) frm.ShowDialog();
                else frm.Show();
                return;
            }
            var screen = session.ScreensAccesses.SingleOrDefault(x => x.ScreenName == frm.Name);
            if (screen != null && screen.CanOpen == true)
            {
                if (OpenInDialog) frm.ShowDialog();
                else frm.Show();
                return;
            }
            else
            {
                XtraMessageBox.Show("You don't have a permission reach for this", "Permission Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void frm_main_Load(object sender, EventArgs e)
        {
            UserLookAndFeel.Default.SkinName = Settings.Default.SkinName.ToString();
            UserLookAndFeel.Default.SetSkinStyle(Settings.Default.SkinName.ToString(), Settings.Default.PalettaName);

            accordionControl1.Elements.Clear();
            var screens = Classes.session.ScreensAccesses.Where(x => x.CanShow == true);
            screens.Where(s => s.ParantScreenID == 0).ToList().ForEach(s =>
            {
                 AccordionControlElement elm = new AccordionControlElement()
                 {
                     Text = s.ScreenCaption,
                     Tag = s.ScreenName,
                     Name = s.ScreenName,
                     Style = ElementStyle.Group
                 };
                 accordionControl1.Elements.Add(elm);
                 AddAccordionElement(elm, s.ScreenID);
             });

        }
        private void AddAccordionElement(AccordionControlElement parant, int parantID)
        {
            var screens = Classes.session.ScreensAccesses.Where(x => x.CanShow == true);
            screens.Where(s => s.ParantScreenID == parantID).ToList().ForEach(s =>
            {
                AccordionControlElement elm = new AccordionControlElement()
                {
                    Text = s.ScreenCaption,
                    Tag = s.ScreenName,
                    Name = s.ScreenName,
                    Style = ElementStyle.Item
                };
                parant.Elements.Add(elm);
            });

        }

        private void frm_main_FormClosing(object sender, FormClosingEventArgs e)
        {
            Settings.Default.SkinName = UserLookAndFeel.Default.SkinName;
            Settings.Default.PalettaName = UserLookAndFeel.Default.ActiveSvgPaletteName;
            Settings.Default.Save();
            Application.Exit();
        }
        //---------------------------------------------------------------------------------------------------------------

        /* private void ace_CompanyInfo_Click(object sender, EventArgs e)
        {
         openbyname("frm_CompanyInfo");
        }

        private void ace_Stores_Click(object sender, EventArgs e)
        {
         openbyname("frm_stores");
        }*/
    }
}
