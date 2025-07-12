CREATE TABLE [dbo].[OrderDetails]
(
	[ID] SMALLINT IDENTITY(1,1) NOT NULL,
	[OrderId] SMALLINT NOT NULL,
	[SeedsSource] NVARCHAR(50) NOT NULL,
	[Germination] TINYINT,
	[Description] NVARCHAR(700),
	CONSTRAINT [PK_OrderDetails] PRIMARY KEY ([ID]),
	CONSTRAINT [FK_OrderDetails_OrderId] FOREIGN KEY ([OrderId]) 
	REFERENCES [Orders] ([ID]) ON DELETE CASCADE,
	CONSTRAINT [CK_OrderDetails_Germination] CHECK ([Germination] > 0 AND [Germination] < 100)
)
