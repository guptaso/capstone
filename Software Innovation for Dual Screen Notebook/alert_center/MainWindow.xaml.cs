using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using alert_center.services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;



namespace alert_center
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            
            this.alerts.ItemsSource = list_items;
       
        }

        ObservableCollection<alert_item> list_items = new ObservableCollection<alert_item>();
        public ObservableCollection<alert_item> list_data
        {
            get
            {
                if(list_items.Count() <= 0)
                {
                    list_items.Add(new alert_item());
                }
                return list_items;
            }
        }


        private async void get_reddit_post(object sender, RoutedEventArgs e)
        {
            Environment.SetEnvironmentVariable("REDDIT_USER", "ABoostedMonkey");
            Environment.SetEnvironmentVariable("REDDIT_PASSWORD", "lacroSSe14");
            Environment.SetEnvironmentVariable("REDDIT_APP_ID", "XiufHRbWtVou5w");
            Environment.SetEnvironmentVariable("REDDIT_APP_SECRET", "PBoaXKaywkPlBQ-6oR-f_CRRFOE");
            alert_apis myProg = new alert_apis();
            string[] args = new string[2];
            reddit_item item = await myProg.get_reddit_post(args);
            list_items.Add(item);
        }
    }

    class alert_apis
    {

        private static readonly string REDDIT_USER = Environment.GetEnvironmentVariable("REDDIT_USER");
        private static readonly string REDDIT_PASSWORD = Environment.GetEnvironmentVariable("REDDIT_PASSWORD");
        private static readonly string REDDIT_APP_ID = Environment.GetEnvironmentVariable("REDDIT_APP_ID");
        private static readonly string REDDIT_APP_SECRET = Environment.GetEnvironmentVariable("REDDIT_APP_SECRET");     

        public async Task<reddit_item> get_reddit_post(string[] args)
        {
            IAuthenticationService authService = OauthAuthenticationService.GetAuthenticationService();
            var token = await authService.Authenticate(REDDIT_USER, REDDIT_PASSWORD, REDDIT_APP_ID, REDDIT_APP_SECRET);
            Console.WriteLine($"Granted token: {token.token.ToString()}\nExpires At: {token.expiresAt.ToShortTimeString()}");
            IRedditClient client = new RedditClient(token);
            var post = await client.get_top_post(REDDIT_USER, null, null, 25);
            return post;
            
        }
    }


}
