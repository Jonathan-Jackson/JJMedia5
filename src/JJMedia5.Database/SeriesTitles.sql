CREATE TABLE [dbo].[SeriesTitles]
(
	[Id] INT NOT NULL IDENTITY(1,1) PRIMARY KEY, 
    [SeriesId] INT NOT NULL, 
    [Title] NVARCHAR(250) NOT NULL, 
    [IsPrimary] BIT NOT NULL, 
    CONSTRAINT [FK_SeriesTitles_SeriesId] FOREIGN KEY (SeriesId) REFERENCES Series(Id)
)

GO

CREATE INDEX [IX_SeriesTitles_SeriesId] ON [dbo].[SeriesTitles] (SeriesId)

GO

CREATE INDEX [IX_SeriesTitles_Title] ON [dbo].[SeriesTitles] (Title)
