using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using Selling.Classes;
using Selling.DAL;
using Selling.Model;
using Selling.UserControl;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using static Selling.Classes.master;

namespace Selling.Forms
{
    public partial class frm_Invoice : frm_master
    {
        DAL.invoiceHeader invoice;
        DAL.dbDataContext generalDB;
        master.InvoiceType type;
        RepositoryItemGridLookUpEdit repoItem;
        RepositoryItemLookUpEdit repoItemAll;
        RepositoryItemLookUpEdit repoUOM;
        RepositoryItemLookUpEdit repoStore;
        RepositoryItemSpinEdit spnQty;
        public frm_Invoice(master.InvoiceType _type)
        {
            InitializeComponent();
            lkp_PartType.EditValueChanged += Lkp_PartType_EditValueChanged;
            type = _type;
            New();
        }
        DAL.InvoiceDetaile ins = new DAL.InvoiceDetaile();
        private void frm_Invoice_Load(object sender, EventArgs e)
        {
            switch (type)
            {
                case InvoiceType.Purchase:
                    this.Text = "Purchase Invoice";
                    this.Name = Classes.Screens.addPurchaseInvoice.ScreenName;
                    chk_PostedToStore.Enabled = false;
                    chk_PostedToStore.Checked = true;
                    break;
                case InvoiceType.Sales:
                    this.Text = "Sales Invoice";
                    this.Name = Classes.Screens.addSalesInvoice.ScreenName;
                    break;
                case InvoiceType.PurchaseReturn:
                    this.Text = "Purchase Return Invoice";
                    break;
                case InvoiceType.SalesReturn:
                    this.Text = "Sales Return Invoice";
                    break;
                default:
                    throw new NotImplementedException();
            }
            but_print.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
            Refresh_Data();
            //InitializeData(lkp_PartType, master.PartTypeList);
            lkp_PartType.InitializeData(master.PartTypeList);   //extention
            lkp_PartType.Properties.PopulateColumns();
            lkp_PartType.Properties.Columns["ID"].Visible = false;
           
            Glkp_PartID.Properties.ValidateOnEnterKey = true;
            Glkp_PartID.Properties.AllowNullInput = DefaultBoolean.False;
            Glkp_PartID.Properties.BestFitMode = BestFitMode.BestFitResizePopup;
            Glkp_PartID.Properties.ImmediatePopup = true;
            var PartIDview = Glkp_PartID.Properties.View;   //GridView in lookuPEdite
            PartIDview.FocusRectStyle = DrawFocusRectStyle.RowFullFocus;
            PartIDview.OptionsSelection.UseIndicatorForSelection = true;
            PartIDview.OptionsView.ShowAutoFilterRow = true;
            PartIDview.PopulateColumns(Glkp_PartID.Properties.DataSource);
            PartIDview.Columns["ID"].Visible = false;
            //PartIDview.Columns["IsActive"].Visible = false;
            //PartIDview.Columns["Type"].Visible = false;
            Glkp_PartID.ButtonClick += Glkp_PartID_ButtonClick;        //We added button from properties in design.

            gridView1.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.Bottom;
            gridView1.Columns[nameof(ins.ID)].Visible = false;
            gridView1.Columns[nameof(ins.InvoiceID)].Visible = false;

            repoItem = new RepositoryItemGridLookUpEdit();
            repoItem.InitializeData(session.ProductsView.Where(x=> x.IsActive==true), gridView1.Columns[nameof(ins.ItemID)], gridControl1);
            repoItem.ValidateOnEnterKey = true;
            repoItem.AllowNullInput = DefaultBoolean.False;
            repoItem.BestFitMode = BestFitMode.BestFitResizePopup;
            repoItem.ImmediatePopup = true;
            repoItem.Buttons.Add(new EditorButton(ButtonPredefines.Plus));
            repoItem.ButtonClick += RepoItem_ButtonClick;
            var repoview = repoItem.View;   //GridView in lookuPEdite
            repoview.FocusRectStyle = DrawFocusRectStyle.RowFullFocus;
            repoview.OptionsSelection.UseIndicatorForSelection = true;
            repoview.OptionsView.ShowAutoFilterRow = true;
            repoview.PopulateColumns(repoItem.DataSource);
            repoview.Columns["ID"].Visible = false;
            repoview.Columns["IsActive"].Visible = false;
            repoview.Columns["Type"].Visible = false;
            
            repoItemAll = new RepositoryItemLookUpEdit();
            repoItemAll.InitializeData(session.ProductsView, gridView1.Columns[nameof(ins.ItemID)], gridControl1);

            repoUOM = new RepositoryItemLookUpEdit();
            repoUOM.InitializeData(session.UnitNames, gridView1.Columns[nameof(ins.ItemUntiID)], gridControl1);

            repoStore = new RepositoryItemLookUpEdit();
            repoStore.InitializeData(session.Stores, gridView1.Columns[nameof(ins.StoreID)], gridControl1);

            gridView1.Columns[nameof(ins.TotalPrice)].OptionsColumn.AllowFocus = false;
            RepositoryItemSpinEdit spn = new RepositoryItemSpinEdit();
            gridView1.Columns[nameof(ins.TotalPrice)].ColumnEdit = spn;
            gridView1.Columns[nameof(ins.Price)].ColumnEdit = spn;
            gridView1.Columns[nameof(ins.ItemQty)].ColumnEdit = spn;
            gridView1.Columns[nameof(ins.DiscountValue)].ColumnEdit = spn;
            gridControl1.RepositoryItems.Add(spn);

            RepositoryItemSpinEdit spnRatio = new RepositoryItemSpinEdit();
            gridView1.Columns[nameof(ins.Discount)].ColumnEdit = spnRatio;
            spnRatio.Increment = .01m;
            spnRatio.Mask.EditMask = "p";
            spnRatio.Mask.UseMaskAsDisplayFormat = true;
            spnRatio.MaxValue = 1;
            gridControl1.RepositoryItems.Add(spnRatio);

            spnQty = new RepositoryItemSpinEdit();
            gridView1.Columns[nameof(ins.ItemQty)].ColumnEdit = spnQty;
            spnQty.Mask.UseMaskAsDisplayFormat = true;
            spnQty.MinValue = 1;
            gridControl1.RepositoryItems.Add(spnRatio);

            gridView1.Columns.Add(new GridColumn() { Name = "clmcode", FieldName = "Code", Caption = "Code", UnboundType = UnboundColumnType.String });
            gridView1.Columns.Add(new GridColumn() { Name = "clmindex", FieldName = "Index", Caption = "Index", UnboundType = UnboundColumnType.Integer, MaxWidth = 30 }); 
            if(type == master.InvoiceType.Sales)
            {
               gridView1.Columns.Add(new GridColumn() { Name = "clmbalance", FieldName = "Balance", Caption = "Balance", UnboundType = UnboundColumnType.Decimal, MaxWidth = 60 });
               gridView1.Columns["Balance"].VisibleIndex = 12;
               gridView1.Columns["Balance"].OptionsColumn.AllowFocus = false;
            }
            gridView1.CustomUnboundColumnData += GridView1_CustomUnboundColumnData;

            gridView1.Columns[nameof(ins.ItemID)].Caption = "Product";
            gridView1.Columns[nameof(ins.ItemUntiID)].Caption = "Unit";
            gridView1.Columns[nameof(ins.ItemQty)].Caption = "Product Qty";
            gridView1.Columns[nameof(ins.Price)].Caption = "Price";
            gridView1.Columns[nameof(ins.TotalPrice)].Caption = "Total Price";
            gridView1.Columns[nameof(ins.CostValue)].Caption = "Cost Value";
            gridView1.Columns[nameof(ins.TotalCostValue)].Caption = "Total Cost Value";
            gridView1.Columns[nameof(ins.Discount)].Caption = "Discount Ratio";
            gridView1.Columns[nameof(ins.DiscountValue)].Caption = "Discount Value";
            gridView1.Columns[nameof(ins.StoreID)].Caption = "Store Name";
            
            gridView1.Columns["Index"].VisibleIndex =0;
            gridView1.Columns["Code"].VisibleIndex = 1;
            gridView1.Columns[nameof(ins.ItemID)].VisibleIndex =2;
            gridView1.Columns[nameof(ins.ItemUntiID)].VisibleIndex =3;
            gridView1.Columns[nameof(ins.ItemQty)].VisibleIndex =4;
            gridView1.Columns[nameof(ins.Price)].VisibleIndex =5;
            gridView1.Columns[nameof(ins.Discount)].VisibleIndex =6;
            gridView1.Columns[nameof(ins.DiscountValue)].VisibleIndex =7;
            gridView1.Columns[nameof(ins.TotalPrice)].VisibleIndex =8;
            gridView1.Columns[nameof(ins.CostValue)].VisibleIndex = 9;
            gridView1.Columns[nameof(ins.StoreID)].VisibleIndex = 11;
           

            gridView1.Columns["Index"].OptionsColumn.AllowFocus = false;
            gridView1.Columns[nameof(ins.CostValue)].OptionsColumn.AllowFocus = false;
            gridView1.Columns[nameof(ins.TotalCostValue)].OptionsColumn.AllowFocus = false;
            gridView1.Columns[nameof(ins.ItemID)].MinWidth =85 ;
            gridView1.Columns[nameof(ins.DiscountValue)].MinWidth = 85;
            gridView1.Columns[nameof(ins.TotalCostValue)].MinWidth = 90;

            gridView1.Appearance.EvenRow.BackColor = Color.FromArgb(255, 249, 196);
            gridView1.OptionsView.EnableAppearanceEvenRow = true;
            gridView1.Appearance.OddRow.BackColor = Color.WhiteSmoke;
            gridView1.OptionsView.EnableAppearanceOddRow = true;

            RepositoryItemButtonEdit buttonEdit = new RepositoryItemButtonEdit();
            gridControl1.RepositoryItems.Add(buttonEdit);
            buttonEdit.Buttons.Clear();
            buttonEdit.Buttons.Add(new EditorButton(ButtonPredefines.Delete));
            buttonEdit.TextEditStyle = TextEditStyles.HideTextEditor;
            buttonEdit.Click += ButtonEdit_Click;
            GridColumn clmnDelete = new GridColumn {
                Name = "clmndelete",
                Caption = "",
                FieldName = "Delete",
                VisibleIndex = 100,
                Width = 15,
                ColumnEdit = buttonEdit
            };
            gridView1.Columns.Add(clmnDelete);
            #region Events
            sp_DiscountValue.Enter += Sp_DiscountValue_Enter;
            sp_DiscountValue.Leave += Sp_DiscountValue_Leave;
            sp_DiscountValue.EditValueChanged += Sp_DiscountValue_EditValueChanged;
            sp_DiscountRation.EditValueChanged += Sp_DiscountValue_EditValueChanged;

            sp_TaxValue.Enter += Sp_TaxValue_Enter;
            sp_TaxValue.Leave += Sp_TaxValue_Leave;
            sp_TaxValue.EditValueChanged += Sp_TaxValue_EditValueChanged;
            sp_Tax.EditValueChanged += Sp_TaxValue_EditValueChanged;

            sp_TaxValue.EditValueChanged += Sp_EditValueChanged;
            sp_DiscountValue.EditValueChanged += Sp_EditValueChanged;
            sp_Expences.EditValueChanged += Sp_EditValueChanged;
            sp_Total.EditValueChanged += Sp_EditValueChanged;

            sp_Paid.EditValueChanged += Sp_Paid_EditValueChanged;
            sp_Net.EditValueChanged += Sp_Paid_EditValueChanged;
            sp_Net.EditValueChanging += Sp_Net_EditValueChanging;
            sp_Net.DoubleClick += Sp_Net_DoubleClick;

            gridView1.CustomRowCellEditForEditing += GridView1_CustomRowCellEditForEditing;
            gridView1.CellValueChanged += GridView1_CellValueChanged;
            gridView1.CellValueChanging += GridView1_CellValueChanging;

            gridView1.RowCountChanged += GridView1_RowCountChanged;
            gridView1.RowUpdated += GridView1_RowUpdated;

            lkp_Branch.EditValueChanging += Lkp_Branch_EditValueChanging;

            gridControl1.ProcessGridKey += GridControl1_ProcessGridKey;
            gridView1.ValidateRow += GridView1_ValidateRow;
            gridView1.InvalidRowException += GridView1_InvalidRowException;
            this.Activated += Frm_Invoice_Activated;
            #endregion
            this.KeyPreview = true;
            this.KeyDown += Frm_Invoice_KeyDown;
        }

