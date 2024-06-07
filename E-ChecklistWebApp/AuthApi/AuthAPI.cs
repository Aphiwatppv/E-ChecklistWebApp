using E_ChecklistWebApp.Models;
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
            _httpClient.BaseAddress = new Uri("https://localhost:44367/");
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<EchecklistAllowingProcess> LoginAsync(EChecklistInputLogIn userInput)
        {
            var json = JsonConvert.SerializeObject(userInput);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("api/Authen/Login", content);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                var users = JsonConvert.DeserializeObject<EchecklistAllowingProcess>(result);


                return users ?? new EchecklistAllowingProcess();
            }

            return new EchecklistAllowingProcess();
        }

        public async Task<string> RegisterAsync(EchecklistInputAuthentication registerModel)
        {
            var json = JsonConvert.SerializeObject(registerModel);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("api/Authen/Register", content);

            return response.IsSuccessStatusCode ? "success" : "failure";
        }
    }
}