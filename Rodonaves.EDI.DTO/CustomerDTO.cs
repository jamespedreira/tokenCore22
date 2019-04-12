namespace Rodonaves.EDI.DTO
{
    public class CustomerDTO : BaseDTO
    {
        public int IdPerson { get; set; }
        public long CompanyId { get; set; }

        public PersonDTO Person { get; set; }
    }
}
