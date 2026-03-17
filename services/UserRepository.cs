namespace finance_tracker_comp586
{
    public class UserRepository
    {
        private List<User> users;

        public User GetUser(string username)
        {
            foreach (User u in users)
            {
                if (u.GetUsername() == username)
                {
                    return u;
                }
            }

            return null;
        }

        public void AddUser(User user)
        {
            users.Add(user);
        }

        public void RemoveUser(string username)
        {
            foreach (User u in users)
            {
                if (u.GetUsername() == username)
                {
                    users.Remove(u);
                }
            }
        }
    }
}