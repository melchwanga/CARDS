using Microsoft.AspNetCore.Identity;
using CARDS.Business.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CARDS.Business.Authorization.Seeds
{
    public static class SeedDefaultUsers
    {
        public static async Task SeedAdminAsync(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            var defaultUser = new ApplicationUser
            {
                UserName = "admin",
                Email = "admin@cards.app",
                EmailConfirmed = true,
                FirstName ="Admin",
                LastName = "User"
            };

            if (userManager.Users.All(u => u.Id != defaultUser.Id))
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, "AdminN0P@$$w0rd");
                    await userManager.AddToRoleAsync(defaultUser, RoleNames.Admin.ToString());
                }
            }
        }
		public static async Task SeedMemberAsync(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
		{
			var defaultUser = new ApplicationUser
			{
				UserName = "MemberOne",
				Email = "memberone@cards.app",
				EmailConfirmed = true,
				FirstName = "Member",
				LastName = "One"
			};

			if (userManager.Users.All(u => u.Id != defaultUser.Id))
			{
				var user = await userManager.FindByEmailAsync(defaultUser.Email);
				if (user == null)
				{
					await userManager.CreateAsync(defaultUser, "MemberN0P@$$w0rd");
					await userManager.AddToRoleAsync(defaultUser, RoleNames.Member.ToString());
				}
			}
		}
	}
}
