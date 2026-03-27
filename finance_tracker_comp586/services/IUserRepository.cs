namespace finance_tracker_comp586.services
{
    public interface IUserRepository
    {
        User? GetUser(string username);
        void AddUser(User user);
        void RemoveUser(string username);
    }
}
