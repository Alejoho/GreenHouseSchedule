CREATE TABLE [dbo].[Clients]
(
	[ID] SMALLINT IDENTITY(1,1) NOT NULL,
	[Name] NVARCHAR(50) NOT NULL,
	[NickName] NVARCHAR(50),
	[PhoneNumber] NVARCHAR(20),
	[OtherNumber] NVARCHAR(20),
	[OrganizationId] SMALLINT NOT NULL,
	CONSTRAINT [PK_Clients] PRIMARY KEY ([ID]),
	CONSTRAINT [UQ_Clients_Name] UNIQUE ([Name]),
	CONSTRAINT [FK_Clients_OrganizationId] FOREIGN KEY ([OrganizationId]) 
	REFERENCES [Organizations] ([ID])
)
