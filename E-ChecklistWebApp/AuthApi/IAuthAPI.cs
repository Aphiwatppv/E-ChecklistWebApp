using E_ChecklistWebApp.Models;
using MySqlUserEngineServices.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace E_ChecklistWebApp.AuthApi
{
    public interface IAuthAPI
    {
        Task<EchecklistAuthenticationWithoutHash> LoginAsync(EChecklistInputLogIn userInput);
        Task<string> RegisterAsync(EchecklistInputAuthentication registerModel);
        Task<IEnumerable<EChecklistAuthenDetails>> GetEntireEN();
    }
}