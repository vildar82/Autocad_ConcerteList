using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using System.Linq;
using Autocad_ConcerteList.Model.ConcreteDB.DataSet.ConcerteDataSetTableAdapters;
using Autocad_ConcerteList.ConcreteDB;

namespace Autocad_ConcerteList
{
    public partial class ItemForm : Form
    {        
        public ItemForm()
        {
            InitializeComponent();
        }        
                
        /// <summary>
        /// Создать марку
        /// </summary>        
        private void okButton_Click(object sender, EventArgs e)
        {
            try
            {
                var resultData = new ItemEntryData(this);
                if (resultData.IsCheck)
                {
                    //check exsist
                    var itemGroupTableList = new I_R_ItemTableAdapter();
                    var oCount = itemGroupTableList.IsMarkExecute(resultData.ItemGroup,
                                                                    resultData.Length,
                                                                    resultData.Height,
                                                                    resultData.Thickness,
                                                                    resultData.Formwork,
                                                                    resultData.BalconyDoor,
                                                                    resultData.BalconyCut,
                                                                    resultData.FormworkMirror,
                                                                    resultData.Electrics);
                    //insert item
                    int iCount;
                    if (int.TryParse(oCount.ToString(), out iCount) && iCount == 0)
                    {
                        // Пока тестирование передачи параметров, без записи в базу
#if !NODB
                        //item                    
                        decimal? itemGroupId = (decimal?)itemGroupComboBox.SelectedValue == -1 ? null : (decimal?)itemGroupComboBox.SelectedValue;
                        var balconyDoorId = (decimal?)balconyDoorComboBox.SelectedValue == -1 ? null : (decimal?)balconyDoorComboBox.SelectedValue;
                        var balconyCutId = (decimal?)balconycutComboBox.SelectedValue == -1 ? null : (decimal?)balconycutComboBox.SelectedValue;
                        var sCreateDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                        decimal? itemId = itemGroupTableList.InsertNewItemStore(itemGroupId, resultData.Length, resultData.Height,
                            resultData.Thicknes, resultData.Formwork, resultData.FormworkMirror, resultData.Electrics,
                            balconyDoorId, balconyCutId, resultData.Weight, resultData.Volume, sCreateDate, sCreateDate) as decimal?;

                        //series
                        if (itemId != null && seriesCheckedListBox.SelectedItems.Count > 0)
                        {
                            var itemSeriesTableList = new I_nn_Item_SeriesTableAdapter();
                            foreach (DataRowView item in seriesCheckedListBox.SelectedItems)
                            {
                                var seriesId = (decimal?)item.Row["SeriesId"];
                                itemSeriesTableList.InsertNewSeries(itemId, seriesId);
                            }
                        }

                        //tier???

                        // Получение записи панели из базы по ItemId
                        var itemRow = itemGroupTableList.GetItemById(itemId.Value).FirstOrDefault();
                        //result message                        
                        MessageBox.Show(this, $"Марка '{itemRow.HandMark}' успешно занесена в БД", "Информация",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
#endif
                        this.Hide();
                        // Запуск лисп функции                        
                        InvokeLisp.CreateBlock(resultData); // Передать параметры панели
                        this.Show();
                    }
                    else
                    {
                        MessageBox.Show(this, $"Марка '{resultData.Mark}' уже существует в БД", "Информация",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка",  MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ItemForm_Load(object sender, EventArgs e)
        {
            //Series
            var seriesTableList = new I_S_SeriesTableAdapter();
            seriesCheckedListBox.DataSource = seriesTableList.GetData();
            seriesCheckedListBox.DisplayMember = "Series";
            seriesCheckedListBox.ValueMember = "SeriesId";
            var defaultValue = (from DataRowView item in seriesCheckedListBox.Items
                                where item["Series"].Equals("ПИК-1.0")
                                select item).SingleOrDefault();

            seriesCheckedListBox.SelectedIndex = seriesCheckedListBox.Items.IndexOf(defaultValue);
            seriesCheckedListBox.SetItemChecked(seriesCheckedListBox.Items.IndexOf(defaultValue), true);
            
            //ItemGroup
            var itemGroupTableList = new I_S_ItemGroupTableAdapter();
            var itemGroupDataTable = itemGroupTableList.GetDataByItemGroup().Copy();
            var newItemGroupRow = itemGroupDataTable.NewRow();
            newItemGroupRow["ItemGroupId"] = -1;
            newItemGroupRow["ItemGroupUsage"] = false;
            newItemGroupRow["ItemGroupExprString"] = "";
            itemGroupDataTable.Rows.Add(newItemGroupRow);
            itemGroupDataTable.DefaultView.Sort = "ItemGroupUsage ASC, ItemGroupLong ASC, ItemGroup ASC";
            itemGroupComboBox.DataSource = itemGroupDataTable;
            itemGroupComboBox.DisplayMember = "ItemGroupExprString";
            itemGroupComboBox.ValueMember = "ItemGroupId";
            
            //BalconyDoor
            var balconyDoorTableList = new I_S_BalconyDoorTableAdapter();
            var balconyDoorDataTable = balconyDoorTableList.GetData().Copy();
            var newBalconyDoorRow = balconyDoorDataTable.NewRow();
            newBalconyDoorRow["BalconyDoorId"] = -1;
            newBalconyDoorRow["BalconyDoor"] = "";
            balconyDoorDataTable.Rows.Add(newBalconyDoorRow);
            balconyDoorDataTable.DefaultView.Sort = "BalconyDoor ASC";
            balconyDoorComboBox.DataSource = balconyDoorDataTable;
            balconyDoorComboBox.DisplayMember = "BalconyDoor";
            balconyDoorComboBox.ValueMember = "BalconyDoorId";
            
            //BalconyCut
            var balconycutTableList = new I_S_BalconyCutTableAdapter();
            var balconyCutDataTable = balconycutTableList.GetData().Copy();
            var newBalconyCutRow = balconyCutDataTable.NewRow();
            newBalconyCutRow["BalconyCutId"] = -1;
            newBalconyCutRow["BalconyCut"] = "";
            balconyCutDataTable.Rows.Add(newBalconyCutRow);
            balconyCutDataTable.DefaultView.Sort = "BalconyCut ASC";
            balconycutComboBox.DataSource = balconyCutDataTable;
            balconycutComboBox.DisplayMember = "BalconyCut";
            balconycutComboBox.ValueMember = "BalconyCutId";            
        }

        private void electricsIdxTextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            e.SuppressKeyPress = true;
            if (((e.KeyCode >= Keys.D0) && (e.KeyCode <= Keys.D9)) || 
                ((e.KeyCode >= Keys.NumPad0)) && (e.KeyCode <= Keys.NumPad9) ||
                (e.KeyCode == Keys.Delete) ||
                (e.KeyCode == Keys.Back))
            {
                e.SuppressKeyPress = false;                
            }
        }

        private void weigthTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            e.SuppressKeyPress = true;
            if (((e.KeyCode >= Keys.D0) && (e.KeyCode <= Keys.D9)) ||
                ((e.KeyCode >= Keys.NumPad0)) && (e.KeyCode <= Keys.NumPad9) ||
                (e.KeyCode == Keys.Delete) ||
                (e.KeyCode == Keys.Back) ||
                (e.KeyCode == Keys.Separator) ||
                (e.KeyCode == Keys.Decimal))
            {
                e.SuppressKeyPress = false;                
            }
        }

        private void volumeTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            e.SuppressKeyPress = true;
            if (((e.KeyCode >= Keys.D0) && (e.KeyCode <= Keys.D9)) ||
                ((e.KeyCode >= Keys.NumPad0)) && (e.KeyCode <= Keys.NumPad9) ||
                (e.KeyCode == Keys.Delete) ||
                (e.KeyCode == Keys.Back) ||
                (e.KeyCode == Keys.Separator) ||
                (e.KeyCode == Keys.Decimal))
            {
                e.SuppressKeyPress = false;                
            }
        }


        private void lenghtTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            e.SuppressKeyPress = true;
            if (((e.KeyCode >= Keys.D0) && (e.KeyCode <= Keys.D9)) ||
                ((e.KeyCode >= Keys.NumPad0)) && (e.KeyCode <= Keys.NumPad9) ||
                (e.KeyCode == Keys.Delete) ||
                (e.KeyCode == Keys.Back))
            {
                e.SuppressKeyPress = false;                
            }
        }

        private void heigthTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            e.SuppressKeyPress = true;
            if (((e.KeyCode >= Keys.D0) && (e.KeyCode <= Keys.D9)) ||
                ((e.KeyCode >= Keys.NumPad0)) && (e.KeyCode <= Keys.NumPad9) ||
                (e.KeyCode == Keys.Delete) ||
                (e.KeyCode == Keys.Back))
            {
                e.SuppressKeyPress = false;                
            }
        }

        private void thicknessTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            e.SuppressKeyPress = true;
            if (((e.KeyCode >= Keys.D0) && (e.KeyCode <= Keys.D9)) ||
                ((e.KeyCode >= Keys.NumPad0)) && (e.KeyCode <= Keys.NumPad9) ||
                (e.KeyCode == Keys.Delete) ||
                (e.KeyCode == Keys.Back))
            {
                e.SuppressKeyPress = false;                
            }
        }

