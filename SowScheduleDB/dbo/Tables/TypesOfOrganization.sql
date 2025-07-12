CREATE TABLE [dbo].[TypesOfOrganization]
(
	[ID] TINYINT IDENTITY(1,1) NOT NULL,
	[Name] NVARCHAR(50) NOT NULL,
	CONSTRAINT [PK_TypesOfOrganization] PRIMARY KEY ([ID]),
	CONSTRAINT [UQ_TypesOfOrganization_Name] UNIQUE ([Name])
)
