using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using Xunit;
using Fundo.Applications.Application.Loans;
using Fundo.Applications.Application.Loans.Models;
using Fundo.Applications.Domain.Entities;
using Fundo.Applications.Infrastructure.Repositories.Loans;
using Fundo.Applications.Infrastructure.Exceptions;
using Fundo.Applications.Application.Loans.Models;

namespace Fundo.Services.Tests.Unit.Services
{
    public class LoanServicesTests
    {
        private readonly Mock<ILoanRepository> _repoMock;
        private readonly LoanService _service;

        public LoanServicesTests()
        {
            _repoMock = new Mock<ILoanRepository>();
            _service = new LoanService(_repoMock.Object);
        }

        [Fact]
        public async Task AddAsync_Should_Call_Repository_And_Return_Dto()
        {
            var dto = new LoanDto
            {
                Amount = 1500m,
                CurrentBalance = 500m,
                ApplicantName = "Maria Silva",
                Status = LoanStatus.Active
            };

            var createdEntity = new Loan
            {
                Id = Guid.NewGuid(),
                Amount = dto.Amount,
                CurrentBalance = dto.CurrentBalance,
                ApplicantName = dto.ApplicantName,
                Status = dto.Status
            };

            _repoMock.Setup(r => r.AddAsync(It.IsAny<Loan>())).ReturnsAsync(createdEntity);
            _repoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);

            var result = await _service.AddAsync(dto);

