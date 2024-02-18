using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CARDS.Business.ViewModels
{
	public class UserVm
	{
		public required string Id { get; set; }
		public required string UserName { get; set; }
		public string? Email { get; set; }
		public string? FirstName { get; set; }
		public string? LastName { get; set; }
		public string? Role { get; set; }
	}

	public class UserProfileVm
	{
		public string? UserId { get; set;}
		public bool CanViewLeadsForOther { get; set; }
		public bool CanEditLeadsForOther { get; set; }
	}
}
