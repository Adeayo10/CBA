using CBA.Context;
using CBA.Models;
using Microsoft.AspNetCore.Mvc;
using CBA.Services;
using Microsoft.AspNetCore.Authorization;
using Asp.Versioning;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace CBA.Controllers;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
public class LedgerController : ControllerBase
{

    private readonly ILedgerService _ledgerService;

    private readonly UserDataContext _context;

   public LedgerController(ILedgerService ledgerService, UserDataContext context)
   {
        _ledgerService = ledgerService;
        _context = context;
   }

    [MapToApiVersion("1.0")]
    [HttpPost]
    [Route("createGLAccount")]
    [AllowAnonymous]

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

            var autoGeneratedAccountNumber = await _ledgerService.GenerateAccountNumber(ledgerRequestDTO.AccountCategory!);

            var isGLAccountExist = await _ledgerService.IsGLAccountExist(autoGeneratedAccountNumber, ledgerRequestDTO.AccountName!);

            if (isGLAccountExist)
            {
                return BadRequest(new LedgerResponse()
                {
                    Message = "Account already exist",
                    Status = false
                });
            }

            var glAccount = new GLAccounts()
            {
                AccountName = ledgerRequestDTO.AccountName,
                AccountNumber = autoGeneratedAccountNumber,
                AccountCategory = ledgerRequestDTO.AccountCategory,
                AccountDescription = ledgerRequestDTO.AccountDescription,
                AccountStatus = default
            };

            await _context.GLAccounts.AddAsync(glAccount);
            await _context.SaveChangesAsync();

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

    [MapToApiVersion("1.0")]
    [HttpGet]
    [Route("getGLAccounts")]
    [AllowAnonymous]
    public async Task<IActionResult> Get()
    {
        try
        {
            var glAccounts = await _context.GLAccounts.ToListAsync();

            return Ok(new LedgerResponse()
            {
                Message = "Successful",
                Status = true,
                Data = JsonSerializer.Serialize(glAccounts) 
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

    [MapToApiVersion("1.0")]    
    [HttpPut]
    [Route("updateGLAccount")]
    [AllowAnonymous]
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

            var glAccount = await _context.GLAccounts.FirstOrDefaultAsync(x => x.AccountName == ledgerRequestDTO.AccountName);

            if (glAccount == null)
            {
                return BadRequest(new LedgerResponse()
                {
                    Message = "Account does not exist",
                    Status = false
                });
            }

            glAccount.AccountName = ledgerRequestDTO.AccountName;
            glAccount.AccountDescription = ledgerRequestDTO.AccountDescription;
            glAccount.AccountCategory = ledgerRequestDTO.AccountCategory;

            _context.GLAccounts.Update(glAccount);  
            await _context.SaveChangesAsync();

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
}

    