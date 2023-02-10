using Quartz;
using System.Data;
using System.Data.SqlClient;

namespace WeatherApp
{
    public class Job : IJob
    {
        private readonly IConfiguration _configuration;

        public Job(IConfiguration configuration) { _configuration = configuration; }
        public Task Execute(IJobExecutionContext context)
        {
            
            context.NextFireTimeUtc.ToString();

            try
            {
                using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DBconn")))
                {

                    SqlCommand insertCommand = new SqlCommand("DeleteOldData", conn);
                    insertCommand.CommandType = CommandType.StoredProcedure;

                    conn.Open();
                    
                    insertCommand.ExecuteNonQuery();
                       
                    conn.Close();
                }
            }
            catch
            {
                throw;
            }

            return Task.CompletedTask;
        }
    }
}
