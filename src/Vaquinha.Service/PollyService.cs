using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using System;
using Vaquinha.Domain;

namespace Vaquinha.Service
{
    public class PollyService : IPollyService
    {
        private readonly GloballAppConfig _GloballAppConfig;
        private readonly ILogger<PollyService> _Logger;

        public PollyService(ILogger<PollyService> logger, GloballAppConfig globallAppConfig)
        {
            _Logger = logger;
            _GloballAppConfig = globallAppConfig;
        }

        public AsyncRetryPolicy CriarPoliticaWaitAndRetryPara(string method)
        {
            var policy = Policy.Handle<Exception>().WaitAndRetryAsync(_GloballAppConfig.Polly.QuantidadeRetry,
                attempt => TimeSpan.FromSeconds(_GloballAppConfig.Polly.TempoDeEsperaEmSegundos),
                (exception, calculatedWaitDuration) =>
                {
                    _Logger.LogError($"Erro ao acionar o metodo {method}. Total de tempo de retry até o momento: {calculatedWaitDuration}s");
                });

            return policy;
        }
    }
}
