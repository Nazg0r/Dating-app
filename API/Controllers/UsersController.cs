using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
	public class UsersController : BaseApiController
	{
		private readonly DataContext _dbContext;

		public UsersController(DataContext dbContext)
		{
			_dbContext = dbContext;
		}

		[Authorize]
		[HttpGet]
		public async Task<ActionResult<IEnumerable<User>>> GetAllUsers()
		{
			IEnumerable<User> users = await _dbContext.Users.ToListAsync();

			return Ok(users);
		}

		[Authorize]
		[HttpGet("{id}")]
		public async Task<ActionResult<User>> GetUser(int id)
		{
			User user = await _dbContext.Users.FindAsync(id);

			return Ok(user);
		}

	}
}

