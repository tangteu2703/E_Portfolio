using E_Contract.Repository;
using E_Contract.Service.News;
using E_Model.News;
using E_Model.Request.News;
using E_Model.Response;
using E_Model.Response.News;
using Microsoft.AspNetCore.Http;

namespace E_Service.News
{
    public class DataNewsService : ServiceBase<data_news>, IDataNewsService
    {
        private readonly IRepositoryWrapper _repositoryWrapper;

        public DataNewsService(IRepositoryWrapper repositoryWrapper) : base(repositoryWrapper)
        {
            _repositoryWrapper = repositoryWrapper;
        }

        public async Task<ResponseBase<object>> GetNewsListAsync(NewsRequest request)
        {
            try
            {
                var newsList = await _repositoryWrapper.News.SelectNewsFilterAsync(request);
                
                return new ResponseBase<object>(true, newsList, "Lấy danh sách bài viết thành công");
            }
            catch (Exception ex)
            {
                return new ResponseBase<object>(false, $"Lỗi: {ex.Message}");
            }
        }

        public async Task<ResponseBase<object>> CreateNewsAsync(NewsRequest request, string userId)
        {
            try
            {
                // Create news object
                var news = new data_news
                {
                    user_code = request.user_code ?? userId,
                    title = request.title,
                    contents = request.contents,
                    status = request.status,
                    location = request.location,
                    privacy_level = request.privacy_level
                };
                news.SetInsertInfo(userId);

                // Handle image uploads
                List<string>? imageUrls = null;
                if (request.images != null && request.images.Any())
                {
                    imageUrls = new List<string>();
                    foreach (var image in request.images)
                    {
                        var imagePath = await SaveImageAsync(image, "News");
                        imageUrls.Add(imagePath);
                    }
                }

                // Insert news with images and tags
                int newsId = await _repositoryWrapper.News.InsertNewsAsync(news, imageUrls, request.tagged_users);

                return new ResponseBase<object>(true, new { news_id = newsId }, "Tạo bài viết thành công");
            }
            catch (Exception ex)
            {
                return new ResponseBase<object>(false, $"Lỗi khi tạo bài viết: {ex.Message}");
            }
        }

        public async Task<ResponseBase<object>> UpdateNewsAsync(int newsId, NewsRequest request, string userId)
        {
            try
            {
                var existingNews = await _repositoryWrapper.News.SelectByIdAsync(newsId);
                if (existingNews == null)
                {
                    return new ResponseBase<object>(false, "Bài viết không tồn tại");
                }

                // Update news properties
                existingNews.title = request.title ?? existingNews.title;
                existingNews.contents = request.contents ?? existingNews.contents;
                existingNews.status = request.status ?? existingNews.status;
                existingNews.location = request.location ?? existingNews.location;
                existingNews.privacy_level = request.privacy_level;
                existingNews.SetUpdateInfo(userId);

                await _repositoryWrapper.News.UpdateNewsAsync(existingNews);

                return new ResponseBase<object>(true, "Cập nhật bài viết thành công");
            }
            catch (Exception ex)
            {
                return new ResponseBase<object>(false, $"Lỗi khi cập nhật bài viết: {ex.Message}");
            }
        }

        public async Task<ResponseBase<object>> DeleteNewsAsync(int newsId, string userId)
        {
            try
            {
                await _repositoryWrapper.News.DeleteNewsAsync(newsId, userId);

                return new ResponseBase<object>(true, "Xóa bài viết thành công");
            }
            catch (Exception ex)
            {
                return new ResponseBase<object>(false, $"Lỗi khi xóa bài viết: {ex.Message}");
            }
        }

        public async Task<ResponseBase<object>> ToggleReactionAsync(int newsId, string userCode, int reactionType)
        {
            try
            {
                await _repositoryWrapper.News.UpsertReactionAsync(newsId, userCode, reactionType);

                return new ResponseBase<object>(true, "Cập nhật cảm xúc thành công");
            }
            catch (Exception ex)
            {
                return new ResponseBase<object>(false, $"Lỗi khi cập nhật cảm xúc: {ex.Message}");
            }
        }

        public async Task<ResponseBase<object>> RemoveReactionAsync(int newsId, string userCode)
        {
            try
            {
                await _repositoryWrapper.News.DeleteReactionAsync(newsId, userCode);

                return new ResponseBase<object>(true, "Xóa cảm xúc thành công");
            }
            catch (Exception ex)
            {
                return new ResponseBase<object>(false, $"Lỗi khi xóa cảm xúc: {ex.Message}");
            }
        }

        public async Task<ResponseBase<object>> AddCommentAsync(CommentRequest request, string userId)
        {
            try
            {
                var comment = new data_news_comment
                {
                    news_id = request.news_id,
                    parent_id = request.parent_id,
                    user_code = request.user_code ?? userId,
                    content = request.content
                };
                comment.SetInsertInfo(userId);

                // Handle image uploads for comment
                List<string>? imageUrls = null;
                if (request.images != null && request.images.Any())
                {
                    imageUrls = new List<string>();
                    foreach (var image in request.images)
                    {
                        var imagePath = await SaveImageAsync(image, "Comments");
                        imageUrls.Add(imagePath);
                    }
                }

                int commentId = await _repositoryWrapper.News.InsertCommentAsync(comment, imageUrls);

                return new ResponseBase<object>(true, new { comment_id = commentId }, "Thêm bình luận thành công");
            }
            catch (Exception ex)
            {
                return new ResponseBase<object>(false, $"Lỗi khi thêm bình luận: {ex.Message}");
            }
        }

        public async Task<ResponseBase<object>> GetCommentsAsync(int newsId)
        {
            try
            {
                var comments = await _repositoryWrapper.News.SelectCommentsByNewsAsync(newsId);

                return new ResponseBase<object>(true, comments, "Lấy danh sách bình luận thành công");
            }
            catch (Exception ex)
            {
                return new ResponseBase<object>(false, $"Lỗi: {ex.Message}");
            }
        }

        public async Task<ResponseBase<object>> ShareNewsAsync(int newsId, string userCode, string? shareContent)
        {
            try
            {
                var share = new data_news_share
                {
                    news_id = newsId,
                    user_code = userCode,
                    share_content = shareContent
                };

                int shareId = await _repositoryWrapper.News.InsertShareAsync(share);

                return new ResponseBase<object>(true, new { share_id = shareId }, "Chia sẻ bài viết thành công");
            }
            catch (Exception ex)
            {
                return new ResponseBase<object>(false, $"Lỗi khi chia sẻ: {ex.Message}");
            }
        }

        private async Task<string> SaveImageAsync(IFormFile file, string folder)
        {
            try
            {
                // Create directory if not exists
                var uploadPath = Path.Combine("wwwroot", folder);
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                // Generate unique filename
                var fileName = $"{Guid.NewGuid()}_{file.FileName}";
                var filePath = Path.Combine(uploadPath, fileName);

                // Save file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Return relative path
                return $"/{folder}/{fileName}";
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lưu file: {ex.Message}");
            }
        }
    }
}

