CREATE TABLE [dbo].[Series]
(
	[Id] INT NOT NULL IDENTITY(1,1) PRIMARY KEY, 
    [Description] NVARCHAR(MAX) NOT NULL, 
    [AirDate] DATETIMEOFFSET NULL, 
    [AddedOn] DATETIMEOFFSET NOT NULL, 
    [SourceId] NVARCHAR(150) NOT NULL, 
    [SourceApi] INT NOT NULL
)

GO

CREATE NONCLUSTERED INDEX [IX_Series_SourceId] ON [dbo].[Series] ([SourceId])
