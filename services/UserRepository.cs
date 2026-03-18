using finance_tracker_comp586.services;
using System.Net.Http;
using System.Text.Json;
using System.Text;

namespace finance_tracker_comp586
{
    public class UserRepository : IUserRepository
    {
        private readonly HttpClient httpClient;

        public UserRepository()
        {
            httpClient = new HttpClient();
        }

        public User GetUser(string username)
        {
            var response = httpClient.GetAsync($"https://your-api.com/users/{username}").Result;

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            string json = response.Content.ReadAsStringAsync().Result;
            var dto = JsonSerializer.Deserialize<UserDto>(json);

            return new User(dto.Username, dto.PasswordHash, dto.Salt, dto.Name);
        }

        public void AddUser(User user)
        {
            var dto = new UserDto
            {
                Username = user.GetUsername(),
                PasswordHash = user.GetPasswordHash(),
                Salt = user.GetSalt(),
                Name = user.Name()
            };

            string json = JsonSerializer.Serialize(dto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            httpClient.PostAsync("https://your-api.com/users", content).Wait();
        }

        public void RemoveUser(string username)
        {
            httpClient.DeleteAsync($"https://your-api.com/users/{username}").Wait();
        }
    }
}