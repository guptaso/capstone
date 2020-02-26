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
        private ObservableCollection<alert_item> Alerts;
        public MainWindow()
        {
            InitializeComponent();
        }


        private async void get_reddit_post(object sender, RoutedEventArgs e)
        {
            Environment.SetEnvironmentVariable("REDDIT_USER", "ABoostedMonkey");
            Environment.SetEnvironmentVariable("REDDIT_PASSWORD", "lacroSSe14");
            Environment.SetEnvironmentVariable("REDDIT_APP_ID", "XiufHRbWtVou5w");
            Environment.SetEnvironmentVariable("REDDIT_APP_SECRET", "PBoaXKaywkPlBQ-6oR-f_CRRFOE");
            program myProg = new program();
            string[] args = new string[2];
            await myProg.auth(args);
        }
    }

    class program
    {

        private static readonly string REDDIT_USER = Environment.GetEnvironmentVariable("REDDIT_USER");
        private static readonly string REDDIT_PASSWORD = Environment.GetEnvironmentVariable("REDDIT_PASSWORD");
        private static readonly string REDDIT_APP_ID = Environment.GetEnvironmentVariable("REDDIT_APP_ID");
        private static readonly string REDDIT_APP_SECRET = Environment.GetEnvironmentVariable("REDDIT_APP_SECRET");     

        public async Task auth(string[] args)
        {
            IAuthenticationService authService = OauthAuthenticationService.GetAuthenticationService();
            var token = await authService.Authenticate(REDDIT_USER, REDDIT_PASSWORD, REDDIT_APP_ID, REDDIT_APP_SECRET);
            Console.WriteLine($"Granted token: {token.token.ToString()}\nExpires At: {token.expiresAt.ToShortTimeString()}");
            IRedditClient client = new RedditClient(token);
            var me = await client.me();
            Console.WriteLine($"Begin processing for /u/{me.name}\nHas {me.link_karma} link karma, and {me.comment_karma} comment karma.");
            var comments = await client.comments(REDDIT_USER, null, null, 25);
            Console.WriteLine($"Request Success: {comments.IsSuccess}");
            var posts = await client.get_top_post(REDDIT_USER, null, null, 25);
            while (!string.IsNullOrEmpty(comments.after) && comments.children.Length != 0)
            {
                Console.WriteLine($"Processing {comments.children.Length} comments.");
                foreach (var comment in comments.children)
                {
                    if (comment.ups < 0)
                    {
                        Console.WriteLine($"Comment {comment.id}, created at {comment.createdDt} has a score of {comment.score}");
                    }
                }
                comments = await client.comments(REDDIT_USER, null, comments.after, 25);
                Console.WriteLine($"Request Success: {comments.IsSuccess}");
                Console.WriteLine($"After is: {comments.after}");
            }
        }
    }

}
