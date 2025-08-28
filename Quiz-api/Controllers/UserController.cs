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
            var isExisting = manager.IsEmailUnique(user);
            if (isExisting != null)
            {
                return BadRequest(new { Message = isExisting });
            }

            var result = manager.CreateUser(user);
            if (!result)
            {
                return BadRequest(new { Message = "Error creating user" });
            }
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


    }
}
