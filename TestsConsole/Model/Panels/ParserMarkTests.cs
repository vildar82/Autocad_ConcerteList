﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Autocad_ConcerteList.Src.Panels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autocad_ConcerteList.Src.ConcreteDB.Panels;

namespace Autocad_ConcerteList.Src.Panels.Tests
{
    [TestClass()]
    public class ParserMarkTests
    {
        public static IParserMark GetParser(string mark)
        {
            MarkPart markPart;
            ParserMark.DefineParts(mark, out markPart);
            return ParserMarkFactory.Create(markPart);
        }

        [TestMethod()]
        public void ParseSlabTest()
        {            
            var parser = GetParser("2П 544.363-1-2э");
            parser.Parse();
            bool check = parser.ItemGroup.Equals("2П") &&
                         parser.GroupIndexClass == 2 &&
                         parser.MarkWoGroupClassIndex == "П 544.363-1-2э" &&
                         parser.Length == 544 &&
                         parser.Height == 363 &&
                         parser.Thickness == null &&
                         parser.Formwork == 1 &&
                         parser.Electrics.Equals("2э") &&
                         string.IsNullOrEmpty(parser.BalconyCut) &&
                         string.IsNullOrEmpty(parser.BalconyDoor);
            Assert.AreEqual(check, true);
        }

        

        [TestMethod()]
        public void ParseSlabTest2()
        {
            var parser = GetParser("2П544.363-1-2э");           
            parser.Parse();
            bool check = parser.ItemGroup.Equals("2П") &&
                         parser.GroupIndexClass == 2 &&
                         parser.MarkWoGroupClassIndex == "П544.363-1-2э" &&
                         parser.Length == 544 &&
                         parser.Height == 363 &&
                         parser.Thickness == null &&
                         parser.Formwork == 1 &&
                         parser.Electrics.Equals("2э") &&
                         string.IsNullOrEmpty(parser.BalconyCut) &&
                         string.IsNullOrEmpty(parser.BalconyDoor);
            Assert.AreEqual(check, true);
        }

        [TestMethod()]
        public void ParseSlabTest3()
        {
            var parser = GetParser("П544.363-1-2э");            
            parser.Parse();
            bool check = parser.ItemGroup.Equals("П") &&
                         parser.GroupIndexClass == 0 &&
                         parser.MarkWoGroupClassIndex == "П544.363-1-2э" &&
                         parser.Length == 544 &&
                         parser.Height == 363 &&
                         parser.Thickness == null &&
                         parser.Formwork == 1 &&
                         parser.Electrics.Equals("2э") &&
                         string.IsNullOrEmpty(parser.BalconyCut) &&
                         string.IsNullOrEmpty(parser.BalconyDoor);
            Assert.AreEqual(check, true);
        }

        [TestMethod()]
        public void ParseSlabTest4()
        {
            var parser = GetParser("П544.363-2-3э");                        
            parser.Parse();
            bool check = parser.ItemGroup.Equals("П") &&
                         parser.GroupIndexClass == 0 &&
                         parser.MarkWoGroupClassIndex == "П544.363-2-3э" &&
                         parser.Length == 544 &&
                         parser.Height == 363 &&
                         parser.Thickness == null &&
                         parser.Formwork == 2 &&
                         parser.Electrics.Equals("3э") &&
                         string.IsNullOrEmpty(parser.BalconyCut) &&
                         string.IsNullOrEmpty(parser.BalconyDoor);
            Assert.AreEqual(check, true);
        }

        [TestMethod()]
        public void ParseEBTest()
        {
            var parser = GetParser("ЭБ-25");            
            parser.Parse();
            bool check = parser.ItemGroup.Equals("ЭБ") &&
                         parser.GroupIndexClass == 0 &&
                         parser.MarkWoGroupClassIndex == "ЭБ-25" &&
                         parser.Length == null &&
                         parser.Height == null &&
                         parser.Thickness == null &&
                         parser.Formwork == 25 &&
                         string.IsNullOrEmpty(parser.Electrics) &&
                         string.IsNullOrEmpty(parser.BalconyCut) &&
                         string.IsNullOrEmpty(parser.BalconyDoor);
            Assert.AreEqual(check, true);
        }

        [TestMethod()]
        public void ParseInnerPanelTest()
        {
            var parser = GetParser("В706.26.18-2-5э");            
            parser.Parse();
            bool check = parser.ItemGroup.Equals("В") &&
                         parser.GroupIndexClass == 0 &&
                         parser.MarkWoGroupClassIndex == "В706.26.18-2-5э" &&
                         parser.Length == 706 &&
                         parser.Height == 26 &&
                         parser.Thickness == 18 &&
                         parser.Formwork == 2 &&
                         parser.Electrics == "5э" &&
                         string.IsNullOrEmpty(parser.BalconyCut) &&
                         string.IsNullOrEmpty(parser.BalconyDoor);
            Assert.AreEqual(check, true);
        }

