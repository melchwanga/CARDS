using Microsoft.AspNetCore.Identity;
using CARDS.Business.Models;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using System.Security.Claims;

namespace CARDS.Business.Authorization.Seeds
{
    public static class SeedDefaultRoles
    {
        public static async Task SeedAsync(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
			try
			{
                foreach(var roleVm in Roles.TheRoles)
                {
					string roleName = roleVm.Name;
					var roleClaims = roleVm.Permissions;
					/*Seed Role*/
					await roleManager.CreateAsync(new IdentityRole(roleName.ToString()));
					var role = await roleManager.FindByNameAsync(roleName.ToString());
					//Get and remove existing claims if any
					var claims = await roleManager.GetClaimsAsync(role);
					foreach (var claim in claims)
					{
						await roleManager.RemoveClaimAsync(role, claim);
					}

					//populate the new claims
					foreach (var permission in roleClaims)
						await roleManager.AddClaimAsync(role, new Claim("Permission", permission));
				}               
            }
            catch(Exception ex)
			{

			}
        }
    }
}
