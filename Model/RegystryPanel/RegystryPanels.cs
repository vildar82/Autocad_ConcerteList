using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcadLib.Errors;
using Autocad_ConcerteList.Model.RegystryPanel.IncorrectMark;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;

namespace Autocad_ConcerteList.Model.RegystryPanel
{
    public class RegystryPanels
    {
        private List<Panel> panels;
        /// <summary>
        /// Панели для регистрации в базе
        /// </summary>
        private List<Panel> panelsToReg;
        /// <summary>
        /// Панели с несоответствующей маркой - в блоке одна марка в базе другая.
        /// </summary>
        private List<Panel> panelsErrors;

        public RegystryPanels(List<Panel> panels)
        {
            this.panels = panels;
        }

        /// <summary>
        /// проверка существования панелей, и регистрация новых панелей в базе.
        /// Если отличается марка панелей в базе и в автокаде, то запись в список несоответствующих марок и показ пользователю.
        /// </summary>
        public int Registry()
        {
            int regPanels = 0;
            DbService.Init();
            panelsToReg = new List<Panel>();
            panelsErrors = new List<Panel>();
            // Поиск панели в базе по ее параметрам

            // Если панель с такими параметрами есть, то получение ее марки
            // сверка этой марки с автокадовской, если несовпадает, то добавление в список несоответствующих марок панелей.

            // Если нет, то добавление этой панели в список новых панелей для регистрации в базе.
            // получение ее марки и сверка с автокадовской.

            // Если не пустой список несоответствующих пнелей, то показ пользователю диалога, с вопросом о исправлении марок на чертеже.

            // Если есть новые панели для регистрации, то показ их пользователю, с вопросом о регистрации этих панелей в базе.

            // Проверка списка панелей.
            CheckPanels();

            // Показать ошибки при проверке панелей, если они есть.
            Inspector.ShowDialog();
            Inspector.Clear();

            // Если есть панели с ошибками. показать форму исправления ошибок.
            if (panelsErrors.Count > 0)
            {
                ShowErrorPanels();
            }

            // Регистрация новых панелей.
            regPanels = RegPanels(panelsToReg);

            return regPanels;
        }

        private void ShowErrorPanels()
        {
            // Если среди ошибок есть критические - то прерывание программы и показ немодальной формы для исправления.
            if (panelsErrors.Any(p => p.ErrorStatus.HasFlag(EnumErrorItem.DifferentParams)))
            {
                var sortedPanelErrors = panelsErrors.OrderBy(p => p.ErrorStatus).ThenBy(p => p.Mark).ToList();
                // Показ немодальной формы с ошибками в панелях                    
                FormPanels formFatalErrPanels = new FormPanels(sortedPanelErrors);
                formFatalErrPanels.BackColor = System.Drawing.Color.DarkRed;
                formFatalErrPanels.Text = "Панели с ошибками";
                formFatalErrPanels.buttonOk.Visible = false;
                formFatalErrPanels.buttonCancel.Visible = false;
                Application.ShowModelessDialog(formFatalErrPanels);

                throw new System.Exception("Найдены критические ошибки в панелях. Необходимо их исправить и запустить регистрацию повторно.");
            }

            // Если есть некорректные марки, то показ их пользователю с запросом исправления блоков на чертеже.
            FormPanels formErrPanels = new FormPanels(panelsErrors.OrderBy(p=>p.Mark).ToList());            
            formErrPanels.Text = "Панели с ошибками";
            if (Application.ShowModalDialog(formErrPanels) != System.Windows.Forms.DialogResult.OK)
            {
                // Прерывание и открытие этого окна в немодальном виде
                formErrPanels.buttonOk.Visible = false;
                formErrPanels.buttonCancel.Visible = false;
                Application.ShowModelessDialog(formErrPanels);
                throw new System.Exception(AcadLib.General.CanceledByUser);
            }

            // Исправить некорректные марки панелей в чертеже
            IncorrectMarks.Fix(panelsErrors);
        }

