using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Flurl.Http;
using Flurl.Http.Configuration;

namespace ConverterValute
{
    class ConverterModel
    {
        private const string Url = "https://www.cbr-xml-daily.ru";
        public ConverterModel()
        {
            FlurlHttp.ConfigureClient(Url,
                settings => settings.Settings.HttpClientFactory = new MyCustomHttpClientFactory());
        }
        public async Task<ValuteData> GetDateAsync(DateTime currentDate)
        {
            ValuteData ListValute = null;
            string date = null;
            string GetData = null;

            for (DateTime newdate = currentDate; newdate<=DateTime.Now &&ListValute == null ; newdate = newdate.AddDays(-1))
            {
                date = $"{newdate:yyyy/MM/dd}".Replace(".", "//");
                GetData = $"{Url}/archive/{date}/daily_json.js";
                try
                {
                    ListValute = await GetData.GetJsonAsync<ValuteData>();
                }
                catch (FlurlHttpException exception)
                {
                    if (exception.StatusCode != 404)
                    {
                        throw new Exception($"Error code: {exception.StatusCode}");
                    }
                }
            }
            if (ListValute != null)
                ListValute.Valute.Add("RUB", new Valute
                {
                    CharCode = "RUB",
                    ID = "0",
                    Name = "Российский рубль",
                    Nominal = 1,
                    Value = 1.0,
                });
            return ListValute;
        }
    }
}

   

