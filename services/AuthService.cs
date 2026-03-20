// Handles login/register logic

using finance_tracker_comp586.services;

namespace finance_tracker_comp586
{
    public class AuthService
    {
        private readonly IUserRepository userRepository;

        public AuthService(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        public User Register(string username, string password, string firstName, string lastName)
        {
            if (userRepository.GetUser(username) != null)
            {
                throw new InvalidOperationException("Username already taken.");
            }

            if (username == password)
            {
                throw new InvalidOperationException("Username and password cannot be the same.");
            }

            User user = new User(username, password, firstName, lastName);
            userRepository.AddUser(user);
            return user;
        }

        public bool Login(string username, string password)
        {
            User? user = userRepository.GetUser(username);

            if (user == null)
            {
                return false;
            }

            return user.VerifyPassword(password);
        }
    }
}