using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Selling.Classes;

namespace Selling.Classes
{
    public class UserSettingsTemplate
    {
        private int ProfileID { get; set; }
        public GeneralSettings General { get; set; }
        public InvoicesSettings Invoices { get; set; }
        public SalesSettings Sales { get; set; }
        public PurchaseSettings Purchase { get; set; }
        public UserSettingsTemplate(int profilId)
        {
            this.ProfileID = profilId;
            General = new GeneralSettings(profilId);
            Invoices = new InvoicesSettings(profilId);
            Sales = new SalesSettings(profilId);
            Purchase = new PurchaseSettings(profilId);
        }
        public static string GetPropCaption(string PropName)
        {
            UserSettingsTemplate ins;
            switch (PropName)
            {
                case nameof(ins.General): 
                    return "General Settings";
                case nameof(ins.General.CanChangeCustomer):
                    return "Allow Change Customer";
                case nameof(ins.General.DefaultStore ):
                    return "Default Store";
                case nameof(ins.General.CanChangeDrawer):
                    return "Allow Change Drawer";
                case nameof(ins.General.CanChangeStore):
                    return "Allow Change Store";
                case nameof(ins.General.CanChangeVendor):
                    return "Allow Change Vendor";
                case nameof(ins.General.canViewDocumentHitory):
                    return "Allow View DocumentHitory ";
                case nameof(ins.General.DefaultCustomer):
                    return "Default Customer";
                case nameof(ins.General.DefaultDrawer):
                    return "General Settings";
                case nameof(ins.General.DefaultRawStore):
                    return "Default Drawer";
                case nameof(ins.General.DefaultVendor):
                    return "Default Vendor";

                case nameof(ins.Invoices):
                    return "Invoices Settings";
                case nameof(ins.Invoices.CanChangeTax):
                    return "Allow Change Tax";
                case nameof(ins.Invoices.CanDeleteItemInInvoices):
                    return "Allow Delete Item in Invoices ";
                case nameof(ins.Invoices.WhenSellingItemRaechedReOrderLevel):
                    return "When Selling Item Raeched ReOrder Level";
                case nameof(ins.Invoices.WhenSellingItemWithQtyMoreThanAvailableQty):
                    return "When Selling Item With Quantity More Than Available Quantity";

                case nameof(ins.Sales):
                    return "Sales Settings";
                case nameof(ins.Sales.CanChangeItemPriceInSales):
                    return "Allow Change Item Price in Sales ";
                case nameof(ins.Sales.CanChangePaidInSales):
                    return "Allow Change Paid in Sales";
                case nameof(ins.Sales.CanChangeQtyInSales):
                    return "Allow Change Quantity in Sales ";
                case nameof(ins.Sales.CanChangeSalesInvoiceDate):
                    return "Allow Change Sales Invoice Date";
                case nameof(ins.Sales.CanHideCostInsales):
                    return "Allow Hide Cost in Sales ";
                case nameof(ins.Sales.CanNotPostToStoreInSales):
                    return "Allow Not Post to Store in Sales ";
                case nameof(ins.Sales.CanSellToVendor):
                    return "Allow Sell to Vendor ";
                case nameof(ins.Sales.DefaultPayMethodInSales):
                    return "Default Pay Method in Sales";
                case nameof(ins.Sales.MaxDisCountInInvoice):
                    return "Max DisCount in Invoice";
                case nameof(ins.Sales.MaxDiscountPerItem):
                    return "Max Discount Per Item";
                case nameof(ins.Sales.WhenSellingItemWithPriceLowerThanCoastPrice):
                    return "When Selling Item With Price Lower Than Coast Price";
                case nameof(ins.Sales.WhenSellingToCustomerExceededMaxCredit):
                    return "When Selling to Customer Exceeded Max Credit";

                case nameof(ins.Purchase):
                    return "Purchase Settings";
                case nameof(ins.Purchase.CanBuyFromCustomers):
                    return "Allow Buy From Customers ";
                case nameof(ins.Purchase.CanChangeItemPriceInPurchase):
                    return "Allow Change Item Price in Purchase";
                case nameof(ins.Purchase.CanChangePurchaseInvoceDate):
                    return "Allow Change Purchase InvoiceDate ";
                default:
                    return "$" + PropName + "$";
            }
        }
        public static BaseEdit GetProprtyControl(string propName, object propertyValue)
        {
            UserSettingsTemplate ins;
            BaseEdit edit = null;
            switch (propName)
            {
                case nameof(ins.General.DefaultStore):
                case nameof(ins.General.DefaultRawStore):
                    edit = new LookUpEdit();
                    ((LookUpEdit)edit).InitializeData(session.Stores);
                    break;
                case nameof(ins.General.DefaultCustomer):
                    edit = new LookUpEdit();
                    ((LookUpEdit)edit).InitializeData(session.Customers);
                    break;
                case nameof(ins.General.DefaultDrawer):
                    edit = new LookUpEdit();
                    ((LookUpEdit)edit).InitializeData(session.Drawers);
                    break;
                case nameof(ins.General.DefaultVendor):
                    edit = new LookUpEdit();
                    ((LookUpEdit)edit).InitializeData(session.Vendors);
                    break;
                case nameof(ins.General.CanChangeCustomer):
                case nameof(ins.General.CanChangeDrawer):
                case nameof(ins.General.CanChangeStore):
                case nameof(ins.General.CanChangeVendor):
                case nameof(ins.General.canViewDocumentHitory):
                    edit = new ToggleSwitch();
                    ((ToggleSwitch)edit).Properties.OnText = "Yes";
                    ((ToggleSwitch)edit).Properties.OffText = "No";
                    break;
                
                case nameof(ins.Invoices.CanChangeTax):
                case nameof(ins.Invoices.CanDeleteItemInInvoices):
                    edit = new ToggleSwitch();
                    ((ToggleSwitch)edit).Properties.OnText = "Yes";
                    ((ToggleSwitch)edit).Properties.OffText = "No";
                    break;
                case nameof(ins.Invoices.WhenSellingItemRaechedReOrderLevel):
                    edit = new LookUpEdit();
                    ((LookUpEdit)edit).InitializeData(master.WarningLevelList);
                    break;
                case nameof(ins.Invoices.WhenSellingItemWithQtyMoreThanAvailableQty):
                    edit = new LookUpEdit();
                    ((LookUpEdit)edit).InitializeData(master.WarningLevelList);
                    break;

                case nameof(ins.Sales.CanChangeItemPriceInSales):
                case nameof(ins.Sales.CanChangePaidInSales):
                case nameof(ins.Sales.CanChangeQtyInSales):
                case nameof(ins.Sales.CanChangeSalesInvoiceDate):
                case nameof(ins.Sales.CanHideCostInsales):
                case nameof(ins.Sales.CanNotPostToStoreInSales):
                case nameof(ins.Sales.CanSellToVendor):
                    edit = new ToggleSwitch();
                    ((ToggleSwitch)edit).Properties.OnText = "Yes";
                    ((ToggleSwitch)edit).Properties.OffText = "No";
                    break;
                case nameof(ins.Sales.DefaultPayMethodInSales):
                    edit = new LookUpEdit();
                    ((LookUpEdit)edit).InitializeData(master.PayMethodsList);                                            
                    break;
                case nameof(ins.Sales.WhenSellingItemWithPriceLowerThanCoastPrice):
                    edit = new LookUpEdit();
                    ((LookUpEdit)edit).InitializeData(master.WarningLevelList);
                    break;
                case nameof(ins.Sales.WhenSellingToCustomerExceededMaxCredit):
                    edit = new LookUpEdit();
                    ((LookUpEdit)edit).InitializeData(master.WarningLevelList);
                    break;
                case nameof(ins.Sales.MaxDisCountInInvoice):
                    edit = new SpinEdit();
                    ((SpinEdit)edit).Properties.Increment = 0.01M;
                    ((SpinEdit)edit).Properties.Mask.EditMask = "p";
                    ((SpinEdit)edit).Properties.Mask.UseMaskAsDisplayFormat = true;
                    ((SpinEdit)edit).Properties.MaxValue = 1;

                    break;
                case nameof(ins.Sales.MaxDiscountPerItem):
                    edit = new SpinEdit();
                    ((SpinEdit)edit).Properties.Increment = 0.01M;
                    ((SpinEdit)edit).Properties.Mask.EditMask = "p";
                    ((SpinEdit)edit).Properties.Mask.UseMaskAsDisplayFormat = true;
                    ((SpinEdit)edit).Properties.MaxValue = 1;
                    break;

                case nameof(ins.Purchase.CanBuyFromCustomers):
                case nameof(ins.Purchase.CanChangeItemPriceInPurchase):
                case nameof(ins.Purchase.CanChangePurchaseInvoceDate):
                    edit = new ToggleSwitch();
                    ((ToggleSwitch)edit).Properties.OnText = "Yes";
                    ((ToggleSwitch)edit).Properties.OffText = "No";
                    break;
                
                default:
                    break;
            }
            if (edit != null)
            {
                edit.Name = propName;
                edit.Properties.NullText = "";
                edit.EditValue = propertyValue;
            }
            return edit;
        }
    }
    public class GeneralSettings
    {
        private int ProfileID { get; set; }
        public GeneralSettings(int profilId)
        {
            this.ProfileID = profilId;
        }
        public int DefaultStore   { get { return master.FromByteArray<int>(master.GetPropertyValue(master.GetCallerName(), ProfileID)); } }
        public int DefaultRawStore   { get { return master.FromByteArray<int>(master.GetPropertyValue(master.GetCallerName(), ProfileID)); } }
        public int DefaultDrawer   { get { return master.FromByteArray<int>(master.GetPropertyValue(master.GetCallerName(), ProfileID)); } }
        public int DefaultCustomer   { get { return master.FromByteArray<int>(master.GetPropertyValue(master.GetCallerName(), ProfileID)); } }
        public int DefaultVendor   { get { return master.FromByteArray<int>(master.GetPropertyValue(master.GetCallerName(), ProfileID)); } }
        public bool CanChangeStore { get { return master.FromByteArray<bool>(master.GetPropertyValue(master.GetCallerName(), ProfileID)); } }
        public bool CanChangeDrawer   { get { return master.FromByteArray<bool>(master.GetPropertyValue(master.GetCallerName(), ProfileID)); } }
        public bool CanChangeCustomer   { get { return master.FromByteArray<bool>(master.GetPropertyValue(master.GetCallerName(), ProfileID)); } }
        public bool CanChangeVendor   { get { return master.FromByteArray<bool>(master.GetPropertyValue(master.GetCallerName(), ProfileID)); } }
        public bool canViewDocumentHitory   { get { return master.FromByteArray<bool>(master.GetPropertyValue(master.GetCallerName(), ProfileID)); } }
    }
    public class InvoicesSettings
    {
        private int ProfileID { get; set; }
        public InvoicesSettings(int profilId)
        {
            this.ProfileID = profilId;
        }
        public bool CanDeleteItemInInvoices   { get { return master.FromByteArray<bool>(master.GetPropertyValue(master.GetCallerName(), ProfileID)); } }
        public bool CanChangeTax   { get { return master.FromByteArray<bool>(master.GetPropertyValue(master.GetCallerName(), ProfileID)); } }
        public master.WarningLevel WhenSellingItemRaechedReOrderLevel { get { return master.FromByteArray<master.WarningLevel>(master.GetPropertyValue(master.GetCallerName(), ProfileID)); } }
        public master.WarningLevel WhenSellingItemWithQtyMoreThanAvailableQty { get { return master.FromByteArray<master.WarningLevel>(master.GetPropertyValue(master.GetCallerName(), ProfileID)); } }

    }
    public class SalesSettings
    {
        private int ProfileID { get; set; }
        public SalesSettings(int profilId)
        {
            this.ProfileID = profilId;
        }
        public master.PayMethods DefaultPayMethodInSales { get { return master.FromByteArray<master.PayMethods>(master.GetPropertyValue(master.GetCallerName(), ProfileID)); } }
        public master.WarningLevel WhenSellingToCustomerExceededMaxCredit   { get { return master.FromByteArray<master.WarningLevel>(master.GetPropertyValue(master.GetCallerName(), ProfileID)); } }
        public master.WarningLevel WhenSellingItemWithPriceLowerThanCoastPrice   { get { return master.FromByteArray<master.WarningLevel>(master.GetPropertyValue(master.GetCallerName(), ProfileID)); } }
        public decimal MaxDisCountInInvoice   { get { return master.FromByteArray<decimal>(master.GetPropertyValue(master.GetCallerName(), ProfileID)); } }
        public decimal MaxDiscountPerItem   { get { return master.FromByteArray<decimal>(master.GetPropertyValue(master.GetCallerName(), ProfileID)); } }
        public bool CanChangePaidInSales   { get { return master.FromByteArray<bool>(master.GetPropertyValue(master.GetCallerName(), ProfileID)); } }
        public bool CanNotPostToStoreInSales   { get { return master.FromByteArray<bool>(master.GetPropertyValue(master.GetCallerName(), ProfileID)); } }
        public bool CanChangeItemPriceInSales   { get { return master.FromByteArray<bool>(master.GetPropertyValue(master.GetCallerName(), ProfileID)); } }
        public bool CanSellToVendor   { get { return master.FromByteArray<bool>(master.GetPropertyValue(master.GetCallerName(), ProfileID)); } }
        public bool CanChangeSalesInvoiceDate   { get { return master.FromByteArray<bool>(master.GetPropertyValue(master.GetCallerName(), ProfileID)); } }
        public bool CanChangeQtyInSales   { get { return master.FromByteArray<bool>(master.GetPropertyValue(master.GetCallerName(), ProfileID)); } }
        public bool CanHideCostInsales   { get { return master.FromByteArray<bool>(master.GetPropertyValue(master.GetCallerName(), ProfileID)); } }

    } 
    public class PurchaseSettings
    {
        private int ProfileID { get; set; }
        public PurchaseSettings(int profilId)
        {
            this.ProfileID = profilId;
        }
        public bool CanChangeItemPriceInPurchase { get; set; }
        public bool CanBuyFromCustomers { get; set; }
        public bool CanChangePurchaseInvoceDate { get; set; }
    }
}
