using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Api
{
    public class PVOutput
    {
        #region Properties

        /// <summary>
        /// Date and time at which the data is collected
        /// </summary>
        public DateTime? DataCollectedAt { get; set; }

        #endregion

        public PVOutput()
        {
            DataCollectedAt = DateTime.Now;

        }

        public void SubmitStatistics()
        {
            // Post the statistics to the PVOutput API
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("X-Pvoutput-Apikey", ConfigurationManager.AppSettings["PVOutputApiKey"]);
                client.DefaultRequestHeaders.Add("X-Pvoutput-SystemId", ConfigurationManager.AppSettings["PVOutputSystemId"]);
                var content = new FormUrlEncodedContent(new[] 
                {
                    DataCollectedAt.HasValue ? new KeyValuePair<string, string>("d", DataCollectedAt.Value.ToString("yyyyMMdd")) : new KeyValuePair<string, string>(),
                    new KeyValuePair<string, string>("t", DataCollectedAt.Value.ToString("HH:mm")),
                    new KeyValuePair<string, string>("v2", statistics.ProductionToday.ToString(CultureInfo.InvariantCulture)),
                    house != null && house.ElectricityCurrentUsage.HasValue ? new KeyValuePair<string, string>("v4", house.ElectricityCurrentUsage.Value.ToString()) : new KeyValuePair<string, string>(),
                    new KeyValuePair<string, string>("v6", statistics.PVVoltageAC1.ToString(CultureInfo.InvariantCulture)),
                    new KeyValuePair<string, string>("c1", "0"),
                    room.Temperature.HasValue ? new KeyValuePair<string, string>("v5", room.Temperature.Value.ToString(CultureInfo.InvariantCulture)) : new KeyValuePair<string, string>()
                });

                var result = client.PostAsync("http://pvoutput.org/service/r2/addstatus.jsp", content).Result;
                var response = result.Content.ReadAsStringAsync();
                Console.WriteLine(response.Status);
        }
    }
}
