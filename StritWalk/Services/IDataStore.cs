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

        Task<bool> Login(string username, string password);        
        Task<string> Post(string id_user, string name, string audio, string lat, string lng, string description);
        Task<string> SignUp(string username, string password, string email);
        Task<User> GetMyUser(User me);
        Task<int> ILikeThis(string post_id, string action);
        Task<string> PostComment(string post_id, string comment);
        Task<IList<CommentsItem>> GetComments(string post_id);
    }
}
