using Fundo.Applications.Domain.Entities;
using Fundo.Applications.Infrastructure.Common;
using Fundo.Applications.Infrastructure.Data;

namespace Fundo.Applications.Infrastructure.Repositories.Loans
{
    public class LoanRepository(AppDbContext context): BaseRepository<Loan>(context), ILoanRepository
    {
    }
}
