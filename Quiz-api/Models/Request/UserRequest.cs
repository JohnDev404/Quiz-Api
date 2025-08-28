namespace Quiz_api.Models.Request
{
    namespace Quiz_api.Models
    {
        // request
        public class UserRegister
        {
            public string Name { get; set; } = null!;
            public string Email { get; set; } = null!;
            public string Password { get; set; } = null!;
        }

        public class UserLogin
        {
            public string Email { get; set; } = null!;
            public string Password { get; set; } = null!;
        }


        // response
        public class UserData
        {
            public string Name { get; set; } = null!;
            public string Email { get; set; } = null!;
        }
        public class LoginResult
        {
            public UserData? User { get; set; }
            public string Message { get; set; } = string.Empty;
        }

    }

}
