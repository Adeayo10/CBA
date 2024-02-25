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

    public LedgerController(ILedgerService ledgerService)
    {
        _ledgerService = ledgerService;
    }

    [HttpPost]
    [Route("createGLAccount")]
    [Authorize(AuthenticationSchemes = "Bearer")]

    public async Task<IActionResult> Post(LedgerRequestDTO ledgerRequestDTO)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new LedgerResponse()
                {
                    Message = "Invalid request",
                    Status = false
                });
            }
            var response = await _ledgerService.AddGLAccount(ledgerRequestDTO);
            if (response.Status == false)
            {
                return BadRequest(new LedgerResponse()
                {
                    Message = "Account already exist",
                    Status = false
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
                Message = "Account created successfully",
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

    [HttpGet]
    [Route("getGLAccounts")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<IActionResult> GetAll([FromQuery]int pageNumber, [FromQuery]int pageSize/* [FromQuery]string filter*/)
    {
        try
        {
            var response = await _ledgerService.GetGlAccount(pageNumber, pageSize/*, filter*/);
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

    [HttpGet]
    [Route("getGLAccountById")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var response = await _ledgerService.GetGLAccountById(id);
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
            if (!ModelState.IsValid)
            {
                return BadRequest(new LedgerResponse()
                {
                    Message = "Invalid request",
                    Status = false
                });
            }
            var response = await _ledgerService.UpdateGLAccount(ledgerRequestDTO);
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
            if (!ModelState.IsValid)
            {
                return BadRequest(new LedgerResponse()
                {
                    Message = "Invalid request",
                    Status = false
                });
            }
            var response = await _ledgerService.LinkUserToGLAccount(userLedgerDto);
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
    public async Task<IActionResult> UnLinkUserToGLAccount(UserLedgerDto userLedgerDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new LedgerResponse()
                {
                    Message = "Invalid request",
                    Status = false
                });
            }
            var response = await _ledgerService.UnLinkUserToGLAccount(userLedgerDto);
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
            if (!ModelState.IsValid)
            {
                return BadRequest(new LedgerResponse()
                {
                    Message = "Invalid request",
                    Status = false
                });
            }
            var response = await _ledgerService.ChangeAccountStatus(id);
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
                Message = "Account status changed successfully",
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

    [HttpGet]
    [Route("viewLedgerAccountBalance")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<IActionResult> ViewLedgerAccountBalance(string accountNumber)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new LedgerResponse()
                {
                    Message = "Invalid request",
                    Status = false
                });
            }
            var response = await _ledgerService.ViewLedgerAccountBalance(accountNumber);
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
                Message = "Account balance found",
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

}




