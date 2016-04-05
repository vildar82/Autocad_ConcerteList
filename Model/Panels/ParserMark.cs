using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcadLib.Errors;

namespace Autocad_ConcerteList.Model.Panels
{
    public class ParserMark
    {
        private string partGroup;
        private string partGab;
        private string partDop;

        public string MarkInput { get; private set; }
        public Error Error { get; private set; }
        // Группа изделия. Например "3НСг".
        public string ItemGroup { get; private set; }
        /// <summary>
        /// Длина - первое число в габаритах марки. Для группы вентблоков это может быть высота.
        /// </summary>        
        public short? Length { get; private set; }
        /// <summary>
        /// Высота - второе число в габаритах марки.
        /// </summary>
        public short? Height { get; private set; }
        /// <summary>
        /// Толщина - третий параметр в габаритах марки
        /// </summary>
        public short? Thickness { get; private set; }
        // Опалубка. Например 1, 2.
        public short? Formwork { get; private set; }
        public short? FormworkMirror { get; private set; }
        // Балкон. Б, Б1.
        public string BalconyDoor { get; private set; }
        // Подрезка. П, П1.
        public string BalconyCut { get; private set; }
        // Электрика. 1э, 2э.
        public string Electrics { get; private set; }

        public ParserMark(string mark)
        {
            MarkInput = mark;
        }

        public void Parse()
        {
            // на входе марка="2П 544.363-1-2э", получить параметры этой панели
            defineParts();
            parsePartGroup();
            parsePartGab();
            parsePartDop();
        }

        private void defineParts()
        {            
            int indexFirstDot = MarkInput.IndexOf('.');
            if (indexFirstDot != -1)
            {
                // Есть точка. Значит группа соеденена с габаритом длины. "2П72"
                string group = separateGroupFromLen(MarkInput, indexFirstDot - 1);
                if (string.IsNullOrEmpty(group))
                {
                    addErrorMsg("Не определена группа панели.");
                }
                else
                {
                    partGroup = group;
                    string gabAndDop = MarkInput.Substring(group.Length);
                    defineGabAndDop(gabAndDop);
                }
            }
            else
            {
                // нет габаритов в марке. Разделить по первому тире
                int indexDash = MarkInput.IndexOf('-');
                if (indexDash == -1)
                {
                    addErrorMsg("В марке определена только группа панели.");
                }
                else
                {
                    partGroup = MarkInput.Substring(0, indexDash);
                    partDop = MarkInput.Substring(indexDash + 1);
                }
            }            
        }

        private string separateGroupFromLen(string markInput, int indexFirstDot)
        {
            // Отделить группу от длины в строке марки. "2П72.29"
            for (int i = indexFirstDot; i >= 0; i--)
            {
                if (!char.IsDigit(markInput[i]))
                {
                    return markInput.Substring(0, i+1);
                }
            }
            return string.Empty;
        }

        private void defineGabAndDop(string input)
        {
            // На входе - "544.363-1-2э", получить 544.363 и 1-2э
            var splitDash = input.Split(new[] { '-' }, 2);
            if (splitDash.Length>1)
            {
                // Есть и габариты и доп параметры
                partGab = splitDash[0];
                partDop = splitDash[1];
            }
            else
            {
                // Нет тире - нет доп параметров. Только габариты.
                partGab = input;                
            }
        }

        private void parsePartGroup()
        {
            // Разбор группы. например partGroup = "2П"
            ItemGroup = partGroup.Replace(" ", "");
        }

        private void parsePartGab()
        {
            // Разбор части строки относящейся к габаритам панели. Они разделены точками. Например partGab = "544.363"
            if (string.IsNullOrEmpty(partGab)) return;
            var splitDot = partGab.Split('.');
            if (splitDot.Length > 3)
            {
                // Ошибка. максимум, только 3 габарита - Длина, Высота, Толщина
                addErrorMsg("Определено больше трех габаритов разделенных точкой.");
            }
            Length = GetShort(splitDot[0], "Длина");
            if (splitDot.Length > 1)
            {
                Height = GetShort(splitDot[1], "Высота");
                if (splitDot.Length > 2)
                {
                    Thickness = GetShort(splitDot[2], "Толщина");
                }
            }
        }

        private short GetShort(string input, string nameParam)
        {
            short res;
            if (!short.TryParse(input, out res))
            {
                throw new Exception($"Не определено '{nameParam}={input}'");
            }
            return res;
        }

        private void parsePartDop()
        {
            // partDop - например "5-1-1э"
            if (string.IsNullOrEmpty(partDop)) return;
            var splitDash = partDop.Split('-');
            if (splitDash.Length>3)
            {
                // Ошибка. Может быть только опалубка и электрика. От Зеркальности отказались.
                addErrorMsg("Определено больше трех возможных дополнительных параметра панели - опалубки, зеркальности и электрики.");
            }
            string val;
            if (splitDash.Length == 1)
            {
                if (partDop.IndexOf("э", StringComparison.OrdinalIgnoreCase) == -1)
                {
                    definePartFormwork(partDop);
                }
                else
                {
                    Electrics = partDop;
                }
            }
            else if (splitDash.Length>2)
            {
                definePartFormwork(splitDash[0]);
                FormworkMirror = GetShort(splitDash[1], "Опалубка");                
                Electrics = splitDash[2];                
            }
            else if (splitDash.Length > 1)
            {
                definePartFormwork(splitDash[0]);
                val = splitDash[1];
                if (val.IndexOf("э", StringComparison.OrdinalIgnoreCase) == -1)
                {
                    FormworkMirror = GetShort(val, "Зеркальность");
                }
                else
                {
                    Electrics = val;
                }                
            }
        }

        private void definePartFormwork(string input)
        {
            // Разбор части опалубки. Могут быть Подрезки и Балконы, типа 2П1Б1, где 2 - опалубка, П1-подреза, Б1-балкон
            int indexP = input.IndexOf("П");
            int indexB = input.IndexOf("Б");
            if (indexP == -1 && indexB == -1)
            {
                Formwork = GetShort(input, "Опалубка");
            }
            else if (indexB ==-1)
            {
                Formwork = GetShort(input.Substring(0, indexP), "Опалубка");
                BalconyCut = input.Substring(indexP);
            }
            else if (indexP == -1)
            {
                Formwork = GetShort(input.Substring(0, indexB), "Опалубка");
                BalconyDoor = input.Substring(indexB);
            }
            else
            {
                if (indexP<indexB)
                {
                    Formwork = GetShort(input.Substring(0, indexP), "Опалубка");
                    BalconyCut = input.Substring(indexP, indexB- indexP);
                    BalconyDoor = input.Substring(indexB);
                }
                else
                {
                    Formwork = GetShort(input.Substring(0, indexB), "Опалубка");
                    BalconyDoor = input.Substring(indexB, indexP- indexB);
                    BalconyCut = input.Substring(indexP);
                }
            }
        }

        private void addErrorMsg(string msg)
        {
            if (Error == null)
            {
                Error = new Error(msg, System.Drawing.SystemIcons.Error);
            }
            else
            {
                Error.AdditionToMessage(msg);
            }
        }
    }
}
