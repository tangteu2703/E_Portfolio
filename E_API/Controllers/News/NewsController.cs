using E_API.Filter;
using E_Contract.Service;
using E_Model.Request.News;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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

        /// <summary>
        /// Lấy danh sách bài viết với phân trang
        /// </summary>
        [Authorize]
        [HttpGet("GetNewsList")]
        public async Task<IActionResult> GetNewsList([FromQuery] NewsRequest request)
        {
            try
            {
                var result = await _serviceWrapper.News.GetNewsListAsync(request);
                
                if (result.is_success)
                    return OK(result.data);
                
                return BadRequest(result.message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Tạo bài viết mới
        /// </summary>
        [Authorize]
        [HttpPost("Create")]
        public async Task<IActionResult> CreateNews([FromForm] NewsRequest request)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                    ?? User.FindFirst("user_code")?.Value 
                    ?? "SYSTEM";
                
                var result = await _serviceWrapper.News.CreateNewsAsync(request, userId);
                
                if (result.is_success)
                    return OK(result.data);
                
                return BadRequest(result.message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Cập nhật bài viết
        /// </summary>
        [Authorize]
        [HttpPut("Update/{newsId}")]
        public async Task<IActionResult> UpdateNews(int newsId, [FromForm] NewsRequest request)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                    ?? User.FindFirst("user_code")?.Value 
                    ?? "SYSTEM";
                
                var result = await _serviceWrapper.News.UpdateNewsAsync(newsId, request, userId);
                
                if (result.is_success)
                    return OK(result.message);
                
                return BadRequest(result.message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Xóa bài viết
        /// </summary>
        [Authorize]
        [HttpDelete("Delete/{newsId}")]
        public async Task<IActionResult> DeleteNews(int newsId)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                    ?? User.FindFirst("user_code")?.Value 
                    ?? "SYSTEM";
                
                var result = await _serviceWrapper.News.DeleteNewsAsync(newsId, userId);
                
                if (result.is_success)
                    return OK(result.message);
                
                return BadRequest(result.message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Thêm/Cập nhật cảm xúc (reaction) cho bài viết
        /// </summary>
        [Authorize]
        [HttpPost("Reaction")]
        public async Task<IActionResult> ToggleReaction([FromBody] ReactionRequest request)
        {
            try
            {
                var userCode = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                    ?? User.FindFirst("user_code")?.Value 
                    ?? request.user_code;
                
                var result = await _serviceWrapper.News.ToggleReactionAsync(
                    request.news_id, 
                    userCode, 
                    request.reaction_type
                );
                
                if (result.is_success)
                    return OK(result.message);
                
                return BadRequest(result.message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Xóa cảm xúc khỏi bài viết
        /// </summary>
        [Authorize]
        [HttpDelete("Reaction/{newsId}")]
        public async Task<IActionResult> RemoveReaction(int newsId)
        {
            try
            {
                var userCode = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                    ?? User.FindFirst("user_code")?.Value 
                    ?? "SYSTEM";
                
                var result = await _serviceWrapper.News.RemoveReactionAsync(newsId, userCode);
                
                if (result.is_success)
                    return OK(result.message);
                
                return BadRequest(result.message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Thêm bình luận vào bài viết
        /// </summary>
        [Authorize]
        [HttpPost("Comment")]
        public async Task<IActionResult> AddComment([FromForm] CommentRequest request)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                    ?? User.FindFirst("user_code")?.Value 
                    ?? "SYSTEM";
                
                var result = await _serviceWrapper.News.AddCommentAsync(request, userId);
                
                if (result.is_success)
                    return OK(result.data);
                
                return BadRequest(result.message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Lấy danh sách bình luận của bài viết
        /// </summary>
        [Authorize]
        [HttpGet("Comments/{newsId}")]
        public async Task<IActionResult> GetComments(int newsId)
        {
            try
            {
                var result = await _serviceWrapper.News.GetCommentsAsync(newsId);
                
                if (result.is_success)
                    return OK(result.data);
                
                return BadRequest(result.message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Chia sẻ bài viết
        /// </summary>
        [Authorize]
        [HttpPost("Share/{newsId}")]
        public async Task<IActionResult> ShareNews(int newsId, [FromBody] string? shareContent = null)
        {
            try
            {
                var userCode = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                    ?? User.FindFirst("user_code")?.Value 
                    ?? "SYSTEM";
                
                var result = await _serviceWrapper.News.ShareNewsAsync(newsId, userCode, shareContent);
                
                if (result.is_success)
                    return OK(result.data);
                
                return BadRequest(result.message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}

