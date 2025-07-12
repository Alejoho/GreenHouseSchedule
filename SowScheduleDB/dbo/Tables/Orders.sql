CREATE TABLE [dbo].[Orders]
(
	[ID] SMALLINT IDENTITY(1,1) NOT NULL,
	[ClientId] SMALLINT NOT NULL,
	[ProductId] TINYINT NOT NULL,
	[AmountOfWishedSeedlings] INT NOT NULL,
	[AmountOfAlgorithmSeedlings] INT NOT NULL,
	[WishDate] DATE NOT NULL,
	[DateOfRequest] DATE NOT NULL,
	[EstimateSowDate] DATE NOT NULL,
	[EstimateDeliveryDate] DATE NOT NULL,
	[RealSowDate] DATE,
	[RealDeliveryDate] DATE,
	[Sown] BIT NOT NULL,
	[Delivered] BIT NOT NULL,
	CONSTRAINT [PK_Orders] PRIMARY KEY ([ID]),
	CONSTRAINT [FK_Orders_ClientId] FOREIGN KEY ([ClientId]) 
	REFERENCES [Clients] ([ID]),
	CONSTRAINT [FK_Orders_ProductId] FOREIGN KEY ([ProductId]) 
	REFERENCES [Products] ([ID]),
	CONSTRAINT [CK_Orders_AmountofWishedSeedlings] CHECK ([AmountofWishedSeedlings] > 0),
	CONSTRAINT [CK_Orders_AmountofAlgorithmSeedlings] CHECK ([AmountofAlgorithmSeedlings] > 0)
)
