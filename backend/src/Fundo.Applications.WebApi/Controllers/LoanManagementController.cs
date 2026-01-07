using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using Fundo.Applications.Application.Loans;
using System.Collections.Generic;
using Fundo.Applications.Application.Loans.Models;
using Microsoft.AspNetCore.Http;
using Fundo.Applications.Infrastructure.Exceptions;

namespace Fundo.Applications.WebApi.Controllers
{
    [Route("/loans")]
    [ApiController]
    public class LoanManagementController(ILoanService loanService) : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(typeof(List<LoanDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var loans = await loanService.GetAllAsync();
            return Ok(loans);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(LoanDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByIdAsync(Guid id)
        {
            var loan = await loanService.GetByIdAsync(id);
            if (loan == null) return NotFound();
            return Ok(loan);
        }

        [HttpPost]
        [ProducesResponseType(typeof(LoanDto), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateAsync([FromBody] LoanDto input)
        {
            if (input == null) return BadRequest();
            var result = await loanService.AddAsync(input);
            return CreatedAtAction("GetById", new { id = result.Id }, result);
        }

        [HttpPost("{id}/payment")]
        [ProducesResponseType(typeof(LoanDto), StatusCodes.Status202Accepted)]
        [ProducesResponseType(typeof(LoanDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(LoanDto), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(LoanDto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> MakePaymentAsync(Guid id, [FromBody] PaymentRequest req)
        {
            try
            {
                if (req == null || req.Amount <= 0)
                    return BadRequest("Invalid payment amount");

                var result = await loanService.MakePaymentAsync(id, req);
                return Accepted(result);
            }
            catch (Exception ex)
            {
                return ex switch
                {
                    ResourceNotFoundException => NotFound(ex.Message),
                    InvalidOperationException => BadRequest(ex.Message),
                    _ => StatusCode(StatusCodes.Status500InternalServerError, ex.Message)
                };
            }
        }
    }
}