        private void CheckPanels()
        {
            // Проверка уникальности марок в списке. 
            // Если есть несколько одинак марок, то это ошибка - прервать процесс регистрации и показать немодальную форму исправления ошибок.
            var panelsGroupByMark = panels.GroupBy(p => p.Mark);

            foreach (var panelGroupMark in panelsGroupByMark)
            {
                // Если в группе больше одной марки - ошибка - несколько панелей одной марки с различными параметрами.
                if (panelGroupMark.Skip(1).Any())
                {
                    // Создать новую панель в которой отличающиеся параметры будут соответственно помечены.
                    Panel panelErrParams = Panel.GetErrParams(panelGroupMark);
                    panelsErrors.Add(panelErrParams);
                }
                else
                {
                    var panel = panelGroupMark.First();
                    // Определение марки панели из базы
                    try
                    {
                        panel.MarkDb = DbService.GetDbMark(panel);
                        if (string.IsNullOrEmpty(panel.MarkDb))
                        {
                            throw new System.Exception("не определена");
                        }
                    }
                    catch (System.Exception ex)
                    {
                        // Ошибка при определении марки панели. Нужно разобираться с параметрами панели и с формулой.
                        string err = $"Ошибка определения марки панели - {ex.Message}.\r\nПараметры панели: {panel.Info}";
                        Inspector.AddError(err, panel.IdBlRef, System.Drawing.SystemIcons.Error);
                        Logger.Log.Fatal(ex, err);
                        continue;
                    }

                    if (!DbService.ExistPanelByParameters(panel))
                    {
                        // Добавление панели в список для регистрации                    
                        panelsToReg.Add(panel);
                    }
                    if (panel.Mark != panel.MarkDb)
                    {
                        // Марка из атрибута блока отличается от марки полученой из базы
                        panel.ErrorStatus = EnumErrorItem.IncorrectMark;
                        panelsErrors.Add(panel);
                    }
                }
            }
        }

        private int RegPanels(List<Panel> panelsToReg)
        {
            if (panelsToReg.Count == 0)
            {
                return 0;
            }
            int regCount = 0;

            // Форма регистрации панелей
            FormPanels formPanels = new FormPanels(panelsToReg);
            formPanels.BackColor = System.Drawing.Color.Green;
            formPanels.Text = "Регистрация новых панелей";
            formPanels.buttonOk.Text = "Регистрация";
            var series = DbService.GetSeries();
            var serPik1 = series.First(s => s.Series.Equals("ПИК-1.0"));
            formPanels.SetSeries(series, serPik1);            
            if (Application.ShowModalDialog(formPanels) == System.Windows.Forms.DialogResult.OK)
            {
                var ser = formPanels.comboBoxSer.SelectedItem as Model.ConcreteDB.DataSet.ConcerteDataSet.I_C_SeriesRow;
                foreach (var item in panelsToReg)
                {
                    if (!DbService.ExistPanelByParameters(item))
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
            return regCount;
        }

        /// <summary>
        /// Возращаемый список панелей с несоответствующими марками для исправления в автокаде,
        /// Или если все ок, то возврат пустого списка (null = nil).
        /// </summary>
        /// <returns></returns>
        public ResultBuffer RbReturn()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Отмена создания спецификации - или ошибка в программе или пользователь прервал.
        /// </summary>
        /// <returns></returns>
        public static ResultBuffer ReturnCancel()
        {
            return new ResultBuffer(new TypedValue[]
                    {
                        new  TypedValue((int)LispDataType.Int32, -1)
                    });
        }
        /// <summary>
        /// Ошибка в программе регистрации панелей.
        /// </summary>
        /// <returns></returns>
        public static ResultBuffer ReturnError()
        {
            return new ResultBuffer(new TypedValue[]
                    {
                        new  TypedValue((int)LispDataType.Int32, 0)
                    });
        }
    }
}
