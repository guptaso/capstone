using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace alert_center
{
    public class alert_item
    {
        private string image;
        private string website;
        public string Website
        { 
          get { return website; }
          set { website = value; }
        }
        private string body_text;
        private string date;

        public alert_item()
        {
            image = "URL";
            website = "siteName";
            body_text = "text/quote";
            date = "";
        }

        public void set_image(string image_url)
        {
            image = image_url;
        }
        public void set_body_text(string new_text)
        {
            body_text = new_text;
        }
        public void set_date(string new_date)
        {
            date = new_date;
        }
    }

    public class reddit_item : alert_item
    {
        private string subreddit;
        private int upvotes;
        private string media_link;
        private string posting_account;
        private string permalink;

        public reddit_item()
        {
            subreddit = "sub";
            upvotes = 0;
            Website = ("https://reddit.com");
            media_link = "";
            posting_account = "";
        }

        public void set_subreddit(string subreddit)
        {
            this.subreddit = subreddit;
        }

        public void set_media_link(string media_link)
        {
            this.media_link = media_link;
        }

        public void set_upvotes(int upvotes)
        {
            this.upvotes = upvotes;
        }

        public void set_posting_account(string posting_account)
        {
            this.posting_account = posting_account;
        }

        public void set_permalink(string permalink)
        {
            this.permalink = permalink;
        }
    }
}
