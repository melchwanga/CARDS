using CARDS.Business;
using CARDS.Business.Services;
using CARDS.Business.Services.API;
using CARDS.Business.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CARDS.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
    public class TokenController : ControllerBase
    {
        /// <summary>
        ///
        /// </summary>
        public readonly IAppAuthenticationService AppAuthService;
        private readonly ApplicationDbContext _context;
		private readonly iExceptionLogger iLogger;

		/// <summary>
		///
		/// </summary>
		/// <param name="appAuthService"></param>
		public TokenController(iExceptionLogger _iLogger,ApplicationDbContext context, IConfiguration configuration, IAppAuthenticationService _AppAuthService)
        {
            _context = context;
            AppAuthService = _AppAuthService; //new AppAuthenticationService(_context, configuration);
            iLogger = _iLogger;
        }

		/// <summary>
		///Login using registered username/email and password
		/// </summary>
		/// <param name="model"></param>
		/// <returns>Token</returns>
		[HttpPost("Login")]
        [AllowAnonymous]
        public async Task<ActionResult<Token>> Login([FromBody] BearerLoginVm model)
        {
            var result = new Token {  };
            var apiResult = new ApiResultsVm { ResultStatus = false};


			try
			{
				var token = await AppAuthService.AuthenticateAsync(model.UserName, model.Password);

				if (token != null && !string.IsNullOrEmpty(token.TokenString))
				{
					var userV = await _context.UsersView
						.Where(x => x.UserName.ToUpper() == model.UserName.ToUpper())
						.SingleAsync();
					
					token.User = new UserVm
					{
						Id = userV.Id,
						UserName = userV.UserName,
						Email = userV.Email,
						FirstName = userV.FirstName,
						LastName = userV.LastName,
						Role = userV.Role,
					};

					token.Permissions = await _context.UserClaimsView.Where(x => x.UserId == userV.Id).Select(x => x.ClaimValue).ToListAsync();

					return Ok(token);
				}
				else
				{
					apiResult.Title = "Invalid Login";
					apiResult.Message = "Invalid login credentials. Please check and try again later.";
					return Unauthorized(apiResult);
				}
			}
			catch (Exception ex)
			{
				iLogger.LogErrorAsync(ex);
				apiResult.Title = "Login Failed";
				apiResult.Message = "Invalid login credentials. Please check and try again later.";
				return BadRequest(apiResult);
			}
		}
    }
}
