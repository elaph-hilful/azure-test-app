using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

namespace SimpleBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly string _connectionString = "Server=<MYSQL_HOST>;Database=<DATABASE_NAME>;User=<USER>;Password=<PASSWORD>;";

        [HttpGet]
        public IActionResult GetUsers()
        {
            var users = new List<object>();
            try
            {
                using var connection = new MySqlConnection(_connectionString);
                connection.Open();

                var command = new MySqlCommand("SELECT name, job_title, company FROM users", connection);
                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    users.Add(new
                    {
                        Name = reader.GetString(0),
                        JobTitle = reader.GetString(1),
                        Company = reader.GetString(2)
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Database error: {ex.Message}");
            }

            return Ok(users);
        }

        [HttpPost]
        public IActionResult AddUser([FromBody] User user)
        {
            try
            {
                using var connection = new MySqlConnection(_connectionString);
                connection.Open();

                var command = new MySqlCommand("INSERT INTO users (name, job_title, company) VALUES (@name, @jobTitle, @company)", connection);
                command.Parameters.AddWithValue("@name", user.Name);
                command.Parameters.AddWithValue("@jobTitle", user.JobTitle);
                command.Parameters.AddWithValue("@company", user.Company);

                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Database error: {ex.Message}");
            }

            return Ok("User added successfully");
        }
    }

    public class User
    {
        public string Name { get; set; }
        public string JobTitle { get; set; }
        public string Company { get; set; }
    }
}
