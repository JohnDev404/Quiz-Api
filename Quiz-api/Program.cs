using Quiz_api.Models.Manager;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

var db_host = Environment.GetEnvironmentVariable("DB_HOST");
var db_port = Environment.GetEnvironmentVariable("DB_PORT");
var db_name = Environment.GetEnvironmentVariable("DB_NAME");
var db_user = Environment.GetEnvironmentVariable("DB_USER");
var db_password = Environment.GetEnvironmentVariable("DB_PASS");

var conn = $"Server={db_host};Port={db_port};Database={db_name};Uid={db_user};" +
                $"Pwd={db_password};Convert Zero Datetime=True";

BaseManager.ConnectionString = conn;

using var connection = BaseManager.GetConnection();
try
{
    connection.Open();
    Console.WriteLine("Database connection successful!");
}
catch (Exception ex)
{
    Console.WriteLine($"Database connection failed: {ex.Message}");
}


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.MapGet("/", () => "hello apo");
app.Run();
