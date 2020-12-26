using DevExpress.Utils;
using Selling.DAL;
using Selling.Forms;
using Selling.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TableDependency.SqlClient;
using TableDependency.SqlClient.Base.Abstracts;
using TableDependency.SqlClient.Base.Enums;
using TableDependency.SqlClient.Base.EventArgs;
using TableDependency.SqlClient.EventArgs;

namespace Selling.Classes
{
    public static class databaseWacher
    {
        public static SqlTableDependency<DAL.Productss> ProductWatcher;   //To watch Changed in DB for (Productss)
        public static void Product_Chanded(object s, RecordChangedEventArgs<DAL.Productss> e)
        { 
            Application.OpenForms[0].Invoke(new Action( ()=> {
                if (e.ChangeType == ChangeType.Insert)
                {
                    session.Products.Add(e.Entity);
                    session.ProductsView.Add(session.GetProduct(e.Entity.ID));  //(e.Entity) is a type of (DAL.Productss) so it can't add to (ProductsView) directly
                }
                else if (e.ChangeType == ChangeType.Update)
                {
                    var indx = session.Products.IndexOf(session.Products.Single(x => x.ID == e.Entity.ID));
                    session.Products.Remove(session.Products.Single(x => x.ID == e.Entity.ID));
                    session.Products.Insert(indx,e.Entity);

                    var Viewindx = session.ProductsView.IndexOf(session.ProductsView.Single(x => x.ID == e.Entity.ID));
                    session.ProductsView.Remove(session.ProductsView.Single(x => x.ID == e.Entity.ID));
                    session.ProductsView.Insert(Viewindx,session.GetProduct(e.Entity.ID));

                }
                else if (e.ChangeType == ChangeType.Delete)
                {
                    session.Products.Remove(session.Products.Single(x => x.ID == e.Entity.ID));
                    session.ProductsView.Remove(session.ProductsView.Single(x => x.ID == e.Entity.ID));
                }
            }));   
        }
        //-----------------------------------------------
        public class CustomerAndVendors : DAL.CustomerAndVendor { }  //For name changed in LINQ(S)

        public static SqlTableDependency<CustomerAndVendors> VendorWatcher;   //To watch Changed in DB for (CustomerAndVendors)
        public static void Vendor_Changed(object s, RecordChangedEventArgs<CustomerAndVendors> e)
        {
            Application.OpenForms[0].Invoke(new Action( ()=>
            {
                if(e.ChangeType == ChangeType.Insert)
                {
                    session.Vendors.Add(e.Entity);
                }
                else if (e.ChangeType == ChangeType.Update)
                {
                    var indx = session.Vendors.IndexOf(session.Vendors.Single(x => x.ID == e.Entity.ID && x.IsCustomer == e.Entity.IsCustomer));
                    session.Vendors.Remove(session.Vendors.Single(x => x.ID == e.Entity.ID));
                    session.Vendors.Insert(indx, e.Entity);
                }
                else if (e.ChangeType == ChangeType.Delete)
                {
                    session.Vendors.Remove(session.Vendors.Single(x=> x.ID == e.Entity.ID));
                }
            }));
        }
        public class VendorsOnly : ITableDependencyFilter
        {
            public string Translate()
            {
                return "[IsCustomer] = 0";    //Select * from CustomerAndVendors where IsCustomer = 0
            }
        }

