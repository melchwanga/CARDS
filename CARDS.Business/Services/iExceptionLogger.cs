using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CARDS.Business.Services
{
	public interface iExceptionLogger
	{
		void LogErrorAsync(Exception ex, string? payload = null);
	}
	/// <summary>  
	/// Summary description for ExceptionLogging  
	/// </summary>  
	public class ExceptionLogger : iExceptionLogger
	{
		private IConfiguration Configuration;

		private string Dir;

		private static String ErrorlineNo, Errormsg, extype, exurl, hostIp, ErrorLocation, HostAdd;

		public ExceptionLogger(IConfiguration config)
		{
			Configuration = config;
			Dir = Configuration["Directory:LogsFolder"];
		}

		public void LogErrorAsync(Exception ex, string? payload = null)
		{
			var line = Environment.NewLine + Environment.NewLine;

			var words = ex?.StackTrace?.Split();
			ErrorlineNo = string.Join(" ", words.Skip(words.Length - 2));

			// ErrorlineNo = ex.StackTrace.Substring(ex.StackTrace.Length - 6, 6);
			Errormsg = ex.GetType().Name.ToString();
			if (ex?.InnerException != null)
				Errormsg += ": " + ex?.InnerException.Message;
			extype = ex.GetType().ToString();
			ErrorLocation = ex.Message.ToString();

			try
			{
				string filepath = Dir;

				if (!Directory.Exists(filepath))
				{
					Directory.CreateDirectory(filepath);

				}
				filepath = filepath + DateTime.Today.ToString("dd-MM-yyyy") + ".txt";   //Text File Name
				if (!File.Exists(filepath))
				{
					File.Create(filepath).Dispose();
				}
				using (StreamWriter sw = File.AppendText(filepath))
				{
					string error = "Log Written Date:" + " " + DateTime.Now.ToString() + line + "Error Line :" + " " + ErrorlineNo + line + "Error Message:" + " " + Errormsg + line + "Exception Type:" + " " + extype + line + "Error Location :" + " " + ErrorLocation + line;

					if (!string.IsNullOrEmpty(payload))
					{
						sw.WriteLine("-----------Received Payload on " + " " + DateTime.Now.ToString() + "-----------------");
						sw.WriteLine("-------------------------------------------------------------------------------------");
						sw.WriteLine(payload);
					}

					sw.WriteLine("-----------Exception Details on " + " " + DateTime.Now.ToString() + "-----------------");
					sw.WriteLine("-------------------------------------------------------------------------------------");
					sw.WriteLine(line);
					sw.WriteLine(error);
					sw.WriteLine("--------------------------------*End*------------------------------------------");
					sw.WriteLine(line);
					sw.Flush();
					sw.Close();

				}

			}
			catch (Exception e)
			{
				e.ToString();
			}
		}

	}
}
