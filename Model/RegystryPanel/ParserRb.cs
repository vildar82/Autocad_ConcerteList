using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;

namespace Autocad_ConcerteList.RegystryPanel
{
    public class ParserRb
    {
        private ResultBuffer arg;

        public List<Panel> Panels { get; private set; }

        public ParserRb(ResultBuffer arg)
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
            Panels = new List<Panel>();

            Panels.Add(new Panel()
            {
                ItemGroup = "3НСг",
                Lenght = 7489,
                Height = 2890,
                Thickness = 320,
                Formwork = 12,
                BalconyCut = "П1",
                BalconyDoor = "Б1",
                Electrics = "1э",
                Weight = 1,
                Volume = 1,
                Mark = "3НСг 75.29.32-12Б1П1-1э"
            });

            Panels.Add(new Panel()
            {
                ItemGroup = "В",
                Lenght = 2660,
                Height = 2620,
                Thickness = 160,                
                Weight = 10,
                Volume = 10,
                Mark = "В266.26.16"
            });
        }
    }
}
