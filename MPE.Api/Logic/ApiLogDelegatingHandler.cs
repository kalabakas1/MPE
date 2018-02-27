using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Functional.Maybe;
using MPE.Api.Extensions;
using MPE.Api.Interfaces;
using MPE.Api.Models;
using MPE.Api.Repositories;

namespace MPE.Api.Logic
{
    public class ApiLogDelegatingHandler : DelegatingHandler
    {
        private readonly IApiLogService _logService;
        private readonly IApiKeyRepository _keyRepository;
        public ApiLogDelegatingHandler()
        {
            _logService = new ApiLogService();
            _keyRepository = new ApiKeyRepository();
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var authHeaderValue = request.Headers.GetValues(ApiConstants.AuthorizationHeaderName).FirstOrDefault();
            Maybe<ApiKey> maybeKey = Maybe<ApiKey>.Nothing;
            if (!string.IsNullOrEmpty(authHeaderValue))
            {
                maybeKey = _keyRepository.Get(authHeaderValue);
            }

            var apiLog = _logService.Create();
            apiLog.Ip = request.GetClientIpAddress();
            apiLog.Key = authHeaderValue;
            apiLog.KeyId = maybeKey.HasValue ? maybeKey.Value.Id : (int?)null;
            apiLog.RequestTimestamp = DateTime.Now;
            apiLog.RequestContent = request.Content.ReadAsStringAsync().Result;
            apiLog.RequestMethod = request.Method.Method;
            apiLog.RequestUrl = request.RequestUri.ToString();

            HttpResponseMessage response = null;
            try
            {
                response = await base.SendAsync(request, cancellationToken);
                apiLog.ResponseTimestamp = DateTime.Now;
                apiLog.ResponseContent = response.Content.ReadAsStringAsync().Result;
                apiLog.ResponseStatusCode = response.StatusCode;
            }
            catch
            {

            }
            finally
            {
                _logService.Save(apiLog);
            }

            return response;
        }
    }
}
