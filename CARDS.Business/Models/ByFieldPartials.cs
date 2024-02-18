using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CARDS.Business.Models
{
	public class CreatedByFields
	{
		[DisplayName("Created By")]
		public string? CreatedById { get; set; }

		[JsonIgnore]
		public ApplicationUser? CreatedBy { get; set; }

		[DisplayName("Created On")]
		[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd-MM-yyyy HH:mm}")]
		public DateTime CreatedOn { get; set; }
	}

	public class ModifiedByFields : CreatedByFields
	{
		[DisplayName("Modified By")]
		public string? ModifiedById { get; set; }

		[JsonIgnore]
		public ApplicationUser? ModifiedBy { get; set; }

		[DisplayName("Modified On")]
		[DisplayFormat(ApplyFormatInEditMode = false, DataFormatString = "{0:dd-MM-yyyy HH:mm}")]
		public DateTime? ModifiedOn { get; set; }
	}

}
