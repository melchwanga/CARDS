using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Text.Encodings.Web;
using CARDS.Business.Services.API;
using CARDS.Business.Services;
using CARDS.Business.ViewModels;
using CARDS.Business.Views;
using CARDS.Business;
using System.Net.Mail;
using CARDS.Business.Models;
using CARDS.Business.Services.Repository;

namespace CARDS.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class CardsController : ControllerBase
	{
		public readonly IAppAuthenticationService _authService;

		private readonly ApplicationDbContext _context;
		private readonly iExceptionLogger iLogger;
		private readonly DatabaseService _dbServices;
		private readonly IConfiguration _config;

		public CardsController(DatabaseService dbServices, iExceptionLogger _iLogger, ApplicationDbContext context, IConfiguration configuration, IAppAuthenticationService _AppAuthService)
		{
			_context = context;
			_authService = _AppAuthService;
			iLogger = _iLogger;
			_config = configuration;
			_dbServices = dbServices;

		}

		/// <summary>
		/// Create card by providing name and, optionally, description and color
		/// </summary>
		/// <param name="vm"></param>
		/// <returns></returns>
		///<remarks>
		/// Sample request:	
		///     {
		///        "Name": "Unit Testing",
		///        "Description": "Testing various independent system units as they are developed.",
		///        "Color": "#EEEEEE"
		///     }
		///
		/// </remarks>
		[HttpPost("Create")]
		//[Authorize("Permissions.Cards.Create")]
		public async Task<ActionResult<ApiResultsVm>> Create([FromBody] CardViewModel vm)
		{
			var msg = "";

			var user = await _authService.GetAuthencticatedUserAsync(this.User);

			var result = new ApiResultsVm();

			if (ModelState.IsValid)
			{
				try
				{
					var parameters = new
					{
						Id = vm.Id,
						Name = vm.Name,
						Description = vm.Description,
						Color = vm.Color,
						UserId = user.Id
					};

					var sp = "AddEditCard";

					var rs = await _dbServices.QueryAsync<CardsView>(sp, parameters, 300).ConfigureAwait(true);

					if (rs.Code != 200)
					{
						msg = rs.Msg;
					}
					else
					{
						result.ResultStatus = true;
						result.Title = "Success";
						result.Message = "Card successfully created";

						return Ok(result);
					}
				}
				catch (Exception ex)
				{
					msg = ex.Message;
				}
			}
			else
			{
				foreach (var modelState in ModelState.Values)
				{
					foreach (var modelError in modelState.Errors)
					{
						msg += modelError.ErrorMessage + "\n";
					}
				}
			}
			//var err = new { ResultStatus = false, Message = msg };
			result.Message = msg;

			return Ok(result);
		}

		/// <summary>
		/// Update card details by supplying the card id and the updated card details
		/// </summary>
		/// <param name="id"></param>
		/// <param name="vm"></param>
		/// <returns></returns>
		/// <remarks>
		/// Sample request:	
		///   {
		///		   "Id": 2,
		///		   "Name": "Requirements Engineering",
		///		   "Description": "Requirements gathering, analysis, validation and specification",
		///		   "Color": "#909090",
		///		   "StatusId": 2
		///   }
		///  </remarks>
		[HttpPut("Update/{id}")]
		public async Task<ActionResult<ApiResultsVm>> Update([FromRoute] int id, [FromBody] CardViewModel vm)
		{
			var msg = "";

			var user = await _authService.GetAuthencticatedUserAsync(this.User);

			var result = new ApiResultsVm ();

			if (ModelState.IsValid)
			{
				try
				{
					if (id != vm.Id)
					{
						return BadRequest("Invalid Id. Route id parameter does not belong the the packet id.");
					}
					var parameters = new
					{
						Id = vm.Id,
						StatusId = vm.StatusId,
						Name = vm.Name,
						Description = vm.Description,
						Color = vm.Color,
						UserId = user.Id
					};

					var sp = "AddEditCard";

					var rs = await _dbServices.QueryAsync<CardsView>(sp, parameters, 300).ConfigureAwait(true);

					if (rs.Code != 200)
					{
						msg = rs.Msg;
					}
					else
					{
						result.ResultStatus = true;
						result.Title = "Success";
						result.Message = "Card successfully updated";

						return Ok(result);
					}
				}
				catch (Exception ex)
				{
					msg = ex.Message;
				}
			}
			else
			{
				foreach (var modelState in ModelState.Values)
				{
					foreach (var modelError in modelState.Errors)
					{
						msg += modelError.ErrorMessage + "\n";
					}
				}
			}
			result.Message = msg;

			return Ok(result);
		}

		/// <summary>
		/// Get list of cards. Optionally specify various filters and the sorting field. Order can be ASC or DESC
		/// </summary>
		/// <remarks>
		/// Sample request:	
		/// {
		///	  "PageSize": 10,
		///	  "Page": 1,
		///	  "Name": "",
		///	  "Color": "#909090",
		///	  "StatusId": 3,
		///	  "CreatedFrom": "2024-02-16",
		///	  "CreatedTo": "2024-02-20",
		///	  "SortById": 6,
		///	  "Order": "ASC"
		/// }
		/// </remarks>

		[HttpPost("GetCards")]
		public async Task<ActionResult<ApiGetCardsVm>> GetCards([FromBody] ApiGetParamVm vm)
		{
			var result = new ApiGetCardsVm { ResultStatus = false };

			if (!ModelState.IsValid)
			{
				result.Message = "Invalid Params received.";
				return BadRequest(result);
			}

			var user = await _authService.GetUserProfileAsyc(this.User);

			try
			{
				int pageSize = vm.PageSize;

				var skip = (vm.Page - 1) * pageSize;

				var filters = GetCardsFilteredExpression(user, vm);

				var query = _context.CardsView.Where(filters);

				if(vm.SortById != null && !string.IsNullOrEmpty(vm.Order))
				{
					var sortBy = await _context.SystemCodeDetails.FindAsync(vm.SortById);
				    if(sortBy != null)
					{
						var sortCode = sortBy.Code;
						var order = vm.Order;
						/**
						 In each case, of order is specified as DESC order as such 
						 Order as such otherwise default to ASCENDING ORDER
						 */
						if(sortCode == "Name")
						{
							if(order == "DESC")
								query = query.OrderByDescending(x => x.Name);
							else
								query = query.OrderBy(x => x.Name);
						}
						else if (sortCode == "Color")
						{
							if (order == "DESC")
								query = query.OrderByDescending(x => x.Color);
							else
								query = query.OrderBy(x => x.Color);
						}
						else if (sortCode == "Status")
						{
							if (order == "DESC")
								query = query.OrderByDescending(x => x.Status);
							else
								query = query.OrderBy(x => x.Status);
						}
						else if (sortCode == "DateCreated")
						{
							if (order == "DESC")
								query = query.OrderByDescending(x => x.CreatedOn);
							else
								query = query.OrderBy(x => x.CreatedOn);
						}

					}
				}
				else
				{
					//if no specific sort field specified, default to date created
					query = query.OrderBy(x => x.CreatedOn);
				}

				var items = await  query.Skip(skip).Take(pageSize).ToListAsync();

				var cardIds = items.Select(x => x.Id).ToList();

				var history = await _context.CardStatusHistoriesView
					.Where(x => cardIds.Contains(x.CardId)).ToListAsync();

				items.All(x => {
					x.StatusHistory = history.Where(y => y.CardId == x.Id).ToList();
					return true;
				});

				result.Cards = items; 

				result.Page = vm.Page;
				result.PageSize = vm.PageSize;
				result.Records = query.Count();
				result.ResultStatus = true;
				return result;
			}
			catch (Exception ex)
			{
				iLogger.LogErrorAsync(ex);
				result.Message = ex.Message;
				return BadRequest(result);
			}
		}

		/// <summary>
		/// Get details of a specific card by specifying its id
		/// </summary>
		[HttpGet("GetCard/{id}")]
		public async Task<ActionResult<CardsView>> GetCard([FromRoute] int id)
		{
			var result = new ApiGetCardsVm { ResultStatus = false };

			var user = await _authService.GetUserProfileAsyc(this.User);
			try
			{
				var card = await _context.CardsView
					.FirstOrDefaultAsync(x => x.Id == id && (x.CreatedById == user.UserId || user.CanViewLeadsForOther));

				if(card != null)
				{
					card.StatusHistory = await _context.CardStatusHistoriesView
					.Where(x => x.CardId == id)
					.OrderBy(x => x.CreatedOn)
					.ToListAsync();
				}

				return card;
			}
			catch (Exception ex)
			{
				iLogger.LogErrorAsync(ex);
				result.Message = ex.Message;
				return BadRequest(result);
			}
		}

		/// <summary>
		/// Delete card details and related status history by specifying its id
		/// </summary>
		[HttpDelete("Delete/{id}")]
		public async Task<ActionResult<ApiResultsVm>> Delete([FromRoute] int id)
		{
			var msg = "";

			var user = await _authService.GetUserProfileAsyc(this.User);

			var result = new ApiResultsVm ();

			try
			{
				var parameters = new
				{
					Id = id,
					UserId = user.UserId
				};

				var sp = "DeleteCard";

				var rs = await _dbServices.QueryAsync<SpOutput>(sp, parameters, 300).ConfigureAwait(true);

				if (rs.Code != 200)
				{
					msg = rs.Msg;
				}
				else
				{
					result.ResultStatus = true;
					result.Title = "Success";
					result.Message = "Card successfully deleted";

					return Ok(result);
				}
			}
			catch (Exception ex)
			{
				msg = ex.Message;
			}

			result.Message = msg;

			return Ok(result);
		}

		private Expression<Func<CardsView, bool>> GetCardsFilteredExpression(UserProfileVm user, ApiGetParamVm vm)
		{
			Expression<Func<CardsView, bool>> filter = x => x.CreatedById == user.UserId || user.CanViewLeadsForOther;
			Expression<Func<CardsView, bool>> filterx;

			try
			{
				if(vm != null)
				{
					//search for partial match
					if (!string.IsNullOrEmpty(vm.Name))
					{
						filterx = x => x.Name.ToLower().Contains(vm.Name.ToLower());
						filter = filter.And(filterx);
					}
					//search for exact match
					if (!string.IsNullOrEmpty(vm.Color))
					{
						filterx = x => x.Color.ToLower().Contains(vm.Color.ToLower());
						filter = filter.And(filterx);
					}
					//search for exact match
					if (vm.StatusId != null)
					{
						filterx = x => x.StatusId == vm.StatusId;
						filter = filter.And(filterx);
					}
					//search for exact match
					if (vm.StatusId != null)
					{
						filterx = x => x.StatusId == vm.StatusId;
						filter = filter.And(filterx);
					}
					//filter cards created from specified date
					if (vm.CreatedFrom != null)
					{
						filterx = x => x.CreatedOn.Date >= vm.CreatedFrom.Value.Date;
						filter = filter.And(filterx);
					}
					//filter cards created no later than end date
					if (vm.CreatedTo != null)
					{
						filterx = x => x.CreatedOn.Date <= vm.CreatedTo.Value.Date;
						filter = filter.And(filterx);
					}
				}
			}
			catch (Exception ex)
			{

			}

			return filter;
		}

	}
}
