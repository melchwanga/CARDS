using CARDS.Business.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CARDS.Business.ViewModels
{
	public class ErrorApi
	{
		public string? Title { get; set; } = "Failed";
		public string? Message { get; set; }
	}
	public class ApiResultsVm : ErrorApi
	{
		public bool ResultStatus { get; set; } = false;
	}

	public class ApiGetParamVm
	{
		[DisplayName("Number of items per page")]
		public int PageSize { get; set; } = 10;
		public int Page { get; set; } = 1;
		public string? Name { get; set; }
		[RegularExpression(@"^#([a-fA-F0-9]{6}|[a-fA-F0-9]{3})$", ErrorMessage = "Invalid color code. Color MUST conform to Tthe '6 alphanumeric characters prefixed with a #' format")]
		public string? Color { get; set; }
		public int? StatusId { get; set; }
		public DateTime? CreatedFrom { get; set; }
		public DateTime? CreatedTo { get; set; }
		public int? SortById { get; set; }
		public string? Order { get; set; } = "ASC";
	}
	public class ApiGetResultsVm : ErrorApi
	{
		public int? PageSize { get; set; } = 10;
		public int? Page { get; set; } = 1;
		public int? Records { get; set; }
		public bool ResultStatus { get; set; }
	}

	public class ApiGetCardsVm : ApiGetResultsVm
	{
		public IEnumerable<CardsView>? Cards { get; set; }
	}
}
