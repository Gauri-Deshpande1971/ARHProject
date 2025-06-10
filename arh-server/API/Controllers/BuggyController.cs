using API.Errors;
using Core.Entities.Identity;
using Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class BuggyController : BaseApiController
    {
        // private readonly HRServerContext _context;
        // public BuggyController(HRServerContext context)
        // {
        //     _context = context;
        // }

        // [HttpGet("notfound")]
        // public ActionResult GetNotFoundRequest()
        // {
        //     var v = _context.Cities.Find(10);
        //     if (v == null) 
        //     {
        //         return NotFound(new ApiResponse(404));
        //     }

        //     return Ok();
        // }

        // [HttpGet("servererror")]
        // public ActionResult GetServerError()
        // {
        //     var v = _context.Cities.Find(10);
            
        //     var t = v.ToString();

        //     return Ok();
        // }

        [HttpGet("badrequest")]
        public ActionResult GetBadRequest()
        {
            return BadRequest(new ApiResponse(400));
        }

        [HttpGet("badrequest/{id}")]
        public ActionResult GetNotFoundRequest(int id)
        {
            return Ok();
        }

        [HttpGet("testauth")]
        [Authorize]
        public ActionResult<string> GetSecretStuff()
        {
            return "You know the secret";
        }
    }
}
