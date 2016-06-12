using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autocad_ConcerteList.Src.RegystryPanel;
using NUnit.Framework;

namespace TestAcadConsole.Tests.DB
{
    [TestFixture]
    public class DbServiceTests
    {
        [OneTimeSetUp]
        public void Init()
        {
            DbService.Init();
        }

        [Test(Description = "Тест поиска одной панели по параметрам")]
        public void FindPanelByParametersTest()
        {
            // В706.26.18-1-3э
            var item = DbService.FindByParameters("В", 7060, 2620, 180, 1, null, null, "3э");
            Assert.IsNotNull(item);
        }
    }
}
