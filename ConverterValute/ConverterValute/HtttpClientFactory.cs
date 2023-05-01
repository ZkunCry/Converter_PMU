using Flurl.Http.Configuration;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace ConverterValute
{
    class MyCustomHttpClientFactory : DefaultHttpClientFactory
    {
        public override HttpMessageHandler CreateMessageHandler()
        {
            return new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) 
                                                                        => true
            };
        }
    }
}
