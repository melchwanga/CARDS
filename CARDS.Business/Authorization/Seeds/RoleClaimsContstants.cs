using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CARDS.Business.Authorization.Seeds
{
	public class RoleNames
	{
		public static string Admin => "Admin";
		public static string Member => "Member";
		public static string AdminId => "0012DA5C-A513-4EBF-B022-05A73819EFD7";
		public static string MemberId => "0035BA55-5C85-4FC5-92B7-25A341BB3255";
	}

	public class RolePermissions
	{
		public static string CardsView => "Permissions.Cards.View";
		public static string CardsViewForOthers => "Permissions.Cards.View For Others";
		public static string CardsCreate => "Permissions.Cards.Create";
		public static string CardsEdit => "Permissions.Cards.Edit";
		public static string CardsEditForOthers => "Permissions.Cards.Edit For Others";
	}
	public class Roles
	{
		public static List<RoleVm> TheRoles => new List<RoleVm>
		{
			new RoleVm
			{
				Id=RoleNames.AdminId,
				Name = RoleNames.Admin,
				Permissions = new List<string> {
					RolePermissions.CardsView,
					RolePermissions.CardsViewForOthers,
					RolePermissions.CardsCreate,
					RolePermissions.CardsEdit,
					RolePermissions.CardsEditForOthers
				}
			}
			,new RoleVm
			{
				Id = RoleNames.MemberId,
				Name = RoleNames.Member,
				Permissions = new List<string> {
					RolePermissions.CardsView,
					RolePermissions.CardsCreate,
					RolePermissions.CardsEdit,
				}
			}
		};
	}

	public class RoleVm
	{
		public required string Id { get; set; }
		public required string Name { get; set; }
		public List<string> Permissions { get; set; } = new List<string>();
	}
}