            Assert.NotNull(result);
            Assert.Equal(createdEntity.Id, result.Id);
            Assert.Equal(createdEntity.Amount, result.Amount);
            Assert.Equal(createdEntity.ApplicantName, result.ApplicantName);
            _repoMock.Verify(r => r.AddAsync(It.IsAny<Loan>()), Times.Once);
            _repoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task GetAllAsync_Should_Return_All_Loans()
        {
            var loans = new List<Loan>
            {
                new Loan { Id = Guid.NewGuid(), Amount = 1000m, CurrentBalance = 200m, ApplicantName = "A", Status = LoanStatus.Active },
                new Loan { Id = Guid.NewGuid(), Amount = 2000m, CurrentBalance = 0m, ApplicantName = "B", Status = LoanStatus.Paid }
            };

            _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(loans);

            var result = await _service.GetAllAsync();

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            _repoMock.Verify(r => r.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_Should_Return_Loan_When_Found()
        {
            var id = Guid.NewGuid();
            var loan = new Loan { Id = id, Amount = 500m, CurrentBalance = 100m, ApplicantName = "C", Status = LoanStatus.Active };

            _repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(loan);

            var result = await _service.GetByIdAsync(id);

            Assert.NotNull(result);
            Assert.Equal(id, result!.Id);
            _repoMock.Verify(r => r.GetByIdAsync(id), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_Should_Return_Null_When_NotFound()
        {
            var id = Guid.NewGuid();
            _repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Loan?)null);

            var result = await _service.GetByIdAsync(id);

            Assert.Null(result);
            _repoMock.Verify(r => r.GetByIdAsync(id), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_Should_Throw_When_Id_Null()
        {
            var dto = new LoanDto
            {
                Id = null,
                Amount = 100m,
                CurrentBalance = 50m,
                ApplicantName = "D",
                Status = LoanStatus.Active
            };

            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.UpdateAsync(dto));
        }

        [Fact]
        public async Task UpdateAsync_Should_Update_And_Return_Dto()
        {
            var id = Guid.NewGuid();
            var existing = new Loan { Id = id, Amount = 100m, CurrentBalance = 50m, ApplicantName = "D", Status = LoanStatus.Active };

            var dto = new LoanDto
            {
                Id = id,
                Amount = 150m,
                CurrentBalance = 0m,
                ApplicantName = "D Updated",
                Status = LoanStatus.Paid
            };

            _repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(existing);
            _repoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);

            var result = await _service.UpdateAsync(dto);

            Assert.NotNull(result);
            Assert.Equal(id, result.Id);
            Assert.Equal(dto.Amount, result.Amount);
            Assert.Equal(dto.ApplicantName, result.ApplicantName);
            Assert.Equal(dto.Status, result.Status);
            _repoMock.Verify(r => r.GetByIdAsync(id), Times.Once);
            _repoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_Should_Call_Delete_And_SaveChanges()
        {
            var id = Guid.NewGuid();

            _repoMock.Setup(r => r.DeleteAsync(id)).ReturnsAsync(true);
            _repoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);

            var result = await _service.DeleteAsync(id);

            Assert.True(result);
            _repoMock.Verify(r => r.DeleteAsync(id), Times.Once);
            _repoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_Should_Return_False_When_SaveChanges_Fails()
        {
            var id = Guid.NewGuid();

            _repoMock.Setup(r => r.DeleteAsync(id)).ReturnsAsync(true);
            _repoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(false);

            var result = await _service.DeleteAsync(id);

            Assert.False(result);
            _repoMock.Verify(r => r.DeleteAsync(id), Times.Once);
            _repoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task AddAsync_Should_Throw_When_Amount_Is_NonPositive()
        {
            var dto = new LoanDto
            {
                Amount = 0m,
                CurrentBalance = 0m,
                ApplicantName = "Test",
                Status = LoanStatus.Active
            };

            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.AddAsync(dto));
            _repoMock.Verify(r => r.AddAsync(It.IsAny<Loan>()), Times.Never);
        }

        [Fact]
        public async Task AddAsync_Should_Throw_When_ApplicantName_Is_Empty()
        {
            var dto = new LoanDto
            {
                Amount = 100m,
                CurrentBalance = 0m,
                ApplicantName = "",
                Status = LoanStatus.Active
            };

            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.AddAsync(dto));
            _repoMock.Verify(r => r.AddAsync(It.IsAny<Loan>()), Times.Never);
        }

        [Fact]
        public async Task AddAsync_Should_Throw_When_CurrentBalance_Greater_Than_Amount()
        {
            var dto = new LoanDto
            {
                Amount = 100m,
                CurrentBalance = 150m,
                ApplicantName = "Test",
                Status = LoanStatus.Active
            };

            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.AddAsync(dto));
            _repoMock.Verify(r => r.AddAsync(It.IsAny<Loan>()), Times.Never);
        }

        [Fact]
        public async Task MakePaymentAsync_Should_Reduce_Balance_And_SaveChanges()
        {
            var id = Guid.NewGuid();
            var existing = new Loan { Id = id, Amount = 1500m, CurrentBalance = 500m, ApplicantName = "Payer", Status = LoanStatus.Active };

            _repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(existing);
            _repoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);

            var req = new PaymentRequest { Amount = 100m };

            var result = await _service.MakePaymentAsync(id, req);

            Assert.NotNull(result);
            Assert.Equal(400m, result.CurrentBalance);
            Assert.Equal(LoanStatus.Active, result.Status);
            _repoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task MakePaymentAsync_Should_Set_Status_Paid_When_PaidOff()
        {
            var id = Guid.NewGuid();
            var existing = new Loan { Id = id, Amount = 1000m, CurrentBalance = 100m, ApplicantName = "Payer", Status = LoanStatus.Active };

            _repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(existing);
            _repoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);

            var req = new PaymentRequest { Amount = 100m };

            var result = await _service.MakePaymentAsync(id, req);

            Assert.NotNull(result);
            Assert.Equal(0m, result.CurrentBalance);
            Assert.Equal(LoanStatus.Paid, result.Status);
            _repoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task MakePaymentAsync_Should_Throw_When_Loan_NotFound()
        {
            var id = Guid.NewGuid();
            _repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Loan?)null);

            await Assert.ThrowsAsync<ResourceNotFoundException>(() => _service.MakePaymentAsync(id, new PaymentRequest { Amount = 50m }));
        }

        [Fact]
        public async Task MakePaymentAsync_Should_Throw_When_InvalidAmount()
        {
            var id = Guid.NewGuid();
            var existing = new Loan { Id = id, Amount = 500m, CurrentBalance = 200m, ApplicantName = "Payer", Status = LoanStatus.Active };
            _repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(existing);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.MakePaymentAsync(id, new PaymentRequest { Amount = 0m }));
        }

        [Fact]
        public async Task MakePaymentAsync_Should_Throw_When_Amount_Exceeds_Balance()
        {
            var id = Guid.NewGuid();
            var existing = new Loan { Id = id, Amount = 500m, CurrentBalance = 50m, ApplicantName = "Payer", Status = LoanStatus.Active };
            _repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(existing);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.MakePaymentAsync(id, new PaymentRequest { Amount = 100m }));
        }
    }
}