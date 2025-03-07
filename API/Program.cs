using API.Data;
using API.Interfaces;
using API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

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
		builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
			.AddJwtBearer(options =>
			{
				string tokenKey = builder.Configuration["TokenKey"] ?? throw new Exception("TokenKey not found");
				options.TokenValidationParameters = new TokenValidationParameters()
				{
					ValidateIssuerSigningKey = true,
					IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey)),
					ValidateIssuer = false,
					ValidateAudience = false
				};
			});

		var app = builder.Build();

		app.UseHttpsRedirection();

		app.UseCors(x => x.AllowAnyHeader().AllowAnyMethod().
			WithOrigins("http://localhost:4200", "https://localhost:4200"));

		app.UseAuthentication();
		app.UseAuthorization();

		app.MapControllers();

		app.Run();
	}
}