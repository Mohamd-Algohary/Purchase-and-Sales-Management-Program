using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraLayout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Selling.Classes.master;

namespace Selling.UserControl
{
    public class CostDistributionOption : XtraUserControl
    {
        public RadioGroup group = new RadioGroup();
        public CostDistributionOptions selectedOption { get
            {
                if (group.EditValue != null)
                {
                    return ((CostDistributionOptions)group.EditValue);
                }
                else return CostDistributionOptions.ByQyt;
            }}
        public CostDistributionOption()
        {
            LayoutControl lc = new LayoutControl();
            lc.Dock = DockStyle.Fill;
            lc.AddItem("Please select the method of distribution of expenses on products", group).TextLocation = Locations.Top;
          
            group.Properties.Items.AddRange(new RadioGroupItem[] 
            {
                new RadioGroupItem(CostDistributionOptions.ByPrice,"By Price"),
                new RadioGroupItem(CostDistributionOptions.ByQyt, "By Quantity")
            });
            group.SelectedIndex = 1;
            this.Dock = DockStyle.Fill;
            this.RightToLeft = RightToLeft.No;
            this.Controls.Add(lc);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // CostDistributionOption
            // 
            this.Name = "CostDistributionOption";
            this.Size = new System.Drawing.Size(273, 221);
            this.ResumeLayout(false);

        }
    }
}
