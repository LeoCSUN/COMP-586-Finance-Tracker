// Handles login/register logic

namespace finance_tracker_comp586
{
    public class AuthService
    {
        private UserRepository userRepository;

        public AuthService(UserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        public User Register(string username, string password, string name)
        {
            if (userRepository.GetUser(username) != null)
            {
                throw new InvalidOperationException("Username already taken.");
            }

            if (username == password)
            {
                throw new InvalidOperationException("Username and password cannot be the same.");
            }

            User user = new User(username, password, name);
            userRepository.AddUser(user);
            return user;
        }

        public bool Login(string username, string password)
        {
            User user = userRepository.GetUser(username);
            return user.VerifyPassword(password);
        }
    }
}