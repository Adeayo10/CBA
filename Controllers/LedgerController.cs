using CBA.Models;
using Microsoft.AspNetCore.Mvc;
using CBA.Services;
using Microsoft.AspNetCore.Authorization;
using Asp.Versioning;

namespace CBA.Controllers;
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
public class LedgerController : ControllerBase
{
    private readonly ILedgerService _ledgerService;
    private readonly ILogger<LedgerController> _logger;

    public LedgerController(ILedgerService ledgerService, ILogger<LedgerController> logger)
    {
        _ledgerService = ledgerService;
        _logger = logger;
        _logger.LogInformation("Ledger Controller has been created");
    }
    [HttpPost]
    [Route("createGLAccount")]
    [Authorize(AuthenticationSchemes = "Bearer")]

    public async Task<IActionResult> Post(LedgerRequestDTO ledgerRequestDTO)
    {
        try
        {
            _logger.LogInformation("Creating account");
            var response = await _ledgerService.AddGLAccountAsync(ledgerRequestDTO);
            if (response.Status == false)
            {
                return BadRequest(new LedgerResponse()
                {
                    Message = response.Message,
                    Status = response.Status
                });
            } 
            // return CreatedAtAction(nameof(GetAll), new { id = response.Data!.AccountNumber }, new LedgerResponse()
            // {
            //     Message = "Account created successfully",
            //     Status = true,
            //     Data = response.Data
            // });
            return Ok(new LedgerResponse()
            {
                Message = response.Message,
                Status = response.Status,
                
            });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new LedgerResponse()
            {
                Message = ex.Message,
                Status = false
            });
        }
    }

    [HttpGet]
    [Route("getGLAccounts")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<IActionResult> GetAll([FromQuery]int pageNumber, [FromQuery]int pageSize/* [FromQuery]string filter*/)
    {
        try
        {
            _logger.LogInformation("Getting all accounts");
            var response = await _ledgerService.GetGlAccountsAsync(pageNumber, pageSize/*, filter*/);
            if (response.Status == false)
            {
                return NotFound(new LedgerResponse()
                {
                    Message = "No account found",
                    Status = false
                });
            }
            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new LedgerResponse()
            {
                Message = ex.Message,
                Status = false
            });
        }
    }

    [HttpGet]
    [Route("getGLAccountById")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            _logger.LogInformation("Getting account by id");
            var response = await _ledgerService.GetGLAccountByIdAsync(id);
            if (response.Status == false)
            {
                return NotFound(new LedgerResponse()
                {
                    Message = "No account found",
                    Status = false
                }); 
            }
            return Ok(new LedgerResponse()
            {
                Message = "Account found",
                Status = true,
                Data = response.Data
            });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new LedgerResponse()
            {
                Message = ex.Message,
                Status = false
            });
        }
    }

    [HttpPut]
    [Route("updateGLAccount")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<IActionResult> Update(LedgerRequestDTO ledgerRequestDTO)
    {
        try
        {
            _logger.LogInformation("Updating account");   
            var response = await _ledgerService.UpdateGLAccountAsync(ledgerRequestDTO);
            if (response.Status == false)
            {
                return BadRequest(new LedgerResponse()
                {
                    Message = "Account does not exist",
                    Status = false
                });
            }
            return Ok(new LedgerResponse()
            {
                Message = "Account updated successfully",
                Status = true
            });

        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new LedgerResponse()
            {
                Message = ex.Message,
                Status = false
            });
        }
    }
    [HttpPost]
    [Route("linkUserToGLAccount")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<IActionResult> LinkUserToGLAccount(UserLedgerDto userLedgerDto)
    {
        try
        {
            _logger.LogInformation("Linking user to account");
            var response = await _ledgerService.LinkUserToGLAccountAsync(userLedgerDto);
            if (response.Status == false)
            {
                return BadRequest(new LedgerResponse()
                {
                    Message = "User does not exist",
                    Status = false
                });
            }
            return Ok(new LedgerResponse()
            {
                Message = "User linked to account successfully",
                Status = true
            });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new LedgerResponse()
            {
                Message = ex.Message,
                Status = false
            });
        }
    }

    [HttpPost]
    [Route("unLinkUserToGLAccount")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<IActionResult> UnLinkUserToGLAccount(UserLedgerid userLedgerid)
    {
        try
        {
            _logger.LogInformation("Unlinking user to account");
            var response = await _ledgerService.UnLinkUserToGLAccountAsync(userLedgerid);
            if (response.Status == false)
            {
                return BadRequest(new LedgerResponse()
                {
                    Message = "User does not exist",
                    Status = false
                });
            }
            return Ok(new LedgerResponse()
            {
                Message = "User unlinked from account successfully",
                Status = true
            });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new LedgerResponse()
            {
                Message = ex.Message,
                Status = false
            });
        }
    }

    [HttpPost]
    [Route("changeAccountStatus")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<IActionResult> ChangeAccountStatus(int id)
    {
        try
        {
            _logger.LogInformation("Changing account status");
            var response = await _ledgerService.ChangeAccountStatusAsync(id);
            if (response.Status == false)
            {
                return BadRequest(new LedgerResponse()
                {
                    Message = "Account does not exist",
                    Status = false
                });
            }
            return Ok(new LedgerResponse()
            {
                Message = response.Message,
                Status = response.Status
            });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new LedgerResponse()
            {
                Message = ex.Message,
                Status = false
            });
        }
    }

    [HttpGet]
    [Route("viewLedgerAccountBalance")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<IActionResult> ViewLedgerAccountBalance(string accountNumber)
    {
        try
        {
            _logger.LogInformation("Viewing account balance");
            var response = await _ledgerService.ViewLedgerAccountBalanceAsync(accountNumber);
            if (response.Status == false)
            {
                return BadRequest(new LedgerResponse()
                {
                    Message = response.Message,
                    Status = response.Status
                });
            }
            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new LedgerResponse()
            {
                Message = ex.Message,
                Status = false
            });
        }
    }

    [HttpGet]
    [Route("getLedgerAccountByCategory")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public  IActionResult GetLedgerAccountByCategory()
    {
        try
        {
            _logger.LogInformation("Getting account by category");
            var response = _ledgerService.GetLedgerAccountByCategory();
            return Ok(new { response });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new LedgerResponse()
            {
                Message = ex.Message,
                Status = false
            });
        }

    }
    [HttpPost]
    [Route("validateLinkedUser")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<IActionResult> ValidateLinkedUser(ValidateLinkedUserDTO validateLinkedUserDTO)
    {
        try
        {
            _logger.LogInformation("Validating linked user");
            var response = await _ledgerService.ValidateLinkedUser(validateLinkedUserDTO);
            if (response == false)
            {
                return BadRequest(new LedgerResponse()
                {
                    Message = "User does not exist",
                    Status = false
                });
            }
            return Ok(new LedgerResponse()
            {
                Message = "User linked to account successfully",
                Status = true
            });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new LedgerResponse()
            {
                Message = ex.Message,
                Status = false
            });
        }
    }

}




