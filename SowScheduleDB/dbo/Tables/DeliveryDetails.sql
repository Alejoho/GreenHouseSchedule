CREATE TABLE [dbo].[DeliveryDetails]
(
	[ID] INT IDENTITY(1,1) NOT NULL,
	[BlockId] INT NOT NULL,
	[DeliveryDate] DATE NOT NULL,
	[SeedTrayAmountDelivered] SMALLINT NOT NULL,
	CONSTRAINT [PK_DeliveryDetails] PRIMARY KEY ([ID]),
	CONSTRAINT [FK_DeliveryDetails_BlockId] FOREIGN KEY ([BlockId]) 
	REFERENCES [Blocks] ([ID]) ON DELETE CASCADE,
	CONSTRAINT [CK_DeliveryDetails_SeedTrayAmountDelivered] CHECK ([SeedTrayAmountDelivered] > 0)
)
