﻿CREATE TABLE [dbo].[RssFeeds]
(
	[Id] INT NOT NULL IDENTITY(1,1) PRIMARY KEY, 
    [Url] NVARCHAR(500) NOT NULL, 
    [Info] NVARCHAR(1000) NOT NULL, 
    [IsSubscribed] BIT NOT NULL, 
    [CreatedDate] DATETIMEOFFSET NOT NULL, 
    [StartDate] DATETIMEOFFSET NOT NULL, 
    [RegexMatch] NVARCHAR(100) NOT NULL 
)
