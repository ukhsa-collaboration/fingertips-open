using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Profiles.MainUI.APIUserAccess.Response;
using Profiles.MainUI.Models.UserAccess;

namespace Profiles.MainUI.APIUserAccess
{
    public class LoginClient : ClientBase, ILoginClient
    {
        private const string RegisterUri = "register";

        private const string LoginUri = "authtoken";
        //private IApiClient apiClient;
        public LoginClient(IApiClient apiClient) : base(apiClient)
        {
        }

        public async Task<RegisterResponse> Register(RegisterViewModel viewModel)
        {
            var response = await apiClient.PostJsonEncodedContent(RegisterUri, viewModel);
            var registerResponse = await CreateJsonResponse<RegisterResponse>(response);
            return registerResponse;
        }

        public async Task<TokenResponse> Login(LoginViewModel viewModel)
        {
            var response = await apiClient.PostFormEncodedContent(LoginUri, 
                new KeyValuePair<string, string>("grant_type", "password"), 
                new KeyValuePair<string, string>("username", viewModel.UserName), 
                new KeyValuePair<string, string>("password", viewModel.Password));
            var tokenResponse = await CreateJsonResponse<TokenResponse>(response);
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await DecodeContent<dynamic>(response);
                tokenResponse.ErrorState = new ErrorStateResponse
                {
                    ModelState = new Dictionary<string, string[]>
                    {
                        {errorContent["error"], new string[] {errorContent["error_description"]}}
                    }
                };
                return tokenResponse;
            }

            var tokenData = await DecodeContent<dynamic>(response);
            tokenResponse.Data = tokenData["access_token"];
            return tokenResponse;
        }
    }
}