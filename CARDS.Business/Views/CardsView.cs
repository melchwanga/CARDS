using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CARDS.Business.Views
{
	[Table("CardsView"), NotMapped]
	public class CardsView
	{
		public int Id { get; set; }
		public required string Name { get; set; }
		public string? Description { get; set; }
		public string? Color { get; set; }
		public int StatusId { get; set; }
		public string? CreatedById { get; set; }
		public DateTime CreatedOn { get; set; }
		public string? ModifiedById { get; set; }
		public DateTime? ModifiedOn { get; set; }
		public required string StatusCode { get; set; }
		public required string Status { get; set; }
		public required string CreatedBy { get; set; }
		public string? ModifiedBy { get; set; }
		public List<CardStatusHistoriesView>? StatusHistory { get; set; }
	}

	[Table("CardStatusHistoriesView"), NotMapped]
	public class CardStatusHistoriesView
	{
		public int Id { get; set; }
		public int CardId { get; set; }
		public int StatusId { get; set; }
		public string? CreatedById { get; set; }
		public DateTime CreatedOn { get; set; }
		public string? StatusCode { get; set; }
		public string? Status { get; set; }
		public string? CreatedBy { get; set; }
	}
}
