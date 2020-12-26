using Selling.DAL;
using Selling.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using static Selling.Classes.databaseWacher;

namespace Selling.Classes
{
    public static class session
    {
        public static class Defualt
        {
            public static int Drawer { get => 4; }   // get{ return 1;}  
            public static int Customer { get => 1; }
            public static int Vender { get => 4; }
            public static int Store { get => 1019; }
            public static int RawStore { get => 1019; }
            public static int DiscountReceivedAccount { get => 1023; }
            public static int DiscountAllowedAccount { get => 1024; }
            public static int SalesTax { get => 1049; }
            public static int PurchasesTax { get => 1050; }
            public static int PurchasesExpences { get => 1051; }
        }
        //---------------------------------------------------------------------------------------------------------------
        private static DAL.CompanyInfo _companyInfo;             //Fieled
        public static DAL.CompanyInfo CompanyInfo                //Property
        {
            get        
            {
                if (_companyInfo == null)
                {
                    using (var db = new DAL.dbDataContext())
                    {
                        _companyInfo = db.CompanyInfos.FirstOrDefault();
                    }
                }
                return _companyInfo;
            }
        }
        //-------------------------------------------------------------------------------------------------------------------
        public static class GlobalSettings
        {
            //20-00009-180-9    (الرقم المقروء من ميزان الباركود)
            public static bool ReadFromScaleBarCode { get => true; }
            public static bool IgnoreCheckDigit { get => true; }    // check code = 9
            public static string ScaleBarCodePrefix { get => "20"; }   //20
            public static byte BarCodeLength { get => 13; }
            public static byte ProductCodeLength { get => 5; } //00009
            public static byte ValueCodeLength { get => 5; }   //180 may be weight or value
            public static byte DivideValueBy { get => 3; }
            public static ReadValueMode ReadMode { get => ReadValueMode.weight; }
            public enum ReadValueMode
            {
                weight,
                price
            }
        }
        //------------------------------------------------------------------------------------------------------------
        private static BindingList<DAL.Productss> _products;
        public static BindingList<DAL.Productss> Products
        {
            get
            {
                if (_products == null)
                {
                    using (var db = new dbDataContext())
                    {
                        _products = new BindingList<DAL.Productss>(db.Productsses.ToList());
                    }
                    databaseWacher.ProductWatcher = new TableDependency.SqlClient.SqlTableDependency<Productss>(Properties.Settings.Default.SalesDBConnectionString2);
                    databaseWacher.ProductWatcher.OnChanged += databaseWacher.Product_Chanded;
                    databaseWacher.ProductWatcher.Start();
                }
                return _products;
            }
        }
        //--------------------------------------------------------------------------------------------------------------
        private static BindingList<ProductViewClass> _productsView;
        public static BindingList<ProductViewClass> ProductsView
        {
            get
            {
                if (_productsView == null)
                {
                    using (var db = new dbDataContext())
                    {
                        var data = from pr in session.Products
                                   join cg in db.ProductCategories on pr.CategoryID equals cg.ID  // when you make join make sure that DB is Clear
                                   select new ProductViewClass
                                   {
                                       ID = pr.ID,
                                       Code = pr.Code,
                                       Name = pr.Name,
                                       CategoryName = cg.Name,
                                       Description = pr.Description,
                                       IsActive = pr.IsActive,
                                       Type = pr.Type,
                                       Units = (from u in db.ProductUnits
                                                where u.ProductID == pr.ID
                                                join un in db.UnitNames on u.UnitID equals un.ID
                                                select new ProductUOMView
                                                {
                                                    UnitID = u.UnitID,
                                                    UnitName = un.Name,
                                                    Factor = u.Factor,
                                                    BuyPrice = u.BuyPrice,
                                                    SellPrice = u.SalePrice,
                                                    BarCode = u.Barcode,
                                                }).ToList()
                                   };
                        _productsView = new BindingList<ProductViewClass>(data.ToList());
                    }
                }
                return _productsView;
            }
        }
        public class ProductViewClass   //Such as DAL.Products
        {
            public int ID { get; set; }
            public string Code { get; set; }
            public string Name { get; set; }
            public string CategoryName { get; set; }
            public string Description { get; set; }
            public bool IsActive { get; set; }
            public byte Type { get; set; }
            public List<ProductUOMView> Units { get; set; }
        }
        public class ProductUOMView      //Such as DAL.ProductUnits
        {
            public int UnitID { get; set; }
            public string UnitName { get; set; }
            public double Factor { get; set; }
            public double SellPrice { get; set; }
            public double BuyPrice { get; set; }
            public string BarCode { get; set; }
        }
        public static ProductViewClass GetProduct(int id) //To return item as same as (ProductViewClass) type to add in (ProductView BindingList)
        {
            using (var db = new dbDataContext())
            {
                var data = from pr in session.Products
                           where pr.ID == id
                           join cg in db.ProductCategories on pr.CategoryID equals cg.ID  // when you make join make sure that DB is Clear
                           select new ProductViewClass
                           {
                               ID = pr.ID,
                               Code = pr.Code,
                               Name = pr.Name,
                               CategoryName = cg.Name,
                               Description = pr.Description,
                               IsActive = pr.IsActive,
                               Type = pr.Type,
                               Units = (from u in db.ProductUnits
                                        where u.ProductID == pr.ID
                                        join un in db.UnitNames on u.UnitID equals un.ID
                                        select new ProductUOMView
                                        {
                                            UnitID = u.UnitID,
                                            UnitName = un.Name,
                                            Factor = u.Factor,
                                            BuyPrice = u.BuyPrice,
                                            SellPrice = u.SalePrice,
                                            BarCode = u.Barcode,
                                        }).ToList()
                               //Units = db.ProductUnits.Where(x => x.ProductID == pr.ID).Select(u => new
                               //{
                               //     UnitName = db.UnitNames.Single(un => un.ID == u.UnitID).Name,
                               //     UnitName = un.Name,
                               //     Factor = u.Factor,
                               //     BuyPrice = u.BuyPrice,
                               //     SellPrice = u.SalePrice,
                               //     BarCode = u.Barcode,
                               //}).ToList()
                           };
                return data.First();
            }
        }
        //-------------------------------------------------------------------------------------------------------
        private static BindingList<DAL.UnitName> _unitNames;
        public static BindingList<DAL.UnitName> UnitNames
        {
            get
            {
                if (_unitNames == null)
                {
                    using (var db = new DAL.dbDataContext())
                    {
                        _unitNames = new BindingList<UnitName>(db.UnitNames.ToList());
                    }
                    databaseWacher.UnitNameWatcher = new TableDependency.SqlClient.SqlTableDependency<UnitNames>(Properties.Settings.Default.SalesDBConnectionString2);
                    databaseWacher.UnitNameWatcher.OnChanged += UnitName_Changed;
                    databaseWacher.UnitNameWatcher.Start();
                }
                return _unitNames;
            }
        }
        //----------------------------------------------------------------------------------------------------------------
        private static BindingList<DAL.CustomerAndVendor> _vendors;
        public static BindingList<DAL.CustomerAndVendor> Vendors
        {
            get
            {
                if (_vendors == null)
                {
                    using (var db = new dbDataContext())
                    {
                        _vendors = new BindingList<CustomerAndVendor>(db.CustomerAndVendors.Where(x => x.IsCustomer == false).ToList());
                    }
                    databaseWacher.VendorWatcher = new TableDependency.SqlClient.SqlTableDependency<CustomerAndVendors>(
                        Properties.Settings.Default.SalesDBConnectionString2, filter: new databaseWacher.VendorsOnly());
                    databaseWacher.VendorWatcher.OnChanged += databaseWacher.Vendor_Changed;
                    databaseWacher.VendorWatcher.Start();
                }
                return _vendors;
            }
        }
        private static BindingList<DAL.CustomerAndVendor> _customers;
        public static BindingList<DAL.CustomerAndVendor> Customers
        {
            get
            {
                if (_customers == null)
                {
                    using (var db = new dbDataContext())
                    {
                        _customers = new BindingList<CustomerAndVendor>(db.CustomerAndVendors.Where(x => x.IsCustomer == true).ToList());
                    }
                    databaseWacher.CustomerWatcher = new TableDependency.SqlClient.SqlTableDependency<CustomerAndVendors>(
                        Properties.Settings.Default.SalesDBConnectionString2, filter: new databaseWacher.CustomersOnly());
                    databaseWacher.CustomerWatcher.OnChanged += databaseWacher.Customer_Changed;
                    databaseWacher.CustomerWatcher.Start();
                }
                return _customers;
            }
        }
        //------------------------------------------------------------------------------------------------------------------
        private static BindingList<DAL.Drawer> _drawers;
        public static BindingList<DAL.Drawer> Drawers
        {
            get
            {
                if (_drawers == null)
                {
                    using (var db = new dbDataContext())
                    {
                        _drawers = new BindingList<Drawer>(db.Drawers.ToList());
                    }

                }
                return _drawers;
            }
        }
        //----------------------------------------------------------------------------------------------------------------
        private static BindingList<DAL.Store> _stores;
        public static BindingList<DAL.Store> Stores
        {
            get
            {
                if (_stores == null)
                {
                    using (var db = new dbDataContext())
                    {
                        _stores = new BindingList<Store>(db.Stores.ToList());
                    }
                    databaseWacher.StoreWatcher = new TableDependency.SqlClient.SqlTableDependency<Store>(Properties.Settings.Default.SalesDBConnectionString2);
                    databaseWacher.StoreWatcher.OnChanged += Store_Changed;
                    databaseWacher.StoreWatcher.Start();
                }
                return _stores;
            }
        }
        //----------------------------------------------------------------------------------------------------------------------
        public static DAL.User User { get; private set; }
        public static List<ScreensAccessProfile> ScreensAccesses { get; private set; }
        public static void SetUser(DAL.User user)
        {
            User = user;
            using (var db = new DAL.dbDataContext())
            {
                ScreensAccesses = (from s in Classes.Screens.GetScreens
                        from d in db.UserAccessProfileDetails.Where(x => x.ProfileID == User.ID && x.ScreenID == s.ScreenID).DefaultIfEmpty()
                        select new Classes.ScreensAccessProfile(s.ScreenName)
                        {
                            Actions = s.Actions,
                            ScreenName = s.ScreenName,
                            ScreenID = s.ScreenID,
                            ParantScreenID = s.ParantScreenID,
                            ScreenCaption = s.ScreenCaption,
                            CanAdd = (d == null) ? true : d.CanAdd,
                            CanDelete = (d == null) ? true : d.CanDelete,
                            CanEdit = (d == null) ? true : d.CanEdit,
                            CanOpen = (d == null) ? true : d.CanOpen,
                            CanPrint = (d == null) ? true : d.CanPrint,
                            CanShow = (d == null) ? true : d.CanShow
                        }).ToList();
            }
        }
        public static class CurrentUserSettings
        {
            public static master.PrintMode InvoicePrintMode { get => master.PrintMode.ShowPreview; }

