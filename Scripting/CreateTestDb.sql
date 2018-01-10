use test
set ANSI_NULLS ON;
set quoted_Identifier on;

go
IF OBJECT_ID('dbo.Street') IS NOT NULL
	DROP TABLE dbo.Street
go

IF OBJECT_ID('dbo.City') IS NOT NULL
	DROP TABLE dbo.City
go

IF OBJECT_ID('dbo.Logging') IS NOT NULL
	DROP TABLE dbo.Logging
go

CREATE TABLE dbo.City (
	CityId int IDENTITY
, Name nvarchar(200) not null
, Population int not null
CONSTRAINT PK_City_CityId PRIMARY KEY CLUSTERED (CityId)
);

CREATE TABLE dbo.Street (
	StreetId int IDENTITY
, CityId int not null
, StreetName NVarchar(200) not null
CONSTRAINT PK_Street_StreetId PRIMARY KEY CLUSTERED (StreetId)
CONSTRAINT FK_Street_CityId FOREIGN KEY (CityId) REFERENCES dbo.City (CityId)
)
create table dbo.Logging (
	LoggingId int primary key Identity
, TableName sysname not null
, Operation sysname not null
, EntityId int not null
, Moment datetime not null DEFAULT GETUTCDATE()
)
go

if OBJECT_ID('dbo.Logging_Add') IS NOT NULL
	DROP PROC dbo.Logging_Add
GO

GO
CREATE PROCEDURE dbo.Logging_Add @TableName sysname, @Operation sysname, @EntityId int
AS

IF @TableName IS NULL
BEGIN
	RAISERROR('@TableName is empty!', 16, 1)
END

IF @Operation IS NULL
BEGIN
	RAISERROR('@Operation is empty!', 16, 1)
END

INSERT dbo.Logging (TableName, Operation, EntityId)
VALUES (@TableName, @Operation, @EntityId)
GO


IF OBJECT_ID('dbo.City_Add') is not null
	DROP PROC dbo.City_Add;
GO

GO
CREATE PROCEDURE dbo.City_Add @CityName NVarchar(200), @Population int
as
/**************************************
 very long comment
 *
 *
 *
 *
 *
 *
 *
 **
 *
 *
 *
 *
 *
 *
 *
 **
 *
 *
 *
 *
 *
 *
 *
 **
 *
 *
 *
 *
 *
 *
 *
 **
 *
*********************/
	DECLARE @CityId INT;

	SELECT TOP(1) @CityId = c.CityId
	FROM dbo.City c
	WHERE c.Name = @CityName

	IF @CityId IS NOT NULL
	BEGIN
		UPDATE dbo.City
		SET Population = @Population
		WHERE [Name] = @CityName

		EXEC dbo.Logging_Add @TableName = 'dbo.City', @Operation = 'UPDATE', @EntityId = @CityId
	END
	ELSE
	BEGIN
		INSERT dbo.City (Name, Population)
		VALUES (@CityName, @Population)

		SET @CityId = SCOPE_IDENTITY();

		EXEC dbo.Logging_Add @TableName = 'dbo.City', @Operation = 'INSERT', @EntityId = @CityId;
	END
GO

exec dbo.City_Add 'Moscow', 2000000
exec dbo.City_Add 'Moscow', 2000000
exec dbo.City_Add 'Piter', 1500000

declare @CityId int;
SELECT TOP(1) @CityId = c.CityID
FROM dbo.City c
WHERE c.Name = 'Piter';

INSERT dbo.Street (StreetName, CityId)
VALUES (N'Невский', @CityId);

select * from dbo.City
select * from dbo.Street
select * from dbo.Logging