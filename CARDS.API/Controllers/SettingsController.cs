using CARDS.Business.Services.API;
using CARDS.Business.Services;
using CARDS.Business.ViewModels;
using CARDS.Business;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CARDS.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class SettingsController : ControllerBase
	{
		public readonly IAppAuthenticationService AppAuthService;

		private readonly ApplicationDbContext _context;
		private readonly iExceptionLogger iLogger;
		/// <summary>
		///
		/// </summary>
		public SettingsController(iExceptionLogger _iLogger, ApplicationDbContext context, IConfiguration configuration, IAppAuthenticationService _AppAuthService)
		{
			_context = context;
			AppAuthService = _AppAuthService;
			iLogger = _iLogger;
		}
		/// <summary>
		///Get all parameter options including Card Status Options, Card sortable fields, etc
		/// </summary>
		[HttpGet("GetOptions")]
		public async Task<ActionResult<SystemOptionsCollectionVm>> GetOptions()
		{
			var result = new ApiResultsVm { ResultStatus = false };
			try
			{
				var userVm = await AppAuthService.GetAuthencticatedUserAsync(this.User);

				if (userVm == null)
				{
					result.Message = "Access token decryption failed. Please try again.";
					return Unauthorized(result);
				}

				if (userVm != null)
				{

					var systemCodesList = new List<string> { "CardStatus", "CardSortableFields" };

					var optionsList = _context.SystemCodes
						.Include(x => x.SystemCodeDetails)
						.Where(x => systemCodesList.Contains(x.Code))
						.Select(x => new SystemOptionVm
						{
							Code = x.Code,
							Description = x.Description,
							Details = x.SystemCodeDetails.Select(y => new SystemOptionDetailVm
							{
								Id = y.Id,
								Code = y.Code,
								Name = y.Description
							}).ToList(),
						}).AsEnumerable();

					var list = new SystemOptionsCollectionVm { 
							 SystemOptions = optionsList
							};

					return Ok(list);
				}
				else
				{
					result.Title = "Auth Error";
					result.Message = "Account verification failed. Please contact system admin for further assistance";
					return Unauthorized(result);
				}
			}
			catch (Exception ex)
			{
				iLogger.LogErrorAsync(ex);
				result.Title = "Failed";
				result.Message = $"Error Message: {ex.ToString()}";
				return Ok(result);
			}
		}
	}
}
