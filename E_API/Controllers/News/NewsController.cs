using E_API.Filter;
using E_Contract.Service;
using E_Model.Request.News;
using E_Model.Response.News;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static E_Model.Response.News.NewsResponse;

namespace E_API.Controllers.News
{
    [Route("api/[controller]")]
    [ApiController]
    public class NewsController : BaseController
    {
        private readonly IServiceWrapper _serviceWrapper;

        public NewsController(IServiceWrapper serviceWrapper)
        {
            _serviceWrapper = serviceWrapper;
        }

        #region Public Endpoints

        [LoginAuthorize]
        [HttpGet("Select-news")]
        public async Task<IActionResult> GetNewsList([FromQuery] NewsRequest request)
        {
            try
            {
                var fakeData = GenerateFakeNewsData();
                await Task.Delay(100); // Simulate async operation
                return OK(fakeData);
            }
            catch (Exception ex)
            {
                return InternalServerError($"Internal server error: {ex.Message}", ex);
            }
        }

        [LoginAuthorize]
        [HttpPost("Create")]
        public async Task<IActionResult> CreateNews([FromForm] NewsRequest request)
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _serviceWrapper.News.CreateNewsAsync(request, userId);
                return HandleServiceResult(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [LoginAuthorize]
        [HttpPut("Update/{newsId}")]
        public async Task<IActionResult> UpdateNews(int newsId, [FromForm] NewsRequest request)
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _serviceWrapper.News.UpdateNewsAsync(newsId, request, userId);
                return HandleServiceResult(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [LoginAuthorize]
        [HttpDelete("Delete/{newsId}")]
        public async Task<IActionResult> DeleteNews(int newsId)
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _serviceWrapper.News.DeleteNewsAsync(newsId, userId);
                return HandleServiceResult(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [LoginAuthorize]
        [HttpPost("Reaction")]
        public async Task<IActionResult> ToggleReaction([FromBody] ReactionRequest request)
        {
            try
            {
                var userCode = GetCurrentUserCode() ?? request.user_code;
                var result = await _serviceWrapper.News.ToggleReactionAsync(
                    request.news_id,
                    userCode,
                    request.reaction_type
                );
                return HandleServiceResult(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [LoginAuthorize]
        [HttpDelete("Reaction/{newsId}")]
        public async Task<IActionResult> RemoveReaction(int newsId)
        {
            try
            {
                var userCode = GetCurrentUserCode();
                var result = await _serviceWrapper.News.RemoveReactionAsync(newsId, userCode);
                return HandleServiceResult(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [LoginAuthorize]
        [HttpPost("Comment")]
        public async Task<IActionResult> AddComment([FromForm] CommentRequest request)
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _serviceWrapper.News.AddCommentAsync(request, userId);
                return HandleServiceResult(result, returnData: true);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [LoginAuthorize]
        [HttpGet("Comments/{newsId}")]
        public async Task<IActionResult> GetComments(int newsId)
        {
            try
            {
                var result = await _serviceWrapper.News.GetCommentsAsync(newsId);
                return HandleServiceResult(result, returnData: true);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [LoginAuthorize]
        [HttpPost("Share/{newsId}")]
        public async Task<IActionResult> ShareNews(int newsId, [FromBody] string? shareContent = null)
        {
            try
            {
                var userCode = GetCurrentUserCode();
                var result = await _serviceWrapper.News.ShareNewsAsync(newsId, userCode, shareContent);
                return HandleServiceResult(result, returnData: true);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        #endregion

        #region Private Helpers

        /// <summary>
        /// Lấy User ID hiện tại từ Claims
        /// </summary>
        private string GetCurrentUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? User.FindFirst("user_code")?.Value
                ?? "SYSTEM";
        }

        /// <summary>
        /// Lấy User Code hiện tại từ Claims
        /// </summary>
        private string GetCurrentUserCode()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? User.FindFirst("user_code")?.Value
                ?? "SYSTEM";
        }

        /// <summary>
        /// Xử lý kết quả trả về từ Service Layer
        /// </summary>
        private IActionResult HandleServiceResult(dynamic result, bool returnData = false)
        {
            if (result.is_success)
            {
                return returnData ? OK(result.data) : OK(result.message);
            }
            return BadRequest(result.message);
        }

        /// <summary>
        /// Tạo dữ liệu News giả cho demo
        /// </summary>
        private List<NewsResponse> GenerateFakeNewsData()
        {
            return new List<NewsResponse>
            {
                CreateFakeNews(
                    id: 1,
                    userCode: "NV001",
                    title: "Tin tức 1",
                    content: "Lưu giữ những bức ảnh đẹp của tôi",
                    status: "vui vẻ",
                    taggedUsers: new List<string> { "NV001", "NV002" },
                    images: new List<string>
                    {
                        "/assetsCMS/media/stock/600x600/img-55.jpg",
                        "/assetsCMS/media/stock/600x600/img-60.jpg",
                        "/assetsCMS/media/stock/600x600/img-56.jpg",
                        "/assetsCMS/media/stock/600x600/img-63.jpg",
                        "/assetsCMS/media/stock/600x600/img-58.jpg"
                    },
                    reactions: new List<Reaction>
                    {
                        new Reaction { reaction_type = 1, count = 101 },
                        new Reaction { reaction_type = 2, count = 20 },
                        new Reaction { reaction_type = 3, count = 11 }
                    },
                    comments: new List<Comment>
                    {
                        CreateFakeComment(1, 0, "NV001", "ảnh thật đẹp", "2024-10-01 10:00", new List<string> { "img-11.jpg" }),
                        CreateFakeComment(2, 0, "NV002", "Phải làm sao... Phải chịu", "2024-10-01 10:00", new List<string> { "img-12.jpg" })
                    }
                ),
                CreateFakeNews(
                    id: 2,
                    userCode: "NV002",
                    title: "Tin tức 2",
                    content: "Nhiều hoạt động thú vị tại phố đi bộ Nguyễn Huệ dịp tuần lễ 'Khỏe để xây dựng và bảo vệ Tổ quốc'",
                    status: "thú vị",
                    taggedUsers: new List<string>(),
                    images: new List<string>(),
                    reactions: new List<Reaction>
                    {
                        new Reaction { reaction_type = 1, count = 101 },
                        new Reaction { reaction_type = 2, count = 20 },
                        new Reaction { reaction_type = 3, count = 11 }
                    },
                    comments: new List<Comment>
                    {
                        CreateFakeComment(11, 0, "NV001", "Cuối tuần này nhé bro.", "2024-10-01 10:00", new List<string>())
                    },
                    sharedInfo: new List<SharedInfo>
                    {
                        new SharedInfo { user_code = "NV006", enter_time = DateTime.Now },
                        new SharedInfo { user_code = "NV008", enter_time =  DateTime.Now}
                    }
                )
            };
        }

        /// <summary>
        /// Tạo một bản tin News giả
        /// </summary>
        private NewsResponse CreateFakeNews(
            int id,
            string userCode,
            string title,
            string content,
            string status,
            List<string> taggedUsers,
            List<string>? images,
            List<Reaction> reactions,
            List<Comment> comments,
            List<SharedInfo>? sharedInfo = null)
        {
            return new NewsResponse
            {
                news_id = id,
                user_code = userCode,
                title = title,
                contents = content,
                updated_at = DateTime.Now,
                status = status,
                tagged_users = taggedUsers,
                images = images,
                reactions = reactions,
                comments = comments,
                shares = sharedInfo ?? new List<SharedInfo>()
            };
        }

        /// <summary>
        /// Tạo một Comment giả
        /// </summary>
        private Comment CreateFakeComment(
            int commentId,
            int parentId,
            string userCode,
            string content,
            string enterTime,
            List<string>? images)
        {
            return new Comment
            {
                comment_id = commentId,
                parent_id = parentId,
                user_code = userCode,
                contents = content,
                enter_time = DateTime.Now,
                images = images,
                reactions = new List<Reaction>
                {
                    new Reaction { reaction_type = 1, count = 1 },
                    new Reaction { reaction_type = 2,   count = 1 },
                    new Reaction { reaction_type = 3, count = 1 }
                }
            };
        }

        #endregion
    }
}