        public static SqlTableDependency<CustomerAndVendors> CustomerWatcher;   //To watch Changed in DB for (CustomerAndVendors)
        public static void Customer_Changed(object s, RecordChangedEventArgs<CustomerAndVendors> e)
        {
            Application.OpenForms[0].Invoke(new Action( ()=>
            {
                if (e.ChangeType == ChangeType.Insert)
                {
                    session.Customers.Add(e.Entity);
                }
                else if (e.ChangeType == ChangeType.Update)
                {
                    var indx = session.Customers.IndexOf(session.Customers.Single(x => x.ID == e.Entity.ID && x.IsCustomer == e.Entity.IsCustomer));
                    session.Customers.Remove(session.Customers.Single(x => x.ID == e.Entity.ID));
                    session.Customers.Insert(indx, e.Entity);
                }
                else if (e.ChangeType == ChangeType.Delete)
                {
                    session.Customers.Remove(session.Customers.Single(x => x.ID == e.Entity.ID));
                }
            }));
        }
        public class CustomersOnly : ITableDependencyFilter
        {
            public string Translate()
            {
                return "[IsCustomer] = 1";    //Select * from CustomerAndVendors where IsCustomer = 1
            }
        }
        //---------------------------------------------------
        public static SqlTableDependency<DAL.Store> StoreWatcher;
        public static void Store_Changed(object s, RecordChangedEventArgs<DAL.Store> e)
        {
            Application.OpenForms[0].Invoke(new Action( ()=>
            {
                if (e.ChangeType == ChangeType.Insert)
                {
                    session.Stores.Add(e.Entity);
                }
                else if (e.ChangeType == ChangeType.Update)
                {
                    var indx = session.Stores.IndexOf(session.Stores.Single(x => x.ID == e.Entity.ID));
                    session.Stores.Remove(session.Stores.Single(x => x.ID == e.Entity.ID));
                    session.Stores.Insert(indx, e.Entity);
                }
                else if (e.ChangeType == ChangeType.Delete)
                {
                    session.Stores.Remove(session.Stores.Single(x => x.ID == e.Entity.ID));
                }
            }));
        }
        //--------------------------------------------------------
        public class UnitNames : DAL.UnitName { }

        public static SqlTableDependency<UnitNames> UnitNameWatcher;
        public static void UnitName_Changed(object s, RecordChangedEventArgs<UnitNames> e)
        {
            frm_main.Instance.Invoke(new Action( ()=>
            {
                if (e.ChangeType == ChangeType.Insert)
                {
                    session.UnitNames.Add(e.Entity);
                }
                else if (e.ChangeType == ChangeType.Update)
                {
                    var indx = session.UnitNames.IndexOf(session.UnitNames.Single(x => x.ID == e.Entity.ID));
                    session.UnitNames.Remove(session.UnitNames.Single(x => x.ID == e.Entity.ID));
                    session.UnitNames.Insert(indx, e.Entity);
                }
                else if (e.ChangeType == ChangeType.Delete)
                {
                    session.UnitNames.Remove(session.UnitNames.Single(x => x.ID == e.Entity.ID));
                }
            }));
        }
        //------------------------------------------------------------
        public class UserSettingsProfileProperties : DAL.UserSettingsProfileProperty { }
        public static SqlTableDependency<UserSettingsProfileProperties> ProfilePropertiesWatcher;
        public static void ProfileProperties_Changed(object s, RecordChangedEventArgs<UserSettingsProfileProperties> e)
        {
            frm_main.Instance.Invoke(new Action(() =>
            {
                if(e.ChangeType == ChangeType.Insert)
                {
                    session.ProfileProperties.Add(e.Entity);
                }
                else if(e.ChangeType == ChangeType.Update)
                {
                    var index = session.ProfileProperties.IndexOf(session.ProfileProperties.Single(x => x.ID == e.Entity.ID));
                    session.ProfileProperties.Remove(session.ProfileProperties.Single(x => x.ID == e.Entity.ID));
                    session.ProfileProperties.Insert(index, e.Entity);
                }
                else if (e.ChangeType == ChangeType.Delete)
                {
                    session.ProfileProperties.Remove(session.ProfileProperties.Single(x => x.ID == e.Entity.ID));
                }
            }));
        }
        //-------------------------------------------------------------------------------------------------------------

        public static SqlTableDependency<StoreLog> StoreLogWatcher;
        public static void StoreLog_Changed(object s, RecordChangedEventArgs<StoreLog> e)
        {
            frm_main.Instance.Invoke(new Action(() =>
            {
                //----------------Only Update----------------------
                var balance = MasterInventory.GetProductBalanceInStore(e.Entity.ProductID, e.Entity.storeID);
                session.ProductsBalance.Remove(session.ProductsBalance.FirstOrDefault(x => x.ProductID == e.Entity.ProductID &&
                                                x.StoreID == e.Entity.storeID));
                session.ProductsBalance.Add(new session.ProductBalance
                {
                    Balance = balance,
                    ProductID = e.Entity.ProductID,
                    StoreID = e.Entity.storeID
                });
            }));
        }
    }
}
