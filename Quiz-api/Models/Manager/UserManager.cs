using MySql.Data.MySqlClient;
using Quiz_api.Models.Request.Quiz_api.Models;

namespace Quiz_api.Models.Manager
{
    public class UserManager : BaseManager
    {

        public bool CreateUser(UserRegister user)
        {
            var isok = false;
            Console.WriteLine(user.Name);
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                try
                {
                    string query = "INSERT INTO Users (Name,Email,PasswordHash)" +
                        "VALUES (@Name,@Email,@PasswordHash)";
                    MySqlCommand sql = new MySqlCommand(query, conn);
                    sql.Parameters.AddWithValue("@Name", user.Name);
                    sql.Parameters.AddWithValue("@Email", user.Email);

                    string hashedPassword = BCrypt.Net.BCrypt.HashPassword(user.Password);
                    sql.Parameters.AddWithValue("@PasswordHash", hashedPassword);

                    isok = true;
                    sql.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error Query: {ex.Message}");
                }
                finally { conn.Close(); }
            }

            return isok;
        }

        public LoginResult LoginUser(UserLogin user)
        {
            using var conn = GetConnection();
            conn.Open();

            const string query = "SELECT Name, Email, PasswordHash FROM Users WHERE Email = @Email LIMIT 1";
            using var sql = new MySqlCommand(query, conn);
            sql.Parameters.AddWithValue("@Email", user.Email);

            try
            {
                using var reader = sql.ExecuteReader();
                if (!reader.Read())
                {
                    return new LoginResult { User = null, Message = "User not found" };
                }

                string storedHash = reader["PasswordHash"].ToString();

                if (!BCrypt.Net.BCrypt.Verify(user.Password, storedHash))
                {
                    return new LoginResult { User = null, Message = "Incorrect password" };
                }

                return new LoginResult
                {
                    User = new UserData
                    {
                        Name = reader["Name"].ToString(),
                        Email = reader["Email"].ToString()
                    },
                    Message = "Login successful"
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database error: {ex.Message}");
                return new LoginResult { User = null, Message = "Database error" };
            }
        }



        #region Helper
        public String IsEmailUnique(UserRegister user)
        {
            using var conn = GetConnection();
            conn.Open();

            string qry = "SELECT 1 FROM Users WHERE Email = @Email LIMIT 1";
            using var sql = new MySqlCommand(qry, conn);
            sql.Parameters.AddWithValue("@Email", user.Email);

            using var reader = sql.ExecuteReader();

            if (reader.HasRows)
            {
                return "Email is Already Use";
            }
            return null;
        }
        #endregion

    }
}