        private void GridView1_CellValueChanging(object sender, CellValueChangedEventArgs e)
        {
            //حتي يتم تغير الوحدة عند تغير صنف مضاف مسبقا
            if(e.Column.FieldName == "ItemID")
            {
                var row = gridView1.GetRow(e.RowHandle) as DAL.InvoiceDetaile;
                if(row != null)
                {
                    if (row.ItemID != 0 && e.Value.Equals(row.ItemID) == false)
                        row.ItemUntiID = 0;
                }

            }
        }

        private void RepoItem_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            if (e.Button.Kind == ButtonPredefines.Plus)
                frm_main.openbyname(nameof(frm_Products));
        }
        private void Frm_Invoice_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F5:
                    MoveFocusToGrid(e.Modifiers == Keys.Shift);
                    break;
                case Keys.F17:
                    //Todos go to product by index
                    break;
                case Keys.F6:
                    Glkp_PartID.Focus();
                    break;
                case Keys.F8:
                    txt_Code.Focus();
                    break;
                case Keys.F9:
                    sp_DiscountValue.Focus();
                    break;
                case Keys.F10:
                    sp_TaxValue.Focus();
                    break;
                case Keys.F11:
                    sp_Expences.Focus();
                    break;
                case Keys.F12:
                    sp_Paid.Focus();
                    break;
                default:
                    break;
            }
        }

        #region GridControl
        private void GridView1_InvalidRowException(object sender, InvalidRowExceptionEventArgs e)
        {
            var row = e.Row as DAL.InvoiceDetaile;
            if(e.Row == null || row.ItemID ==0)
            {
                e.ExceptionMode = ExceptionMode.Ignore;
            }

        }
        private void GridView1_ValidateRow(object sender, ValidateRowEventArgs e)
        {
            var row = e.Row as DAL.InvoiceDetaile;
            if(row == null || row.ItemID == 0)
            {
                e.Valid = false;
                return;
            }
        }
        //-----------------to Focused on Code Column--------------------------------
        private void GridControl1_ProcessGridKey(object sender, KeyEventArgs e)
        {
            GridControl Control = sender as GridControl;
            if (Control == null) return;
            GridView view = Control.FocusedView as GridView;
            if (view == null) return;
            if (view.FocusedColumn == null) return;
            
            if (e.KeyCode == Keys.Return)   // Return == Enter
            {
                string FocusedColumns = view.FocusedColumn.FieldName;
                if (FocusedColumns == "Code" || FocusedColumns == "ItemID")
                {
                    GridControl1_ProcessGridKey(sender, new KeyEventArgs(Keys.Tab));
                }
                if(view.FocusedRowHandle < 0)
                {
                    view.AddNewRow();
                    view.FocusedColumn = view.Columns[FocusedColumns];
                }
                e.Handled = true;
            }
            else if(e.KeyCode == Keys.Tab)
            {
                view.FocusedColumn = view.VisibleColumns[view.FocusedColumn.VisibleIndex - 1];
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Delete)
            {
                if(XtraMessageBox.Show("Do you want to delete this item?","Delete Meassage", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    int id = Convert.ToInt32(gridView1.GetFocusedRowCellValue("ID"));
                    gridView1.DeleteRow(id);
                    XtraMessageBox.Show("Item was deleted succussfully", "Delete Message", MessageBoxButtons.OK, MessageBoxIcon.Information); 
                }
            }
        }
        //---------------------------------------------------------------------
        string enteredCode="";
        private void GridView1_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            if(e.Column.FieldName == "Code")
            {
                if (e.IsSetData)
                {
                    enteredCode = e.Value.ToString();
                }
                if (e.IsGetData)
                {
                    e.Value = enteredCode;
                }
            }
            else if (e.Column.FieldName == "Index")
            {
                e.Value = gridView1.GetVisibleRowHandle(e.ListSourceRowIndex) + 1;
            }
            else if (e.Column.FieldName == "Balance")
            {
                var row = e.Row as DAL.InvoiceDetaile;
                if (row == null || row.ItemID == 0 || row.StoreID == 0)
                {
                    e.Value = null;
                    return;
                }
                else 
                {
                    var balance = session.ProductsBalance.FirstOrDefault(x => x.ProductID == row.ItemID && x.StoreID == row.StoreID);
                    if (balance == null)
                    {
                        e.Value = 0;
                        spnQty.MaxValue = 0;
                        return;
                    }
                    var ProductBalance = balance.Balance;
                    var factor = session.ProductsView.Single(x => x.ID == row.ItemID).Units.Single(u => u.UnitID == row.ItemUntiID).Factor; 

                    var Balance = ProductBalance / factor;
                    e.Value = Balance;
                    spnQty.MaxValue = Convert.ToDecimal(Balance);
                    if (Balance == 0)
                    {
                      XtraMessageBox.Show("The balance of this product isn't enough");
                    }    
                    
                }
            }
        }
        private void GridView1_RowUpdated(object sender, RowObjectEventArgs e)
        {
            var item = gridView1.DataSource as Collection<DAL.InvoiceDetaile>;
            if (item == null)
            {
                sp_Total.EditValue = 0;
            }
            else
            {
                sp_Total.EditValue = item.Sum(x => x.TotalPrice);
            }
        }
        int currentRowsCounts = 1;
        private void GridView1_RowCountChanged(object sender, EventArgs e)
        {
            if (currentRowsCounts < gridView1.RowCount)
            {
                var rows = gridView1.DataSource as Collection<DAL.InvoiceDetaile>;
                var lastrow = rows.Last();
                var row = rows.FirstOrDefault(x => x.ItemID == lastrow.ItemID && x.ItemUntiID == lastrow.ItemUntiID && x != lastrow);
                if (row != null)
                {
                    row.ItemQty += lastrow.ItemQty;
                    GridView1_CellValueChanged(sender, new CellValueChangedEventArgs(gridView1.findRowHandleByRowObject(row), gridView1.Columns[nameof(row.ItemQty)], row.ItemQty));
                    rows.Remove(lastrow);
                }
                currentRowsCounts = gridView1.RowCount;   //in order to that (if) is false when row was deleted

                /*var rows = gridView1.DataSource as Collection<DAL.InvoiceDetaile>;
                var lastrow = rows.Last();
                var item = session.ProductsView.Single(x => x.ID == lastrow.ItemID);
                var unit = item.Units.Single(x => x.UnitID == lastrow.ItemUntiID);
                DAL.InvoiceDetaile currentrow;
                for (int index = 0; index < gridView1.RowCount; index++)
                {
                    currentrow = gridView1.GetRow(index) as DAL.InvoiceDetaile;
                    if (currentrow.ItemID == item.ID && currentrow.ItemUntiID == unit.UnitID)
                    {
                        currentrow.ItemQty += lastrow.ItemQty;
                        rows.Remove(lastrow);
                    }
                }*/
                
            }

            GridView1_RowUpdated(sender, null);
        }
        private void GridView1_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            var row = gridView1.GetRow(e.RowHandle) as DAL.InvoiceDetaile;
            if (row == null) return;

            session.ProductViewClass itemv = null;
            session.ProductUOMView unitv = null;
            if (e.Column.FieldName == "Code")
            {
                string itemcode = e.Value.ToString();   //20-00009-180-9 
                if (session.GlobalSettings.ReadFromScaleBarCode && itemcode.Length == session.GlobalSettings.BarCodeLength
                    && itemcode.StartsWith(session.GlobalSettings.ScaleBarCodePrefix))
                {
                    var itemcodestring = e.Value.ToString().Substring(
                        session.GlobalSettings.ScaleBarCodePrefix.Length, session.GlobalSettings.ProductCodeLength);  //00009
                    itemcode = Convert.ToInt32(itemcodestring).ToString();
                    string Readvalue = e.Value.ToString().Substring(
                        session.GlobalSettings.ScaleBarCodePrefix.Length + session.GlobalSettings.ProductCodeLength); //1809
                    if (session.GlobalSettings.IgnoreCheckDigit)
                        Readvalue = Readvalue.Remove(Readvalue.Length - 1, 1);  //180
                    double Value = Convert.ToDouble(Readvalue);                 // 180 gm OR Penny
                    Value = Value / (Math.Pow(10, session.GlobalSettings.DivideValueBy));  //0.180  Kg OR EGP

                    if (session.GlobalSettings.ReadMode == session.GlobalSettings.ReadValueMode.weight)
                    {
                        row.ItemQty = Value;
                    }
                    else if (session.GlobalSettings.ReadMode == session.GlobalSettings.ReadValueMode.price)
                    {
                        itemv = session.ProductsView.FirstOrDefault(x => x.Units.Select(u => u.BarCode).Contains(itemcode));
                        unitv = itemv.Units.First(x => x.BarCode == itemcode);
                        switch (type)
                        {
                            case InvoiceType.Purchase:
                            case InvoiceType.PurchaseReturn:
                                row.ItemQty = Value / unitv.BuyPrice;
                                break;
                            case InvoiceType.Sales:
                            case InvoiceType.SalesReturn:
                                row.ItemQty = Value / unitv.SellPrice;
                                break;
                            default:
                                break;
                        }
                    }
                }
                if (itemv == null)
                    itemv = session.ProductsView.FirstOrDefault(x => x.Units.Select(u => u.BarCode).Contains(itemcode));
                if (itemv != null)
                {
                    row.ItemID = itemv.ID;
                    GridView1_CellValueChanged(sender, new CellValueChangedEventArgs(e.RowHandle, gridView1.Columns[nameof(ins.ItemID)], row.ItemID));
                    if (unitv == null)
                        unitv = itemv.Units.First(x => x.BarCode == itemcode);
                    row.ItemUntiID = unitv.UnitID;
                    GridView1_CellValueChanged(sender, new CellValueChangedEventArgs(e.RowHandle, gridView1.Columns[nameof(ins.ItemUntiID)], row.ItemUntiID));
                    enteredCode = string.Empty;
                    return;
                }
                enteredCode = string.Empty;
            }
            //-------------------------------------------
            if (row.ItemID == 0) return;
            itemv = session.ProductsView.Single(X => X.ID == row.ItemID);
            if (row.ItemUntiID == 0)      //for just choosing item it choose UnitName
            {
                row.ItemUntiID = itemv.Units.First().UnitID;
                GridView1_CellValueChanged(sender, new CellValueChangedEventArgs(e.RowHandle, gridView1.Columns[nameof(ins.ItemUntiID)], row.ItemUntiID));
            }
            unitv = itemv.Units.Single(X => X.UnitID == row.ItemUntiID);
            switch (e.Column.FieldName)
            {
                case nameof(ins.ItemID):
                    if (row.StoreID == 0 && lkp_Branch.IsEditeValueValidAndNotZero())
                    {
                        row.StoreID = Convert.ToInt32(lkp_Branch.EditValue);
                    }
                    //GridView1_CellValueChanged(sender, new CellValueChangedEventArgs(e.RowHandle, gridView1.Columns[nameof(ins.ItemUntiID)], row.ItemUntiID));
                    break;
                case nameof(ins.ItemUntiID):
                    switch (type)
                    {
                        case InvoiceType.Purchase:
                            row.Price = unitv.BuyPrice;
                            break;
                        case InvoiceType.Sales:
                            row.Price = unitv.SellPrice;
                            break;
                        case InvoiceType.PurchaseReturn:
                            
                            break;
                        case InvoiceType.SalesReturn:
                         
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                    
                    if (row.ItemQty == 0) row.ItemQty = 1;
                    GridView1_CellValueChanged(sender, new CellValueChangedEventArgs(e.RowHandle, gridView1.Columns[nameof(ins.Price)], row.Price));
                    //TODO calculate item balance
                    break;
                case nameof(ins.Price):
                case nameof(ins.Discount):
                case nameof(ins.ItemQty):
                    //TODO يجب جلب سعر التكلفة من المخزن عند تغير كمية الصنف او الصنف نفسة
                    row.DiscountValue = row.Discount * (row.ItemQty * row.Price);
                    GridView1_CellValueChanged(sender, new CellValueChangedEventArgs(e.RowHandle, gridView1.Columns[nameof(ins.DiscountValue)], row.DiscountValue));
                    break;
                case nameof(ins.DiscountValue):
                    if (gridView1.FocusedColumn.FieldName == nameof(ins.DiscountValue))
                    {
                        row.Discount = row.DiscountValue / (row.ItemQty * row.Price);
                    }
                    row.TotalPrice = (row.ItemQty * row.Price) - row.DiscountValue;
                    switch (type)
                    {
                        case InvoiceType.Purchase:
                            row.CostValue = row.TotalPrice / row.ItemQty;
                            row.TotalCostValue = row.TotalPrice;
                            break;
                        case InvoiceType.Sales:
                            var storeId = (row.StoreID == 0) ?Convert.ToInt32(lkp_Branch.EditValue) : row.StoreID;
                            var costPerMainUnit = MasterInventory.GetCostOfNextProduct(row.ItemID, storeId, row.ItemQty);
                            row.CostValue = costPerMainUnit / unitv.Factor;
                            row.TotalCostValue = row.CostValue * row.ItemQty;
                            break;
                        case InvoiceType.PurchaseReturn:
                        case InvoiceType.SalesReturn:
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                    break;
                default:
                    break;
            }
        }
        private void GridView1_CustomRowCellEditForEditing(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            var ins = new DAL.InvoiceDetaile();
            if (e.Column.FieldName == nameof(ins.ItemUntiID))
            {
                RepositoryItemLookUpEdit repo = new RepositoryItemLookUpEdit();
                repo.NullText = "";
                e.RepositoryItem = repo;
                var row = gridView1.GetRow(e.RowHandle) as DAL.InvoiceDetaile;
                if (row == null)
                    return;
                var item = session.ProductsView.SingleOrDefault(X => X.ID == row.ItemID);
                if (item == null)
                    return;
                repo.DataSource = item.Units;
                repo.DisplayMember = "UnitName";
                repo.ValueMember = "UnitID";
                e.RepositoryItem = repo;
            }
            else if (e.Column.FieldName == nameof(ins.ItemID))
            {
                e.RepositoryItem = repoItem;
            }
        }
        #endregion
        #region SpinEditeCalculation
        //-------------discounted------------------------
        bool IsDiscountValueFocused;
        private void Sp_DiscountValue_Enter(object sender, EventArgs e)
        {
            IsDiscountValueFocused = true;
        }
        private void Sp_DiscountValue_Leave(object sender, EventArgs e)
        {
            IsDiscountValueFocused = false;
        }
        private void Sp_DiscountValue_EditValueChanged(object sender, EventArgs e)
        {
            var total = Convert.ToDouble(sp_Total.EditValue);
            var discountval = Convert.ToDouble(sp_DiscountValue.EditValue);
            var discountratio = Convert.ToDouble(sp_DiscountRation.EditValue);
            if (IsDiscountValueFocused)
            {
                sp_DiscountRation.EditValue = discountval / total;
            }
            else
            {
                sp_DiscountValue.EditValue = total * discountratio;
            }
        }
        //-------------tax------------------------
        bool IsTaxValueFocused;
        private void Sp_TaxValue_Enter(object sender, EventArgs e)
        {
            IsTaxValueFocused = true;
        }
        private void Sp_TaxValue_Leave(object sender, EventArgs e)
        {
            IsTaxValueFocused = false;
        }
        private void Sp_TaxValue_EditValueChanged(object sender, EventArgs e)
        {
            var tatal = Convert.ToDouble(sp_Total.EditValue);
            var taxvalue = Convert.ToDouble(sp_TaxValue.EditValue);
            var taxratio = Convert.ToDouble(sp_Tax.EditValue);
            if (IsTaxValueFocused)
            {
                sp_Tax.EditValue = taxvalue / tatal;
            }
            else
            {
                sp_TaxValue.EditValue = tatal * taxratio;
            }
        }
        //------------Net----------------------
        private void Sp_EditValueChanged(object sender, EventArgs e)
        {
            var tatal = Convert.ToDouble(sp_Total.EditValue);
            var taxvalue = Convert.ToDouble(sp_TaxValue.EditValue);
            var discountratio = Convert.ToDouble(sp_DiscountRation.EditValue);
            var expences = Convert.ToDouble(sp_Expences.EditValue);

            sp_Net.EditValue = tatal + taxvalue + expences - discountratio;
        }
        private void Sp_Paid_EditValueChanged(object sender, EventArgs e)
        {
            var net = Convert.ToDouble(sp_Net.EditValue);
            var paid = Convert.ToDouble(sp_Paid.EditValue);
            sp_Remaing.EditValue = net - paid;
        }
        //------------------------------------------------------
        private void Sp_Net_DoubleClick(object sender, EventArgs e)
        {
            sp_Paid.EditValue = sp_Net.EditValue;
        }
        private void Sp_Net_EditValueChanging(object sender, ChangingEventArgs e)
        {
            if (Convert.ToDouble(e.OldValue) == Convert.ToDouble(sp_Paid.EditValue))
            {
                sp_Paid.EditValue = e.NewValue;
            }
        }
        #endregion 
        private void Lkp_Branch_EditValueChanging(object sender, ChangingEventArgs e)
        {
            var item = gridView1.DataSource as Collection<DAL.InvoiceDetaile>;
            if (e.OldValue is int && e.NewValue is int)
            {
                foreach (var row in item)
                {
                    if (row.StoreID == Convert.ToInt32(e.OldValue))
                    {
                        row.StoreID = Convert.ToInt32(e.NewValue);
                    }
                }
            }
        }
        private void Lkp_PartType_EditValueChanged(object sender, EventArgs e)
        {
            if(lkp_PartType.EditValue is int || lkp_PartType.EditValue is byte)
            {
                int PartType = Convert.ToInt32(lkp_PartType.EditValue);
                if(PartType == (int)master.PartType.Customer)
                {
                    Glkp_PartID.InitializeData(session.Customers);
                }
                else if (PartType == (int)master.PartType.Vendor)
                {
                    Glkp_PartID.InitializeData(session.Vendors);
                }
            }
        }
        private void Glkp_PartID_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            if (e.Button.Kind == ButtonPredefines.Plus)  //only when click plus Button
            {
                using (var frm = new frm_CustomerAndVendor(Convert.ToInt32(lkp_PartType.EditValue) == (int)master.PartType.Customer))
                {
                    frm.ShowDialog();
                    Refresh_Data();
                }
            }

        }
        private void ButtonEdit_Click(object sender, EventArgs e)
        {
            if (XtraMessageBox.Show("Do you want to delete this item?", "Delete Meassage", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                int id = Convert.ToInt32(gridView1.GetFocusedRowCellValue("ID"));
                gridView1.DeleteRow(id);
                XtraMessageBox.Show("Item was deleted succussfully", "Delete Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void Frm_Invoice_Activated(object sender, EventArgs e)
        {
            MoveFocusToGrid();
        }
        private void MoveFocusToGrid(bool focusToItem = false)
        {
            this.gridView1.Focus();
            gridView1.FocusedColumn = focusToItem? gridView1.Columns["ItemID"] : gridView1.Columns["Code"];
            gridView1.AddNewRow();
            gridView1.UpdateCurrentRow();
        }
        private string GetNewInvoiceCode()
        {
            string Maxcode;
            using (var db = new dbDataContext())
            {
                Maxcode = db.invoiceHeaders.Where(z => z.InviceType == (int)type).Select(x => x.Code).Max();
            }
            return GetNextCodeInstring(Maxcode);
        }
        #region OverrideMethod
        public override void Refresh_Data()
        {
            lkp_Drawer.InitializeData(session.Drawers);
            lkp_Branch.InitializeData(session.Stores);
            base.Refresh_Data();
        }
        public override bool IsDataValid()
        {
            int NumberOfError = 0;
            if(gridView1.DataRowCount == 0)
            {
                NumberOfError++;
                XtraMessageBox.Show("You must enter one product At Least", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            NumberOfError += txt_Code.IsTextValid() ? 0 : 1;
            NumberOfError += lkp_PartType.IsEditeValueValid() ? 0 : 1;
            NumberOfError += lkp_Drawer.IsEditeValueValid() ? 0 : 1;
            NumberOfError += lkp_Branch.IsEditeValueValid() ? 0 : 1;
            NumberOfError += Glkp_PartID.IsEditeValueValidAndNotZero() ? 0 : 1;
            NumberOfError += dt_Date.IsDateValid() ? 0 : 1;
           
            if(chk_PostedToStore.Checked == true)
            {
                NumberOfError += dt_PostedStoreDate.IsDateValid() ? 0 : 1;
                ore.Expanded = true;
            }
            return (NumberOfError == 0);
        }
        public override void Getdata()
        {
            txt_Code.Text = invoice.Code;
            lkp_PartType.EditValue = invoice.PartType;
            lkp_Branch.EditValue = invoice.Branch;
            lkp_Drawer.EditValue = invoice.Drawer;
            Glkp_PartID.EditValue = invoice.PartID;
            dt_Date.DateTime = invoice.Date;
            dt_DelivartDate.EditValue = invoice.DelivartDate;
            dt_PostedStoreDate.EditValue = invoice.PostedStoreDate;
            me_Notes.Text = invoice.Notes;
            me_ShippingAddress.Text = invoice.ShippingAddress;
            chk_PostedToStore.Checked = invoice.PostedToStore;
            sp_DiscountRation.EditValue = invoice.DiscountRation;
            sp_DiscountValue.EditValue = invoice.DiscountValue;
            sp_Expences.EditValue = invoice.Expences;
            sp_Net.EditValue = invoice.Net;
            sp_Paid.EditValue = invoice.Paid;
            sp_Remaing.EditValue = invoice.Remaing;
            sp_Tax.EditValue = invoice.Tax;
            sp_TaxValue.EditValue = invoice.TaxValue;
            sp_Total.EditValue = invoice.Total;

            generalDB = new DAL.dbDataContext();
            gridControl1.DataSource = generalDB.InvoiceDetailes.Where(x => x.InvoiceID == invoice.ID);
            base.Getdata();
        }
        public override void Setdata()
        {
            invoice.Code = txt_Code.Text;
            invoice.PartType = Convert.ToByte(lkp_PartType.EditValue); 
            invoice.Branch = Convert.ToInt32( lkp_Branch.EditValue);
            invoice.Drawer = Convert.ToInt32(lkp_Drawer.EditValue);
            invoice.PartID = Convert.ToInt32(Glkp_PartID.EditValue);
            invoice.Date = dt_Date.DateTime;
            invoice.DelivartDate = Convert.ToDateTime(dt_DelivartDate.EditValue);
            invoice.PostedStoreDate = dt_PostedStoreDate.EditValue as DateTime?;
            invoice.Notes = me_Notes.Text;
            invoice.ShippingAddress = me_ShippingAddress.Text;
            invoice.PostedToStore = chk_PostedToStore.Checked;
            invoice.DiscountRation = Convert.ToDouble(sp_DiscountRation.EditValue);
            invoice.DiscountValue = Convert.ToDouble(sp_DiscountValue.EditValue);
            invoice.Expences = Convert.ToDouble(sp_Expences.EditValue);
            invoice.Net = Convert.ToDouble(sp_Net.EditValue);
            invoice.Paid = Convert.ToDouble(sp_Paid.EditValue);
            invoice.Remaing = Convert.ToDouble(sp_Remaing.EditValue);
            invoice.Tax = Convert.ToDouble(sp_Tax.EditValue);
            invoice.TaxValue = Convert.ToDouble(sp_TaxValue.EditValue);
            invoice.Total = Convert.ToDouble(sp_Total.EditValue);
            invoice.InviceType = (byte)type;
            base.Setdata();
        }
        public override void New()
        {
            invoice = new DAL.invoiceHeader()
            {
                Drawer = session.Defualt.Drawer,
                Date = DateTime.Now,
                PostedStoreDate = DateTime.Now,
                PostedToStore = true,
                Code = GetNewInvoiceCode()
            };
            switch (type)
            {
                case master.InvoiceType.PurchaseReturn:
                case master.InvoiceType.Purchase:
                    invoice.PartType = (int)master.PartType.Customer;
                    invoice.PartID = session.Defualt.Customer;
                    invoice.Branch = session.Defualt.RawStore;
                    break;
                case master.InvoiceType.SalesReturn:
                case master.InvoiceType.Sales:
                    invoice.PartType = (int)master.PartType.Vendor;
                    invoice.PartID = (int)session.Defualt.Vender;
                    invoice.Branch = session.Defualt.Store;
                    break;
                default:
                    throw new NotImplementedException();
            }
            MoveFocusToGrid();
            base.New();
        }
        public override void Save()
        {
            gridView1.UpdateCurrentRow();
            
            if(gridView1.FocusedRowHandle < 0)
               GridView1_ValidateRow(gridView1, new ValidateRowEventArgs(gridView1.FocusedRowHandle,gridView1.GetRow(gridView1.FocusedRowHandle)));
            var db = new DAL.dbDataContext();
            if(invoice.ID == 0)
            {
                db.invoiceHeaders.InsertOnSubmit(invoice);
            }
            else
            {
                db.invoiceHeaders.Attach(invoice);
            }
            Setdata();
            //-------------- Expenses Distribution---------------------------------
            var items = gridView1.DataSource as Collection<DAL.InvoiceDetaile>;
            if (type == master.InvoiceType.Purchase) 
            {
                if (invoice.Expences > 0)
                {
                    var totalPrice = items.Sum(x => x.TotalPrice);
                    var totalQyt = items.Sum(x => x.ItemQty);

                    var byPriceUnit = invoice.Expences / totalPrice;  // السعر لكل جنية
                    var byQtyUnit = invoice.Expences / totalQyt;      // السعر لكل قطعة واحدة من الوحدة المختارة

                    CostDistributionOption distributionOption = new CostDistributionOption();

                    XtraDialogArgs args = new XtraDialogArgs();
                    args.Caption = "";
                    args.Content = distributionOption;
                    args.Buttons = new DialogResult[]
                    {
                        DialogResult.OK
                    };
                    args.Showing += Args_Showing;
                    XtraDialog.Show(args);

                    foreach (var x in items)
                    {
                        if (distributionOption.selectedOption == CostDistributionOptions.ByPrice)
                            x.CostValue = (x.TotalPrice / x.ItemQty) + (byPriceUnit * x.Price);  //توزيع بالسعر
                        else
                            x.CostValue = (x.TotalPrice / x.ItemQty) + (byQtyUnit);  //توزيع بالكمية

                        x.TotalCostValue = x.CostValue * x.ItemQty;
                    }
                }
                else
                {
                    foreach (var row in items)
                    {
                        row.CostValue = row.TotalPrice / row.ItemQty;
                        row.TotalCostValue = row.TotalPrice;
                    }
                }
                db.SubmitChanges();
            }
            foreach (var row in items)
                row.InvoiceID = invoice.ID;       //كل المنتجات اللي في الفاتورة تبقي واخدة نفس رقم الفاتورة
            generalDB.SubmitChanges();
            //---------------------------------------------------------------------------
            #region Journals
            db.Journals.DeleteAllOnSubmit(db.Journals.Where(x => x.SourceID == invoice.ID && x.SourceType == (byte)type));
            db.SubmitChanges();

            var store = db.Stores.Single(x => x.ID == invoice.Branch);
            var drawer = db.Drawers.Single(x => x.ID == invoice.Drawer);
            var partAccountID = db.CustomerAndVendors.Single(x => x.ID == invoice.PartID).AccountID;

            int storeAccount = 0;
            int taxAccount = 0;
            int discountAccount = 0;
            bool isPartCredit = true;
            bool insertCostOfSoldGoodsJournal = true;    //(القيد الثالث في عملية البيع(تكلفة البضاعة المباعة 
            bool IsIn = true;
            string msg = "";

            switch (type)
            {
                case InvoiceType.Purchase:
                    storeAccount = store.InventoryAccountID;
                    taxAccount = session.Defualt.PurchasesTax;
                    discountAccount = session.Defualt.DiscountReceivedAccount;
                    isPartCredit = true;
                    insertCostOfSoldGoodsJournal = false;
                    IsIn = true;
                    msg = $"purchase Invoice  number {invoice.ID} for customer {Glkp_PartID.Text}";
                    break;
                case InvoiceType.Sales:
                    storeAccount = store.SalesAccountID;
                    taxAccount = session.Defualt.SalesTax;
                    discountAccount = session.Defualt.DiscountAllowedAccount;
                    isPartCredit = false;
                    insertCostOfSoldGoodsJournal = true;
                    IsIn = false;
                    msg = $" sale Invoice number {invoice.ID} for customer {Glkp_PartID.Text}";
                    break;
                case InvoiceType.PurchaseReturn:
                    break;
                case InvoiceType.SalesReturn:
                    break;
                default:
                    break;
            }

            db.Journals.InsertOnSubmit(new Journal  //Part
            {
                Code = 57547,
                AccountID = partAccountID,
                Debit = (!isPartCredit)? invoice.Total + invoice.TaxValue + invoice.Expences : 0,   //مدين
                Credit =(isPartCredit)? invoice.Total + invoice.TaxValue + invoice.Expences : 0,  //دائن
                InsertDate = invoice.Date,
                SourceID = invoice.ID,
                SourceType = (byte)type,
                Note = msg,
            });
            db.Journals.InsertOnSubmit(new Journal  //Store Inventory
            {
                Code = 57547,
                AccountID = storeAccount,
                Debit = (isPartCredit) ? invoice.Total + invoice.Expences : 0,   //مدين
                Credit = (!isPartCredit) ? invoice.Total + invoice.Expences : 0,  //دائن
                InsertDate = invoice.Date,
                SourceID = invoice.ID,
                SourceType = (byte)type,
                Note = msg,
            });
            if(invoice.Tax > 0)
                db.Journals.InsertOnSubmit(new Journal  //Store Tax
                {
                    Code = 57547,
                    AccountID = taxAccount,
                    Debit = (isPartCredit) ? invoice.TaxValue : 0,   //مدين
                    Credit = (!isPartCredit) ? invoice.TaxValue : 0,  //دائن
                    InsertDate = invoice.Date,
                    SourceID = invoice.ID,
                    SourceType = (byte)type,
                    Note = msg + "Additional tax",
                });
            if (invoice.DiscountValue > 0)
            {
                db.Journals.InsertOnSubmit(new Journal  //Discount
                {
                    Code = 57547,
                    AccountID =discountAccount,
                    Debit = (!isPartCredit) ? invoice.DiscountValue : 0,   //مدين
                    Credit = (isPartCredit) ? invoice.DiscountValue : 0,  //دائن
                    InsertDate = invoice.Date,
                    SourceID = invoice.ID,
                    SourceType = (byte)type,
                    Note = msg + " Purchase discount",
                });
                db.Journals.InsertOnSubmit(new Journal  //Discount
                {
                    Code = 57547,
                    AccountID = partAccountID,
                    Debit = (isPartCredit) ? invoice.DiscountValue : 0,   //مدين
                    Credit = (!isPartCredit) ? invoice.DiscountValue : 0,  //دائن
                    InsertDate = invoice.Date,
                    SourceID = invoice.ID,
                    SourceType = (byte)type,
                    Note = msg + " Purchase discount",
                });
            }
            if (insertCostOfSoldGoodsJournal)
            {
                var TotalCost = items.Sum(z => z.TotalCostValue);
                db.Journals.InsertOnSubmit(new Journal  //Cost of Sold Goods
                {
                    Code = 57547,
                    AccountID = store.InventoryAccountID,
                    Debit = (isPartCredit) ? TotalCost : 0,   //مدين
                    Credit = (!isPartCredit) ? TotalCost : 0,  //دائن
                    InsertDate = invoice.Date,
                    SourceID = invoice.ID,
                    SourceType = (byte)type,
                    Note = msg + " Cost of Sold Goods",
                });
                db.Journals.InsertOnSubmit(new Journal  //Cost Of Sold Goods
                {
                    Code = 57547,
                    AccountID = store.CostOfSoldAccountID,
                    Debit = (!isPartCredit) ? TotalCost : 0,   //مدين
                    Credit = (isPartCredit) ? TotalCost : 0,  //دائن
                    InsertDate = invoice.Date,
                    SourceID = invoice.ID,
                    SourceType = (byte)type,
                    Note = msg + " Cost of Sold Goods",
                });
            }
            if (invoice.Paid > 0)
            {
                db.Journals.InsertOnSubmit(new Journal  //Paid
                {
                    Code = 57547,
                    AccountID = drawer.AccountID,
                    Debit = (!isPartCredit) ? invoice.Paid : 0,   //مدين
                    Credit = (isPartCredit) ? invoice.Paid : 0,  //دائن
                    InsertDate = invoice.Date,
                    SourceID = invoice.ID,
                    SourceType = (byte)type,
                    Note = msg + " Paid",
                });
                db.Journals.InsertOnSubmit(new Journal  //Paid
                {
                    Code = 57547,
                    AccountID = partAccountID,
                    Debit = (isPartCredit) ? invoice.Paid : 0,   //مدين
                    Credit = (!isPartCredit) ? invoice.Paid : 0,  //دائن
                    InsertDate = invoice.Date,
                    SourceID = invoice.ID,
                    SourceType = (byte)type,
                    Note = msg + " Paid",
                });
            }
            #endregion
            //------------------------------------------------------------------------------------

            db.StoreLogs.DeleteAllOnSubmit(db.StoreLogs.Where(x => x.SourceID == invoice.ID && x.SourceType == (byte)type));
            db.SubmitChanges();
            if (invoice.PostedToStore)
            {
                foreach (var row in items)
                {
                    var unitv = session.ProductsView.Single(x => x.ID == row.ItemID).Units.Single(u => u.UnitID == row.ItemUntiID);
                    db.StoreLogs.InsertOnSubmit(new StoreLog
                    {
                        ProductID = row.ItemID,
                        InsertTime = invoice.PostedStoreDate.Value,
                        SourceID = row.ID,
                        SourceType = (byte)type,
                        storeID = row.StoreID,
                        IsInTransaction = IsIn,
                        Qty = row.ItemQty * unitv.Factor,
                        CostValue = row.CostValue / unitv.Factor,
                        Notes = msg
                    });
                }
            }
            db.SubmitChanges();
            base.Save();
        }
        private void Args_Showing(object sender, XtraMessageShowingArgs e)
        {
            e.Form.ControlBox = false;
            e.Form.Height = 160;
            e.Form.Width = 350;
            e.Buttons[DialogResult.OK].Text = "Continue and Save";
            e.Buttons[DialogResult.OK].Width = 100;
        }
        public override void Print()
        {
            using (var db = new DAL.dbDataContext())
            {
                var invoiceReport = (from Inv in db.invoiceHeaders
                                     join Str in db.Stores on Inv.Branch equals Str.ID
                                     //join Prt in db.CustomerAndVendors on Inv.PartID equals Prt.ID
                                     from Prt in db.CustomerAndVendors.Where(x => x.ID == Inv.PartID).DefaultIfEmpty()
                                     from Drw in db.Drawers.Where(x => x.ID == Inv.Drawer).DefaultIfEmpty()
                                     where Inv.ID == invoice.ID
                                     select new   //Temporary Class
                                     {
                                         Inv.ID,
                                         Inv.Code,
                                         Store = Str.Name,
                                         Drawer = Drw.Name,
                                         PartName = Prt.Name,
                                         Prt.Phone,
                                         Inv.Date,
                                         InvoiceType = (Inv.InviceType == (byte)master.InvoiceType.Purchase) ? "فاتورة مبيعات" :
                                                      (Inv.InviceType == (byte)master.InvoiceType.PurchaseReturn) ? "فاتورة مردود مبيعات " :
                                                      (Inv.InviceType == (byte)master.InvoiceType.Sales) ? "فاتورة مشتريات" :
                                                      (Inv.InviceType == (byte)master.InvoiceType.SalesReturn) ? "فاتورة مردود مشتريات" : "Undefiend",
                                         Inv.DiscountValue,
                                         Inv.Expences,
                                         Inv.Net,
                                         Inv.Notes,
                                         Inv.Paid,
                                         Inv.Remaing,
                                         Inv.TaxValue,
                                         Inv.Total,
                                         PartType = (Inv.PartType == (byte)master.PartType.Customer) ? "عميل" :
                                                    (Inv.PartType == (byte)master.PartType.Vendor) ? "مورد" : "Undefiend",

                                         Products = (
                                                       from d in db.InvoiceDetailes.Where(x => x.InvoiceID == Inv.ID)
                                                       from p in db.Productsses.Where(x => x.ID == d.ItemID)
                                                       from u in db.UnitNames.Where(x => x.ID == d.ItemUntiID).DefaultIfEmpty()
                                                       select new
                                                       {
                                                           
                                                           ProductName = p.Name,
                                                           UnitName = u.Name,
                                                           d.ItemQty,
                                                           d.Price,
                                                           d.TotalPrice
                                                       }).ToList(),
                                         ProductCount = db.InvoiceDetailes.Where(x => x.InvoiceID == Inv.ID).Count()
                                     }).ToList();

                Reporting.rpt_invoice.printReport(invoiceReport);
            }
            base.Print();
        }
        #endregion
        public override void Delete()
        {
            if (XtraMessageBox.Show("Are you sure from delete this invoice?", "Delete message", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                using(var db = new DAL.dbDataContext())
                {
                    db.StoreLogs.DeleteAllOnSubmit(
                       db.StoreLogs.Where(x => x.SourceType == (byte)type &&
                       db.InvoiceDetailes.Where(u => u.InvoiceID == invoice.ID).Select(u => u.ID).Contains(x.SourceID)));
                    db.SubmitChanges();
                    db.InvoiceDetailes.DeleteAllOnSubmit(db.InvoiceDetailes.Where(x => x.InvoiceID == invoice.ID));
                    db.SubmitChanges();
                    db.invoiceHeaders.Attach(invoice);
                    db.invoiceHeaders.DeleteOnSubmit(invoice);
                    //db.invoiceHeaders.DeleteOnSubmit(db.invoiceHeaders.SingleOrDefault(x => x.ID == invoice.ID));
                    db.SubmitChanges();
                }
                base.Delete();
                New();
            }

        }
    }

}
