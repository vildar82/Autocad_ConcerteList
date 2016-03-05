using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;

namespace Autocad_ConcerteList.SpecRegistration
{
    public class RegystryPanels
    {
        private List<Panel> panels;

        public RegystryPanels(List<Panel> panels)
        {
            this.panels = panels;
        }

        /// <summary>
        /// проверка существования панелей, и регистрация новых панелей в базе.
        /// Если отличается марка панелей в базе и в автокаде, то запись в список несоответствующих марок и показ пользователю.
        /// </summary>
        public void Registry()
        {
            // Поиск панели в базе по ее параметрам

            // Если панель с такими параметрами есть, то получение ее марки
            // сверка этой марки с автокадовской, если несовпадает, то добавление в список несоответствующих марок панелей.

            // Если нет, то добавление этой панели в список новых панелей для регистрации в базе.
            // получение ее марки и сверка с автокадовской.

            // Если не пустой список несоответствующих пнелей, то показ пользователю диалога, с вопросом о исправлении марок на чертеже.

            // Если есть новые панели для регистрации, то показ их пользователю, с вопросом о регистрации этих панелей в базе.

            throw new NotImplementedException();
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
                    }
                );
        }            
    }
}
