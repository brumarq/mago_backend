using Application.DTOs;
using Application.Exceptions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Application.ApplicationServices.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using IAuthenticationService = Application.ApplicationServices.Interfaces.IAuthenticationService;

namespace Application.ApplicationServices
{
    public class NotificationTokenService : INotificationTokenService
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly HttpClient _httpClient;
        private readonly string? _baseUri;
        private readonly string? _orchestratorApiKey;

        public NotificationTokenService(IHttpClientFactory httpClientFactory, IConfiguration configuration, IAuthenticationService authenticationService)
        {
            this._httpClientFactory = httpClientFactory;
            _httpClient = httpClientFactory.CreateClient();
            _baseUri = configuration["ApiRequestUris:NotificationBaseUri"];
            _orchestratorApiKey = configuration["OrchestratorApiKey"];
            _authenticationService = authenticationService;
        }

        public async Task<NotificationTokenOnUserDTO> GetNotificationTokensByUserIdAsync(string userId)
        {
            try
            {
                var getRequest = new HttpRequestMessage(HttpMethod.Get, $"{_baseUri}notification/notificationToken/user/{userId}");
                getRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _authenticationService.GetToken());
                getRequest.Headers.Add("X-Orchestrator-Key", _orchestratorApiKey);

                var response = await _httpClient.SendAsync(getRequest);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var notificationTokenOnUserDTO = JsonConvert.DeserializeObject<NotificationTokenOnUserDTO>(responseContent);

                    return notificationTokenOnUserDTO;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new CustomException(errorContent, response.StatusCode);
                }
            }
            catch (HttpRequestException e)
            {
                throw new Exception($"Request failed: {e.Message}");
            }
        }
    }
}
