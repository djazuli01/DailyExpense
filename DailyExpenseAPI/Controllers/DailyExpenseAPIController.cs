using DailyExpenseAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace DaliyExpenseAPI.Controllers
{
	[Route("api")]
	[ApiController]

	public class ExpenseAPIController : ControllerBase
	{
		private static readonly ExpenseAPIController instance = new ExpenseAPIController();
		public IList<Expense> DaliyExpense = new List<Expense>();
		public IList<Expense> listExpense = new List<Expense>();

		public static ExpenseAPIController Instance
		{
			get
			{
				return instance;
			}
		}

		[HttpGet]
		public IEnumerable<Expense> Get([FromQuery] Expense parameters)
		{
			if (parameters.id == 0)
            {
				parameters.id = null;
            }

			try
			{
				//String connectionString = "Data Source=localhost;Initial Catalog=AMDJZL;Integrated Security=True";
				String connectionString = "Data Source=192.168.100.189;Initial Catalog=AMDJZL;Integrated Security=True";
				using (SqlConnection connection = new SqlConnection(connectionString))
				{
					connection.Open();
					String sql = "EXEC GetExpenseByCriteria @id, @pengeluaran, @jmlPengeluaran, @jmlPengeluaran2, " +
						"@tglPengeluaran, @tglPengeluaran2";

					using (SqlCommand command = new SqlCommand(sql, connection))
					{
						command.Parameters.Add(new SqlParameter("@id", parameters.id ?? (object)DBNull.Value));
						command.Parameters.AddWithValue("@pengeluaran", string.IsNullOrEmpty(parameters.nama_pengeluaran) ? (object)DBNull.Value : parameters.nama_pengeluaran);
						command.Parameters.AddWithValue("@jmlPengeluaran", string.IsNullOrEmpty(parameters.jumlah_pengeluaran) ? (object)DBNull.Value : parameters.jumlah_pengeluaran);
						command.Parameters.AddWithValue("@jmlPengeluaran2", string.IsNullOrEmpty(parameters.jumlah_pengeluaran2) ? (object)DBNull.Value : parameters.jumlah_pengeluaran2);
						command.Parameters.AddWithValue("@tglPengeluaran", string.IsNullOrEmpty(parameters.tanggal_pengeluaran) ? (object)DBNull.Value : parameters.tanggal_pengeluaran);
						command.Parameters.AddWithValue("@tglPengeluaran2", string.IsNullOrEmpty(parameters.tanggal_pengeluaran2) ? (object)DBNull.Value : parameters.tanggal_pengeluaran2);
										
						using (SqlDataReader reader = command.ExecuteReader())
						{
							while (reader.Read())
							{
								Expense Expense = new Expense();
								Expense.id = reader.GetInt32(0);
								Expense.nama_pengeluaran = reader.GetString(1);
								Expense.jumlah_pengeluaran = "" + reader.GetDecimal(2);
								Expense.tanggal_pengeluaran = "" + reader.GetDateTime(3).ToString("yyyy-MM-dd");
								if (!reader.IsDBNull(4))
								{
									Expense.catatan = reader.GetString(4);
								}
								else
								{
									// Handle the case where the value is null, e.g., assign a default value or handle it as needed.
									Expense.catatan = "";
								}

								DaliyExpense.Add(Expense);
							}
						}
					}
				}
				return DaliyExpense;
			}
			catch (Exception ex)
			{
				Console.WriteLine("Exception: " + ex.ToString());
				return DaliyExpense;
			}
			finally
			{
				DaliyExpense = new List<Expense>();
			}
		}

		[HttpPost]
		public void Post([FromBody] Expense Expense)
		{
			try
			{
				String connectionString = "Data Source=localhost;Initial Catalog=AMDjZL;Integrated Security=True";
				using (SqlConnection connection = new SqlConnection(connectionString))
				{
					connection.Open();
					String sql = "insert into pengeluaran " +
						"values(@pengeluaran, @jmlPengeluaran, @tglPengeluaran, @note, 'ahmad djazuli', getdate())";

					using (SqlCommand command = new SqlCommand(sql, connection))
					{
						command.Parameters.AddWithValue("@pengeluaran", Expense.nama_pengeluaran);
						command.Parameters.AddWithValue("@jmlPengeluaran", Expense.jumlah_pengeluaran);
						command.Parameters.AddWithValue("@tglPengeluaran", DateTime.Parse(Expense.tanggal_pengeluaran));
						command.Parameters.AddWithValue("@note", string.IsNullOrEmpty(Expense.catatan) ? (object)DBNull.Value : Expense.catatan);
						command.ExecuteNonQuery();
					}
				}
			}
			catch (Exception ex)
			{
				return;
			}
		}

		[HttpPut]
		public void Put([FromQuery] Expense exp)
		{
			try
			{
				String connectionString = "Data Source=localhost;Initial Catalog=AMDJZL;Integrated Security=True";
				using (SqlConnection connection = new SqlConnection(connectionString))
				{
					connection.Open();
					String sql = "EXEC UpdateExpense @id, @pengeluaran, @jmlPengeluaran, @tglPengeluaran, @note";

					using (SqlCommand command = new SqlCommand(sql, connection))
					{
						command.Parameters.AddWithValue("@id", exp.id);
						command.Parameters.AddWithValue("@pengeluaran", exp.nama_pengeluaran);
						command.Parameters.AddWithValue("@jmlPengeluaran", exp.jumlah_pengeluaran);
						command.Parameters.AddWithValue("@tglPengeluaran", DateTime.Parse(exp.tanggal_pengeluaran));
						command.Parameters.AddWithValue("@note", string.IsNullOrEmpty(exp.catatan) ? (object)DBNull.Value : exp.catatan);

						command.ExecuteNonQuery();
					}
				}
			}
			catch (Exception ex)
			{
				return;
			}
		}


	}
}