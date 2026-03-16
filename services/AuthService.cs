public class AuthService
{
    private UserRepository userRepository;

    public AuthService(UserRepository userRepository)
    {
        this.userRepository = userRepository;
    }

    // --- ADD USERNAME + PASSWORD RULES TO THIS! ---
    public User Register(string username, string password, string name)
    {
        if (userRepository.GetUser(username) != null)
        {
            throw new InvalidOperationException("Username already taken.");
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