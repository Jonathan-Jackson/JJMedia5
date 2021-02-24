CREATE TABLE [dbo].[Table1]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [Url] NCHAR(500) NOT NULL, 
    [Info] NCHAR(1000) NOT NULL, 
    [IsSubscribed] BIT NOT NULL, 
    [CreatedOn] DATETIMEOFFSET NOT NULL
)
