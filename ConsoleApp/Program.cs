using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Net.Http;
using KoenZomers.Omnik.Api;

namespace KoenZomers.Omnik.PVOutput.ConsoleApp
{
    /// <summary>
    /// Console Application which fetches statistics from the Omnik Solar Inverter and posts them to PVOutput
    /// </summary>
    internal class Program
    {
        #region Fields

        /// <summary>
        /// The controller instance used to communicate with the Omnik Solar Inverter
        /// </summary>
        private static Controller _controller;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the Omnik Solar Inverter network address
        /// </summary>
        private static string OmnikInverterAddress
        {
            get { return ConfigurationManager.AppSettings["OmnikAddress"]; }
        }

        /// <summary>
        /// Gets the Omnik Solar Inverter Serial
        /// </summary>
        private static string OmnikInverterSerialNumber
        {
            get { return ConfigurationManager.AppSettings["OmnikSerial"]; }
        }

        /// <summary>
        /// Gets the API Key to use to communicate with the PV Output webservice
        /// </summary>
        private static string PvOutputApiKey
        {
            get { return ConfigurationManager.AppSettings["PVOutputApiKey"]; }
        }

        /// <summary>
        /// Gets the System ID of the solar set as registered with PV Output
        /// </summary>
        private static string PvOutputSystemId
        {
            get { return ConfigurationManager.AppSettings["PVOutputSystemId"]; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Application startup
        /// </summary>
        private static void Main()
        {
            // Verify if all required configuration is present in the App.config file
            if (!IsAllRequiredConfigurationPresent())
            {
                Console.WriteLine("Can't execute due to required configuration missing from App.config file");
                Environment.Exit(1);    
            }

            Console.WriteLine("Initializing Omnik Solar Inverter Controller");
            
            // Create a new instance of the controller to communicate with the Omnik API
            _controller = new Controller();

            // Attach an event handler to the event where statistics have been received from the Omnik Solar Inverter
            _controller.OmnikStatisticsAvailable += OmnikStatisticsAvailable;

            Console.WriteLine("Fetching data from Omnik Solar Inverter with serial number {0} from {1}", OmnikInverterSerialNumber, OmnikInverterAddress);

            // Initiate the pull from the Omnik. Provide its IP address here and its Wifi serial number as that is used as a form of authentication to get the data.
            _controller.PullData(OmnikInverterAddress, OmnikInverterSerialNumber);

            // We need to stay in a loop here to keep the Console Application alive while in the meantime the statistics are being awaited from the Omnik Solar Inverter
            var startedWaiting = DateTime.Now;
            do
            {
                System.Threading.Thread.Sleep(500);

            }
            // Wait at most 2 minutes for the data to come in, otherwise give up and exit
            while (DateTime.Now.Subtract(startedWaiting) < TimeSpan.FromMinutes(2));
        }

        /// <summary>
        /// Verifies if all required configuration has been entered in the App.config file
        /// </summary>
        /// <returns>True if all required configuration is present, false if not</returns>
        private static bool IsAllRequiredConfigurationPresent()
        {
            var allConfigPresent = true;

            if (string.IsNullOrWhiteSpace(OmnikInverterAddress))
            {
                Console.WriteLine("You need to enter the Omnik Solar Inverter network address in the App.config file");
                allConfigPresent = false;
            }

            if (string.IsNullOrWhiteSpace(OmnikInverterSerialNumber))
            {
                Console.WriteLine("You need to enter the Omnik Solar Inverter serial number in the App.config file");
                allConfigPresent = false;
            }

            if (string.IsNullOrWhiteSpace(PvOutputApiKey))
            {
                Console.WriteLine("You need to enter the PV Output API Key in the App.config file");
                allConfigPresent = false;
            }

            if (string.IsNullOrWhiteSpace(PvOutputSystemId))
            {
                Console.WriteLine("You need to enter the PV Output System ID in the App.config file");
                allConfigPresent = false;
            }

            return allConfigPresent;
        }

        /// <summary>
        /// Triggered when statistics have become available in response to a data push or pull action
        /// </summary>
        /// <param name="statistics">Statistics instance with all the information parsed from the information retrieved from the Omnik</param>
        private static void OmnikStatisticsAvailable(Statistics statistics)
        {
            Console.WriteLine("Statistics ready @ {0:dddd d MMMM yyyy HH:mm:ss}", DateTime.Now);
            Console.WriteLine("Wifi Module Serial number: {0}", statistics.WifiModuleSerialNumber);
            Console.WriteLine("Inverter Serial number: {0}", statistics.InverterSerialNumber);
            Console.WriteLine("Inverter main firmware version: {0}", statistics.MainFirmwareVersion);
            Console.WriteLine("Inverter slave firmware version: {0}", statistics.SlaveFirmwareVersion);
            Console.WriteLine("Temperature: {0}C", statistics.Temperature);
            Console.WriteLine("Hours active since last reset: {0} hours", statistics.HoursActive);
            Console.WriteLine("Todays production: {0} kWh", statistics.ProductionToday);
            Console.WriteLine("Total production: {0} kWh", statistics.ProductionTotal);
            Console.WriteLine("Current production #1: {0} Watts", statistics.ProductionCurrent1);
            Console.WriteLine("DC Input #1: {0} volt", statistics.PVVoltageDC1);
            Console.WriteLine("DC Input #1: {0} Amps", statistics.IVAmpsDC1);
            Console.WriteLine("AC Output #1: {0} volt", statistics.PVVoltageAC1);
            Console.WriteLine("AC Output #1: {0} Amps", statistics.IVAmpsAC1);
            Console.WriteLine("Frequency AC: {0} Hz", statistics.FrequencyAC);

            SendStatisticsToPvOutput(PvOutputSystemId, PvOutputApiKey, statistics.ProductionCurrent1, statistics.ProductionToday, statistics.PVVoltageAC1, statistics.Temperature, null);

            Environment.Exit(0);
        }

        /// <summary>
        /// Sends solar statistics to PV Output
        /// </summary>
        /// <param name="pvOutputSystemId">The system ID of the solar set as registered and generated by PVOutput. This can be found under settings at the PVOutput website at the bottom.</param>
        /// <param name="pvOutPutApiKey">The API key of PVOutput. This needs to be enabled and generated from the bottom API Settings section under settings at the PVOutput website.</param>
        /// <param name="electricityProductionCurrent">The current electricity production in Watts</param>
        /// <param name="electricityProductionToday">The electricity production today in kWh</param>
        /// <param name="electricityCurrentVoltage">The current voltage delivered to the net</param>
        /// <param name="temperature">The outside temperature</param>
        /// <param name="electricityCurrentUsage">Your current electricity usage in Watts</param>
        private static void SendStatisticsToPvOutput(string pvOutputSystemId, string pvOutPutApiKey, decimal? electricityProductionCurrent, decimal? electricityProductionToday, decimal? electricityCurrentVoltage, decimal? temperature, int? electricityCurrentUsage)
        {
            // Post the statistics to the PVOutput API
            Console.WriteLine("Posting statistics to PVOutput for system {0}", pvOutputSystemId);

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("X-Pvoutput-Apikey", pvOutPutApiKey);
                client.DefaultRequestHeaders.Add("X-Pvoutput-SystemId", pvOutputSystemId);

                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("d", DateTime.Now.ToString("yyyyMMdd")),
                    new KeyValuePair<string, string>("t", DateTime.Now.ToString("HH:mm")),
                    electricityProductionToday.HasValue ? new KeyValuePair<string, string>("v1", decimal.Multiply(electricityProductionToday.Value, 1000).ToString(CultureInfo.InvariantCulture)) : new KeyValuePair<string, string>(),
                    electricityProductionCurrent.HasValue ? new KeyValuePair<string, string>("v2", electricityProductionCurrent.Value.ToString(CultureInfo.InvariantCulture)) : new KeyValuePair<string, string>(),
                    electricityCurrentUsage.HasValue? new KeyValuePair<string, string>("v4", electricityCurrentUsage.ToString()) : new KeyValuePair<string, string>(),
                    electricityCurrentVoltage.HasValue ? new KeyValuePair<string, string>("v6", electricityCurrentVoltage.Value.ToString(CultureInfo.InvariantCulture)) : new KeyValuePair<string, string>(),
                    new KeyValuePair<string, string>("c1", "0"),
                    temperature.HasValue ? new KeyValuePair<string, string>("v5", temperature.Value.ToString(CultureInfo.InvariantCulture)) : new KeyValuePair<string, string>()
                });

                var result = client.PostAsync("http://pvoutput.org/service/r2/addstatus.jsp", content).Result;
                var response = result.Content.ReadAsStringAsync();
                Console.WriteLine(response.Result);
            }
        }

        #endregion
    }
}
