using System.Data;
using System.Data.SqlClient;
using System.Text.Json;
using WeatherApp.Models.WeatherModels;

namespace WeatherApp.Services.Weather
{
    public class WeatherService
    {
        private readonly IConfiguration _configuration;

        public WeatherService(IConfiguration configuration) { _configuration = configuration; }

        public List<WeatherModel> GetWeater(DateTime day)
        {
            var weather = new List<WeatherModel>();
            using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DBconn")))
            {
                conn.Open();
                var cmd = new SqlCommand("GetWeather", conn);

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter("@day", day));

                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        weather.Add(new WeatherModel()
                        {
                            WeatherDate = (DateTime)rdr["WeatherDate"],
                            Degree = (decimal)rdr["Degree"]
                        });
                    }
                }
                conn.Close();

                return weather;
            }
        }

        public List<UpcomingWeather> GetUpcomingWeater()
        {
            var upcomingWeather = new List<UpcomingWeather>();
            using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DBconn")))
            {
                conn.Open();
                var cmd = new SqlCommand("GetUpcomingWeater", conn);

                cmd.CommandType = CommandType.StoredProcedure;

                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        upcomingWeather.Add(new UpcomingWeather()
                        {
                            WeatherDate = (DateTime)rdr["WeatherDate"],
                            Degree = (decimal)rdr["Degree"],
                            UpcomingWeatherDate = rdr["UpcomingDate"] == DBNull.Value ? null: (DateTime)rdr["UpcomingDate"]
                        });
                    }
                }
                conn.Close();

                return upcomingWeather;
            }
        }
        public bool SetOrUpdateWeather(List<WeatherModel> weatherModels)
        {
            var result = false;
            DataTable weather = new DataTable();
            weather.Columns.Add("WeatherDate", typeof(DateTime));
            weather.Columns.Add("Degree", typeof(decimal));
            using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DBconn")))
            {
                
                foreach (WeatherModel weatherModel in weatherModels)
                {
                    if (weatherModel.WeatherDate > DateTime.Now.Date.AddDays(8))
                        return false;

                    DataRow row = weather.NewRow();
                    row["WeatherDate"] = weatherModel.WeatherDate;
                    row["Degree"] = weatherModel.Degree;
                    weather.Rows.Add(row);
                }

                
                SqlCommand insertCommand = new SqlCommand("SetOrUpdateWeather", conn);
                insertCommand.CommandType = CommandType.StoredProcedure;
                SqlParameter tvpParam = insertCommand.Parameters.AddWithValue("@weahterData", weather);
                tvpParam.SqlDbType = SqlDbType.Structured;

                
                conn.Open();
                try
                {
                    insertCommand.ExecuteNonQuery();
                    result = true;
                }
                catch { 
                
                }
                
                conn.Close();
            }
            return result;
        }

        public bool SetWeather(List<WeatherModel> weatherModels)
        {
            var result = false;
            DataTable weather = new DataTable();
            weather.Columns.Add("WeatherDate", typeof(DateTime));
            weather.Columns.Add("Degree", typeof(decimal));


            foreach (WeatherModel weatherModel in weatherModels)
            {
                if (weatherModel.WeatherDate > DateTime.Now.Date.AddDays(8))
                    return false;

                DataRow row = weather.NewRow();
                row["WeatherDate"] = weatherModel.WeatherDate;
                row["Degree"] = weatherModel.Degree;
                weather.Rows.Add(row);
            }

            using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DBconn")))
            {

                SqlCommand insertCommand = new SqlCommand("SetWeather", conn);
                insertCommand.CommandType = CommandType.StoredProcedure;
                SqlParameter tvpParam = insertCommand.Parameters.AddWithValue("@weahterData", weather);
                tvpParam.SqlDbType = SqlDbType.Structured;


                conn.Open();
                try
                {
                    insertCommand.ExecuteNonQuery();
                    result = true;
                }
                catch
                {

                }

                conn.Close();
            }
            return result;
        }
    }
}
