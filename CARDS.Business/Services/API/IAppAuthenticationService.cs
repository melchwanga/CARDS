using CARDS.Business.Authorization.Seeds;
using CARDS.Business.Models;
using CARDS.Business.ViewModels;
using CARDS.Business.Views;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace CARDS.Business.Services.API
{
	public interface IAppAuthenticationService
	{
		Task<Business.ViewModels.Token> AuthenticateAsync(string username, string password);
		TokenValidator ValidateToken(string AccessTokenValue);
		ApplicationUser GetUser(string username, string password);
		Task<UserProfileVm> GetUserProfileAsyc(ClaimsPrincipal claimsPrincipal);
		Task<UsersView> GetAuthencticatedUserAsync(ClaimsPrincipal claimsPrincipal);
	}

	public class AppAuthenticationService : IAppAuthenticationService
	{
		private readonly ApplicationDbContext _context;
		public readonly IConfiguration Configuration;
		private readonly iExceptionLogger iLogger;

		public AppAuthenticationService(ApplicationDbContext context, IConfiguration vConfiguration, iExceptionLogger _iLogger)
		{
			_context = context;
			Configuration = vConfiguration;
			iLogger = _iLogger;
		}

		public async Task<UsersView> GetAuthencticatedUserAsync(ClaimsPrincipal claimsPrincipal)
		{
			var currentUser = claimsPrincipal;

			try
			{
				var userId = currentUser.Claims.Where(u => u.Type == ClaimTypes.NameIdentifier).SingleOrDefault().Value;

				if (!string.IsNullOrEmpty(userId))
				{
					var user = await _context.UsersView.Where(x => x.Id == userId).FirstOrDefaultAsync(); ;
					if (user != null)
					{
						return user;
					}
				}
			}
			catch (Exception ex)
			{
				return null;
			}

			return null;
		}
		public async Task<UserProfileVm> GetUserProfileAsyc(ClaimsPrincipal claimsPrincipal)
		{
			var currentUser = claimsPrincipal;
			var profile = new UserProfileVm();

			try
			{
				var userId = currentUser.Claims.Where(u => u.Type == ClaimTypes.NameIdentifier).SingleOrDefault().Value;

				if (!string.IsNullOrEmpty(userId))
				{
					var user = await _context.UsersView.Where(x => x.Id == userId).FirstOrDefaultAsync(); ;
					if (user != null)
					{
						var claims = await _context.UserClaimsView.Where(x => x.UserId == user.Id).Select(x => x.ClaimValue).ToListAsync();
						profile = new UserProfileVm
						{
							UserId = user.Id,
							CanEditLeadsForOther = claims.Contains(RolePermissions.CardsEditForOthers),
							CanViewLeadsForOther = claims.Contains(RolePermissions.CardsViewForOthers)
						};
					}
				}
			}
			catch (Exception ex)
			{
				
			}

			return profile;
		}
		public ApplicationUser GetUser(string username, string password)
		{
			var user = _context.ApplicationUsers.SingleOrDefault(x => x.Email == username);
			var isValid = PasswordHasher.VerifyIdentityV3Hash(password, user.PasswordHash);
			if (isValid)
				return user;
			else
			{
				return null;
			}
		}

		public TokenValidator ValidateToken(string AccessTokenValue)
		{
			try
			{
				var tokenHandler = new JwtSecurityTokenHandler();
				var validationParameters = GetValidationParameters();

				SecurityToken validatedToken;

				IPrincipal principal = tokenHandler.ValidateToken(AccessTokenValue, validationParameters, out validatedToken);
				var userspecifics = $"{validatedToken}".Replace("{\"alg\":\"HS256\",\"typ\":\"JWT\"}.", "");
				var user = JsonConvert.DeserializeObject<TokenValidator>(userspecifics);

				return user;
			}
			catch (Exception)
			{
				return null;
			}
		}


		private TokenValidationParameters GetValidationParameters()
		{
			var apiKey = Configuration["AppSettings:Secret"];
			var key = Encoding.ASCII.GetBytes(apiKey);

			return new TokenValidationParameters()
			{
				ValidateLifetime = true,
				ValidateAudience = false,
				ValidateIssuer = false,
				IssuerSigningKey = new SymmetricSecurityKey(key) // The same key as the one that generate the token
			};
		}

		public async Task<Business.ViewModels.Token> AuthenticateAsync(string username, string password)
		{
			Business.ViewModels.Token tokenVm = null;

			try
			{
				var user = _context.ApplicationUsers.FirstOrDefault(x => x.Email == username || x.UserName == username);

				var minsStr = Configuration["AppSettings:TokenExpiryMinutes"].ToString();
				var mins = int.Parse(minsStr);

				// return null if user not found
				if (user == null)
					return null;

				if (!PasswordHasher.VerifyIdentityV3Hash(password, user.PasswordHash))
					return null;

				user.LastLoginDate = DateTime.Now;
				_context.Update(user);
				await _context.SaveChangesAsync();

				// authentication successful so generate jwt token
				var tokenHandler = new JwtSecurityTokenHandler();

				var apiKey = Configuration["AppSettings:Secret"];
				var key = Encoding.ASCII.GetBytes(apiKey);
				var tokenDescriptor = new SecurityTokenDescriptor
				{
					Subject = new ClaimsIdentity(new Claim[]
					{
					new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
					}),
					IssuedAt = DateTime.UtcNow,
					Expires = DateTime.UtcNow.AddMinutes(mins),
					SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
				};

				var token = tokenHandler.CreateToken(tokenDescriptor);
				var tokenString = tokenHandler.WriteToken(token);

				tokenVm = new Business.ViewModels.Token
				{
					TokenString = tokenString,
					TokenExpiryMinutes = mins,
				};
				return tokenVm;
			}
			catch (Exception ex)
			{
				iLogger.LogErrorAsync(ex);
			}

			return tokenVm;
		}
	}

	public static class PasswordHasher
	{
		public static string GenerateIdentityV3Hash(string password, KeyDerivationPrf prf = KeyDerivationPrf.HMACSHA256, int iterationCount = 10000, int saltSize = 16)
		{
			var rng = RandomNumberGenerator.Create();
			var salt = new byte[saltSize];
			rng.GetBytes(salt);

			var pbkdf2Hash = KeyDerivation.Pbkdf2(password, salt, prf, iterationCount, 32);
			return Convert.ToBase64String(ComposeIdentityV3Hash(salt, (uint)iterationCount, pbkdf2Hash));
		}

		private static byte[] ComposeIdentityV3Hash(byte[] salt, uint iterationCount, byte[] passwordHash)
		{
			var hash = new byte[1 + 4/*KeyDerivationPrf value*/ + 4/*Iteration count*/ + 4/*salt size*/ + salt.Length /*salt*/ + 32 /*password hash size*/];
			hash[0] = 1; //Identity V3 marker

			Buffer.BlockCopy(ConvertToNetworkOrder((uint)KeyDerivationPrf.HMACSHA256), 0, hash, 1, sizeof(uint));
			Buffer.BlockCopy(ConvertToNetworkOrder((uint)iterationCount), 0, hash, 1 + sizeof(uint), sizeof(uint));
			Buffer.BlockCopy(ConvertToNetworkOrder((uint)salt.Length), 0, hash, 1 + 2 * sizeof(uint), sizeof(uint));
			Buffer.BlockCopy(salt, 0, hash, 1 + 3 * sizeof(uint), salt.Length);
			Buffer.BlockCopy(passwordHash, 0, hash, 1 + 3 * sizeof(uint) + salt.Length, passwordHash.Length);

			return hash;
		}

		public static bool VerifyIdentityV3Hash(string password, string passwordHash)
		{
			var identityV3HashArray = Convert.FromBase64String(passwordHash);
			if (identityV3HashArray[0] != 1) throw new InvalidOperationException("passwordHash is not Identity V3");

			var prfAsArray = new byte[4];
			Buffer.BlockCopy(identityV3HashArray, 1, prfAsArray, 0, 4);
			var prf = (KeyDerivationPrf)ConvertFromNetworOrder(prfAsArray);

			var iterationCountAsArray = new byte[4];
			Buffer.BlockCopy(identityV3HashArray, 5, iterationCountAsArray, 0, 4);
			var iterationCount = (int)ConvertFromNetworOrder(iterationCountAsArray);

			var saltSizeAsArray = new byte[4];
			Buffer.BlockCopy(identityV3HashArray, 9, saltSizeAsArray, 0, 4);
			var saltSize = (int)ConvertFromNetworOrder(saltSizeAsArray);

			var salt = new byte[saltSize];
			Buffer.BlockCopy(identityV3HashArray, 13, salt, 0, saltSize);

			var savedHashedPassword = new byte[identityV3HashArray.Length - 1 - 4 - 4 - 4 - saltSize];
			Buffer.BlockCopy(identityV3HashArray, 13 + saltSize, savedHashedPassword, 0, savedHashedPassword.Length);

			var hashFromInputPassword = KeyDerivation.Pbkdf2(password, salt, prf, iterationCount, 32);

			return AreByteArraysEqual(hashFromInputPassword, savedHashedPassword);
		}

		private static bool AreByteArraysEqual(byte[] array1, byte[] array2)
		{
			if (array1.Length != array2.Length) return false;

			var areEqual = true;
			for (var i = 0; i < array1.Length; i++)
			{
				areEqual &= (array1[i] == array2[i]);
			}
			//If you stop as soon as the arrays don't match you'll be disclosing information about how different they are by the time it takes to compare them
			//this way no information is disclosed
			return areEqual;
		}

		private static byte[] ConvertToNetworkOrder(uint number)
		{
			return BitConverter.GetBytes(number).Reverse().ToArray();
		}

		private static uint ConvertFromNetworOrder(byte[] reversedUint)
		{
			return BitConverter.ToUInt32(reversedUint.Reverse().ToArray(), 0);
		}
	}
}
