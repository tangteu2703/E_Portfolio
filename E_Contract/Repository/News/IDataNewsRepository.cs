using E_Model.News;
using E_Model.Request.News;
using E_Model.Response.News;

namespace E_Contract.Repository.News
{
    public interface IDataNewsRepository : IRepositoryBase<data_news>
    {
        Task<IEnumerable<NewsResponse>> SelectNewsFilterAsync(NewsRequest request);
        Task<int> InsertNewsAsync(data_news news, List<string>? imageUrls, List<string>? taggedUsers);
        Task<bool> UpdateNewsAsync(data_news news);
        Task<bool> DeleteNewsAsync(int newsId, string userId);
        
        // Reaction operations
        Task<bool> UpsertReactionAsync(int newsId, string userCode, int reactionType);
        Task<bool> DeleteReactionAsync(int newsId, string userCode);
        
        // Comment operations
        Task<int> InsertCommentAsync(data_news_comment comment, List<string>? imageUrls);
        Task<IEnumerable<CommentResponse>> SelectCommentsByNewsAsync(int newsId);
        
        // Image operations
        Task<int> InsertImageAsync(data_news_image image);
        
        // Tag operations
        Task<int> InsertTagAsync(data_news_tag tag);
        
        // Share operations
        Task<int> InsertShareAsync(data_news_share share);
    }
}

