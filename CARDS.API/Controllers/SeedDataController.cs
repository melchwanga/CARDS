using CARDS.Business;
using CARDS.Business.Authorization.Seeds;
using CARDS.Business.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
/*
namespace CARDS.API.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class SeedDataController : ControllerBase
	{
		private readonly ILogger<SeedDataController> _logger;
		private readonly UserManager<IdentityUser> _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;
		private readonly ApplicationDbContext _context;

		public SeedDataController(ILogger<SeedDataController> logger, ApplicationDbContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
		{
			_logger = logger;
			_userManager = userManager;
			_roleManager = roleManager;
			_context = context;
		}

		[HttpGet(Name = "Seed")]
		public async Task Seed()
		{
			await SeedDefaultRoles.SeedAsync(_userManager,_roleManager);
			await SeedDefaultUsers.SeedAdminAsync(_userManager,_roleManager);
			await SeedDefaultUsers.SeedMemberAsync(_userManager,_roleManager);
		}
	}
}
*/