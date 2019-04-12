using System;
using System.Collections.Generic;
using System.Text;

namespace Rodonaves.EDI.Enums
{
    public enum ExecutionEventCodeEnum
    {
        ProcessStarted = 100,
        QueuingForTranslation = 101,
        QueuedForTranslation = 102,
        QueuingFailedForTranslation = 103,
        ProcessingFailure = 200,
        ProcessConcluded = 202,
        Dequeued,
        TranslationOfTheLayoutStarted ,
        TranslationOfTheCompletedLayout,
        LayoutTranslationFailed,
        StartingFileGeneration,
        FileGeneratedSuccessfully,
        FailedToGenerateTheFile,
        QueuingForShipping = 300,
        QueuedForShipping = 301,
        QueuingFailedToSend = 302,
        Sent = 300
    }
}
