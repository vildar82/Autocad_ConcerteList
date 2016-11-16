using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autocad_ConcerteList.Src.ConcreteDB
{
    /// <summary>
    /// Изделие
    /// </summary>
    public interface iItem
    {        
        string Mark { get; }               
        string Item_group { get;  }
        short? Length { get;  }
        short? Height { get; }
        short? Thickness { get; }        
        short? Formwork { get; }
        string Balcony_door { get; }
        string Balcony_cut { get; }
        //short? FormworkMirror { get; }
        string Electrics { get; }
        float? Weight { get; }
        float? Volume { get; }
        /// <summary>
        /// Закладная. Например: Д или пусто
        /// </summary>
        string Mount_element { get; }
        /// <summary>
        /// Зубец. Например: Г или пусто
        /// </summary>
        string Prong { get; }
        /// <summary>
        /// Высота ступени - индекс - 1,2,3 (ЛМ-1.11-15)
        /// </summary>
        int? Step_height { get; }
        /// <summary>
        /// Кол. ступеней. - 11,12
        /// </summary>
        int? Steps { get; }
        /// <summary>
        /// Высота первой ступени
        /// </summary>
        int? First_step { get; }
        /// <summary>
        /// Что это наружная стеновая панель
        /// </summary>
        bool IsExteriorWall { get; }
        bool IsInnerWall { get; }
    }
}
