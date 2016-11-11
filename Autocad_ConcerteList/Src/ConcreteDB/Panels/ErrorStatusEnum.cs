using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autocad_ConcerteList.Src.ConcreteDB.Panels
{
    /// <summary>
    /// Статус - проверки панели
    /// </summary>
    [Flags]
    public enum ErrorStatusEnum
    {
        /// <summary>
        /// Нет ошибок
        /// </summary>
        None = 0x0,
        /// <summary>
        /// Несоответствие марки и параметров в блоке
        /// </summary>
        IncorrectMarkAndParams = 0x1,        
        /// <summary>
        /// Несоответствие имени блока и марки (атр)
        /// </summary>
        IncorrectBlockName = 0x2,
        /// <summary>
        /// В марке есть латинские символы
        /// </summary>
        MarkHasLatin = 0x4,
        /// <summary>
        /// Марка не соответствует формуле
        /// </summary>
        IncorrectMarkAndFormula = 0x8,
        /// <summary>
        /// Различные параметры в панелях одной марки
        /// </summary>
        DifferentParamInGroup = 0x10,
            /// <summary>
            /// Прочая ошибка
            /// </summary>
        OtherError = 0x20
    }
}
