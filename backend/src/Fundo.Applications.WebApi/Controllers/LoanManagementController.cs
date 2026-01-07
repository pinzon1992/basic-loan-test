using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using Fundo.Applications.Application.Loans;
using System.Collections.Generic;
using Fundo.Applications.Application.Loans.Models;
using Microsoft.AspNetCore.Http;
using Fundo.Applications.Infrastructure.Exceptions;
using Microsoft.Extensions.Logging;

namespace Fundo.Applications.WebApi.Controllers
{
    [Route("/loans")]
    [ApiController]
    public class LoanManagementController(ILoanService loanService, ILogger<LoanManagementController> logger) : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(typeof(List<LoanDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var loans = await loanService.GetAllAsync();
                return Ok(loans);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while getting all loans");
                return ex switch
                {
                    _ => StatusCode(StatusCodes.Status500InternalServerError, "An error ocurred getting all loans")
                };
            }
            
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(LoanDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByIdAsync(Guid id)
        {
            try
            {
                var loan = await loanService.GetByIdAsync(id);
                if (loan == null) return NotFound();
                return Ok(loan);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while getting loan by id {LoanId}", id);
                return ex switch
                {
                    _ => StatusCode(StatusCodes.Status500InternalServerError, "An error ocurred getting loan by id")
                };
            }
            
        }

        [HttpPost]
        [ProducesResponseType(typeof(LoanDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateAsync([FromBody] LoanDto input)
        {
            try
            {
                if (input == null) return BadRequest();
                var result = await loanService.AddAsync(input);
                return CreatedAtAction("GetById", new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while creating a new loan");
                return ex switch
                {
                    _ => StatusCode(StatusCodes.Status500InternalServerError, "An error ocurred creating loan")
                };
            }
            
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
                logger.LogError(ex, "Error occurred while making payment for loan id {LoanId}", id);
                return ex switch
                {
                    ResourceNotFoundException => NotFound(ex.Message),
                    InvalidOperationException => BadRequest(ex.Message),
                    _ => StatusCode(StatusCodes.Status500InternalServerError, $"Error occurred while making payment for loan id {id}")
                };
            }
        }
    }
}