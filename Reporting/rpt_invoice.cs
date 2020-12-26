using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using Selling.Classes;

namespace Selling.Reporting
{
    public partial class rpt_invoice : DevExpress.XtraReports.UI.XtraReport
    {
        public rpt_invoice()
        {
            InitializeComponent();
            lbl_companyName.Text = session.CompanyInfo.CompanyName;
            lbl_companyadress.Text = session.CompanyInfo.Address;
            lbl_companyPhone.Text = session.CompanyInfo.Phone;
        }
        private void Bind_Data()
        {
            lbl_invoiceType.DataBindings.Add("Text", this.DataSource, "InvoiceType");
            lbl_code.DataBindings.Add("Text", this.DataSource, "Code");
            xrBarCode1.DataBindings.Add("Text", this.DataSource, "ID");
            lbl_date.DataBindings.Add("Text", this.DataSource, "Date");
            lbl_partType.DataBindings.Add("Text", this.DataSource, "PartType");
            lbl_partName.DataBindings.Add("Text", this.DataSource, "PartName");
            lbl_phone.DataBindings.Add("Text", this.DataSource, "Phone");
            lbl_branch.DataBindings.Add("Text", this.DataSource, "Store");
            lbl_Qty.DataBindings.Add("Text", this.DataSource, "ProductCount");
            lbl_total.DataBindings.Add("Text", this.DataSource, "Total");
            lbl_tax.DataBindings.Add("Text", this.DataSource, "TaxValue");
            lbl_Discount.DataBindings.Add("Text", this.DataSource, "DiscountValue");
            lbl_Expences.DataBindings.Add("Text", this.DataSource, "Expences");
            lbl_Net.DataBindings.Add("Text", this.DataSource, "Net");
            lbl_Paid.DataBindings.Add("Text", this.DataSource, "Paid");
            lbl_remaining.DataBindings.Add("Text", this.DataSource, "Remaing");
 
            //cell_index.ExpressionBindings.Add(new ExpressionBinding("BeforePrint", "Text", "ProductName"));
            cell_product.ExpressionBindings.Add(new ExpressionBinding("BeforePrint", "Text", "ProductName"));     // By (ExpressionBindings) we can do mathematical operation such(ItemQty * Price )
            cell_unit.ExpressionBindings.Add(new ExpressionBinding("BeforePrint", "Text", "UnitName"));
            cell_number.ExpressionBindings.Add(new ExpressionBinding("BeforePrint", "Text", "ItemQty"));
            cell_price.ExpressionBindings.Add(new ExpressionBinding("BeforePrint", "Text", "Price"));
            cell_total.ExpressionBindings.Add(new ExpressionBinding("BeforePrint", "Text", "TotalPrice"));
        }
        public static void printReport(object ds)
        {
            Reporting.rpt_invoice rpt = new rpt_invoice();
            rpt.DataSource = ds;
            rpt.DetailReport.DataSource = ds;
            rpt.DetailReport.DataMember = "Products";
            rpt.Bind_Data();

            switch (session.CurrentUserSettings.InvoicePrintMode)
            {
                case master.PrintMode.Direct:
                    rpt.Print();
                    break;
                case master.PrintMode.ShowPreview:
                    rpt.ShowPreview();
                    break;
                case master.PrintMode.ShowDialog:
                    rpt.PrintDialog();
                    break;
                default:
                    break;
            }
        }
        private void lbl_netText_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            lbl_netText.Text = ConvertNumberToText.CNTT.ConvertMoneyToArabicText(lbl_Net.Text.ToString());
        }
        int index = 1;
        private void cell_index_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            cell_index.Text = (index++).ToString();
        }
    }
}
