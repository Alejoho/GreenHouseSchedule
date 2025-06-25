CREATE TABLE [dbo].[Municipalities]
(
	[ID] SMALLINT IDENTITY(1,1),
	[Name] NVARCHAR(50) NOT NULL,
	[ProvinceId] TINYINT NOT NULL,
	CONSTRAINT [PK_Municipalities] PRIMARY KEY ([ID]),
	CONSTRAINT [FK_Municipalities_ProvinceId] FOREIGN KEY ([ProvinceId]) 
	REFERENCES "Provinces" ([ID])
)
