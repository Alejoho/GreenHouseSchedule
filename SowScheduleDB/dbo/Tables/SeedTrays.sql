CREATE TABLE [dbo].[SeedTrays]
(
	[ID] TINYINT IDENTITY NOT NULL,
	[Name] NVARCHAR(50) NOT NULL,
	[TotalAlveolus] SMALLINT NOT NULL,
	[AlveolusLength] TINYINT,
	[AlveolusWidth] TINYINT,
	[TrayLength] NUMERIC(3,2),
	[TrayWidth] NUMERIC(3,2),
	[TrayArea] NUMERIC(5,4),
	[LogicalTrayArea] NUMERIC(5,4) NOT NULL,
	[TotalAmount] SMALLINT NOT NULL,
	[Material] NVARCHAR(20),
	[Active] BIT NOT NULL,
	[Selected] BIT NOT NULL,
	CONSTRAINT [PK_SeedTrays] PRIMARY KEY ([ID]),
	CONSTRAINT [UQ_SeedTrays_Name] UNIQUE ([Name]),
	CONSTRAINT [CK_SeedTrays_TotalAlveolus] CHECK ([TotalAlveolus] > 0 AND [TotalAlveolus] < 400),
	CONSTRAINT [CK_SeedTrays_AlveolusLength] CHECK ([AlveolusLength] IS NULL OR ([AlveolusLength] > 0 AND [AlveolusLength] < 50)),
	CONSTRAINT [CK_SeedTrays_AlveolusWidth] CHECK ([AlveolusWidth] IS NULL OR ([AlveolusWidth] > 0 AND [AlveolusWidth] < 50)),
	CONSTRAINT [CK_SeedTrays_TrayLength] CHECK ([TrayLength] IS NULL OR ([TrayLength] > 0 AND [TrayLength] < 1.5)),
	CONSTRAINT [CK_SeedTrays_TrayWidth] CHECK ([TrayWidth] IS NULL OR ([TrayWidth] > 0 AND [TrayWidth] < 1.5)),
	CONSTRAINT [CK_SeedTrays_TrayArea] CHECK ([TrayArea] IS NULL OR ([TrayArea] > 0 AND [TrayArea] < 2.25)),
	CONSTRAINT [CK_SeedTrays_LogicalTrayArea] CHECK (([LogicalTrayArea] >= [TrayArea]) AND [LogicalTrayArea] < 4),
	CONSTRAINT [CK_SeedTrays_TotalAmount] CHECK ([TotalAmount] > 0 AND [TotalAmount] < 10000)

)
