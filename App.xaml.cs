using finance_tracker_comp586.services;
using System.Windows;

namespace finance_tracker_comp586
{
    public partial class App : Application
    {
        public static StockApiService StockApi { get; private set; } = null!;
        public static UserRepository Users { get; private set; } = null!;
        public static AuthService Auth { get; private set; } = null!;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            StockApi = new StockApiService("INSERT_API_KEY");
            Users = new UserRepository();
            Auth = new AuthService(Users);
        }
    }

}
