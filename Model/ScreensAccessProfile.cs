using DevExpress.DataProcessing;
using Selling.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Selling.Classes
{
    public class ScreensAccessProfile
    {
        public static int MaxID = 1;
        public int ScreenID { get; set; }
        public int ParantScreenID { get; set; }
        public string ScreenName { get; set; }
        public string ScreenCaption { get; set; }
        public bool CanShow { get; set; }
        public bool CanOpen { get; set; }
        public bool CanAdd { get; set; }
        public bool CanEdit { get; set; }
        public bool CanDelete { get; set; }
        public bool CanPrint { get; set; }
        public List<master.Actions> Actions { get; set; }
        public ScreensAccessProfile(string Name, ScreensAccessProfile Parant = null)
        {
            ScreenName = Name;
            ScreenID = MaxID++;
            if (Parant != null) ParantScreenID = Parant.ScreenID;
            else ParantScreenID = 0;
            Actions = new List<master.Actions>()
            {
              master.Actions.Add,
              master.Actions.Delete,
              master.Actions.Edit,
              master.Actions.Open,
              master.Actions.Print,
              master.Actions.Show
            };
        }
    }
    public static class Screens
    {
        public static ScreensAccessProfile mainSettings = new ScreensAccessProfile("elm_MainSettings")
        {
            Actions = new List<master.Actions>() { master.Actions.Show },
            ScreenCaption = "Main Settings"
        };
        public static ScreensAccessProfile companyInfo = new ScreensAccessProfile(nameof(frm_CompanyInfo), mainSettings)
        {
            Actions = new List<master.Actions>() { master.Actions.Show, master.Actions.Open, master.Actions.Edit },
            ScreenCaption = "Company Info"
        };

        public static ScreensAccessProfile store = new ScreensAccessProfile("elm_Stores")
        {
            Actions = new List<master.Actions>() {master.Actions.Show },
            ScreenCaption = "Stores"
        };
        public static ScreensAccessProfile addStore = new ScreensAccessProfile(nameof(frm_stores), store)
        {
            ScreenCaption = "Add Store"
        };
        public static ScreensAccessProfile viewStore = new ScreensAccessProfile(nameof(frm_StoreList), store)
        {
            ScreenCaption = "Stores List"
        };

        public static ScreensAccessProfile drawer = new ScreensAccessProfile("elm_Drawer")
        {
            Actions = new List<master.Actions>() { master.Actions.Show },
            ScreenCaption = "Drawers"
        };
        public static ScreensAccessProfile addDrawer = new ScreensAccessProfile(nameof(frm_Drawer), drawer)
        {
            ScreenCaption = "Add Drawer"
        };
        public static ScreensAccessProfile viewDrawer = new ScreensAccessProfile(nameof(frm_DrawerList), drawer)
        {
            ScreenCaption = "Drawers List"
        };

        public static ScreensAccessProfile customers = new ScreensAccessProfile("elm_Customers")
        {
            Actions = new List<master.Actions>() { master.Actions.Show },
            ScreenCaption = "Customers"
        };
        public static ScreensAccessProfile addCustomer = new ScreensAccessProfile("frm_Customer", customers)
        {
            ScreenCaption = "Add Customer"
        };
        public static ScreensAccessProfile viewCustomers = new ScreensAccessProfile("frm_CustomersList", customers)
        {
            ScreenCaption = "Customers List"
        };

        public static ScreensAccessProfile vendors = new ScreensAccessProfile("elm_Vendors")
        {
            Actions = new List<master.Actions>() { master.Actions.Show },
            ScreenCaption = "Vendors"
        };
        public static ScreensAccessProfile addVendor = new ScreensAccessProfile("frm_vendor", vendors)
        {
            ScreenCaption = "Add Vendor"
        };
        public static ScreensAccessProfile viewVendors = new ScreensAccessProfile("frm_VendorList", vendors)
        {
            ScreenCaption = "Vendors List"
        };

        public static ScreensAccessProfile products = new ScreensAccessProfile("elm_Products")
        {
            Actions = new List<master.Actions>() { master.Actions.Show },
            ScreenCaption = "Products"
        };
        public static ScreensAccessProfile addProduct = new ScreensAccessProfile(nameof(frm_Products), products)
        {
            ScreenCaption = "Add Product"
        };
        public static ScreensAccessProfile viewProducts = new ScreensAccessProfile(nameof(frm_ProductList), products)
        {
            ScreenCaption = "Products List"
        };
        public static ScreensAccessProfile productsCatagories = new ScreensAccessProfile(nameof(frm_Product_Category), products)
        {
            ScreenCaption = "Product Catagories"
        };

        public static ScreensAccessProfile Purchases = new ScreensAccessProfile("elm_Purchases")
        {
            Actions = new List<master.Actions>() { master.Actions.Show },
            ScreenCaption = "Purchases"
        };
        public static ScreensAccessProfile addPurchaseInvoice = new ScreensAccessProfile("frm_purchaseinvoice", Purchases)
        {
            ScreenCaption = "Add Purchase Invoice"
        };
        public static ScreensAccessProfile Sales = new ScreensAccessProfile("elm_Sales")
        {
            Actions = new List<master.Actions>() { master.Actions.Show },
            ScreenCaption = "Sales"
        };
        public static ScreensAccessProfile addSalesInvoice = new ScreensAccessProfile("frm_Salesinvoice", Sales)
        {
            ScreenCaption = "Add Sales Invoice"
        };
        public static ScreensAccessProfile usersManagement = new ScreensAccessProfile("elm_UsersManagement")
        {
            Actions = new List<master.Actions>() { master.Actions.Show },
            ScreenCaption = "Users Managment"
        };
        public static ScreensAccessProfile usersSettings = new ScreensAccessProfile(nameof(frm__UserSettingsProfile), usersManagement)
        {
            ScreenCaption = "User Settings"
        };
        public static ScreensAccessProfile usersList = new ScreensAccessProfile(nameof(frm_UserSettingProfileList), usersManagement)
        {
            ScreenCaption = "Users Settings List"
        };
        public static ScreensAccessProfile addAccessProfile = new ScreensAccessProfile(nameof(frm_AccessProfile), usersManagement)
        {
            ScreenCaption = "Add Access Profile"
        };
        public static ScreensAccessProfile viewAccessProfile = new ScreensAccessProfile(nameof(frm_AccessProfileList), usersManagement)
        {
            ScreenCaption = "Access Profile List"
        };
        public static ScreensAccessProfile addUser = new ScreensAccessProfile(nameof(frm_User), usersManagement)
        {
            ScreenCaption = "Add User"
        };
        public static ScreensAccessProfile viewUsers = new ScreensAccessProfile(nameof(frm_usersList), usersManagement)
        {
            ScreenCaption = "Users List"
        };
       
        public static List<ScreensAccessProfile> _getScreens;
        public static List<ScreensAccessProfile> GetScreens {

            get
            {
                Type t = typeof(Screens);
                FieldInfo[] fields = t.GetFields(BindingFlags.Public | BindingFlags.Static);
                _getScreens = new List<ScreensAccessProfile>();
                fields.ForEach(e =>
                {
                    var obj = e.GetValue(null);
                    if (obj != null && obj.GetType() == typeof(ScreensAccessProfile))
                        _getScreens.Add((ScreensAccessProfile)obj);
                });
                return _getScreens;
            }
        }
    }

}
