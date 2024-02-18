using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CARDS.Business.Models
{
	public class SystemCode
	{
		public int Id { get; set; }
		public required string Code {  get; set; }
		public required string Description { get; set; }
		public bool IsUserMaintained { get; set; }
		public ICollection<SystemCodeDetail>? SystemCodeDetails { get; set; }	
	}

	public class SystemCodeDetail:CreatedByFields
	{
		public int Id { get; set; }
		public required string Code { get; set; }
		public required string Description { get; set; }
		public SystemCode? SystemCode { get; set; }
		public int SystemCodeId { get; set; }
	}
}
