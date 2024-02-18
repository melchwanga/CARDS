using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace CARDS.Business.Models
{
	public class ApplicationUser : IdentityUser
	{
		[Required]
		[StringLength(50, ErrorMessage = "{0} length must be between {2} and {1}.", MinimumLength = 2)]
		[Display(Name = "First Name")]
		public required string FirstName { get; set; }

		[Required]
		[StringLength(50, ErrorMessage = "{0} length must be between {2} and {1}.", MinimumLength = 2)]
		[Display(Name = "Last Name")]
		public required string LastName { get; set; }
		public string FullName => $"{FirstName} {LastName}";
		public DateTime? LastLoginDate { get; set; }
		public DateTime? CreatedOn { get; set; }

		[DisplayName("Created By")]
		[StringLength(450, ErrorMessage = "{0} length must be between {2} and {1}.", MinimumLength = 2)]
		public string? CreatedById { get; set; }
		public ApplicationUser? CreatedBy { get; set; }
		public DateTime? ModifiedOn { get; set; }

		[DisplayName("Modified By")]
		public string? ModifiedById { get; set; }

		[DisplayName("Deactivated By")]
		public string? DeactivatedById { get; set; }
		public DateTime? DeactivatedOn { get; set; }
		public DateTime? PasswordChangedOn { get; set; }
	}

	public class ApplicationRole : IdentityRole
	{
		[StringLength(200, MinimumLength = 2)]

		public string? Description { get; set; }

		[DisplayName("Created By")]
		public string? CreatedById { get; set; }

		[JsonIgnore]
		public ApplicationUser? CreatedBy { get; set; }

		[DisplayName("Created On")]
		public DateTime? CreatedOn { get; set; }

		[DisplayName("Modifed By")]
		public string? ModifiedById { get; set; }

		[JsonIgnore]
		public ApplicationUser? ModifiedBy { get; set; }

		[DisplayName("Modified On")]
		public DateTime? ModifiedOn { get; set; }

		[Required]
		[DisplayName("Hierarchy")]
		public int Hierarchy { get; set; }
	}

}
