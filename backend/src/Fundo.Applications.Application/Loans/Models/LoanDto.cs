using Fundo.Applications.Application.Common;
using Fundo.Applications.Domain.Entities;

namespace Fundo.Applications.Application.Loans.Models
{
    public class LoanDto : BaseDto
    {
        public decimal Amount { get; set; }
        public decimal CurrentBalance { get; set; }
        public string ApplicantName { get; set; }
        public LoanStatus Status { get; set; }
    }
}
