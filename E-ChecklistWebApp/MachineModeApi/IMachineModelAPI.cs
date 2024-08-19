using E_ChecklistWebApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace E_ChecklistWebApp.MachineModeApi
{
    public interface IMachineModelAPI
    {
        Task<IEnumerable<pmisMachineName>> GetMachinesNameByListId(List<int> intsModel);
        Task<IEnumerable<EChecklistMachineModel>> GetModelMachines();
    }
}