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
        private string body_text;
        private string date;

        public alert_item()
        {
            image = "URL";
            website = "siteName";
            body_text = "text/quote";
            date = "date of post";
        }

        public void set_image(string image_url)
        {
            image = image_url;
        }
        public void set_website(string site_name)
        {
            website = site_name;
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
}
