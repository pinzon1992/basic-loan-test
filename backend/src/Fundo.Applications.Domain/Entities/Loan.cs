namespace Fundo.Applications.Domain.Entities
{
    public class Loan : BaseEntity
    {
        public decimal Amount { get; set; }
        public decimal CurrentBalance { get; set; }
        public string ApplicantName { get; set; }
        public LoanStatus Status { get; set; }
    }
}
