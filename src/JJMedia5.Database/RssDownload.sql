CREATE TABLE [dbo].[RssDownload]
(
	[Id] INT NOT NULL IDENTITY(1,1) PRIMARY KEY, 
    [Hash] VARCHAR(1600) NOT NULL, 
    [RssFeedId] INT NOT NULL, 
    [CreatedOn] DATETIMEOFFSET NOT NULL, 
    CONSTRAINT [FK_RssDownload_RssFeed] FOREIGN KEY (RssFeedId) REFERENCES [RssFeeds]([Id])
)

GO

CREATE NONCLUSTERED INDEX [IX_RssDownload_Hash] ON [dbo].[RssDownload] ([Hash])
