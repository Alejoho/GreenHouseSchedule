CREATE TABLE [dbo].[Municipalities]
(
	[ID] SMALLINT IDENTITY(1,1) NOT NULL,
	[Name] NVARCHAR(50) NOT NULL,
	[ProvinceId] TINYINT NOT NULL,
	CONSTRAINT [PK_Municipalities] PRIMARY KEY ([ID]),
	CONSTRAINT [UQ_Municipalities_Name] UNIQUE ([Name]),
	CONSTRAINT [FK_Municipalities_ProvinceId] FOREIGN KEY ([ProvinceId]) 
	REFERENCES "Provinces" ([ID])
)
