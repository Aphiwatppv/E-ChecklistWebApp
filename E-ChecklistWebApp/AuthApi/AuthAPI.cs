using E_ChecklistWebApp.Models;
using MySqlUserEngineServices.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace E_ChecklistWebApp.AuthApi
{
    public class AuthAPI : IAuthAPI
    {
        private readonly HttpClient _httpClient;

        public AuthAPI(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("http://th1sroeeii1:1250/");
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<EchecklistAuthenticationWithoutHash> LoginAsync(EChecklistInputLogIn userInput)
        {
            var json = JsonConvert.SerializeObject(userInput);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("api/Authen/Login", content);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                var users = JsonConvert.DeserializeObject<EchecklistAuthenticationWithoutHash>(result);

                return users ?? new EchecklistAuthenticationWithoutHash();
            }

            return new EchecklistAuthenticationWithoutHash();
        }


        public async Task<string> RegisterAsync(EchecklistInputAuthentication registerModel)
        {
            var json = JsonConvert.SerializeObject(registerModel);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("api/Authen/Register", content);

            return response.IsSuccessStatusCode ? "success" : "failure";
        }

        public async Task<IEnumerable<EChecklistAuthenDetails>> GetEntireEN()
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/Authen/GetAllUser");

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    var machines = JsonConvert.DeserializeObject<IEnumerable<EChecklistAuthenDetails>>(result);

                    return machines ?? Enumerable.Empty<EChecklistAuthenDetails>();
                }
                else
                {
                    return Enumerable.Empty<EChecklistAuthenDetails>();
                }
            }
            catch (Exception ex)
            {
                // Log the exception as needed
                return Enumerable.Empty<EChecklistAuthenDetails>();
            }
        }

    }
}