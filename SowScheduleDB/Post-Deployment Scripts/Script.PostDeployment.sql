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

