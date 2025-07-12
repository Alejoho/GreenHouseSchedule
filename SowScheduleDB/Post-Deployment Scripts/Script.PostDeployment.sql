/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/

-- Adds a missing relationship

ALTER TABLE [OrderLocations]
ADD CONSTRAINT [FK_OrderLocations_OrderId] FOREIGN KEY ([OrderId]) 
REFERENCES [Orders] ([ID]) ON DELETE CASCADE;


-- Adds the initial records for development

-- Adds the initial records for production

GO


IF NOT EXISTS(SELECT 1 FROM [dbo].[Provinces])
BEGIN
	INSERT INTO [dbo].[Provinces] VALUES
		('Pinar del Río'),
		('Artemisa'),
		('La Habana'),	
		('Mayabeque'),
		('Matanzas'),
		('Cienfuegos'),
		('Villa Clara'),
		('Sancti Spíritus'),
		('Ciego de Ávila'),
		('Camagüey'),
		('Las Tunas'),
		('Granma'),
		('Holguín'),
		('Santiago'),
		('Guantánamo'),
		('Isla de la Juventud'),
		('N.I.');
END

GO
IF NOT EXISTS(SELECT 1 FROM [dbo].[TypesOfOrganization])
BEGIN
	INSERT INTO [dbo].[TypesOfOrganization] VALUES
		('CCS'),
		('CPA'),
		('ECV'),
		('GU'),
		('UBPC'),
		('UM'),
		('N.I.');
END