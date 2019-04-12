namespace Rodonaves.EDI.WebAPI.Requests
{
    public class LayoutHeaderRequest : PagedRequest
    {
        public int? id { get; set; }
        public long? layoutGroupId { get; set; }
        public string description { get; set; }
        public bool? active { get; set; }
        public long? scriptId { get; set; }
        public string processType { get; set; }
    }
}
