﻿using System;
using System.Collections.Generic;
using System.Text;
using Rodonaves.EDI.Enums;
using Rodonaves.Engine.BaseObjects;

namespace Rodonaves.EDI.Model
{
    public class LayoutColumnInfo : BaseInfo
    {
        public LayoutColumnInfo ()
        {
            LayoutDictionaries = new List<LayoutDictionaryInfo> ();
        }

        public int Id { get; set; }
        public string Register { get; set; }
        public int Sequence { get; set; }
        private int begin;

        public int Begin
        {
            get { return begin; }
            set { begin = value; }
        }

        private int end;

        public int End
        {
            get { return end; }
            set { end = value; }
        }
        public string Visual { get; set; }
        public string Comments { get; set; }
        public ColumnType ColumnType { get; set; }
        public int LayoutBandId { get; set; }
        public int CustomFreightDataId { get; set; }
        public string ColumnFormatType { get; set; }
        public bool ControlBreak { get; set; }

        public int Size
        {
            get { return end - begin + 1; }
            set { }
        }

        public string Element { get; set; }
        public string ElementValue { get; set; }
        public bool Multiple { get; set; }
        public List<LayoutDictionaryInfo> LayoutDictionaries { get; set; }
        public int LayoutFileNameId { get; set; }

    }
}