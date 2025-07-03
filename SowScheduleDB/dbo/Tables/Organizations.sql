CREATE TABLE [dbo].[Organizations]
(
	[ID] SMALLINT IDENTITY(1,1) NOT NULL,
	[Name] NVARCHAR(50) NOT NULL,
	[MunicipalityId] SMALLINT NOT NULL,
	[TypeOfOrganizationId] TINYINT NOT NULL,
	CONSTRAINT [PK_Organizations] PRIMARY KEY ([ID]),
	CONSTRAINT [UQ_Organizations_Name] UNIQUE ([Name]),
	CONSTRAINT [FK_Organizations_MunicipalityId] FOREIGN KEY ([MunicipalityId]) 
	references [Municipalities] ([ID]),
	CONSTRAINT [FK_Organizations_TypesOfOrganizationId] FOREIGN KEY ([TypeOfOrganizationId]) 
	references [TypesOfOrganization] ([ID])
)
