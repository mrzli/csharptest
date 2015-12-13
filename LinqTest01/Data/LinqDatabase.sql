-- CREATE TABLES
IF OBJECT_ID('dbo.Person', 'U') IS NOT NULL
    DROP TABLE dbo.Person;

IF OBJECT_ID('dbo.[Place]', 'U') IS NOT NULL
    DROP TABLE dbo.[Place];

CREATE TABLE [dbo].[Place] (
    [Id]      INT           NOT NULL,
    [Name]    NVARCHAR (50) NOT NULL,
    [ZipCode] INT           NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

CREATE TABLE [dbo].[Person] (
    [Id]        INT           NOT NULL,
    [FirstName] NVARCHAR (50) NOT NULL,
    [LastName]  NVARCHAR (50) NOT NULL,
    [Age]       SMALLINT      NOT NULL,
    [PlaceId] INT NOT NULL, 
    PRIMARY KEY CLUSTERED ([Id] ASC), 
    CONSTRAINT [FK_Person_Place] FOREIGN KEY ([PlaceId]) REFERENCES [dbo].[Place]([Id])
);
-- END CREATE TABLES

-- CREATE STORED PROCEDURES
IF OBJECT_ID('dbo.GetAllPersonsData', 'P') IS NOT NULL
    DROP PROCEDURE dbo.GetAllPersonsData;

GO

CREATE PROCEDURE [dbo].[GetAllPersonsData]
AS
SELECT Person.*, Place.Name, Place.ZipCode FROM Person LEFT JOIN Place ON Person.PlaceId = Place.Id
RETURN 0;

GO
-- END CREATE STORED PROCEDURES

-- ENTER DATA
INSERT INTO Place(Id, Name, ZipCode) VALUES (1, 'Duga Rasa', 47250);
INSERT INTO Place(Id, Name, ZipCode) VALUES (2, 'Zagreb', 10000);

INSERT INTO Person(Id, FirstName, LastName, Age, PlaceId) VALUES (1, 'Goran', 'Mržljak', 31, 1);
INSERT INTO Person(Id, FirstName, LastName, Age, PlaceId) VALUES (2, 'Dražen', 'Medved', 46, 2);
-- END ENTER DATA