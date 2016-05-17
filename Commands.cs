﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autocad_ConcerteList.Model.ConcreteDB;
using Autocad_ConcerteList.Model.RegystryPanel;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;

[assembly: CommandClass(typeof(Autocad_ConcerteList.Commands))]

namespace Autocad_ConcerteList
{
    public class Commands
    {
        /// <summary>
        /// Проверка наличия панели в базе данных
        /// По переданным параметрам панели
        /// Возвращает nil или список панелей (handMark и марку по формуле)
        /// </summary>
        [LispFunction("KR_NR_CheckPanelInDb")]
        public ResultBuffer KR_NR_CheckPanelInDb(ResultBuffer args)
        {
            DbService.Init();
            ParserRb parserRb = ParseArgs(args);
            var panel = parserRb.Panels.First();
            // Проверка наличия панели в базе по набору параметров            
            var dtItems = DbService.FindByParameters(panel);
            if (dtItems.Count == 0)
            {
                // Такой панели нет - return nil
                return null;
            }            
            // Формирование возвращаемого списка - найденных панелей (HandMark + марка по формуле)            
            ResultBuffer resVal = new ResultBuffer();
            resVal.Add(new TypedValue((int)LispDataType.ListBegin));
            foreach (var item in dtItems)
            {
                // HandMark - точечная пара                
                resVal.Add(new TypedValue((int)LispDataType.ListBegin));
                resVal.Add(new TypedValue((int)LispDataType.Text, "HandMark"));
                resVal.Add(new TypedValue((int)LispDataType.DottedPair));
                resVal.Add(new TypedValue((int)LispDataType.Text, item.HandMarkNoColour));
                resVal.Add(new TypedValue((int)LispDataType.ListEnd));
                // Марка по формуле - точечная пара
                resVal.Add(new TypedValue((int)LispDataType.ListBegin));
                resVal.Add(new TypedValue((int)LispDataType.Text, "ByFormula"));
                resVal.Add(new TypedValue((int)LispDataType.DottedPair));
                resVal.Add(new TypedValue((int)LispDataType.Text, panel.MarkDbWoSpace));
                resVal.Add(new TypedValue((int)LispDataType.ListEnd));
            }
            resVal.Add(new TypedValue((int)LispDataType.ListEnd));
            return resVal;
        }        

        /// <summary>
        /// Регистрация панели в базе
        /// </summary>
        [LispFunction("KR_NR_RegisterPanelInDb")]
        public void KR_NR_RegisterPanelInDb(ResultBuffer args)
        {
            DbService.Init();
            ParserRb parserRb = ParseArgs(args);
            var panel = parserRb.Panels.First();            
            // Определение серии ПИК1
            var series = DbService.GetSeries();
            var serPik1 = series.First(s => s.Series.Equals("ПИК-1.0"));
            // Регистрация панели в базе
            DbService.Register(panel, serPik1);
        }

        /// <summary>
        /// Удаление панели из базы
        /// </summary>
        [LispFunction("KR_NR_RemovePanelFromDb")]
        public void KR_NR_RemovePanelFromDb(ResultBuffer args)
        {
            DbService.Init();
            ParserRb parserRb = ParseArgs(args);
            var panel = parserRb.Panels.First();            
            DbService.RemovePanel(panel);
        }

        private static ParserRb ParseArgs(ResultBuffer args)
        {
            // Парсинг аргументов - параметров панели            
            ParserRb parserRb = new ParserRb(args);
            parserRb.Parse();
            if (parserRb.Panels == null || parserRb.Panels.Count == 0)
                throw new ArgumentException("Не определены параметры панели при парсинге переданных аргументов.");

            if (parserRb.Panels.Count > 1)
                throw new ArgumentException("Передано больше одной панели.");
            return parserRb;
        }        
    }
}