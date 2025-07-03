CREATE TABLE [dbo].[OrderLocations]
(
	[ID] INT IDENTITY(1,1) NOT NULL,
	[GreenHouseId] TINYINT NOT NULL,
	[SeedTrayId] TINYINT NOT NULL,
	[OrderId] SMALLINT NOT NULL,
	[SeedTrayAmount] SMALLINT NOT NULL,
	[SeedlingAmount] INT NOT NULL,
	[EstimateSowDate] DATE,
    [EstimateDeliveryDate] DATE,
	[RealSowDate] DATE,
	[RealDeliveryDate] DATE,
	CONSTRAINT [PK_OrderLocations] PRIMARY KEY ([ID]),
	CONSTRAINT [FK_OrderLocations_SeedTrayId] FOREIGN KEY ([SeedTrayId]) 
	REFERENCES [SeedTrays] ([ID]),
	CONSTRAINT [FK_OrderLocations_GreenHouseId] FOREIGN KEY ([GreenHouseId]) 
	REFERENCES [GreenHouses] ([ID]),
	CONSTRAINT [CK_OrderLocations_SeedTrayAmount] CHECK ([SeedTrayAmount] > -1),
	CONSTRAINT [CK_OrderLocations_SeedlingAmount] CHECK ([SeedlingAmount] > -1)
)
