using Fundo.Applications.Application.Loans.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Fundo.Applications.Application.Loans
{
    public interface ILoanService
    {
        Task<List<LoanDto>> GetAllAsync();
        Task<LoanDto?> GetByIdAsync(Guid id);
        Task<LoanDto> AddAsync(LoanDto loan);
        Task<LoanDto> UpdateAsync(LoanDto loan);
        Task<LoanDto> MakePaymentAsync(Guid id, PaymentRequest loan);
        Task<bool> DeleteAsync(Guid id);
    }
}
