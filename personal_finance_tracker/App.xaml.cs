using personal_finance_tracker.data;
using System.Windows;

namespace personal_finance_tracker
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

            StockApi = new StockApiService(apiKey);
            Users = new SqliteUserRepository();
            Auth = new AuthService(Users);
        }
    }
}
