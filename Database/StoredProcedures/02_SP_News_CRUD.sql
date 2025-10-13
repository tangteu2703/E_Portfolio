-- =============================================
-- Author:      E_Portfolio Team
-- Create date: 2024-10-10
-- Description: Stored Procedures for News CRUD operations
-- =============================================

-- =============================================
-- SP: data_news_insert
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[data_news_insert]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[data_news_insert]
GO

CREATE PROCEDURE [dbo].[data_news_insert]
    @user_code NVARCHAR(50),
    @title NVARCHAR(500) = NULL,
    @contents NVARCHAR(MAX) = NULL,
    @status NVARCHAR(100) = NULL,
    @location NVARCHAR(500) = NULL,
    @privacy_level INT = 1,
    @created_by NVARCHAR(50),
    @id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    INSERT INTO [dbo].[data_news] (
        [user_code], [title], [contents], [status], [location], 
        [privacy_level], [created_at], [created_by], [is_deleted]
    )
    VALUES (
        @user_code, @title, @contents, @status, @location,
        @privacy_level, GETDATE(), @created_by, 0
    )
    
    SET @id = SCOPE_IDENTITY()
END
GO

-- =============================================
-- SP: data_news_update
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[data_news_update]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[data_news_update]
GO

CREATE PROCEDURE [dbo].[data_news_update]
    @id INT,
    @title NVARCHAR(500) = NULL,
    @contents NVARCHAR(MAX) = NULL,
    @status NVARCHAR(100) = NULL,
    @location NVARCHAR(500) = NULL,
    @privacy_level INT = NULL,
    @updated_by NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE [dbo].[data_news]
    SET 
        [title] = ISNULL(@title, [title]),
        [contents] = ISNULL(@contents, [contents]),
        [status] = ISNULL(@status, [status]),
        [location] = ISNULL(@location, [location]),
        [privacy_level] = ISNULL(@privacy_level, [privacy_level]),
        [updated_at] = GETDATE(),
        [updated_by] = @updated_by
    WHERE [id] = @id AND [is_deleted] = 0
END
GO

-- =============================================
-- SP: data_news_delete (Soft delete)
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[data_news_delete]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[data_news_delete]
GO

CREATE PROCEDURE [dbo].[data_news_delete]
    @id INT,
    @user_id NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE [dbo].[data_news]
    SET 
        [is_deleted] = 1,
        [updated_at] = GETDATE(),
        [updated_by] = @user_id
    WHERE [id] = @id
END
GO

-- =============================================
-- SP: data_news_select_by_id
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[data_news_select_by_id]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[data_news_select_by_id]
GO

CREATE PROCEDURE [dbo].[data_news_select_by_id]
    @id INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT * FROM [dbo].[data_news]
    WHERE [id] = @id AND [is_deleted] = 0
END
GO

-- =============================================
-- SP: data_news_select_filter (With pagination and full details)
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[data_news_select_filter]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[data_news_select_filter]
GO

CREATE PROCEDURE [dbo].[data_news_select_filter]
    @user_code NVARCHAR(50) = NULL,
    @page_index INT = 1,
    @page_size INT = 10
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @offset INT = (@page_index - 1) * @page_size;
    
    SELECT 
        n.*,
        -- Aggregated images
        (
            SELECT [id], [image_url], [image_order]
            FROM [dbo].[data_news_image]
            WHERE [news_id] = n.[id] AND [is_deleted] = 0
            ORDER BY [image_order]
            FOR JSON PATH
        ) AS images_json,
        -- Aggregated tags
        (
            SELECT [user_code]
            FROM [dbo].[data_news_tag]
            WHERE [news_id] = n.[id]
            FOR JSON PATH
        ) AS tags_json,
        -- Reaction counts
        (
            SELECT 
                [reaction_type],
                COUNT(*) AS count
            FROM [dbo].[data_news_reaction]
            WHERE [news_id] = n.[id]
            GROUP BY [reaction_type]
            FOR JSON PATH
        ) AS reactions_json,
        -- Comment count
        (SELECT COUNT(*) FROM [dbo].[data_news_comment] WHERE [news_id] = n.[id] AND [is_deleted] = 0) AS comment_count,
        -- Share count
        (SELECT COUNT(*) FROM [dbo].[data_news_share] WHERE [news_id] = n.[id]) AS share_count
    FROM [dbo].[data_news] n
    WHERE 
        n.[is_deleted] = 0
        AND (@user_code IS NULL OR n.[user_code] = @user_code)
    ORDER BY n.[is_pinned] DESC, n.[created_at] DESC
    OFFSET @offset ROWS
    FETCH NEXT @page_size ROWS ONLY
END
GO

-- =============================================
-- SP: data_news_image_insert
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[data_news_image_insert]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[data_news_image_insert]
GO