        [TestMethod()]
        public void ParseInnerPanelTest2()
        {
            var parser = GetParser("В526.28.18-3э");            
            parser.Parse();
            bool check = parser.ItemGroup.Equals("В") &&
                         parser.GroupIndexClass == 0 &&
                         parser.MarkWoGroupClassIndex == "В526.28.18-3э" &&
                         parser.Length == 526 &&
                         parser.Height == 28 &&
                         parser.Thickness == 18 &&
                         parser.Formwork == null &&
                         parser.Electrics == "3э" &&
                         string.IsNullOrEmpty(parser.BalconyCut) &&
                         string.IsNullOrEmpty(parser.BalconyDoor);
            Assert.AreEqual(check, true);
        }

        [TestMethod()]
        public void ParseInnerPanelTest3()
        {
            var parser = GetParser("2В526.28.18-3э");            
            parser.Parse();
            bool check = parser.ItemGroup.Equals("2В") &&
                         parser.GroupIndexClass == 2 &&
                         parser.MarkWoGroupClassIndex == "В526.28.18-3э" &&
                         parser.Length == 526 &&
                         parser.Height == 28 &&
                         parser.Thickness == 18 &&
                         parser.Formwork == null &&
                         parser.Electrics == "3э" &&
                         string.IsNullOrEmpty(parser.BalconyCut) &&
                         string.IsNullOrEmpty(parser.BalconyDoor);
            Assert.AreEqual(check, true);
        }

        [TestMethod()]
        public void ParseInnerPanelTest4()
        {
            var parser = GetParser("3В526.28.18-3э");            
            parser.Parse();
            bool check = parser.ItemGroup.Equals("3В") &&
                         parser.GroupIndexClass == 3 &&
                         parser.MarkWoGroupClassIndex == "В526.28.18-3э" &&
                         parser.Length == 526 &&
                         parser.Height == 28 &&
                         parser.Thickness == 18 &&
                         parser.Formwork == null &&
                         parser.Electrics == "3э" &&
                         string.IsNullOrEmpty(parser.BalconyCut) &&
                         string.IsNullOrEmpty(parser.BalconyDoor);
            Assert.AreEqual(check, true);
        }

        [TestMethod()]
        public void ParseOutPanelTest()
        {
            var parser = GetParser("3НСНг2 60.29.42-10-1Э");            
            parser.Parse();
            bool check = parser.ItemGroup.Equals("3НСНг2") &&
                         parser.GroupIndexClass == 2 &&
                         parser.MarkWoGroupClassIndex == "3НСНг 60.29.42-10-1Э" &&
                         parser.Length == 60 &&
                         parser.Height == 29 &&
                         parser.Thickness == 42 &&
                         parser.Formwork == 10 &&
                         parser.Electrics == "1Э" &&
                         string.IsNullOrEmpty(parser.BalconyCut) &&
                         string.IsNullOrEmpty(parser.BalconyDoor);
            Assert.AreEqual(check, true);
        }

        [TestMethod()]
        public void ParseOutPanelTest2()
        {
            var parser = GetParser("3НСНг2 60.29.42-10Б1П1-1э");            
            parser.Parse();
            bool check = parser.ItemGroup.Equals("3НСНг2") &&
                         parser.GroupIndexClass == 2 &&
                         parser.MarkWoGroupClassIndex == "3НСНг 60.29.42-10Б1П1-1э" &&
                         parser.Length == 60 &&
                         parser.Height == 29 &&
                         parser.Thickness == 42 &&
                         parser.Formwork == 10 &&
                         parser.Electrics == "1э" &&
                         parser.BalconyCut == "П1" &&
                         parser.BalconyDoor == "Б1";
            Assert.AreEqual(check, true);
        }

        [TestMethod()]
        public void ParseOutPanelTest23()
        {
            var parser = GetParser("3НСНгн 60.29.42-10Б1П1-1э");            
            parser.Parse();
            bool check = parser.ItemGroup.Equals("3НСНгн") &&
                         parser.GroupIndexClass == 0 &&
                         parser.MarkWoGroupClassIndex == "3НСНгн 60.29.42-10Б1П1-1э" &&
                         parser.Length == 60 &&
                         parser.Height == 29 &&
                         parser.Thickness == 42 &&
                         parser.Formwork == 10 &&
                         parser.Electrics == "1э" &&
                         parser.BalconyCut == "П1" &&
                         parser.BalconyDoor == "Б1";
            Assert.AreEqual(check, true);
        }

