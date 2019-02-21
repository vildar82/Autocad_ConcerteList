using System;

namespace Autocad_ConcerteList.Src.ConcreteDB
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

        public override string Message => $"Проигнорирован блок '{BlName}', это не блок ЖБИ.";
    }
}
