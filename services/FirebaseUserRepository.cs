using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace finance_tracker_comp586.services
{
    class FirebaseUserRepository : IUserRepository
    {
        private readonly HttpClient _httpClient;
        private readonly string _projectId;

        public FirebaseUserRepository(string projectId)
        {
            _httpClient = new HttpClient();
            _projectId = projectId;
        }
        public User GetUser(string username)
        {
            string url = $"https://firestore.googleapis.com/v1/projects/{_projectId}/databases/(default)/documents/users/{username}";

            var response = _httpClient.GetAsync(url).Result;

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            string json = response.Content.ReadAsStringAsync().Result;
            using JsonDocument doc = JsonDocument.Parse(json);

            var fields = doc.RootElement.GetProperty("fields");

            string uname = fields.GetProperty("username").GetProperty("stringValue").GetString();
            string hash = fields.GetProperty("passwordHash").GetProperty("stringValue").GetString();
            string salt = fields.GetProperty("salt").GetProperty("stringValue").GetString();
            string name = fields.GetProperty("name").GetProperty("stringValue").GetString();

            return new User(uname, hash, salt, name);
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

            string json = JsonSerializer.Serialize(new
            {
                fields = new
                {
                    username = new { stringValue = dto.Username },
                    passwordHash = new { stringValue = dto.PasswordHash },
                    salt = new { stringValue = dto.Salt },
                    name = new { stringValue = dto.Name }
                }
            });

            string url = $"https://firestore.googleapis.com/v1/projects/{_projectId}/databases/(default)/documents/users?documentId={dto.Username}";

            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = _httpClient.PostAsync(url, content).Result;

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to add user to Firebase: " + response.Content.ReadAsStringAsync().Result);
            }
        }
        public void RemoveUser(string username)
        {
            string url = $"https://firestore.googleapis.com/v1/projects/{_projectId}/databases/(default)/documents/users/{username}";

            var response = _httpClient.DeleteAsync(url).Result;

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to delete user: " + response.Content.ReadAsStringAsync().Result);
            }
        }
    }
}
