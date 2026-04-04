using System.Threading.Tasks;

namespace finance_tracker_comp586.data
{
    public interface IUserRepository
    {
        User? GetUser(string username);
        void AddUser(User user);
        Task UpdateUser(User user);
        void RemoveUser(string username);
    }
}
