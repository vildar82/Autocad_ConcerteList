using Microsoft.VisualStudio.TestTools.UnitTesting;
using Autocad_ConcerteList.Src.RegystryPanel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autocad_ConcerteList.Src.RegystryPanel.Tests
{
    [TestClass()]
    public class DbServiceTests
    {
        [ClassInitialize]
        [TestMethod()] 
        public static void InitTest(TestContext context)
        {
            DbService.Init();            
        }

        [TestMethod()]
        public void FindByParametersTest()
        {
            // В706.26.18-1-3э
            var item = DbService.FindByParameters("В", 7060, 2620, 180, 1, null, null, "3э");
            Assert.IsNotNull(item);
        }

        [TestMethod()]
        public void GetSeriesTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetDbMarkTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void RegisterTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void RegColorTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void FindGroupTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetBalconyCutIdTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetBalconyDoorIdTest()
        {
            Assert.Fail();
        }
    }
}