﻿CREATE TABLE [dbo].[Products]
(
	[ID] TINYINT IDENTITY(1,1) NOT NULL,
	[SpecieId] TINYINT NOT NULL,
	[Variety] NVARCHAR(50) NOT NULL,
	CONSTRAINT [PK_Products] PRIMARY KEY ([ID]),
	CONSTRAINT [FK_Products_SpecieId] FOREIGN KEY ([SpecieId]) 
	REFERENCES [Species] ([ID])
)
