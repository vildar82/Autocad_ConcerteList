using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autocad_ConcerteList.Src.Panels.BaseParams;
using Autocad_ConcerteList.Src.RegystryPanel;
using Autocad_ConcerteList.Src.RegystryPanel.IncorrectMark;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.EditorInput;

namespace Autocad_ConcerteList.Src.Panels
{
    public class RegPanels
    {
        public List<Panel> Panels { get; private set; }
        public List<Panel> RegsPanels { get; private set; }

        public RegPanels(List<Panel> panels)
        {
            Panels = panels;
        }

        public int Registry()
        {
            int regCount = 0;
            RegsPanels = Panels.Where(p => p.DbItem == null).ToList();

            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            if (RegsPanels.Count == 0)
            {
                ed.WriteMessage($"\nНет новых панелей.");
            }
            else
            {
                // Если есть панели с ошибками, то показ немодальной формы без возможности регистрации.
                var errPanels = RegsPanels.Where(p => p.ErrorStatus != EnumErrorItem.None);
                if (errPanels.Any())
                {
                    FormPanels panelForm = new FormPanels(errPanels.ToList());
                    panelForm.Text = "Новые панели";                    
                    panelForm.BackColor = System.Drawing.Color.Red;
                    panelForm.buttonCancel.Visible = false;
                    panelForm.buttonOk.Visible = false;
                    Application.ShowModelessDialog(panelForm);
                }
                else
                {
                    // Проверка все ли подрезки и балконы определены в базе
                    CheckBaseParams.Check(RegsPanels);

                    // Форма регистрации панелей
                    FormPanels formPanels = new FormPanels(RegsPanels);
                    formPanels.SetGroupedPanels(true);
                    formPanels.BackColor = System.Drawing.Color.Green;
                    formPanels.Text = "Регистрация новых панелей";
                    formPanels.buttonOk.Text = "Регистрация";
                    var series = DbService.GetSeries();
                    var serPik1 = series.First(s => s.Series.Equals("ПИК-1.0"));
                    formPanels.SetSeries(series, serPik1);
                    if (Application.ShowModalDialog(formPanels) == System.Windows.Forms.DialogResult.OK)
                    {
                        var ser = formPanels.comboBoxSer.SelectedItem as Src.ConcreteDB.DataSet.ConcerteDataSet.I_C_SeriesRow;
                        foreach (var item in RegsPanels)
                        {
                            if (DbService.FindByParameters(item).Count==0)
                            {
                                if (DbService.Register(item, ser))
                                {
                                    regCount++;
                                }
                            }
                        }
                    }
                    else
                    {
                        // Прерывание и открытие этого окна в немодальном виде                
                        formPanels.buttonOk.Visible = false;
                        formPanels.buttonCancel.Visible = false;
                        Application.ShowModelessDialog(formPanels);
                        throw new System.Exception(AcadLib.General.CanceledByUser);
                    }
                }
            }
            return regCount;
        }
    }
}
