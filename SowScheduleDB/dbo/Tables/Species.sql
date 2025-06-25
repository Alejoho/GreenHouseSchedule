CREATE TABLE [dbo].[Species]
(
	[ID] TINYINT IDENTITY(1,1), 
	[Name] NVARCHAR(50) NOT NULL,
	[ProductionDays] TINYINT NOT NULL,
	[WeightOf1000Seeds] NUMERIC(7,3),
	[AmountOfSeedsPerHectare] INT NOT NULL,
	[WeightOfSeedsPerHectare] NUMERIC(7,3) NOT NULL,
	CONSTRAINT [PK_Species] PRIMARY KEY ([ID]),
	CONSTRAINT [UC_Species_Name] UNIQUE ([Name]),
	CONSTRAINT [CK_Species_ProductionDays] CHECK ([ProductionDays] > 0 AND [ProductionDays] < 100),
	CONSTRAINT [CK_Species_WeightOf1000Seeds] CHECK ([WeightOf1000Seeds] > 0 AND [WeightOf1000Seeds] < 2000),
	CONSTRAINT [CK_Species_AmountOfSeedsPerHectare] CHECK ([AmountOfSeedsPerHectare] > 0),
	CONSTRAINT [CK_Species_WeightOfSeedsPerHectare] CHECK ([WeightOfSeedsPerHectare] > 0)
)
