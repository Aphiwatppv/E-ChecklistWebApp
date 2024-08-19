using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Web;
using System.Threading.Tasks;
using MySqlUserEngineServices.Model;
using E_ChecklistWebApp.Models;
using Newtonsoft.Json;
using System.Text;

namespace E_ChecklistWebApp.MachineModeApi
{
    public class MachineModelAPI : IMachineModelAPI
    {
        private readonly HttpClient _httpClient;
        private string _plant;

        public MachineModelAPI(HttpClient httpClient, string plant)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("http://th1sroeeii1:6001/ModelMachineAPI/");
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _plant = plant;
        }

        public async Task<IEnumerable<EChecklistMachineModel>> GetModelMachines()
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/dev/GetMachineModels?plant={Uri.EscapeDataString(_plant)}");

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    var machines = JsonConvert.DeserializeObject<IEnumerable<EChecklistMachineModel>>(result);
                    _httpClient.Dispose();
                    return machines ?? Enumerable.Empty<EChecklistMachineModel>();
                   
                }
                else
                {
                    return Enumerable.Empty<EChecklistMachineModel>();
                }
            }
            catch (Exception ex)
            {
                // Log the exception as needed
                return Enumerable.Empty<EChecklistMachineModel>();
            }
        }

        public async Task<IEnumerable<pmisMachineName>> GetMachinesNameByListId(List<int> intsModel)
        {
            try
            {
                var modelIds = string.Join(",", intsModel);
                var response = await _httpClient.GetAsync($"api/dev/GetMachineName?plant={Uri.EscapeDataString(_plant)}&ints={Uri.EscapeDataString(modelIds)}");

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    var machines = JsonConvert.DeserializeObject<IEnumerable<pmisMachineName>>(result);

                    return machines ?? Enumerable.Empty<pmisMachineName>();
                }
                else
                {
                    return Enumerable.Empty<pmisMachineName>();
                }
            }
            catch (Exception ex)
            {
                // Log the exception as needed
                return Enumerable.Empty<pmisMachineName>();
            }
        }


    }
}