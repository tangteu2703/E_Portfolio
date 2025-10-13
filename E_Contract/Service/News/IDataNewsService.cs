using E_Model.News;
using E_Model.Request.News;
using E_Model.Response;
using E_Model.Response.News;

namespace E_Contract.Service.News
{
    public interface IDataNewsService : IServiceBase<data_news>
    {
        Task<ResponseBase<object>> GetNewsListAsync(NewsRequest request);
        Task<ResponseBase<object>> CreateNewsAsync(NewsRequest request, string userId);
        Task<ResponseBase<object>> UpdateNewsAsync(int newsId, NewsRequest request, string userId);
        Task<ResponseBase<object>> DeleteNewsAsync(int newsId, string userId);
        
        // Reaction
        Task<ResponseBase<object>> ToggleReactionAsync(int newsId, string userCode, int reactionType);
        Task<ResponseBase<object>> RemoveReactionAsync(int newsId, string userCode);
        
        // Comment
        Task<ResponseBase<object>> AddCommentAsync(CommentRequest request, string userId);
        Task<ResponseBase<object>> GetCommentsAsync(int newsId);
        
        // Share
        Task<ResponseBase<object>> ShareNewsAsync(int newsId, string userCode, string? shareContent);
    }
}

