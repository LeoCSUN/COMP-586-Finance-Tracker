using finance_tracker_comp586.data;

namespace finance_tracker_comp586
{
    public class AuthService
    {
        private readonly IUserRepository _userRepository;

        public AuthService(IUserRepository userRepository) => _userRepository = userRepository;

        public User Register(string username, string password, string firstName, string lastName)
        {
            if (_userRepository.GetUser(username) != null)
                throw new InvalidOperationException("Username already taken.");

            if (username == password)
                throw new InvalidOperationException("Username and password cannot be the same.");

            User user = new User(username, password, firstName, lastName);
            _userRepository.AddUser(user);
            return user;
        }

        public User? Login(string username, string password)
        {
            User? user = _userRepository.GetUser(username);
            return user?.VerifyPassword(password) == true ? user : null;
        }
    }
}