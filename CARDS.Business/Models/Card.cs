using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CARDS.Business.Models
{
	public class Card:ModifiedByFields
	{
		public int Id { get; set; }

		[Required]
		[StringLength(200, ErrorMessage = "{0} length must be between {2} and {1}.", MinimumLength = 1)]

		public required string Name { get; set; }

		[StringLength(800, ErrorMessage = "{0} length must be between {2} and {1}.", MinimumLength = 1)]

		public string? Description { get; set; }

		[RegularExpression(@"^#([a-fA-F0-9]{6}|[a-fA-F0-9]{3})$", ErrorMessage = "Invalid color code. Color MUST conform to Tthe '6 alphanumeric characters prefixed with a #' format")]
		[StringLength(7, ErrorMessage = "{0} length must be {1}.", MinimumLength = 7)]
        public string? Color { get; set; }
		public SystemCodeDetail? Status { get; set; }
		public int StatusId { get; set; }
		public ICollection<CardStatusHistory>? StatusHistory { get; set; }
	}

	public class CardStatusHistory : CreatedByFields
	{
		public int Id { get; set; }
		public Card? Card { get; set; }
		public int CardId { get; set; }
		public SystemCodeDetail? Status { get; set; }
		public int StatusId { get; set; }
	}
}
