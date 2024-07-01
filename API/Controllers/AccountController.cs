using API.Data;
using API.DTOs;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;

namespace API.Controllers
{
	public class AccountController : BaseApiController
	{
		private readonly DataContext _dbContext;

		public AccountController(DataContext dbContext)
        {
			_dbContext = dbContext;
		}

        [HttpPost("register")] // api/account/register 
		public async Task<ActionResult<User>> Register(RegisterDTO registerDTO) {

			using HMACSHA512 hmac = new HMACSHA512();

			if (await UserExist(registerDTO.name)) return BadRequest("Selected name is already used");

			User user = new User
			{
				Name = registerDTO.name,
				Password = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDTO.password)),
				Salt = hmac.Key
			};

			_dbContext.Users.Add(user);
			await _dbContext.SaveChangesAsync();

			return Ok(user);
		}

		[HttpPost("login")]
		public async Task<ActionResult<User>> Login(LoginDTO loginDTO)
		{
			if (!await UserExist(loginDTO.name)) return Unauthorized("Invalid username");

			User user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Name == loginDTO.name);

			if (!await CheckPassword(user, loginDTO.password)) return Unauthorized("Invalid password");

			return Ok(user);
		}

		private async Task<bool> UserExist(string name)
		{
			return await _dbContext.Users.AnyAsync( u => u.Name.ToLower() == name.ToLower());
		}

		private async Task<bool> CheckPassword(User user, string password)
		{
			using HMAC hmac = new HMACSHA512(user.Salt);

			byte[] computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

            for (int i = 0; i < user.Password.Length; i++)
            {
                if (user.Password[i] != computedHash[i]) return false;
            }

            return true;
		}
	}
}
