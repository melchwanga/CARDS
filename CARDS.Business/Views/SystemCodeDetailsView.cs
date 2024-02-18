using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CARDS.Business.Views
{
	[Table("SystemCodeDetailsView"), NotMapped]
	public class SystemCodeDetailsView
	{
		public int Id { get; set; }
		public required string Code { get; set; }
		public required string Description { get; set; }
		public int SystemCodeId { get; set; }
		public string? CreatedById { get; set; }
		public DateTime CreatedOn { get; set; }
		public required string SystemCode { get; set; }
	}
}
