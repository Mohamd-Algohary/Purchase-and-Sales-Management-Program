using Selling.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Selling.Classes.master;

namespace Selling.Model
{
    public static class MasterInventory
    {
        public enum CostCalculationMethod
        {
            FIFO = 1,
            LIFO,
            WAVP
        };
        public static List<ValueAndID> CostCalculationMethodList = new List<ValueAndID>() {
                new ValueAndID() { ID=(int)CostCalculationMethod.FIFO, Name = "First in first out" },
                new ValueAndID(){ ID=(int)CostCalculationMethod.LIFO, Name ="Last in first out" },
                new ValueAndID(){ ID=(int)CostCalculationMethod.WAVP, Name ="Weight average price" }
        };

        public static double GetCostOfNextProduct(int ProductID, int StoreID, double Qty)
        {
            using(var db = new DAL.dbDataContext())
            {
                var query = db.StoreLogs.Where(s => s.ProductID == ProductID && s.storeID == StoreID).OrderBy(q => q.InsertTime);
                double TotalQtyIn = query.Where(q => q.IsInTransaction == true).Sum(q => (double?)q.Qty) ?? 0;
                double TotalQtyOut = query.Where(q => q.IsInTransaction == false).Sum(q => (double?)q.Qty) ?? 0;
                double Balance = TotalQtyIn - TotalQtyOut;

                //تقوم بجمع الكميات الداخلة تنازليا بشكل تراكمي وعند الوصول إلي الصف الذي تكون فية الكمية أكبر من الكمية الخارجة  كلها يقوم بجلب جميع الصفوت الداخلة من أول هذا الصف إلي نهاية الجدول
                var subquery = query.Where(q => query.Where(u => u.IsInTransaction == true && u.InsertTime <= q.InsertTime)
                              .Sum(u => u.Qty) > TotalQtyOut && q.IsInTransaction == true).ToList();

                var subqueryBalance = subquery.Where(q => q.IsInTransaction == true).Sum(q => q.Qty); 
                if (subquery == null) return 0;

                /*
                 *----> Console.Write(query);
                 * ProductID StoreID Qty IsInTransaction CostValue
                 *    44      1019   24       True         10          24
                 *    44      1019   12       False        12           -
                 *    44      1019   60       True         12          84      >   72
                 *    44      1019   60       True         12 
                 *    44      1019   60       True         12 
                 *    44      1019   60       False        14
                 *    
                 *----> Console.Write(subquery);
                 * ProductID StoreID Qty IsInTransaction CostValue
                 *    44       1019  60     True             12
                 *    44       1019  60     True             12 
                 *    44       1019  60     True             12 
                 *    
                 *    
                 *  Console.WriteLine(TotalQtyIn); --->204
                 *  Console.WriteLine(TotalQtyOut); --->72
                 *  Console.WriteLine(Balance); ------>132
                 *  Console.WriteLine(subqueryBalance); ---->180
                 */

                if (subqueryBalance > Balance)       
                {
                    var diff = subqueryBalance - Balance;       // 180 - 132 = 48
                    subquery[0].Qty -= diff;                    // 60 - 48 = 12
                }                                               //  أصبحت صافي الكميات الداخلة (subquery)كدا اكميات في 

                //المنتج ممكن كل مرة يكون تكلفة الشراء بتاعو مختلف نتيجة الزيادة في تكاليف النقل مثلا لذلك
                double fifo;         //المنتج الداخل أولا يباع أولا بالتكلفة اللي داخل بيها
                double Lifo;         // المنتج الداخل أخرا يباع أولا بالتكلفة اللي داخل بيها
                double WAC;          //متوسط التكلفة لكل الداخل من المنتج

                if (subquery[0].Qty < Qty)     //If the (required Qty) > the Qty in the first row
                {
                    int i = 0;
                    var qtyX = Qty;
                    double SumPrice = 0;
                    while(qtyX > 0)
                    {
                        var row = subquery[i];
                        double qtyZ = (qtyX > row.Qty) ? row.Qty : qtyX;
                        SumPrice += qtyZ * row.CostValue;
                        qtyX = qtyZ;
                        i++;
                    }
                    fifo = SumPrice / Qty;
                }
                else
                {
                    fifo = subquery.First().CostValue;
                }

                subquery = subquery.OrderByDescending(q => q.InsertTime).ToList();
                if (subquery[0].Qty < Qty)
                {
                    int i = 0;
                    var qtyX = Qty;
                    double SumPrice = 0;
                    while (qtyX > 0)
                    {
                        var row = subquery[i];
                        double qtyZ = (qtyX > row.Qty) ? row.Qty : qtyX;
                        SumPrice += qtyZ * row.CostValue;
                        qtyX = qtyZ;
                        i++;
                    }
                    Lifo = SumPrice / Qty;
                }
                else
                {
                    Lifo = subquery.First().CostValue;
                }

                WAC = subquery.Select(q => q.CostValue * q.Qty).Sum() / Balance;

                var method = (CostCalculationMethod)session.Products.Single(u => u.ID == ProductID).CostCalculationMethod;
                switch (method)
                {
                    case CostCalculationMethod.FIFO: return fifo;     
                    case CostCalculationMethod.LIFO: return Lifo; 
                    case CostCalculationMethod.WAVP: return WAC;  
                    default: return fifo;
                }
            }
        }
        public static double GetProductBalanceInStore(int ProductID, int StoreID)
        {
            using (var db = new DAL.dbDataContext())
            {
                var query = db.StoreLogs.Where(s => s.ProductID == ProductID && s.storeID == StoreID);
                double TotalQtyIn = query.Where(q => q.IsInTransaction == true).Sum(q => (double?)q.Qty) ?? 0;
                double TotalQtyOut = query.Where(q => q.IsInTransaction == false).Sum(q => (double?)q.Qty) ?? 0;
                double Balance = TotalQtyIn - TotalQtyOut;
                return Balance;
            }
        }
    }
}
