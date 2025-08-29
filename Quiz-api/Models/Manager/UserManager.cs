using MySql.Data.MySqlClient;
using Quiz_api.Models.Request.Quiz_api.Models;

namespace Quiz_api.Models.Manager
{
    public class UserManager : BaseManager
    {
        /*
        ExecuteReader() → pangkuha ng maraming rows/columns (gamit DataReader).
        ExecuteNonQuery() → pang-execute ng commands na walang result set (INSERT/UPDATE/DELETE), return ay rows affected.
        ExecuteScalar() → pangkuha ng isang value lang (unang column ng unang row).
         */

        public bool CreateUser(UserRegister user)
        {
            var isok = false;
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


        public UpdateResult UserUpdate(UserData user)
        {
            using var conn = GetConnection();
            conn.Open();

            try
            {
                const string getQuery = "SELECT Name, Email, PasswordHash FROM Users WHERE Id = @Id LIMIT 1";
                using var getCmd = new MySqlCommand(getQuery, conn);
                getCmd.Parameters.AddWithValue("@Id", user.Id);


                using var reader = getCmd.ExecuteReader();
                if (!reader.Read()) 
                    return new UpdateResult { Success = false, Message = "User not found" };

                string currentName = reader.GetString("Name");
                string currentEmail = reader.GetString("Email");
                string currentHash = reader.GetString("PasswordHash");
                reader.Close();


                if (!BCrypt.Net.BCrypt.Verify(user.Password, currentHash))
                    return new UpdateResult { Success = false, Message = "Incorrect password" };

                bool nameChanged = !currentName.Equals(user.Name, StringComparison.OrdinalIgnoreCase);
                bool emailChanged = !currentEmail.Equals(user.Email, StringComparison.OrdinalIgnoreCase);
                bool passChanged = !currentHash.Equals(user.Password, StringComparison.OrdinalIgnoreCase);


                if (!nameChanged && !emailChanged && !emailChanged)
                    return new UpdateResult { Success = false, Message = "No changes detected" };

                if (emailChanged)
                {
                    const string checkEmail = "SELECT * FROM Users WHERE Email = @Email AND Id != @Id LIMIT 1";
                    using var checkCmd = new MySqlCommand(checkEmail, conn);
                    checkCmd.Parameters.AddWithValue("@Email", user.Email);
                    checkCmd.Parameters.AddWithValue("@Id", user.Id);

                    using var readerEmail = checkCmd.ExecuteReader();
                    if (readerEmail.HasRows)
                        return new UpdateResult { Success = false, Message = "Email already exists" };
                }


                if (nameChanged)
                {
                    const string checkName = "SELECT * FROM Users WHERE Name = @Name AND Id != @Id";
                    using var checkCmd = new MySqlCommand(checkName, conn);
                    checkCmd.Parameters.AddWithValue("@Name", user.Name);
                    checkCmd.Parameters.AddWithValue("@Id", user.Id);
                    using var readerName = checkCmd.ExecuteReader();

                    if (readerName.HasRows)
                        return new UpdateResult { Success = false, Message = "Name already exists" };
                }

                var updates = new List<string>();
                if (nameChanged) updates.Add("Name = @Name");
                if (emailChanged) updates.Add("Email = @Email");

                string updateQuery = $"UPDATE Users SET {string.Join(", ", updates)} WHERE Id = @Id";

                using var updateCmd = new MySqlCommand(updateQuery, conn);
                updateCmd.Parameters.AddWithValue("@Id", user.Id);
                if (nameChanged) updateCmd.Parameters.AddWithValue("@Name", user.Name);
                if (emailChanged) updateCmd.Parameters.AddWithValue("@Email", user.Email);

                int rows = updateCmd.ExecuteNonQuery();
                return rows > 0
                    ? new UpdateResult { Success = true, Message = "User updated successfully" }
                    : new UpdateResult { Success = false, Message = "No changes made" };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database error: {ex.Message}");
                return new UpdateResult { Success = false, Message = "Database error" };
            }
        }







        #region Helper
        public String IsEmailUnique(String Email)
        {
            using var conn = GetConnection();
            conn.Open();

            string qry = "SELECT 1 FROM Users WHERE Email = @Email LIMIT 1";
            using var sql = new MySqlCommand(qry, conn);
            sql.Parameters.AddWithValue("@Email", Email);

            using var reader = sql.ExecuteReader();

            if (reader.HasRows)
            {
                return "Email is Already Use";
            }
            return null;
        }
        public String IsNameUnique(string Name)
        {
            using var conn = GetConnection();
            conn.Open();

            string qry = "SELECT 1 FROM Users WHERE Name = @Name LIMIT 1";
            using var sql = new MySqlCommand(qry, conn);
            sql.Parameters.AddWithValue("@Name", Name);

            using var reader = sql.ExecuteReader();

            if (reader.HasRows)
            {
                return "Name is Already Use";
            }
            return null;
        }
        #endregion

    }
}
