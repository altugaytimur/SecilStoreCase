using SecilStoreCase.Entities.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecilStoreCase.Dal.Interfaces.Repositories
{
    public interface IConfigurationItemRepository
    {
        Task<IEnumerable<ConfigurationItemModel>> GetAllConfigurationsAsync(string applicationName);
        Task<IEnumerable<ConfigurationItemModel>> GetAllConfigurationsAsync();
        Task<ConfigurationItemModel> GetConfigurationByIdAsync(string id);
        Task CreateConfigurationAsync(ConfigurationItemModel configurationItemModel);
        Task<bool> UpdateConfigurationAsync(ConfigurationItemModel configurationItemModel);
        Task<bool> DeleteAsync(string id);
    }
}
