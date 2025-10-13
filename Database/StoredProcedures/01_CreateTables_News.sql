-- =============================================
-- Author:      E_Portfolio Team
-- Create date: 2024-10-10
-- Description: Create tables for News/Post system
-- =============================================

-- Table: data_news (Bài viết)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[data_news]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[data_news](
        [id] [int] IDENTITY(1,1) NOT NULL,
        [user_code] [nvarchar](50) NOT NULL,
        [title] [nvarchar](500) NULL,
        [contents] [nvarchar](max) NULL,
        [status] [nvarchar](100) NULL, -- Cảm xúc: vui vẻ, hạnh phúc, buồn, etc.
        [location] [nvarchar](500) NULL, -- Check-in location
        [privacy_level] [int] NOT NULL DEFAULT(1), -- 1: Public, 2: Friends, 3: Private
        [is_pinned] [bit] NOT NULL DEFAULT(0), -- Ghim bài viết
        [created_at] [datetime] NULL DEFAULT(GETDATE()),
        [created_by] [nvarchar](50) NULL,
        [updated_at] [datetime] NULL,
        [updated_by] [nvarchar](50) NULL,
        [is_deleted] [bit] NOT NULL DEFAULT(0),
        [note] [nvarchar](500) NULL,
        CONSTRAINT [PK_data_news] PRIMARY KEY CLUSTERED ([id] ASC)
    )
END
GO

-- Table: data_news_image (Hình ảnh trong bài viết)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[data_news_image]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[data_news_image](
        [id] [int] IDENTITY(1,1) NOT NULL,
        [news_id] [int] NOT NULL,
        [image_url] [nvarchar](1000) NOT NULL,
        [image_order] [int] NOT NULL DEFAULT(0), -- Thứ tự hiển thị
        [created_at] [datetime] NULL DEFAULT(GETDATE()),
        [created_by] [nvarchar](50) NULL,
        [is_deleted] [bit] NOT NULL DEFAULT(0),
        CONSTRAINT [PK_data_news_image] PRIMARY KEY CLUSTERED ([id] ASC),
        CONSTRAINT [FK_data_news_image_data_news] FOREIGN KEY([news_id]) 
            REFERENCES [dbo].[data_news] ([id]) ON DELETE CASCADE
    )
END
GO

-- Table: data_news_tag (Gắn thẻ người dùng trong bài viết)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[data_news_tag]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[data_news_tag](
        [id] [int] IDENTITY(1,1) NOT NULL,
        [news_id] [int] NOT NULL,
        [user_code] [nvarchar](50) NOT NULL,
        [created_at] [datetime] NULL DEFAULT(GETDATE()),
        [created_by] [nvarchar](50) NULL,
        CONSTRAINT [PK_data_news_tag] PRIMARY KEY CLUSTERED ([id] ASC),
        CONSTRAINT [FK_data_news_tag_data_news] FOREIGN KEY([news_id]) 
            REFERENCES [dbo].[data_news] ([id]) ON DELETE CASCADE
    )
END
GO

-- Table: data_news_reaction (Cảm xúc của bài viết: Like, Love, Haha, Wow, Sad, Angry)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[data_news_reaction]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[data_news_reaction](
        [id] [int] IDENTITY(1,1) NOT NULL,
        [news_id] [int] NOT NULL,
        [user_code] [nvarchar](50) NOT NULL,
        [reaction_type] [int] NOT NULL, -- 1:Like, 2:Love, 3:Haha, 4:Wow, 5:Sad, 6:Angry
        [created_at] [datetime] NULL DEFAULT(GETDATE()),
        [updated_at] [datetime] NULL,
        CONSTRAINT [PK_data_news_reaction] PRIMARY KEY CLUSTERED ([id] ASC),
        CONSTRAINT [FK_data_news_reaction_data_news] FOREIGN KEY([news_id]) 
            REFERENCES [dbo].[data_news] ([id]) ON DELETE CASCADE,
        CONSTRAINT [UQ_data_news_reaction] UNIQUE([news_id], [user_code]) -- Mỗi user chỉ 1 reaction/bài
    )
END
GO

