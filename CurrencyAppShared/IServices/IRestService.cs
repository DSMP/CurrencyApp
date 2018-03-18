using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyAppShared.IServices
{
    public interface IRestService
    {
        Task<string> GetDataAsync(string urlPrefix);
        Task PostDataAsync(string urlPrefix, string item, bool isNewItem = false);
        Task UpdateDataAsync(string item);
        Task DeleteDataAsync(int id);
    }
}
