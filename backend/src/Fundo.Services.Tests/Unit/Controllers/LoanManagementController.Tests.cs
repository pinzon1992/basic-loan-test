using Xunit;
using Moq;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Fundo.Applications.WebApi.Controllers;
using Fundo.Applications.Application.Loans;
using System.Collections.Generic;
using Fundo.Applications.Application.Loans.Models;
using Fundo.Applications.Infrastructure.Exceptions;

namespace Fundo.Services.Tests.Unit.Controllers
{
    public class LoanManagementControllerTests
    {
        private readonly Mock<ILoanService> _loanService = new();
        private readonly Mock<ILogger<LoanManagementController>> _logger = new();

        [Fact]
        public async Task GetAll_ReturnsOkWithList()
        {
            _loanService.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<LoanDto>());

            var controller = new LoanManagementController(_loanService.Object, _logger.Object);
            var result = await controller.GetAll();

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task GetById_ReturnsOk_WhenFound()
        {
            var id = Guid.NewGuid();
            _loanService.Setup(s => s.GetByIdAsync(id)).ReturnsAsync(new LoanDto { Id = id });

            var controller = new LoanManagementController(_loanService.Object, _logger.Object);
            var result = await controller.GetByIdAsync(id);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenMissing()
        {
            var id = Guid.NewGuid();
            _loanService.Setup(s => s.GetByIdAsync(id)).ReturnsAsync((LoanDto?)null);

            var controller = new LoanManagementController(_loanService.Object, _logger.Object);
            var result = await controller.GetByIdAsync(id);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Create_ReturnsCreated_WhenValid()
        {
            var input = new LoanDto { Id = Guid.NewGuid(), ApplicantName = "Test" };
            _loanService.Setup(s => s.AddAsync(It.IsAny<LoanDto>())).ReturnsAsync(input);

            var controller = new LoanManagementController(_loanService.Object, _logger.Object);
            var result = await controller.CreateAsync(input);

            var created = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(201, created.StatusCode);
        }

        [Fact]
        public async Task Create_ReturnsBadRequest_WhenNull()
        {
            var controller = new LoanManagementController(_loanService.Object, _logger.Object);
            var result = await controller.CreateAsync(null!);

            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task MakePayment_ReturnsAccepted_OnSuccess()
        {
            var id = Guid.NewGuid();
            var req = new PaymentRequest { Amount = 100 };
            _loanService.Setup(s => s.MakePaymentAsync(id, req)).ReturnsAsync(new LoanDto { Id = id });

            var controller = new LoanManagementController(_loanService.Object, _logger.Object);
            var result = await controller.MakePaymentAsync(id, req);

            Assert.IsType<AcceptedResult>(result);
        }

        [Fact]
        public async Task MakePayment_ReturnsBadRequest_OnInvalidAmount()
        {
            var id = Guid.NewGuid();
            var req = new PaymentRequest { Amount = 0 };

            var controller = new LoanManagementController(_loanService.Object, _logger.Object);
            var result = await controller.MakePaymentAsync(id, req);

            var bad = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, bad.StatusCode);
        }

        [Fact]
        public async Task MakePayment_ReturnsNotFound_WhenServiceThrowsNotFound()
        {
            var id = Guid.NewGuid();
            var req = new PaymentRequest { Amount = 100 };
            _loanService.Setup(s => s.MakePaymentAsync(id, req)).ThrowsAsync(new ResourceNotFoundException("not found"));

            var controller = new LoanManagementController(_loanService.Object, _logger.Object);
            var result = await controller.MakePaymentAsync(id, req);

            Assert.IsType<NotFoundObjectResult>(result);
        }
    }
}