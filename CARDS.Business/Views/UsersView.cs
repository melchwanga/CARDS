using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CARDS.Business.Views
{
	[Table("UsersView"), NotMapped]
	public class UsersView
	{
		public required string Id { get; set; }
		public required string Discriminator { get; set; }
		public string? FirstName { get; set; }
		public string? LastName { get; set; }
		public DateTime? LastLoginDate { get; set; }
		public DateTime? CreatedOn { get; set; }
		public string? CreatedById { get; set; }
		public DateTime? ModifiedOn { get; set; }
		public string? ModifiedById { get; set; }
		public string? DeactivatedById { get; set; }
		public DateTime? DeactivatedOn { get; set; }
		public DateTime? PasswordChangedOn { get; set; }
		public required string UserName { get; set; }
		public required string NormalizedUserName { get; set; }
		public string? Email { get; set; }
		public string? NormalizedEmail { get; set; }
		public bool EmailConfirmed { get; set; }
		public string? PasswordHash { get; set; }
		public string? SecurityStamp { get; set; }
		public string? ConcurrencyStamp { get; set; }
		public string? PhoneNumber { get; set; }
		public bool PhoneNumberConfirmed { get; set; }
		public bool TwoFactorEnabled { get; set; }
		public DateTimeOffset? LockoutEnd { get; set; }
		public bool LockoutEnabled { get; set; }
		public int AccessFailedCount { get; set; }
		public required string RoleId { get; set; }
		public string? Role { get; set; }
	}

	[Table("UserClaimsView"), NotMapped]
	public class UserClaimsView
	{
		public required string UserId { get; set; }
		public required string ClaimType { get; set; }
		public required string ClaimValue { get; set; }
	}
}
