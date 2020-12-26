using DevExpress.Pdf.Native;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Formatters.Binary;

namespace Selling.Classes
{
    public static class master
    {
        public class ValueAndID
        {
            public int ID { get; set; }
            public string Name { get; set; }
        }
        public enum ProductType
        {
            Inventory = 1,
            Service
        };
        public static List<ValueAndID> PruductTypeList = new List<ValueAndID>() {
                new ValueAndID() { ID = 0, Name = "Inventory" },
                new ValueAndID(){ ID = 1, Name ="Service" } };

        public enum PartType
        {
            Vendor = 1, 
            Customer
        };

        public static List<ValueAndID> PartTypeList = new List<ValueAndID>() {
            new ValueAndID(){ ID =0, Name = "Vendor" },
            new ValueAndID() { ID= 1, Name = "Customer"} };

        public static void InitializeData(this LookUpEdit lkp, object DataSource, string DisplayMember, string ValueMember) //use (this) for extention
        {
            lkp.Properties.DataSource = DataSource;
            lkp.Properties.DisplayMember = DisplayMember;
            lkp.Properties.ValueMember = ValueMember;
            lkp.Properties.PopulateColumns();
            //  lkp.Properties.Columns[ValueMember].Visible = false;
        }
        public static void InitializeData(this LookUpEdit lkp, object DataSource)  //use (this) for extention
        {
            lkp.InitializeData(DataSource, "Name", "ID");
        }
        public static void InitializeData(this GridLookUpEdit lkp, object DataSource, string DisplayMember, string ValueMember) //use (this) for extention
        {
            lkp.Properties.DataSource = DataSource;
            lkp.Properties.DisplayMember = DisplayMember;
            lkp.Properties.ValueMember = ValueMember;
        }
        public static void InitializeData(this GridLookUpEdit lkp, object DataSource)  //use (this) for extention
        {
            lkp.InitializeData(DataSource, "Name", "ID");
        }
        //---------Validtion-----------------
        public static bool IsTextValid(this TextEdit txt)
        {
            if (txt.Text.Trim() == string.Empty)
            {
                txt.ErrorText = "This field is required";
                return false;
            }
            return true;
        }
        public static bool IsEditeValueValid(this LookUpEditBase upEdit)
        {
            if (upEdit.EditValue is int == false)
            {
                upEdit.ErrorText = "This field is required";
                return false;
            }
            return true;
        }
        public static bool IsEditeValueValidAndNotZero(this LookUpEditBase Edit)
        {
            if (Edit.EditValue is int == false || Convert.ToInt32(Edit.EditValue) == 0)
            {
                Edit.ErrorText = "This field is required";
                return false;
            }
            return true;
        }
        public static bool IsDateValid(this DateEdit dt)
        {
            if (dt.DateTime.Year < 1950)
            {
                dt.ErrorText = "This field is required";
                return false;
            }
            return true;
        }
        //----------------------------------
        public enum InvoiceType
        {
            Purchase = SourceTypes.Purchase,
            Sales = SourceTypes.Sales,
            PurchaseReturn = SourceTypes.PurchaseReturn,
            SalesReturn = SourceTypes.SalesReturn
        };
        public enum SourceTypes
        {
            Purchase,
            Sales,
            PurchaseReturn,
            SalesReturn
        };
        public static List<ValueAndID> InvoiceTypeLiist = new List<ValueAndID>()
        {
            new ValueAndID(){ID =(int)InvoiceType.Purchase, Name = "Purchase" },
            new ValueAndID(){ID =(int)InvoiceType.Sales, Name = "Sales" },
            new ValueAndID(){ID =(int)InvoiceType.PurchaseReturn, Name = "Purchase Return" },
            new ValueAndID(){ID =(int)InvoiceType.SalesReturn, Name = "Sales Return" }};
        //----------------------------------------
        public static void InitializeData(this RepositoryItemLookUpEditBase repo, object DataSource, GridColumn column, GridControl grid, string DisplayMember, string ValueMember) //use (this) for extention
        {
            repo.NullText = "";
            repo.DataSource = DataSource;
            repo.DisplayMember = DisplayMember;
            repo.ValueMember = ValueMember;
            column.ColumnEdit = repo;
            if (grid != null)
            {
                grid.RepositoryItems.Add(repo);
            }
        }
        public static void InitializeData(this RepositoryItemLookUpEditBase repo, object DataSource, GridColumn column, GridControl grid)  //use (this) for extention
        {
            repo.InitializeData(DataSource, column, grid, "Name", "ID");
        }
        //---------------------------------------------
        public static string GetNextCodeInstring(string maxcode)
        {
            if (maxcode == string.Empty || maxcode == null) return "1";
            string str1 = "";
            foreach (char c in maxcode)
            {
                str1 = char.IsDigit(c) ? str1 = c.ToString() : "";
            }
            if (str1 == string.Empty) return maxcode + "1";

            //PRD01 --->str1=01 --->101--->102 --->PRD02
            //PRD99 --->str1=99 --->199--->200 --->PRD100
            string str2 = str1.Insert(0, "1");
            str2 = (Convert.ToInt32(str2) + 1).ToString();
            string str3 = str2[0] == '1' ? str2.Remove(0, 1) : str2.Remove(0, 1).Insert(0, "1");

            int indx = maxcode.LastIndexOf(str1);
            maxcode = maxcode.Remove(indx);   //Remove from this indix to end of string
            maxcode = maxcode.Insert(indx, str3);
            return maxcode;
        }
        //----------------------------------------------------------
        public enum CostDistributionOptions
        {
            ByPrice,
            ByQyt
        }
        //---------------------------------------------------------
        public static int findRowHandleByRowObject(this GridView view, object row)
        {
            if (row != null)
            {
                for (int i = 0; i < view.DataRowCount; i++)
                {
                    if (row.Equals(view.GetRow(i)))
                        return i;
                }
            }
            return GridControl.InvalidRowHandle;
        }
        //------------------------------------------------------
        public enum PrintMode
        {
            Direct,
            ShowPreview,
            ShowDialog
        }
        //-------------------------------------------------------
        public static List<ValueAndID> PayMethodsList = new List<ValueAndID>()
        {
            new ValueAndID(){ID=(int)PayMethods.Cash, Name ="Cash" },
            new ValueAndID(){ID=(int)PayMethods.Credit, Name ="Credit" }
        };
        public enum PayMethods
        {
            Cash,
            Credit
        }

