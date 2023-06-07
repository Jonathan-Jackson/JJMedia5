CREATE TABLE [dbo].[Episodes]
(
	[Id] INT NOT NULL IDENTITY(1,1) PRIMARY KEY, 
    [Title] NVARCHAR(200) NOT NULL, 
    [EpisodeNumber] INT NOT NULL, 
    [SeasonNumber] INT NOT NULL, 
    [AiredOn] DATETIMEOFFSET NULL, 
    [AddedOn] DATETIMEOFFSET NOT NULL, 
    [SeriesId] INT NOT NULL, 
    [Description] NVARCHAR(MAX) NOT NULL, 
    [SourceApi] INT NOT NULL, 
    [SourceId] NVARCHAR(150) NOT NULL, 
    CONSTRAINT [FK_Episodes_Series] FOREIGN KEY ([SeriesId]) REFERENCES [Series]([Id])
)

GO

CREATE NONCLUSTERED INDEX [IX_Episodes_SourceId] ON [dbo].[Episodes] ([SourceId])
