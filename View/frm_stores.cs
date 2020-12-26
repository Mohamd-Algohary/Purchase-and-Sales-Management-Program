using DevExpress.XtraEditors;
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

namespace Selling.Forms
{
    public partial class frm_stores : frm_master
    {
        public DAL.Store st;
        public frm_stores()
        {
            InitializeComponent();
            New();
        }
        public frm_stores(int id)
        {
            InitializeComponent();
            var db = new dbDataContext();
            st = db.Stores.Where(s => s.ID == id).First();
            Getdata();
        }
        public override void Save()
        {
            if (txt_name.Text.Trim() == string.Empty)
            {
                txt_name.ErrorText = "Enter the Store Name";
                return;
            }

            DAL.Account SalesAccount = new Account();
            DAL.Account SalesReturnAccount = new Account();
            DAL.Account InventoryAccount = new Account();
            DAL.Account CostOfSoldAccount = new Account();
           
            var db = new DAL.dbDataContext();
            if (st.ID == 0)
            {
                db.Stores.InsertOnSubmit(st);  //لو جديد(خد بالك احنا بنعمل لكل مخزن كائن جديد ف في كل أضافة جديد يكون الايدي=0)0  
                db.Accounts.InsertOnSubmit(SalesAccount);
                db.Accounts.InsertOnSubmit(SalesReturnAccount);
                db.Accounts.InsertOnSubmit(InventoryAccount);
                db.Accounts.InsertOnSubmit(CostOfSoldAccount);

                st.DiscountAllowedAccountID = session.Defualt.DiscountAllowedAccount;
                st.DiscountReceivedAccountID = session.Defualt.DiscountReceivedAccount;
            }
            else
            {
                db.Stores.Attach(st);  //لو هتعدل(خد بالك انت بعد مضفت الأسم الجديد الرابط مع قاعدة البيانات اتقطع فبتحتاج تعمل ربط من جديد 
                SalesAccount = db.Accounts.Single(x => x.ID == st.SalesAccountID);
                SalesReturnAccount = db.Accounts.Single(x => x.ID == st.SalesReturnAccountID);
                InventoryAccount = db.Accounts.Single(x => x.ID == st.InventoryAccountID);
                CostOfSoldAccount = db.Accounts.Single(x => x.ID == st.CostOfSoldAccountID);
            }    

            Setdata();     // لو الشرط الأول اتحقق وتم اضافة اسم جديد هيتغير الأيدي بتاعو من صفر ل الأيدي اللي علية الدور في الجدول
            SalesAccount.Name = st.Name + "-Sales";
            SalesReturnAccount.Name = st.Name + "-Sales Return";
            InventoryAccount.Name = st.Name + "-Inventory";
            CostOfSoldAccount.Name = st.Name + "-Cost of Sold Goods";
            db.SubmitChanges();
            st.SalesAccountID = SalesAccount.ID;
            st.SalesReturnAccountID = SalesReturnAccount.ID;
            st.InventoryAccountID = InventoryAccount.ID;
            st.CostOfSoldAccountID = CostOfSoldAccount.ID;
            db.SubmitChanges();
            base.Save();
        }
        public override void New()
        {
            st = new DAL.Store();    // كل اسم مخزن بيتعملو كائن جديد
            base.New();        // هيبقي فاضي حاليا لاننا عملنا كائن جديد
        }
        public override void Delete()
        {
            if (XtraMessageBox.Show("Are you sure from delete this item?", "Delete message", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                var db = new dbDataContext();
                var log = db.StoreLogs.Where(x => x.storeID == st.ID).Count();
                var accountlog = db.Journals.Where(x => x.AccountID == st.SalesAccountID || x.AccountID == st.SalesReturnAccountID ||
                       x.AccountID == st.InventoryAccountID || x.AccountID == st.CostOfSoldAccountID).Count();
                if(log + accountlog > 0)
                {
                    XtraMessageBox.Show(text: "Sorry,you Can not delete this store because its used in system", caption: "Error Message", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                db.Stores.Attach(st);
                db.Stores.DeleteOnSubmit(st);
                db.Accounts.DeleteAllOnSubmit(db.Accounts.Where(x => x.ID == st.SalesAccountID || x.ID == st.SalesReturnAccountID ||
                       x.ID == st.InventoryAccountID || x.ID == st.CostOfSoldAccountID));
                db.SubmitChanges();
                base.Delete();
                New();
            }
        }
        public override void Getdata()
        {
            txt_name.Text = st.Name;     //to empty the textbox.
            base.Getdata();
        }
        public override void Setdata()
        {
            st.Name = txt_name.Text;
            base.Setdata();

        }
       
    }
}
