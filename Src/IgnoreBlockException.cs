using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autocad_ConcerteList.Src
{
    /// <summary>
    /// Исключение - игнорируемые имена блоков - заведомо не изделия ЖБИ
    /// Например - ММС
    /// </summary>
    public class IgnoreBlockException : Exception
    {        
        public string BlName { get; private set; }

        public IgnoreBlockException(string blName)
        {
            BlName = blName;
        }

        public override string Message
        {
            get
            {
                return $"Проигнорирован блок '{BlName}', это не блок ЖБИ.";
            }
        }
    }
}
