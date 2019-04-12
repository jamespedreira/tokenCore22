namespace Rodonaves.EDI.Helpers.DTO
{
    public class ComlumnHeaderForeignKey : ColumnHeader
    {
        public bool isForeignKey { get; set; } = true;

        public string ReferencedBy { get; set; }
    }
}
