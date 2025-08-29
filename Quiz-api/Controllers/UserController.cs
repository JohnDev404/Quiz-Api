using Microsoft.AspNetCore.Mvc;
using Quiz_api.Models.Manager;
using Quiz_api.Models.Request.Quiz_api.Models;

namespace Quiz_api.Controllers
{
    [Route("/api/User")]
    [ApiController]
    public class UserController : Controller
    {
        [HttpGet("test")]
        public IActionResult UserLogin()
        {
            return Ok(new { Message = "Test" });
        }

        [HttpPost("Register")]
        public IActionResult Register([FromBody] UserRegister user)
        {
            var manager = new UserManager();

            var isExsistingName = manager.IsNameUnique(user.Name);
            if (isExsistingName != null)
                return BadRequest(new { Message = isExsistingName });

            var isExisting = manager.IsEmailUnique(user.Email);
            if (isExisting != null)
                return BadRequest(new { Message = isExisting });
           
         
            var result = manager.CreateUser(user);
            if (!result)
                return BadRequest(new { Message = "Error creating user" });

            return Ok(new { Message = "User created successfully" });
        }

        [HttpPost("Login")]
        public IActionResult Login([FromBody] UserLogin user)
        {
            var manager = new UserManager();
            var loginResult = manager.LoginUser(user);

            if (loginResult.User == null)
            {
                return loginResult.Message == "User not found"
                    ? NotFound(new { Message = loginResult.Message })
                    : Unauthorized(new { Message = loginResult.Message });
            }

            // Successful login
            return Ok(new
            {
                Message = loginResult.Message,
                Data = loginResult.User
            });
        }

        [HttpPut("Update")]
        public IActionResult UserUpdate([FromBody] UserData user)
        {
            var manager = new UserManager();
            if (user.Id <= 0)
                return BadRequest(new { Message = "Id is required for update" });
            
            var isokay = manager.UserUpdate(user);
            if (isokay.Success == false)
                return BadRequest(new { Message = isokay.Message });

            return Ok(new { Message = "Update successfully" });


        }
    }
}
