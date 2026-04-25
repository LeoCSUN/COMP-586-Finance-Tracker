using System.Threading.Tasks;

namespace personal_finance_tracker.data
{
    public interface IUserRepository
    {
        User? GetUser(string username);
        void AddUser(User user);
        Task UpdateUser(User user);
        void RemoveUser(string username);
    }
}
