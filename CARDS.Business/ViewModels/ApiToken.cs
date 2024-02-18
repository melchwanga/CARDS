using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CARDS.Business.ViewModels
{
	public class Token
	{
		public string? TokenString { get; set; }

		public UserVm? User { get; set; }

		public double TokenExpiryMinutes { get; set; }

		public List<string>? Permissions { get; set; }
	}

	public class TokenValidator
	{
		public string? UserId { get; set; }
		public long ClientId { get; set; }
		public int nbf { get; set; }
		public int exp { get; set; }
		public int iat { get; set; }
	}

	public class BearerLoginVm
	{
		public required string UserName { get; set; }
		public required string Password { get; set; }
	}
}
