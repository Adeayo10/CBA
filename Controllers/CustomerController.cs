// using Asp.Versioning;
// using CBA.Models;
// using CBA.Services;
// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Mvc;

// namespace CBA.Controllers;
// [ApiVersion("1.0")]
// [Route("api/v{version:apiVersion}/[controller]")]
// [ApiController]
// public class CustomerController : ControllerBase
// {
//     private readonly ICustomerService _customerService;
//     private readonly ILogger<CustomerController> _logger;
//     private readonly IPostingService _postingService;
//     public CustomerController(ICustomerService customerService, ILogger<CustomerController> logger, IPostingService postingService)
//     {
//         _customerService = customerService;
//         _logger = logger;
//         _postingService = postingService;
//         _logger.LogInformation("Customer Controller has been created");
//     }
//     [HttpPost]
//     [Route("createCustomer")]
//     [AllowAnonymous]
//     public async Task<IActionResult> CreateCustomer([FromBody] CustomerDTO customer)
//     {
//         try
//         {
//             _logger.LogInformation("Creating customer");
//             var response = await _customerService.CreateCustomer(customer);
//             if (response.Status)
//             {
//                 _logger.LogInformation("Customer created successfully");
//                 return Ok(new CustomerResponse
//                 {
//                     Message = response.Message,
//                     Status = response.Status,

//                 });
//             }
//             _logger.LogInformation("Customer creation failed");
//             return BadRequest(new CustomerResponse
//             {
//                 Message = response.Message,
//                 Status = response.Status,
//                 Errors = response.Errors
//             });
//         }
//         catch (Exception ex)
//         {
//             _logger.LogError(ex, "An error occurred while creating customer");
//             return StatusCode(500, new CustomerResponse
//             {
//                 Message = "An error occurred while creating customer",
//                 Status = false,
//                 Errors = new List<string> { "An error occurred while creating customer" }
//             });
//         }
//     }

//     [HttpGet]
//     [Route("validateCustomerAccountNumber")]
//     [AllowAnonymous]
//     public async Task<IActionResult> ValidaCustomerAccountNumber(string accountNumber)
//     {
//         try
//         {
//             _logger.LogInformation("Getting customer account number");
//             var response = await _customerService.ValidateCustomerByAccountNumber(accountNumber);
//             if (response.Status)
//             {
//                 _logger.LogInformation("Customer account number retrieved successfully");
//                 return Ok(new CustomerResponse
//                 {
//                     Message = response.Message,
//                     Status = response.Status,
//                     Customer = response.Customer
//                 });
//             }
//             _logger.LogInformation("Customer account number retrieval failed");
//             return BadRequest(new CustomerResponse
//             {
//                 Message = response.Message,
//                 Status = response.Status,
//                 Errors = response.Errors
//             });
//         }
//         catch (Exception ex)
//         {
//             _logger.LogError(ex, "An error occurred while getting customer account number");
//             return StatusCode(500, new CustomerResponse
//             {
//                 Message = "An error occurred while getting customer account number",
//                 Status = false,
//                 Errors = new List<string> { "An error occurred while getting customer account number" }
//             });
//         }
//     }

//     [HttpGet]
//     [Route("getCustomerById")]
//     [AllowAnonymous]
//     public async Task<IActionResult> GetCustomerById(Guid id)
//     {
//         try
//         {
//             _logger.LogInformation("Getting customer by id");
//             var response = await _customerService.GetCustomerById(id);
//             if (response.Status)
//             {
//                 _logger.LogInformation("Customer retrieved successfully");
//                 return Ok(new CustomerResponse
//                 {
//                     Message = response.Message,
//                     Status = response.Status,
//                     Customer = response.Customer
//                 });
//             }
//             _logger.LogInformation("Customer retrieval failed");
//             return BadRequest(new CustomerResponse
//             {
//                 Message = response.Message,
//                 Status = response.Status,
//                 Errors = response.Errors
//             });
//         }
//         catch (Exception ex)
//         {
//             _logger.LogError(ex, "An error occurred while getting customer by id");
//             return StatusCode(500, new CustomerResponse
//             {
//                 Message = "An error occurred while getting customer by id",
//                 Status = false,
//                 Errors = new List<string> { "An error occurred while getting customer by id" }
//             });
//         }
//     }

//     [HttpPut]
//     [Route("updateCustomer")]
//     [AllowAnonymous]
//     public async Task<IActionResult> UpdateCustomer([FromBody] CustomerDTO customer)
//     {
//         try
//         {
//             _logger.LogInformation("Updating customer");
//             var response = await _customerService.UpdateCustomer(customer);
//             if (response.Status)
//             {
//                 _logger.LogInformation("Customer updated successfully");
//                 return Ok(new CustomerResponse
//                 {
//                     Message = response.Message,
//                     Status = response.Status
//                 });
//             }
//             _logger.LogInformation("Customer update failed");
//             return BadRequest(new CustomerResponse
//             {
//                 Message = response.Message,
//                 Status = response.Status,
//                 Errors = response.Errors
//             });
//         }
//         catch (Exception ex)
//         {
//             _logger.LogError(ex, "An error occurred while updating customer");
//             return StatusCode(500, new CustomerResponse
//             {
//                 Message = "An error occurred while updating customer",
//                 Status = false,
//                 Errors = new List<string> { "An error occurred while updating customer" }
//             });
//         }
//     } 

