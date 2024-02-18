using Dapper;
using CARDS.Business.Services.Repository;
using CARDS.Business.ViewModels;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CARDS.Business.Services
{
	public interface IDatabaseService
	{
		Task<StateViewModel<IEnumerable<T>>> QueryAsync<T>(string sql, object param = null, int? commandTimeout = null, CommandType? commandType = CommandType.StoredProcedure);
		void Dispose();
	}

	public class DatabaseService : IDatabaseService
	{
		//protected readonly IDatabaseRepository GenericRepository;

		public DatabaseService()
		{
			//GenericRepository = iRepository;
		}

		/*Dapper function wrappers*/
		
		public async Task<StateViewModel<IEnumerable<T>>> QueryAsync<T>(string sql, object param = null, int? commandTimeout = null, CommandType? commandType = CommandType.StoredProcedure)
		{
			StateViewModel<IEnumerable<T>> state = new StateViewModel<IEnumerable<T>>();
			try
			{
				IEnumerable<T> value;
				using (IDbConnection db = new SqlConnection(DBService.DBConnection()))
				{
					value = await db.QueryAsync<T>(sql + "  ", param, commandType: commandType, commandTimeout: commandTimeout);

				}
				if (value != null)
				{
					state.Code = 200;

					state.Msg = "Success";

					state.Data = value;
				}
				else
				{
					state.Code = 300;

					state.Msg = "No Record Found";
				}
			}
			catch (Exception msg)
			{
				state.Code = 500;

				state.Msg = msg.Message;
			}

			return state;
		}

		public void Dispose()
		{
			//GenericRepository.Dispose();
		}
	}
}
