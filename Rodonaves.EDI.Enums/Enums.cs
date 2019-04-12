namespace Rodonaves.EDI.Enums
{
    public enum ProviderTypeEnum
    {
        Oracle = 0,
        PostgreSQL = 1
    }

    public enum ProcessTypeEnum : short
    {
        None,
        CONEMB,
        NOTFIS,
        CONEMBDOCCOB,
        DOCCOB,
        OCOREN,
        NFE,
        FREE
    }

    public enum ColumnType : short
    {
        Fixed,
        Field,
        FieldFromTo,
        Calculated
    }
       
    public enum ActionTypeEnum
    {
        Object = 0,
    }

    public enum PeriodicityTypeEnum : short
    {
        Immediate,
        Hourly,
        Daily,
        Weekly,
        Monthly
    }

    public enum SendingTypeEnum
    {
        Email = 1,
        Ftp = 2,
        Internal
    }

    public enum ExecutionTypeEnum
    {
        System,
        Manual
    }

    public enum ProgressStatusEnum
    {
        Unprocessed,
        Started,
        Finished,
        Error,
        Late = 10
    }

    public enum ProviderEnum : short {
        Path,
        OneDriver,
        FTP,
        S3
    }

    public enum DeliveryProgressEnum : short {
        Undelivered = 0,
        DeliveredPartial,
        Delivered,
        NotDelivered
    }
}