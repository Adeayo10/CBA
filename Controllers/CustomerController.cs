using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
//using Microsoft.VisualBasic;

namespace CBA.Controllers;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
public class CustomerController : ControllerBase
{
    [MapToApiVersion("1.0")]
    [HttpGet]
    [Authorize(Roles = "Manager")]
    public IEnumerable<string> Get(){
                 
        var listofStrings = new string[]{"John Doe", "Jane Doe"};

         

        return listofStrings;
    }

    
}
