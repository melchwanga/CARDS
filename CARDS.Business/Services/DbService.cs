using System;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CARDS.Business.Models;
using CARDS.Business;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace CARDS.Business.Services
{
	public class DBService
	{
		private readonly ApplicationDbContext _context;
		private readonly UserManager<ApplicationUser> _userManager;
		public static IConfigurationRoot Configuration;
		public DBService(ApplicationDbContext context,
			UserManager<ApplicationUser> userManager)
		{
			_context = context;
			_userManager = userManager;
		}

		public string GeneratePassword(int maxSize)
		{
			var passwords = string.Empty;
			var chArray1 = new char[52];
			var chArray2 = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^*()_+".ToCharArray();
			var data1 = new byte[1];
			using (var cryptoServiceProvider = new RNGCryptoServiceProvider())
			{
				cryptoServiceProvider.GetNonZeroBytes(data1);
				var data2 = new byte[maxSize];
				cryptoServiceProvider.GetNonZeroBytes(data2);
				var stringBuilder = new StringBuilder(maxSize);
				foreach (var num in data2)
					stringBuilder.Append(chArray2[(int)num % chArray2.Length]);
				passwords = stringBuilder.ToString();
				var number = Random("N");
				var upper = Random("S");
				var lower = Random("l");
				passwords += number + upper;
				return passwords;
			}
		}
		public string Random(string type)
		{
			var data2 = new byte[1];
			var passwords = string.Empty;
			switch (type)
			{
				case "N":
					{
						var charArray = "0123456789";
						var stringBuilder = new StringBuilder(2);
						foreach (var num in data2)
							stringBuilder.Append(charArray[(int)num % charArray.Length]);
						passwords = stringBuilder.ToString();
						return passwords;
					}

				case "l":
					{
						var charArray = "abcdefghijklmnopqrstuvwxyz";

						var stringBuilder = new StringBuilder(2);
						foreach (var num in data2)
							stringBuilder.Append(charArray[(int)num % charArray.Length]);
						passwords = stringBuilder.ToString();
						return passwords;
					}

				case "C":
					{
						var charArray = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

						var stringBuilder = new StringBuilder(2);
						foreach (var num in data2)
							stringBuilder.Append(charArray[(int)num % charArray.Length]);
						passwords = stringBuilder.ToString();
						return passwords;
					}

				case "S":
					{
						var charArray = "!@#$%^&*()_+-={}|[]:;<>?,./";
						var stringBuilder = new StringBuilder(2);
						foreach (var num in data2)
							stringBuilder.Append(charArray[(int)num % charArray.Length]);
						passwords = stringBuilder.ToString();
						return passwords;
					}
			}

			return string.Empty;
		}
		public static string DBConnection()
		{

			try
			{
				var builder = new ConfigurationBuilder()
					.SetBasePath(System.IO.Directory.GetCurrentDirectory())
					.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
					.AddEnvironmentVariables();
				builder.AddEnvironmentVariables();
				Configuration = builder.Build();
				string connectionstring = Configuration.GetConnectionString("DefaultConnection");
				return connectionstring;
			}
			catch
			{
				return "";
			}
		}
	}
}