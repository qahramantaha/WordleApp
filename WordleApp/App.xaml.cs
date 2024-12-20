namespace WordleApp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            // Set the starting page to LoginPage
            MainPage = new NavigationPage(new LoginPage());
        }
    }
}
