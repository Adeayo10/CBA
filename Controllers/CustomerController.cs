using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
//using Microsoft.VisualBasic;

namespace CBA.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomerController : ControllerBase
{
    [HttpGet]
    [Authorize(Roles = "Manager")]
    public IEnumerable<string> Get(){
                 
        var listofStrings = new string[]{"John Doe", "Jane Doe"};

         

        return listofStrings;
    }

    
}
