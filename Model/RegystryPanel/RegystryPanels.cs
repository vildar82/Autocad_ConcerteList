using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autocad_ConcerteList.RegystryPanel.IncorrectMark;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;

namespace Autocad_ConcerteList.RegystryPanel
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
        private List<Panel> panelsIncorrectMark;

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
            panelsIncorrectMark = new List<Panel>();
            // Поиск панели в базе по ее параметрам

            // Если панель с такими параметрами есть, то получение ее марки
            // сверка этой марки с автокадовской, если несовпадает, то добавление в список несоответствующих марок панелей.

            // Если нет, то добавление этой панели в список новых панелей для регистрации в базе.
            // получение ее марки и сверка с автокадовской.

            // Если не пустой список несоответствующих пнелей, то показ пользователю диалога, с вопросом о исправлении марок на чертеже.

            // Если есть новые панели для регистрации, то показ их пользователю, с вопросом о регистрации этих панелей в базе.

            foreach (var panel in panels)
            {
                // Определение марки панели из базы
                panel.MarkDb = DbService.GetDbMark(panel);
                if (string.IsNullOrEmpty(panel.MarkDb))
                {
                    continue;
                }                

                if (!DbService.ExistPanel(panel))
                { 
                    // Добавление панели в список для регистрации                    
                    panelsToReg.Add(panel);
                }
                if (panel.Mark != panel.MarkDb)
                {
                    // Марка из атрибута блока отличается от марки полученой из базы
                    panelsIncorrectMark.Add(panel);
                }   
            }

            // Если есть некорректные марки, то показ их пользователю с запросом исправления блоков на чертеже.
            if (IncorrectMarks.Show(panelsIncorrectMark))
            {
                // Исправить некорректные панели на чертеже
                IncorrectMarks.Fix(panelsIncorrectMark);               
            }

            // Регистрация новых панелей.
            regPanels = RegPanels(panelsToReg);

            return regPanels;
        }

        private int RegPanels(List<Panel> panelsToReg)
        {
            if (panelsToReg.Count == 0)
            {
                return 0;
            }
            int regCount = 0;
            FormPanels formPanels = new FormPanels(panelsToReg);
            formPanels.Text = "Регистрация новых панелей";
            formPanels.buttonOk.Text = "Регистрация";
            if (Application.ShowModalDialog(formPanels) == System.Windows.Forms.DialogResult.OK)
            {
                foreach (var item in panelsToReg)
                {
                    if (DbService.Register(item))
                    {
                        regCount++;
                    }
                }
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
