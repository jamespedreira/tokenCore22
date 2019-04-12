namespace Rodonaves.EDI.WebAPI.Reponses
{
    public class QueueStatusResponse
    {
        public int Unprocessed { get; set; }
        public int Finished { get; set; }
        public int Started { get; set; }
    }
}