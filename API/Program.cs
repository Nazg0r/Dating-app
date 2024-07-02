using API.Data;
using API.Interfaces;
using API.Services;
using Microsoft.EntityFrameworkCore;

internal class Program
{
	private static void Main(string[] args)
	{
		var builder = WebApplication.CreateBuilder(args);

		// Add services to the container.

		builder.Services.AddControllers();
		builder.Services.AddCors();
		builder.Services.AddDbContext<DataContext>(opt =>
		{
			opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
		});
		builder.Services.AddScoped<ITokenService, TokenService>();

		var app = builder.Build();

		app.UseHttpsRedirection();

		app.UseAuthorization();

		app.UseCors(x => x.AllowAnyHeader().AllowAnyMethod().
			WithOrigins("http://localhost:4200", "https://localhost:4200"));

		app.MapControllers();

		app.Run();
	}
}