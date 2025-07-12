CREATE TABLE [dbo].[GreenHouses]
(
	[ID] TINYINT IDENTITY(0,1) NOT NULL,
	[Name] NVARCHAR(50) NOT NULL,
	[Description] NVARCHAR(700),
	[Width] NUMERIC(4,2),
	[Length] NUMERIC(4,2),
	[GreenHouseArea] NUMERIC(5,2),
	[SeedTrayArea] NUMERIC(5,2) NOT NULL,
	[AmountOfBlocks] TINYINT NOT NULL,
	[Active] BIT NOT NULL,
	CONSTRAINT [PK_GreenHouses] PRIMARY KEY ([ID]),
	CONSTRAINT [UQ_GreenHouses_Name] UNIQUE ([Name]),
	CONSTRAINT [CK_GreenHouses_Width] CHECK ([Width] IS NULL OR ([Width] > 0 AND [Width] < 200)),
	CONSTRAINT [CK_GreenHouses_Length] CHECK ([Length] IS NULL OR ([Length] > 0 AND [Length] < 200)),
	CONSTRAINT [CK_GreenHouses_GreenHouseArea] CHECK ([GreenHouseArea] IS NULL OR ([GreenHouseArea] > 0)),
	CONSTRAINT [CK_GreenHouses_SeedTrayArea] CHECK ([SeedTrayArea] > 0),
	CONSTRAINT [CK_GreenHouses_AmountOfBlocks] CHECK ([AmountOfBlocks] > 0 AND [AmountOfBlocks] < 10)
)