//     [HttpGet]
//     [Route("getCustomers")]
//     [AllowAnonymous]
//     public async Task<IActionResult> GetCustomers()
//     {
//         try
//         {
//             _logger.LogInformation("Getting customers");
//             var response = await _customerService.GetCustomers();
//             if (response.Any())
//             {
//                 _logger.LogInformation("Customers retrieved successfully");
//                 return Ok(response);
//             }
//             _logger.LogInformation("Customers retrieval failed");
//             return BadRequest(new CustomerResponse
//             {
//                 Message = "Customers retrieval failed",
//                 Status = false,
//                 Errors = new List<string> { "Customers retrieval failed" }
//             });
//         }
//         catch (Exception ex)
//         {
//             _logger.LogError(ex, "An error occurred while getting customers");
//             return StatusCode(500, new CustomerResponse
//             {
//                 Message = "An error occurred while getting customers",
//                 Status = false,
//                 Errors = new List<string> { "An error occurred while getting customers" }
//             });
//         }
//     }

//     [HttpGet]
//     [Route("getCustomerAccountBalance")]
//     [AllowAnonymous]
//     public async Task<IActionResult> GetCustomerAccountBalance(Guid id)
//     {
//         try
//         {
//             _logger.LogInformation("Getting customer account balance");
//             var response = await _customerService.GetCustomerAccountBalance(id);
//             if (response is not null)
//             {
//                 _logger.LogInformation("Customer account balance retrieved successfully");
//                 return Ok(new CustomerAccountBalance
//                 {
//                     Balance = response.Balance,
//                     AccountNumber = response.AccountNumber,
//                     AccountName = response.AccountName
//                 });
//             }
//             _logger.LogInformation("Customer account balance retrieval failed");
//             return BadRequest(new CustomerAccountBalance
//             {
//                 Balance = response!.Balance,
//                 AccountNumber = response.AccountNumber,
//                 AccountName = response.AccountName
//             });
//         }catch (Exception ex)
//         {
//             _logger.LogError(ex, "An error occurred while getting customer account balance");
//             return StatusCode(500, new CustomerAccountBalance
//             {
//                 Balance = 0,
//                 AccountNumber = "Not found",
//                 AccountName = "Not found"
//             });
//         }
//     }

//     [HttpPost]
//     [Route("deposit")]
//     [AllowAnonymous]
//     public async Task<IActionResult> Deposit([FromBody] CustomerDepositDTO customerDeposit)
//     {
//         try
//         {
//             _logger.LogInformation("Depositing into customer account");
//             var response = await _postingService.Deposit(customerDeposit);
//             if (response.Status)
//             {
//                 _logger.LogInformation("Deposit successful");
//                 return Ok(new CustomerResponse
//                 {
//                     Message = response.Message,
//                     Status = response.Status
//                 });
//             }
//             _logger.LogInformation("Deposit failed");
//             return BadRequest(new CustomerResponse
//             {
//                 Message = response.Message,
//                 Status = response.Status,
//                 Errors = response.Errors
//             });
//         }
//         catch (Exception ex)
//         {
//             _logger.LogError(ex, "An error occurred while depositing into customer account");
//             return StatusCode(500, new CustomerResponse
//             {
//                 Message = "An error occurred while depositing into customer account",
//                 Status = false,
//                 Errors = new List<string> { "An error occurred while depositing into customer account" }
//             });
//         }
//     }

//     [HttpPost]
//     [Route("withdraw")]
//     [AllowAnonymous]
//     public async Task<IActionResult> Withdraw([FromBody] CustomerWithdrawDTO customerWithdraw)
//     {
//         try
//         {
//             _logger.LogInformation("Withdrawing from customer account");
//             var response = await _postingService.Withdraw(customerWithdraw);
//             if (response.Status)
//             {
//                 _logger.LogInformation("Withdrawal successful");
//                 return Ok(new CustomerResponse
//                 {
//                     Message = response.Message,
//                     Status = response.Status
//                 });
//             }
//             _logger.LogInformation("Withdrawal failed");
//             return BadRequest(new CustomerResponse
//             {
//                 Message = response.Message,
//                 Status = response.Status,
//                 Errors = response.Errors
//             });
//         }
//         catch (Exception ex)
//         {
//             _logger.LogError(ex, "An error occurred while withdrawing from customer account");
//             return StatusCode(500, new CustomerResponse
//             {
//                 Message = "An error occurred while withdrawing from customer account",
//                 Status = false,
//                 Errors = new List<string> { "An error occurred while withdrawing from customer account" }
//             });
//         }
//     }

//     [HttpPost]
//     [Route("transfer")]
//     [AllowAnonymous]
//     public async Task<IActionResult> Transfer([FromBody] CustomerTransferDTO customerTransfer)
//     {
//         try
//         {
//             _logger.LogInformation("Transferring from customer account");
//             var response = await _postingService.Transfer(customerTransfer);
//             if (response.Status)
//             {
//                 _logger.LogInformation("Transfer successful");
//                 return Ok(new CustomerResponse
//                 {
//                     Message = response.Message,
//                     Status = response.Status
//                 });
//             }   
//             _logger.LogInformation("Transfer failed");
//             return BadRequest(new CustomerResponse
//             {
//                 Message = response.Message,
//                 Status = response.Status,
//                 Errors = response.Errors
//             });
        
//         }catch (Exception ex)
//         {
//             _logger.LogError(ex, "An error occurred while transferring from customer account");
//             return StatusCode(500, new CustomerResponse
//             {
//                 Message = "An error occurred while transferring from customer account",
//                 Status = false,
//                 Errors = new List<string> { "An error occurred while transferring from customer account" }
//             });
//         }
//     }
    
// }

   
    