        [TestMethod()]
        public void ParseOutPanelTest3()
        {
            var parser = GetParser("3НСНг2 60.29.42-10П1Б1-1э");            
            parser.Parse();
            bool check = parser.ItemGroup.Equals("3НСНг2") &&
                         parser.GroupIndexClass == 2 &&
                         parser.MarkWoGroupClassIndex == "3НСНг 60.29.42-10П1Б1-1э" &&
                         parser.Length == 60 &&
                         parser.Height == 29 &&
                         parser.Thickness == 42 &&
                         parser.Formwork == 10 &&
                         parser.Electrics == "1э" &&
                         parser.BalconyCut == "П1" &&
                         parser.BalconyDoor == "Б1";
            Assert.AreEqual(check, true);
        }
        [TestMethod()]
        public void ParseOutPanelTest44()
        {
            var parser = GetParser("3НСг 72.29.32-6Б");            
            parser.Parse();
            bool check = parser.ItemGroup.Equals("3НСг") &&
                         parser.GroupIndexClass == 0 &&
                         parser.MarkWoGroupClassIndex == "3НСг 72.29.32-6Б" &&
                         parser.Length == 72 &&
                         parser.Height == 29 &&
                         parser.Thickness == 32 &&
                         parser.Formwork == 6 &&
                         string.IsNullOrEmpty(parser.Electrics) &&
                         parser.BalconyDoor == "Б";
            Assert.AreEqual(true, check);
        }

        [TestMethod()]
        public void ParseOutPanelTest4()
        {
            var parser = GetParser("3НСг 75.29.32-7П");            
            parser.Parse();
            bool check = parser.ItemGroup.Equals("3НСг") &&
                         parser.GroupIndexClass == 0 &&
                         parser.MarkWoGroupClassIndex == "3НСг 75.29.32-7П" &&
                         parser.Length == 75 &&
                         parser.Height == 29 &&
                         parser.Thickness == 32 &&
                         parser.Formwork == 7 &&
                         string.IsNullOrEmpty(parser.Electrics) &&
                         string.IsNullOrEmpty(parser.BalconyDoor) &&
                         parser.BalconyCut == "П";
            Assert.AreEqual(check, true);
        }

        [TestMethod()]
        public void ParseOutPanelTest5()
        {
            var parser = GetParser("3НСг 75.29.32-7Б");            
            parser.Parse();
            bool check = parser.ItemGroup.Equals("3НСг") &&
                         parser.GroupIndexClass == 0 &&
                         parser.MarkWoGroupClassIndex == "3НСг 75.29.32-7Б" &&
                         parser.Length == 75 &&
                         parser.Height == 29 &&
                         parser.Thickness == 32 &&
                         parser.Formwork == 7 &&
                         string.IsNullOrEmpty(parser.Electrics) &&
                         string.IsNullOrEmpty(parser.BalconyCut) &&
                         parser.BalconyDoor == "Б";
            Assert.AreEqual(check, true);
        }

        [TestMethod()]
        public void ParseOutPanelTest6()
        {
            var parser = GetParser("3НСг 75.29.32-8БП3");            
            parser.Parse();
            bool check = parser.ItemGroup.Equals("3НСг") &&
                         parser.GroupIndexClass == 0 &&
                         parser.MarkWoGroupClassIndex == "3НСг 75.29.32-8БП3" &&
                         parser.Length == 75 &&
                         parser.Height == 29 &&
                         parser.Thickness == 32 &&
                         parser.Formwork == 8 &&
                         string.IsNullOrEmpty(parser.Electrics) &&
                         parser.BalconyCut == "П3" &&
                         parser.BalconyDoor == "Б";
            Assert.AreEqual(check, true);
        }

        [TestMethod()]
        public void ParseOutPanelTest7()
        {
            var parser = GetParser("3НСг 75.29.32-8ПБ3");            
            parser.Parse();
            bool check = parser.ItemGroup.Equals("3НСг") &&
                         parser.GroupIndexClass == 0 &&
                         parser.MarkWoGroupClassIndex == "3НСг 75.29.32-8ПБ3" &&
                         parser.Length == 75 &&
                         parser.Height == 29 &&
                         parser.Thickness == 32 &&
                         parser.Formwork == 8 &&
                         string.IsNullOrEmpty(parser.Electrics) &&
                         parser.BalconyCut == "П" &&
                         parser.BalconyDoor == "Б3";
            Assert.AreEqual(check, true);
        }