-- Table: data_news_comment (Bình luận)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[data_news_comment]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[data_news_comment](
        [id] [int] IDENTITY(1,1) NOT NULL,
        [news_id] [int] NOT NULL,
        [parent_id] [int] NULL, -- NULL: comment gốc, NOT NULL: reply
        [user_code] [nvarchar](50) NOT NULL,
        [content] [nvarchar](max) NOT NULL,
        [created_at] [datetime] NULL DEFAULT(GETDATE()),
        [created_by] [nvarchar](50) NULL,
        [updated_at] [datetime] NULL,
        [updated_by] [nvarchar](50) NULL,
        [is_deleted] [bit] NOT NULL DEFAULT(0),
        [note] [nvarchar](500) NULL,
        CONSTRAINT [PK_data_news_comment] PRIMARY KEY CLUSTERED ([id] ASC),
        CONSTRAINT [FK_data_news_comment_data_news] FOREIGN KEY([news_id]) 
            REFERENCES [dbo].[data_news] ([id]) ON DELETE CASCADE,
        CONSTRAINT [FK_data_news_comment_parent] FOREIGN KEY([parent_id]) 
            REFERENCES [dbo].[data_news_comment] ([id])
    )
END
GO

-- Table: data_news_comment_image (Hình ảnh trong bình luận)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[data_news_comment_image]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[data_news_comment_image](
        [id] [int] IDENTITY(1,1) NOT NULL,
        [comment_id] [int] NOT NULL,
        [image_url] [nvarchar](1000) NOT NULL,
        [created_at] [datetime] NULL DEFAULT(GETDATE()),
        [created_by] [nvarchar](50) NULL,
        CONSTRAINT [PK_data_news_comment_image] PRIMARY KEY CLUSTERED ([id] ASC),
        CONSTRAINT [FK_data_news_comment_image_comment] FOREIGN KEY([comment_id]) 
            REFERENCES [dbo].[data_news_comment] ([id]) ON DELETE CASCADE
    )
END
GO

-- Table: data_news_comment_reaction (Cảm xúc của bình luận)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[data_news_comment_reaction]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[data_news_comment_reaction](
        [id] [int] IDENTITY(1,1) NOT NULL,
        [comment_id] [int] NOT NULL,
        [user_code] [nvarchar](50) NOT NULL,
        [reaction_type] [int] NOT NULL, -- 1:Like, 2:Love, 3:Haha
        [created_at] [datetime] NULL DEFAULT(GETDATE()),
        [updated_at] [datetime] NULL,
        CONSTRAINT [PK_data_news_comment_reaction] PRIMARY KEY CLUSTERED ([id] ASC),
        CONSTRAINT [FK_data_news_comment_reaction_comment] FOREIGN KEY([comment_id]) 
            REFERENCES [dbo].[data_news_comment] ([id]) ON DELETE CASCADE,
        CONSTRAINT [UQ_data_news_comment_reaction] UNIQUE([comment_id], [user_code])
    )
END
GO

-- Table: data_news_share (Chia sẻ bài viết)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[data_news_share]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[data_news_share](
        [id] [int] IDENTITY(1,1) NOT NULL,
        [news_id] [int] NOT NULL,
        [user_code] [nvarchar](50) NOT NULL,
        [share_content] [nvarchar](max) NULL, -- Nội dung khi share
        [created_at] [datetime] NULL DEFAULT(GETDATE()),
        CONSTRAINT [PK_data_news_share] PRIMARY KEY CLUSTERED ([id] ASC),
        CONSTRAINT [FK_data_news_share_data_news] FOREIGN KEY([news_id]) 
            REFERENCES [dbo].[data_news] ([id]) ON DELETE CASCADE
    )
END
GO

-- Create indexes for better performance
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_data_news_user_code' AND object_id = OBJECT_ID('data_news'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_data_news_user_code] ON [dbo].[data_news]([user_code])
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_data_news_created_at' AND object_id = OBJECT_ID('data_news'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_data_news_created_at] ON [dbo].[data_news]([created_at] DESC)
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_data_news_comment_news_id' AND object_id = OBJECT_ID('data_news_comment'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_data_news_comment_news_id] ON [dbo].[data_news_comment]([news_id])
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_data_news_reaction_news_id' AND object_id = OBJECT_ID('data_news_reaction'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_data_news_reaction_news_id] ON [dbo].[data_news_reaction]([news_id])
END
GO

PRINT 'News/Post system tables created successfully!'