        public static List<ValueAndID> WarningLevelList = new List<ValueAndID>()
        {
            new ValueAndID(){ID=(int)WarningLevel.DoNotInterrupt, Name ="Do not interrupt" },
            new ValueAndID(){ID=(int)WarningLevel.ShowWarning, Name ="Warning" },
            new ValueAndID(){ID=(int)WarningLevel.Prevent, Name ="Prevent" }
        };
        public enum WarningLevel
        {
            DoNotInterrupt,
            ShowWarning,
            Prevent
        }
        //----------------------------------------------------------------
        public static T FromByteArray<T>(byte[] data)       // T is a Generic Type Paramter Such as int-string-float......
        {
            if (data == null) return default(T);
            BinaryFormatter formatter = new BinaryFormatter();
            using (MemoryStream stream = new MemoryStream(data))
            {
                return (T)formatter.Deserialize(stream);
            }
        }
        public static byte[] ToByteArray<T>(T obj)
        {
            if (obj == null) return null;
            BinaryFormatter formatter = new BinaryFormatter();
            using (MemoryStream stream = new MemoryStream())
            { 
                formatter.Serialize(stream, obj);
                return stream.ToArray();
            }
        }
        public static byte[] GetPropertyValue(string propertyName, int profileID)
        {
            using (var db = new DAL.dbDataContext())
            {
                var prop = db.UserSettingsProfileProperties.SingleOrDefault(x => x.ProfileID == profileID
                                                                            && x.PropertyName == propertyName);
                if (prop == null) return null;
                return prop.PropertyValue.ToArray();
            }
        }
        public static string GetCallerName([CallerMemberName] string CallerName = "")
        {
            return CallerName;
        }
        //------------------------------------------------------------------------
        public enum Actions
        {
            Show =1,
            Open,
            Add,
            Edit,
            Delete,
            Print
        }
        //-----------------------------------------------------------------------
        public static List<ValueAndID> UserTypeList = new List<ValueAndID>()
        {
            new ValueAndID(){ID=(int)UserType.Admin, Name ="Admin" },
            new ValueAndID(){ID=(int)UserType.User, Name ="User" }
        };
        public enum UserType
        {
            Admin,
            User
        }
    }
}