        private void formworkTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            e.SuppressKeyPress = true;
            if (((e.KeyCode >= Keys.D0) && (e.KeyCode <= Keys.D9)) ||
                ((e.KeyCode >= Keys.NumPad0)) && (e.KeyCode <= Keys.NumPad9) ||
                (e.KeyCode == Keys.Delete) ||
                (e.KeyCode == Keys.Back))
            {
                e.SuppressKeyPress = false;                
            }
        }

        private void formworkmirrorTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            e.SuppressKeyPress = true;
            if (((e.KeyCode >= Keys.D0) && (e.KeyCode <= Keys.D9)) ||
                ((e.KeyCode >= Keys.NumPad0)) && (e.KeyCode <= Keys.NumPad9) ||
                (e.KeyCode == Keys.Delete) ||
                (e.KeyCode == Keys.Back))
            {
                e.SuppressKeyPress = false;                
            }
        }

        private void UpdateItemEntry()
        {
            try
            {
                errorValueProvider.Clear();
                var resultData = new ItemEntryData(this);
                resultData.RefreshItemEntryData();
            }
            catch (Exception ex)
            {
                errorValueProvider.SetError(markLabel, ex.Message);
            }            
        }        

        private void updateItemEntry (object sender, EventArgs e)
        {
            UpdateItemEntry();
        }
    }    
}
