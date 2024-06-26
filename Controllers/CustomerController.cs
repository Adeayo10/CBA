using Asp.Versioning;
using CBA.Models;
using CBA.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace CBA.Controllers;
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
public class CustomerController : ControllerBase
{
    private readonly ICustomerService _customerService;
    private readonly ILogger<CustomerController> _logger;
    public CustomerController(ICustomerService customerService, ILogger<CustomerController> logger)
    {
        _customerService = customerService;
        _logger = logger;
        _logger.LogInformation("Customer Controller has been created");
    }
    [HttpPost]
    [Route("CreateCustomer")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<IActionResult> CreateCustomer([FromBody] CustomerDTO customer)
    {
        try
        {
            _logger.LogInformation("Creating customer");
            var result = await _customerService.CreateCustomerAsync(customer);
            if (!result.Status)
            {
                return BadRequest(new CustomerResponse { Message = result.Message, Errors = result.Errors });
            }
            return Ok(new CustomerResponse { Message = result.Message, Status = result.Status, Customer = result.Customer, Data = result.Data });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating customer");
            return StatusCode(500, ex.Message);
        }
    }
    [HttpPut]
    [Route("UpdateCustomer")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<IActionResult> UpdateCustomer([FromBody] CustomerDTO customer)
    {
        try
        {
            _logger.LogInformation("Updating customer");
            var result = await _customerService.UpdateCustomerAsync(customer);
            if (!result.Status)
            {
                return BadRequest(new CustomerResponse { Message = result.Message, Status = result.Status, Errors = result.Errors });
            }
            return Ok(new CustomerResponse { Message = result.Message, Status = result.Status, Customer = result.Customer, Data = result.Data });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating customer");
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet]
    [Route("GetCustomer")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<IActionResult> Get(Guid id)
    {
        try
        {
            _logger.LogInformation("Getting customer");
            var result = await _customerService.GetCustomerByIdAsync(id);
            if (!result.Status)
            {
                return BadRequest(new CustomerResponse { Message = result.Message, Status = result.Status, Errors = result.Errors });
            }
            return Ok(new CustomerResponse { Message = result.Message, Status = result.Status, Customer = result.Customer, Data = result.Data });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting customer");
            return BadRequest(ex.Message);
        }

    }

    [HttpGet]
    [Route("GetAllCustomers")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<IActionResult> GetAll([FromQuery] int pageNumber, [FromQuery] int pageSize, [FromQuery] string? filterValue)
    {
        try
        {
            _logger.LogInformation("Getting all customers");
            var result = await _customerService.GetCustomersAsync(pageNumber, pageSize, filterValue);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all customers");
            return StatusCode(500, ex.Message);
        }

    }

    [HttpPost]
    [Route("ValidateCustomerAccountNumber")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<IActionResult> ValidateCustomerAccountNumber([FromBody] CustomerAccountRequestDTO accountRequest)
    {
        string accountNumber = accountRequest.AccountNumber;
        try
        {
            _logger.LogInformation("Validating customer account number");
            var result = await _customerService.ValidateCustomerByAccountNumberAsync(accountNumber);
            if (!result.Status)
            {
                return BadRequest(new CustomerResponse { Message = result.Message, Status = result.Status, Errors = result.Errors });
            }
            return Ok(new CustomerResponse { Message = result.Message, Status = result.Status, Customer = result.Customer, Data = result.Data });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating customer account number");
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet]
    [Route("GetCustomerAccountBalance")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<IActionResult> GetCustomerAccountBalance(Guid id)
    {
        try
        {
            _logger.LogInformation("Getting customer account balance");
            var result = await _customerService.GetCustomerAccountBalanceAsync(id);
            if (!result.Status)
            {
                return BadRequest(new CustomerResponse { Message = result.Message, Status = result.Status, Errors = result.Errors });
            }
            return Ok(new CustomerResponse { Message = result.Message, Status = result.Status, Customer = result.Customer, Data = result.Data });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting customer account balance");
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPost]
    [Route("ChangeCustomerAccountStatus")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<IActionResult> ChangeCustomerAccountStatus([FromBody] CustomerRequestDTO request)
    {
        try
        {
            _logger.LogInformation("Changing customer account status");
            var result = await _customerService.ChangeAccountStatusAsync(request.CustomerId);
            if (!result.Status)
            {
                return BadRequest(new CustomerResponse { Message = result.Message, Status = result.Status, Errors = result.Errors });
            }
            return Ok(new CustomerResponse { Message = result.Message, Status = result.Status, Customer = result.Customer, Data = result.Data });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error changing customer account status");
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet]
    [Route("AccountTypes")]
    [AllowAnonymous]
    public IActionResult AccountTypes()
    {
        try
        {
            _logger.LogInformation("Getting account types");
            var result = _customerService.GetAccountTypes();
            return Ok(new { result });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting account types");
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPost]
    [Route("GetCustomerTransactions")]
    //[Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<IActionResult> GetCustomerTransactions(TransactionDTO
    transaction)
    {
        try
        {
            _logger.LogInformation("Getting customer transactions");
            var result = await _customerService.GetTransactionsAsync(transaction);
            _logger.LogInformation($"{result}");
            if (!result.Status)
            {
                return Ok(new TransactionResponse { Message = result.Message, Status = result.Status, Errors = result.Errors });
            }
            return Ok(new TransactionResponse { Message = result.Message, Status = result.Status, Transactions = result.Transactions, EndDate = result.EndDate, StartDate = result.StartDate });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting customer transactions");
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPost]
    [Route("CreateAccountStatementPdf")]
    // [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<IActionResult> GetAccountStatementPdfAsync(TransactionDTO transaction)
    {
        try
        {
            _logger.LogInformation("Creating account statement pdf");

            var file = await _customerService.GetAccountStatementPdfAsync(transaction);
            if (file is null)
            {
                return BadRequest(new CustomerResponse { Message = "Error creating account statement pdf", Status = false });
            }
            var pdfStatement = File(file.FileContents, "application/pdf", "AccountStatement.pdf");
            return pdfStatement;

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating account statement pdf");
            return StatusCode(500, new CustomerResponse { Message = ex.Message, Status = false });

        }
    }


}



