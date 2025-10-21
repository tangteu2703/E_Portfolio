using Dapper;
using E_Common;
using E_Contract.Repository.News;
using E_Model.News;
using E_Model.Request.News;
using E_Model.Response.News;
using Newtonsoft.Json;

namespace E_Repository.News
{
    public class DataNewsRepository : RepositoryBase<data_news>, IDataNewsRepository
    {
        public async Task<IEnumerable<NewsResponse>> SelectNewsFilterAsync(NewsRequest request)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@user_code", request.user_code);
                param.Add("@page_index", request.PageNumber);
                param.Add("@page_size", request.PageSize);

                var result = await Connection.SelectAsync<dynamic>("data_news_select_filter", param);
                
                // Convert dynamic to NewsResponse and parse JSON fields
                var newsResponses = result.Select(item => new NewsResponse
                {
                    news_id = item.id,
                    user_code = item.user_code,
                    title = item.title,
                    contents = item.contents,
                    status = item.status,
                    location = item.location,
                    is_pinned = item.is_pinned,
                    created_at = item.created_at,
                    updated_at = item.updated_at,
                    
                    // Parse JSON fields
                    images = !string.IsNullOrEmpty(item.images_json) 
                        ? JsonConvert.DeserializeObject<List<NewsResponse.ImageItem>>(item.images_json) 
                        : new List<NewsResponse.ImageItem>(),
                    
                    tagged_users = !string.IsNullOrEmpty(item.tags_json)
                        ? JsonConvert.DeserializeObject<List<dynamic>>(item.tags_json)?.Select((Func<dynamic, string>)(t => (string)t.user_code)).ToList()
                        : new List<string>(),
                    
                    reactions = !string.IsNullOrEmpty(item.reactions_json)
                        ? JsonConvert.DeserializeObject<List<NewsResponse.Reaction>>(item.reactions_json)
                        : new List<NewsResponse.Reaction>(),
                    
                    comments = item.comment_count,
                    shares = item.share_count
                });

                return newsResponses;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<int> InsertNewsAsync(data_news news, List<string>? imageUrls, List<string>? taggedUsers)
        {
            try
            {
                // Insert news
                var param = DynamicParameterHelper.ConvertWithReturnParam(news, "id");
                int newsId = await Connection.ExcuteScalarAsync("data_news_insert", param, "id");

                // Insert images
                if (imageUrls != null && imageUrls.Any())
                {
                    int order = 0;
                    foreach (var imageUrl in imageUrls)
                    {
                        var imageParam = new DynamicParameters();
                        imageParam.Add("@news_id", newsId);
                        imageParam.Add("@image_url", imageUrl);
                        imageParam.Add("@image_order", order++);
                        imageParam.Add("@created_by", news.created_by);
                        imageParam.Add("@id", dbType: System.Data.DbType.Int32, direction: System.Data.ParameterDirection.Output);
                        
                        await Connection.ExecuteAsync("data_news_image_insert", imageParam);
                    }
                }

                // Insert tags
                if (taggedUsers != null && taggedUsers.Any())
                {
                    foreach (var userCode in taggedUsers)
                    {
                        var tagParam = new DynamicParameters();
                        tagParam.Add("@news_id", newsId);
                        tagParam.Add("@user_code", userCode);
                        tagParam.Add("@created_by", news.created_by);
                        tagParam.Add("@id", dbType: System.Data.DbType.Int32, direction: System.Data.ParameterDirection.Output);
                        
                        await Connection.ExecuteAsync("data_news_tag_insert", tagParam);
                    }
                }

                return newsId;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> UpdateNewsAsync(data_news news)
        {
            try
            {
                var param = DynamicParameterHelper.ConvertWithOutCreatitonParams(news);
                await Connection.ExecuteAsync("data_news_update", param);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> DeleteNewsAsync(int newsId, string userId)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@id", newsId);
                param.Add("@user_id", userId);
                
                await Connection.ExecuteAsync("data_news_delete", param);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> UpsertReactionAsync(int newsId, string userCode, int reactionType)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@news_id", newsId);
                param.Add("@user_code", userCode);
                param.Add("@reaction_type", reactionType);
                
                await Connection.ExecuteAsync("data_news_reaction_upsert", param);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> DeleteReactionAsync(int newsId, string userCode)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@news_id", newsId);
                param.Add("@user_code", userCode);
                
                await Connection.ExecuteAsync("data_news_reaction_delete", param);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<int> InsertCommentAsync(data_news_comment comment, List<string>? imageUrls)
        {
            try
            {
                // Insert comment
                var param = DynamicParameterHelper.ConvertWithReturnParam(comment, "id");
                int commentId = await Connection.ExcuteScalarAsync("data_news_comment_insert", param, "id");

                // Insert comment images if any
                if (imageUrls != null && imageUrls.Any())
                {
                    foreach (var imageUrl in imageUrls)
                    {
                        var imageParam = new DynamicParameters();
                        imageParam.Add("@comment_id", commentId);
                        imageParam.Add("@image_url", imageUrl);
                        imageParam.Add("@created_by", comment.created_by);
                        
                        await Connection.ExecuteAsync("data_news_comment_image_insert", imageParam);
                    }
                }

                return commentId;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<CommentResponse>> SelectCommentsByNewsAsync(int newsId)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@news_id", newsId);
                
                var result = await Connection.SelectAsync<dynamic>("data_news_comment_select_by_news", param);
                
                // Convert dynamic to CommentResponse and parse JSON fields
                var commentResponses = result.Select(item => new CommentResponse
                {
                    id = item.id,
                    news_id = item.news_id,
                    parent_id = item.parent_id,
                    user_code = item.user_code,
                    content = item.content,
                    created_at = item.created_at,
                    
                    images = !string.IsNullOrEmpty(item.images_json)
                        ? JsonConvert.DeserializeObject<List<dynamic>>(item.images_json)?.Select((Func<dynamic, string>)(img => (string)img.image_url)).ToList()
                        : new List<string>(),
                    
                    reactions = !string.IsNullOrEmpty(item.reactions_json)
                        ? JsonConvert.DeserializeObject<List<CommentResponse.ReactionCount>>(item.reactions_json)
                        : new List<CommentResponse.ReactionCount>(),
                    
                    reply_count = 0 // Will be calculated in service layer
                });

                return commentResponses;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<int> InsertImageAsync(data_news_image image)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@news_id", image.news_id);
                param.Add("@image_url", image.image_url);
                param.Add("@image_order", image.image_order);
                param.Add("@created_by", image.created_by);
                param.Add("@id", dbType: System.Data.DbType.Int32, direction: System.Data.ParameterDirection.Output);
                
                await Connection.ExecuteAsync("data_news_image_insert", param);
                return param.Get<int>("@id");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<int> InsertTagAsync(data_news_tag tag)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@news_id", tag.news_id);
                param.Add("@user_code", tag.user_code);
                param.Add("@created_by", tag.created_by);
                param.Add("@id", dbType: System.Data.DbType.Int32, direction: System.Data.ParameterDirection.Output);
                
                await Connection.ExecuteAsync("data_news_tag_insert", param);
                return param.Get<int>("@id");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<int> InsertShareAsync(data_news_share share)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@news_id", share.news_id);
                param.Add("@user_code", share.user_code);
                param.Add("@share_content", share.share_content);
                param.Add("@id", dbType: System.Data.DbType.Int32, direction: System.Data.ParameterDirection.Output);
                
                await Connection.ExecuteAsync("data_news_share_insert", param);
                return param.Get<int>("@id");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}

