using Fundo.Applications.Application.Loans.Models;
using Fundo.Applications.Domain.Entities;
using Fundo.Applications.Infrastructure.Exceptions;
using Fundo.Applications.Infrastructure.Repositories.Loans;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fundo.Applications.Application.Loans
{
    public class LoanService(ILoanRepository loanRepository) : ILoanService
    {
        public async Task<LoanDto> AddAsync(LoanDto loan)
        {
            ValidateLoanDto(loan);

            var createdLoan = new Loan
            {
                Amount = loan.Amount,
                ApplicantName = loan.ApplicantName,
                CurrentBalance = loan.CurrentBalance,
                Status = loan.Status,
            };
            createdLoan = await loanRepository.AddAsync(createdLoan);
            await loanRepository.SaveChangesAsync();

            return createdLoan.Adapt<LoanDto>();
        }

        private void ValidateLoanDto(LoanDto loan)
        {
            if (loan.Amount <= 0)
            {
                throw new InvalidOperationException("Loan amount must be greater than zero.");
            }

            if (string.IsNullOrWhiteSpace(loan.ApplicantName))
            {
                throw new InvalidOperationException("Applicant name cannot be empty.");
            }

            if (loan.CurrentBalance > loan.Amount)
            {
                throw new InvalidOperationException("Current balance cannot be greater than loan amount.");
            }
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            await loanRepository.DeleteAsync(id);
            return await loanRepository.SaveChangesAsync();
        }

        public async Task<List<LoanDto>> GetAllAsync()
        {
            var loans = await loanRepository.GetAllAsync();
            return [.. loans.Select(x => x.Adapt<LoanDto>())];
        }

        public async Task<LoanDto?> GetByIdAsync(Guid id)
        {
            var loan = await loanRepository.GetByIdAsync(id);
            return loan?.Adapt<LoanDto>();
        }

        public async Task<LoanDto> MakePaymentAsync(Guid id, PaymentRequest request)
        {
            var existingLoan = await loanRepository.GetByIdAsync(id);
            if (existingLoan == null) throw new ResourceNotFoundException($"Loan with id {id} not found.");

            ValidatePaymentRequest(existingLoan, request);

            existingLoan.CurrentBalance -= request.Amount;
            if (existingLoan.CurrentBalance <= 0)
            {
                existingLoan.CurrentBalance = 0;
                existingLoan.Status = LoanStatus.Paid;
            }

            await loanRepository.SaveChangesAsync();
            return existingLoan.Adapt<LoanDto>();
        }

        private void ValidatePaymentRequest(Loan loan, PaymentRequest request)
        {
            if (request.Amount <= 0)
            {
                throw new InvalidOperationException("Payment amount must be greater than zero.");
            }

            if (request.Amount > loan.CurrentBalance)
            {
                throw new InvalidOperationException("Amount exceeds current balance.");
            }
        }

        public async Task<LoanDto> UpdateAsync(LoanDto loan)
        {
            if (loan.Id == null)
            {
                throw new InvalidOperationException("Loan ID cannot be null for update operation.");
            }

            var loanEntity = await loanRepository.GetByIdAsync(loan.Id.Value);
            if (loanEntity == null)
            {
                throw new ResourceNotFoundException("Loan not found.");
            }

            loanEntity.Amount = loan.Amount;
            loanEntity.ApplicantName = loan.ApplicantName;
            loanEntity.CurrentBalance = loan.CurrentBalance;
            loanEntity.Status = loan.Status;

            await loanRepository.SaveChangesAsync();

            return loanEntity.Adapt<LoanDto>();
        }
    }
}
