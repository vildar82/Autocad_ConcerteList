using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autocad_ConcerteList.Src.ConcreteDB.Panels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Autocad_ConcerteList.Src.Panels.Tests;

namespace TestsConsole.Model.Panels
{
    [TestClass]
    public class StairParserTest
    {
        [TestMethod]
        public void ParseSlabTest()
        {
            var parser = ParserMarkTests.GetParser("ЛМ-1.11-28");
            parser.Parse();

            var res = parser.StepHeightIndex == 1 &&
                parser.StepsCount == 11 &&
                parser.StepFirstHeight == 28 &&
                parser.Height == 1050;

            Assert.IsTrue(res);
        }

        [TestMethod]
        public void ParseSlabTest2()
        {
            var parser = ParserMarkTests.GetParser("ЛМ-1.9");
            parser.Parse();            

            var res = parser.StepHeightIndex == 1 &&
                parser.StepsCount == 9 &&
                parser.StepFirstHeight == null &&
                parser.Height == 1050;

            Assert.IsTrue(res);
        }

        [TestMethod]
        public void ParseSlabTest3()
        {
            var parser = ParserMarkTests.GetParser("ЛМ-1.11.114");
            parser.Parse();            

            var res = parser.StepHeightIndex == 1 &&
                parser.StepsCount == 11 &&
                parser.StepFirstHeight == null &&
                parser.Height == 1140;

            Assert.IsTrue(res);
        }

        [TestMethod]
        public void ParseSlabTest4()
        {
            var parser = ParserMarkTests.GetParser("ЛМ-1.11.114-28");
            parser.Parse();            

            var res = parser.StepHeightIndex == 1 &&
                parser.StepsCount == 11 &&
                parser.StepFirstHeight == 28 &&
                parser.Height == 1140;

            Assert.IsTrue(res);
        }
    }
}
