using System.Windows;

namespace financial_tracker
{
    public partial class App : Application
    {
        public static StockApiService StockApi { get; private set; }
        public static UserRepository Users { get; private set; }
        public static AuthService Auth { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            StockApi = new StockApiService("INSERT_API_KEY");
            Users = new UserRepository();
            Auth = new AuthService(Users);
        }
    }

}
