CREATE TABLE [dbo].[Blocks]
(
	[ID] INT IDENTITY(1,1) NOT NULL,
	[OrderLocationId] INT NOT NULL,
	[BlockNumber] TINYINT NOT NULL,
	[SeedTrayAmount] SMALLINT NOT NULL,
	[NumberWithinTheBlock] TINYINT NOT NULL,
	CONSTRAINT [PK_Blocks] PRIMARY KEY ([ID]),
	CONSTRAINT [FK_Blocks_OrderLocationId] FOREIGN KEY ([OrderLocationId]) 
	REFERENCES [OrderLocations] ([ID]) ON DELETE CASCADE,
	CONSTRAINT [CK_Blocks_SeedTrayAmount] CHECK ([SeedTrayAmount] > -1),
	CONSTRAINT [CK_Blocks_BlockNumber] CHECK ([BlockNumber] > 0),
	CONSTRAINT [CK_Blocks_NumberWithinTheBlock] CHECK ([NumberWithinTheBlock] >= 0)
)
