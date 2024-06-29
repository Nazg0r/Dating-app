using API.Data;
using API.DTOs;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;

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

		private async Task<bool> UserExist(string name)
		{
			return await _dbContext.Users.AnyAsync( u => u.Name.ToLower() == name.ToLower());
		}
	}
}
