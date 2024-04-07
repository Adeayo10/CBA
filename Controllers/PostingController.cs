using CBA.Models;
using Microsoft.AspNetCore.Mvc;
using CBA.Services;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;

namespace CBA.Controllers;
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
public class PostingController : ControllerBase
{
    private readonly IPostingService _postingService;
    private readonly ILogger<PostingController> _logger;
    public PostingController(IPostingService postingService, ILogger<PostingController> logger)
    {
        _postingService = postingService;
        _logger = logger;
    }
    [HttpPost]
    [Route("Deposit")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<IActionResult> Deposit([FromBody] PostingDTO customerDeposit)
    {
        try
        {
            _logger.LogInformation("Depositing into customer account");
            var result = await _postingService.DepositAsync(customerDeposit);
            if (!result.Status)
            {
                return BadRequest(new CustomerResponse { Message = result.Message, Status = result.Status, Errors = result.Errors });
            }
            return Ok(new CustomerResponse { Message = result.Message, Status = result.Status });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error depositing into customer account");
            return StatusCode(StatusCodes.Status500InternalServerError, new CustomerResponse { Message = ex.Message, Status = false }); 
        }
    }

    [HttpPost]
    [Route("Withdraw")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<IActionResult> Withdraw([FromBody] PostingDTO customerWithdraw)
    {
        try
        {
            _logger.LogInformation("Withdrawing from customer account");
            var result = await _postingService.WithdrawAsync(customerWithdraw);
            if (!result.Status)
            {
                return BadRequest(new CustomerResponse { Message = result.Message, Status = result.Status, Errors = result.Errors });
            }
            return Ok(new CustomerResponse { Message = result.Message, Status = result.Status });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error withdrawing from customer account");
            return StatusCode(StatusCodes.Status500InternalServerError, new CustomerResponse { Message = ex.Message, Status = false });
        }
    }
    
    [HttpPost]
    [Route("Transfer")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<IActionResult> Transfer([FromBody] CustomerTransferDTO customerTransfer)
    {
        try
        {
            _logger.LogInformation("Transferring from customer account");
            var result = await _postingService.TransferAsync(customerTransfer);
            if (!result.Status)
            {
                return BadRequest(new CustomerResponse { Message = result.Message, Status = result.Status, Errors = result.Errors });
            }
            return Ok(new CustomerResponse { Message = result.Message, Status = result.Status });
        }catch (Exception ex)
        {
            _logger.LogError(ex, "Error transferring from customer account");
            return StatusCode(StatusCodes.Status500InternalServerError, new CustomerResponse { Message = ex.Message, Status = false }); 
        }
    }

    [HttpGet]
    [Route("GetPostings")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<IActionResult> GetPostings([FromQuery] int pageNumber, [FromQuery] int pageSize, [FromQuery] string? filterValue = null)
    {
        try
        {
            _logger.LogInformation("Getting all postings");
            var result = await _postingService.GetPostingsAsync(pageNumber, pageSize, filterValue);
            return Ok(new { result });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all postings");
            return StatusCode(StatusCodes.Status500InternalServerError, new CustomerResponse { Message = ex.Message, Status = false });
        }
    }

}




