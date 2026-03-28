using finance_tracker_comp586.data;
using System.Windows;

namespace finance_tracker_comp586
{
    public partial class App : Application
    {
        public static StockApiService StockApi { get; private set; } = null!;
        public static IUserRepository Users { get; private set; } = null!;
        public static AuthService Auth { get; private set; } = null!;
        public static User? CurrentUser { get; set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            string apiKey = Environment.GetEnvironmentVariable("ALPHAVANTAGE_API_KEY") ?? "INSERT_API_KEY";
            string? firebaseProjectId = Environment.GetEnvironmentVariable("FIREBASE_PROJECT_ID");

            if (string.IsNullOrWhiteSpace(firebaseProjectId))
            {
                throw new InvalidOperationException("Missing FIREBASE_PROJECT_ID environment variable.");
            }

            StockApi = new StockApiService(apiKey);
            Users = new FirebaseUserRepository(firebaseProjectId);
            Auth = new AuthService(Users);
        }
    }
}
