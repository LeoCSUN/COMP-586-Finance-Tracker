using finance_tracker_comp586.services;
using System.Windows;

namespace finance_tracker_comp586
{
    public partial class App : Application
    {
        public static StockApiService StockApi { get; private set; }
        public static UserRepository Users { get; private set; }
        public static AuthService Auth { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var firebaseRepo = new FirebaseUserRepository("your-firebase-project-id");
            Auth = new AuthService(firebaseRepo);

            StockApi = new StockApiService("INSERT_API_KEY");
            Users = new UserRepository();
            Auth = new AuthService(Users);
        }
    }

}