            private static UserSettingsTemplate _userSettings;
            public static UserSettingsTemplate UserSettings
            {
                get
                {
                    if (_userSettings == null)
                    {
                        _userSettings = new UserSettingsTemplate(User.SettingsProfileID);
                    }
                    return _userSettings;
                }
            }
        }

        private static BindingList<DAL.UserSettingsProfileProperty> _profileProperties;
        public static BindingList<DAL.UserSettingsProfileProperty> ProfileProperties
        {
            get
            {
                if (_profileProperties == null)
                {
                    using (var db = new dbDataContext())
                    {
                        _profileProperties = new BindingList<UserSettingsProfileProperty>(db.UserSettingsProfileProperties.ToList());
                    }
                    databaseWacher.ProfilePropertiesWatcher = new TableDependency.SqlClient.SqlTableDependency<UserSettingsProfileProperties>(Properties.Settings.Default.SalesDBConnectionString2);
                    databaseWacher.ProfilePropertiesWatcher.OnChanged += ProfileProperties_Changed;
                    databaseWacher.ProfilePropertiesWatcher.Start();
                }
                return _profileProperties;
            }
        }
        //----------------------------------------------------------------------------------------------------------------
        private static BindingList<ProductBalance> _productsBalance;
        public static BindingList<ProductBalance> ProductsBalance
        {
            get
            {
                if (_productsBalance == null)
                {
                    using (var db = new dbDataContext())
                    {
                        var data = from stL in db.StoreLogs
                                   group stL by new { stL.ProductID, stL.storeID } into g
                                   select new ProductBalance
                                   {
                                       Balance = g.Where(x => x.IsInTransaction == true).Sum(q => (double?)q.Qty) ?? 0 -
                                                g.Where(x => x.IsInTransaction == false).Sum(q => (double?)q.Qty) ?? 0,
                                       ProductID = g.Key.ProductID,
                                       StoreID = g.Key.storeID
                                   };
                        _productsBalance = new BindingList<ProductBalance>(data.ToList());
                    }
                    databaseWacher.StoreLogWatcher = new TableDependency.SqlClient.SqlTableDependency<StoreLog>(Properties.Settings.Default.SalesDBConnectionString2);
                    databaseWacher.StoreLogWatcher.OnChanged += StoreLog_Changed;
                    databaseWacher.StoreLogWatcher.Start();
                }
                return _productsBalance;
            }
        }
        public  class ProductBalance
        {
            public int ProductID { get; set; }
            public int StoreID { get; set; }
            public double Balance { get; set; }

        }

    }
}
