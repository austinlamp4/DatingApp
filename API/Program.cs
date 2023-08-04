using API.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//Add Controllers
builder.Services.AddControllers();
builder.Services.AddDbContext<DataContext>(opt => {
    opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")); //Uses builder.Configuration.GetConnectionString to get the connection string from the Config file
}); //Adding our DB service, opt is specifying our options

var app = builder.Build();

//Middleware to map the controllers
app.MapControllers();

app.Run();