using System.Threading.Tasks;
using alert_center.models;

namespace alert_center
{
    public interface IRedditClient
    {
        Task<MeResponse> me();
        Task<CommentResponse> comments(string user, string before, string after, int limit);
        Task<alert_item> get_top_post(string user, string before, string after, int limit);
    }
}