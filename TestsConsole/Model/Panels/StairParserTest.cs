using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autocad_ConcerteList.Src.ConcreteDB.Panels;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestsConsole.Model.Panels
{
    [TestClass()]
    public class StairParserTest
    {
        [TestMethod()]
        public void ParseSlabTest ()
        {
            var parserBase = new ParserMark("ЛМ-1.11-28");
            var stairMarkParser = new StairMarkParser();
            stairMarkParser.Parse(parserBase);

            var res = parserBase.StepHeightIndex == 1 &&
                parserBase.StepsCount == 11 &&
                parserBase.StepFirstHeight == 28 &&
                parserBase.Height == 1050;

            Assert.IsTrue(res);
        }

        [TestMethod()]
        public void ParseSlabTest2()
        {
            var parserBase = new ParserMark("ЛМ-1.9");
            var stairMarkParser = new StairMarkParser();
            stairMarkParser.Parse(parserBase);

            var res = parserBase.StepHeightIndex == 1 &&
                parserBase.StepsCount == 9 &&
                parserBase.StepFirstHeight == null &&
                parserBase.Height == 1050;

            Assert.IsTrue(res);
        }

        [TestMethod()]
        public void ParseSlabTest3()
        {
            var parserBase = new ParserMark("ЛМ-1.11.114");
            var stairMarkParser = new StairMarkParser();
            stairMarkParser.Parse(parserBase);

            var res = parserBase.StepHeightIndex == 1 &&
                parserBase.StepsCount == 11 &&
                parserBase.StepFirstHeight == null &&
                parserBase.Height == 1140;

            Assert.IsTrue(res);
        }

        [TestMethod()]
        public void ParseSlabTest4()
        {
            var parserBase = new ParserMark("ЛМ-1.11.114-28");
            var stairMarkParser = new StairMarkParser();
            stairMarkParser.Parse(parserBase);

            var res = parserBase.StepHeightIndex == 1 &&
                parserBase.StepsCount == 11 &&
                parserBase.StepFirstHeight == 28 &&
                parserBase.Height == 1140;

            Assert.IsTrue(res);
        }        
    }
}
