using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CARDS.Business.ViewModels
{
	public class StateViewModel<T>
	{
		public int Code { set; get; }
		public string? Msg { set; get; }
		public T? Data { set; get; }
		public int Count { set; get; }
	}

	public class SpOutput
	{
		public int? NoOfRows { set; get; }
		public int? Output { set; get; }
	}

	public class SystemOptionsCollectionVm
	{
		public IEnumerable<SystemOptionVm>? SystemOptions { get; set; }
	}

	public class SystemOptionVm
	{
		public string? Code { get; set; }
		public string? Description { get; set; }
		public List<SystemOptionDetailVm>? Details { get; set; }
	}

	public class SystemOptionDetailVm
	{
		public int Id { get; set; }
		public string? Code { get; set; }
		public string? Name { get; set; }
	}
}
