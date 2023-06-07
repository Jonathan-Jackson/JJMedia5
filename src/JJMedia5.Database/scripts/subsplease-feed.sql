/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/

INSERT INTO
    RssFeeds
VALUES (
    'https://subsplease.org/rss/?t&r=1080',
    'SubsPlease RSS Feed',
    1,
    '2021-04-12 18:44:23.1833333 +00:00',
    '2020-04-12 18:44:23.1833333 +00:00',
    '.*(?<![Batch])$')