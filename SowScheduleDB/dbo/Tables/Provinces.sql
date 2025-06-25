CREATE TABLE [dbo].[Provinces]
(
	[ID] TINYINT IDENTITY(1,1),
	[Name] NVARCHAR(50) NOT NULL,
	CONSTRAINT [PK_Provinces] PRIMARY KEY ("ID"),
	CONSTRAINT [UC_Provinces_Name] UNIQUE ("Name")
)