CREATE PROCEDURE [dbo].[data_news_image_insert]
    @news_id INT,
    @image_url NVARCHAR(1000),
    @image_order INT = 0,
    @created_by NVARCHAR(50),
    @id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    INSERT INTO [dbo].[data_news_image] (
        [news_id], [image_url], [image_order], [created_at], [created_by]
    )
    VALUES (
        @news_id, @image_url, @image_order, GETDATE(), @created_by
    )
    
    SET @id = SCOPE_IDENTITY()
END
GO

-- =============================================
-- SP: data_news_tag_insert
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[data_news_tag_insert]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[data_news_tag_insert]
GO

CREATE PROCEDURE [dbo].[data_news_tag_insert]
    @news_id INT,
    @user_code NVARCHAR(50),
    @created_by NVARCHAR(50),
    @id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Check if tag already exists
    IF NOT EXISTS (SELECT 1 FROM [dbo].[data_news_tag] WHERE [news_id] = @news_id AND [user_code] = @user_code)
    BEGIN
        INSERT INTO [dbo].[data_news_tag] (
            [news_id], [user_code], [created_at], [created_by]
        )
        VALUES (
            @news_id, @user_code, GETDATE(), @created_by
        )
        
        SET @id = SCOPE_IDENTITY()
    END
END
GO

-- =============================================
-- SP: data_news_reaction_upsert (Insert or Update reaction)
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[data_news_reaction_upsert]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[data_news_reaction_upsert]
GO

CREATE PROCEDURE [dbo].[data_news_reaction_upsert]
    @news_id INT,
    @user_code NVARCHAR(50),
    @reaction_type INT
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Check if reaction exists
    IF EXISTS (SELECT 1 FROM [dbo].[data_news_reaction] WHERE [news_id] = @news_id AND [user_code] = @user_code)
    BEGIN
        -- Update existing reaction
        UPDATE [dbo].[data_news_reaction]
        SET [reaction_type] = @reaction_type, [updated_at] = GETDATE()
        WHERE [news_id] = @news_id AND [user_code] = @user_code
    END
    ELSE
    BEGIN
        -- Insert new reaction
        INSERT INTO [dbo].[data_news_reaction] ([news_id], [user_code], [reaction_type], [created_at])
        VALUES (@news_id, @user_code, @reaction_type, GETDATE())
    END
END
GO

-- =============================================
-- SP: data_news_reaction_delete
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[data_news_reaction_delete]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[data_news_reaction_delete]
GO

CREATE PROCEDURE [dbo].[data_news_reaction_delete]
    @news_id INT,
    @user_code NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    
    DELETE FROM [dbo].[data_news_reaction]
    WHERE [news_id] = @news_id AND [user_code] = @user_code
END
GO

-- =============================================
-- SP: data_news_comment_insert
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[data_news_comment_insert]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[data_news_comment_insert]
GO

CREATE PROCEDURE [dbo].[data_news_comment_insert]
    @news_id INT,
    @parent_id INT = NULL,
    @user_code NVARCHAR(50),
    @content NVARCHAR(MAX),
    @created_by NVARCHAR(50),
    @id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    INSERT INTO [dbo].[data_news_comment] (
        [news_id], [parent_id], [user_code], [content], 
        [created_at], [created_by], [is_deleted]
    )
    VALUES (
        @news_id, @parent_id, @user_code, @content,
        GETDATE(), @created_by, 0
    )
    
    SET @id = SCOPE_IDENTITY()
END
GO

-- =============================================
-- SP: data_news_comment_select_by_news
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[data_news_comment_select_by_news]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[data_news_comment_select_by_news]
GO

CREATE PROCEDURE [dbo].[data_news_comment_select_by_news]
    @news_id INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        c.*,
        -- Aggregated comment images
        (
            SELECT [image_url]
            FROM [dbo].[data_news_comment_image]
            WHERE [comment_id] = c.[id]
            FOR JSON PATH
        ) AS images_json,
        -- Reaction counts for comment
        (
            SELECT 
                [reaction_type],
                COUNT(*) AS count
            FROM [dbo].[data_news_comment_reaction]
            WHERE [comment_id] = c.[id]
            GROUP BY [reaction_type]
            FOR JSON PATH
        ) AS reactions_json
    FROM [dbo].[data_news_comment] c
    WHERE 
        c.[news_id] = @news_id 
        AND c.[is_deleted] = 0
    ORDER BY c.[created_at] ASC
END
GO

-- =============================================
-- SP: data_news_share_insert
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[data_news_share_insert]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[data_news_share_insert]
GO

CREATE PROCEDURE [dbo].[data_news_share_insert]
    @news_id INT,
    @user_code NVARCHAR(50),
    @share_content NVARCHAR(MAX) = NULL,
    @id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    INSERT INTO [dbo].[data_news_share] (
        [news_id], [user_code], [share_content], [created_at]
    )
    VALUES (
        @news_id, @user_code, @share_content, GETDATE()
    )
    
    SET @id = SCOPE_IDENTITY()
END
GO

PRINT 'News CRUD Stored Procedures created successfully!'

