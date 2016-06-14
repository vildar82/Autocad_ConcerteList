using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autocad_ConcerteList.Src.ConcreteDB;
using Autocad_ConcerteList.Src.ConcreteDB.Panels;
using NUnit.Framework;

namespace TestAcadConsole.Tests.DB
{
    [TestFixture]
    public class DbServiceTests
    {
        [OneTimeSetUp]
        public void Init()
        {
            HibernatingRhinos.Profiler.Appender.EntityFramework.EntityFrameworkProfiler.Initialize();
            DbService.Init();
        }

        [Test(Description = "Тест поиска одной панели по параметрам")]
        public void FindPanelByParametersTest()
        {
            // В706.26.18-1-3э
            var item = DbService.FindByParameters("В", 7060, 2620, 180, 1, null, null, "3э");
            Assert.IsNotNull(item);
        }

        [Test(Description = "Тест загрузки всех панелей - и поиска")]
        public void FindPanelByAllParametersTest ()
        {
            // В706.26.18-1-3э
            Panel p = new Panel {
                ItemGroup = "В",
                Lenght = 7060,
                Height = 2620,
                Thickness = 180,
                Formwork = 1,
                Electrics = "3э"
            };
            var item = DbService.FindByParametersFromAllLoaded(p);
            Assert.IsNotNull(item);
        }
    }
}
