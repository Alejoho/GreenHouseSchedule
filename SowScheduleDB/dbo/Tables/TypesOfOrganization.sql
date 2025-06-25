CREATE TABLE [dbo].[TypesOfOrganization]
(
	[ID] TINYINT IDENTITY(1,1),
	[Name] NVARCHAR(50) NOT NULL,
	CONSTRAINT [PK_TypesOfOrganization] PRIMARY KEY ([ID]),
	CONSTRAINT [UC_TypesOfOrganization_Name] UNIQUE ([Name])
)
