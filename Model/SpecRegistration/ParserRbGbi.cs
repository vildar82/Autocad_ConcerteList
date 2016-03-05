using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;

namespace Autocad_ConcerteList.SpecRegistration
{
    public class ParserRbGbi
    {
        private ResultBuffer arg;

        public List<Panel> Panels { get; private set; }

        public ParserRbGbi(ResultBuffer arg)
        {
            this.arg = arg;
        }

        /// <summary>
        /// парсинг списка ResultBuffer переданного из лиспа
        /// Получение списка панелей - Panels
        /// При выбрасывании исключения, все прерывается.
        /// </summary>
        public void Parse()
        {
            throw new NotImplementedException();
        }
    }
}
