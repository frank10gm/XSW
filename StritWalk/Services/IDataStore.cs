using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StritWalk
{
    public interface IDataStore<T>
    {
        Task<bool> AddItemAsync(T item);
        Task<bool> UpdateItemAsync(T item);
        Task<bool> DeleteItemAsync(string id);
        Task<T> GetItemAsync(string id);
        Task<IList<T>> GetItemsAsync(bool forceRefresh = false, int start = 0);
        Task<IList<T>> GetMapItemsAsync(bool forceRefresh = false, int start = 0);
    }
}