        [TestMethod()]
        public void ParseOLTest()
        {
            var parser = GetParser("ОЛ 8.66");            
            parser.Parse();
            bool check = parser.ItemGroup.Equals("ОЛ") &&
                         parser.GroupIndexClass == 0 &&
                         parser.MarkWoGroupClassIndex == "ОЛ 8.66" &&
                         parser.Length == 8 &&
                         parser.Height == 66 &&
                         parser.Thickness == null &&
                         parser.Formwork == null &&
                         string.IsNullOrEmpty(parser.Electrics) &&
                         string.IsNullOrEmpty(parser.BalconyCut) &&
                         string.IsNullOrEmpty(parser.BalconyDoor);
            Assert.AreEqual(check, true);
        }

        [TestMethod()]
        public void ParseLPTest()
        {
            var parser = GetParser("1 ЛП 25.14-4");            
            parser.Parse();
            bool check = parser.ItemGroup.Equals("1ЛП") &&
                         parser.GroupIndexClass == 0 &&
                         parser.MarkWoGroupClassIndex == "1 ЛП 25.14-4" &&
                         parser.Length == 25 &&
                         parser.Height == 14 &&
                         parser.Thickness == null &&
                         parser.Formwork == 4 &&
                         string.IsNullOrEmpty(parser.Electrics) &&
                         string.IsNullOrEmpty(parser.BalconyCut) &&
                         string.IsNullOrEmpty(parser.BalconyDoor);
            Assert.AreEqual(check, true);
        }

        [TestMethod()]
        public void ParseOutPanelMountTest()
        {
            var parser = GetParser("3НСНг2 60.29.42-10Д-1э");            
            parser.Parse();
            bool check = parser.ItemGroup.Equals("3НСНг2") &&
                         parser.GroupIndexClass == 2 &&
                         parser.MarkWoGroupClassIndex == "3НСНг 60.29.42-10Д-1э" &&
                         parser.Length == 60 &&
                         parser.Height == 29 &&
                         parser.Thickness == 42 &&
                         parser.Formwork == 10 &&
                         parser.Electrics == "1э" &&
                         parser.MountIndex == "Д" &&
                         string.IsNullOrEmpty(parser.BalconyCut) &&
                         string.IsNullOrEmpty(parser.BalconyDoor);
            Assert.AreEqual(check, true);
        }
        [TestMethod()]
        public void ParseOutPanelMountTest2()
        {
            var parser = GetParser("3НСНг2 60.29.42-10ДГ-1э");            
            parser.Parse();
            bool check = parser.ItemGroup.Equals("3НСНг2") &&
                         parser.GroupIndexClass == 2 &&
                         parser.MarkWoGroupClassIndex == "3НСНг 60.29.42-10ДГ-1э" &&
                         parser.Length == 60 &&
                         parser.Height == 29 &&
                         parser.Thickness == 42 &&
                         parser.Formwork == 10 &&
                         parser.Electrics == "1э" &&
                         parser.MountIndex == "Д" &&
                         parser.ProngIndex == "Г" &&
                         string.IsNullOrEmpty(parser.BalconyCut) &&
                         string.IsNullOrEmpty(parser.BalconyDoor);
            Assert.AreEqual(check, true);
        }

        [TestMethod()]
        public void ParseOutPanelTest45()
        {
            var parser = GetParser("3НСг 72.29.32Д");            
            parser.Parse();
            bool check = parser.ItemGroup.Equals("3НСг") &&
                         parser.GroupIndexClass == 0 &&
                         parser.MarkWoGroupClassIndex == "3НСг 72.29.32Д" &&
                         parser.Length == 72 &&
                         parser.Height == 29 &&
                         parser.Thickness == 32 &&
                         string.IsNullOrEmpty(parser.Electrics) &&
                         parser.MountIndex == "Д";
            Assert.AreEqual(true, check);
        }        

        [TestMethod()]
        public void ParseLMPanelTest2()
        {
            var parser = GetParser("ЛМ-1.9");            
            parser.Parse();
            bool check = parser.ItemGroup.Equals("ЛМ") &&
                         parser.GroupIndexClass == 0 &&
                         parser.MarkWoGroupClassIndex == "ЛМ-1.9" &&
                         parser.StepHeightIndex == 1 &&
                         parser.StepsCount == 9 &&
                         parser.StepFirstHeight == null;
            Assert.AreEqual(true, check);
        }

        [TestMethod()]
        public void ParseLMPanelTest()
        {
            var parser = GetParser("ЛМ-1.9-18");
            parser.Parse();
            bool check = parser.ItemGroup.Equals("ЛМ") &&
                         parser.GroupIndexClass == 0 &&
                         parser.MarkWoGroupClassIndex == "ЛМ-1.9-18" &&
                         parser.StepHeightIndex == 1 &&
                         parser.StepsCount == 9 &&
                         parser.StepFirstHeight == 18;
            Assert.AreEqual(true, check);
        }
    }
}