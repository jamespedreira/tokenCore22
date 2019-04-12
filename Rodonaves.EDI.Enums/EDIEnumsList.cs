using System.Collections.Generic;
using System.Linq;

namespace Rodonaves.EDI.Enums
{
    public static class EDIEnumsList
    {
        public static List<KeyDescriptionValue<ProcessTypeEnum>> GetProcessType (params ProcessTypeEnum[] parms)
        {
            var lst = new List<KeyDescriptionValue<ProcessTypeEnum>> ();

            if (!parms.ToList ().Exists (p => p == ProcessTypeEnum.CONEMB))
                lst.Add (new KeyDescriptionValue<ProcessTypeEnum> (ProcessTypeEnum.CONEMB, "CONEMB", "1"));
            if (!parms.ToList ().Exists (p => p == ProcessTypeEnum.CONEMBDOCCOB))
                lst.Add (new KeyDescriptionValue<ProcessTypeEnum> (ProcessTypeEnum.CONEMBDOCCOB, "CONEMB DE DOCCOB", "3"));
            if (!parms.ToList ().Exists (p => p == ProcessTypeEnum.DOCCOB))
                lst.Add (new KeyDescriptionValue<ProcessTypeEnum> (ProcessTypeEnum.DOCCOB, "DOCCOB", "4"));
            if (!parms.ToList ().Exists (p => p == ProcessTypeEnum.OCOREN))
                lst.Add (new KeyDescriptionValue<ProcessTypeEnum> (ProcessTypeEnum.OCOREN, "OCOREN", "5"));

            if (!parms.ToList ().Exists (p => p == ProcessTypeEnum.FREE))
                lst.Add (new KeyDescriptionValue<ProcessTypeEnum> (ProcessTypeEnum.FREE, "LIVRE", "8"));

            return lst;
        }

        public static List<KeyDescriptionValue<ColumnType>> GetColumnType (params ColumnType[] parms)
        {
            var lst = new List<KeyDescriptionValue<ColumnType>> ();

            if (!parms.ToList ().Exists (p => p == ColumnType.Fixed))
                lst.Add (new KeyDescriptionValue<ColumnType> (ColumnType.Fixed, "FIXO", "0"));
            if (!parms.ToList ().Exists (p => p == ColumnType.Field))
                lst.Add (new KeyDescriptionValue<ColumnType> (ColumnType.Field, "CAMPO", "1"));
            if (!parms.ToList ().Exists (p => p == ColumnType.FieldFromTo))
                lst.Add (new KeyDescriptionValue<ColumnType> (ColumnType.FieldFromTo, "CAMPO DE PARA", "2"));
            if (!parms.ToList ().Exists (p => p == ColumnType.Calculated))
                lst.Add (new KeyDescriptionValue<ColumnType> (ColumnType.Calculated, "CALCULADA", "3"));
            return lst;
        }

        public static List<KeyDescriptionValue<ActionTypeEnum>> GetActionType (params ActionTypeEnum[] parms)
        {
            var lst = new List<KeyDescriptionValue<ActionTypeEnum>> ();

            if (!parms.ToList ().Exists (p => p == ActionTypeEnum.Object))
                lst.Add (new KeyDescriptionValue<ActionTypeEnum> (ActionTypeEnum.Object, "OBJETO", "1"));
            return lst;
        }

        public static List<KeyDescriptionValue<PeriodicityTypeEnum>> GetPeriodicityType (params PeriodicityTypeEnum[] parms)
        {
            var lst = new List<KeyDescriptionValue<PeriodicityTypeEnum>> ();
            if (!parms.ToList ().Exists (p => p == PeriodicityTypeEnum.Immediate))
                lst.Add (new KeyDescriptionValue<PeriodicityTypeEnum> (PeriodicityTypeEnum.Immediate, "IMEDIATO", "0"));
            if (!parms.ToList ().Exists (p => p == PeriodicityTypeEnum.Hourly))
                lst.Add (new KeyDescriptionValue<PeriodicityTypeEnum> (PeriodicityTypeEnum.Hourly, "POR HORA", "4"));
            if (!parms.ToList ().Exists (p => p == PeriodicityTypeEnum.Daily))
                lst.Add (new KeyDescriptionValue<PeriodicityTypeEnum> (PeriodicityTypeEnum.Daily, "DIARIO", "3"));
            if (!parms.ToList ().Exists (p => p == PeriodicityTypeEnum.Weekly))
                lst.Add (new KeyDescriptionValue<PeriodicityTypeEnum> (PeriodicityTypeEnum.Weekly, "SEMANAL", "1"));
            if (!parms.ToList ().Exists (p => p == PeriodicityTypeEnum.Monthly))
                lst.Add (new KeyDescriptionValue<PeriodicityTypeEnum> (PeriodicityTypeEnum.Monthly, "MENSAL", "2"));

            return lst;
        }

        public static List<KeyDescriptionValue<SendingTypeEnum>> GetSendingType (params SendingTypeEnum[] parms)
        {
            var lst = new List<KeyDescriptionValue<SendingTypeEnum>> ();
            if (!parms.ToList ().Exists (p => p == SendingTypeEnum.Email))
                lst.Add (new KeyDescriptionValue<SendingTypeEnum> (SendingTypeEnum.Email, "EMAIL", "E"));
            if (!parms.ToList ().Exists (p => p == SendingTypeEnum.Ftp))
                lst.Add (new KeyDescriptionValue<SendingTypeEnum> (SendingTypeEnum.Ftp, "FTP", "F"));
            if (!parms.ToList ().Exists (p => p == SendingTypeEnum.Internal))
                lst.Add (new KeyDescriptionValue<SendingTypeEnum> (SendingTypeEnum.Internal, "INTERNO", "I"));

            return lst;
        }
    }
}