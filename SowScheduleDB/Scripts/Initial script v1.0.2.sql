﻿/*
Deployment script for SowScheduleDB

This code was generated by a tool.
Changes to this file may cause incorrect behavior and will be lost if
the code is regenerated.
*/

GO
SET ANSI_NULLS, ANSI_PADDING, ANSI_WARNINGS, ARITHABORT, CONCAT_NULL_YIELDS_NULL, QUOTED_IDENTIFIER ON;

SET NUMERIC_ROUNDABORT OFF;


GO
:setvar DatabaseName "SowScheduleDB"
:setvar DefaultFilePrefix "SowScheduleDB"
:setvar DefaultDataPath "C:\Users\Alejandro\AppData\Local\Microsoft\Microsoft SQL Server Local DB\Instances\MSSQLLocalDB\"
:setvar DefaultLogPath "C:\Users\Alejandro\AppData\Local\Microsoft\Microsoft SQL Server Local DB\Instances\MSSQLLocalDB\"

GO
:on error exit
GO
/*
Detect SQLCMD mode and disable script execution if SQLCMD mode is not supported.
To re-enable the script after enabling SQLCMD mode, execute the following:
SET NOEXEC OFF; 
*/
:setvar __IsSqlCmdEnabled "True"
GO
IF N'$(__IsSqlCmdEnabled)' NOT LIKE N'True'
    BEGIN
        PRINT N'SQLCMD mode must be enabled to successfully execute this script.';
        SET NOEXEC ON;
    END


GO
USE [master];


GO

IF (DB_ID(N'$(DatabaseName)') IS NOT NULL) 
BEGIN
    ALTER DATABASE [$(DatabaseName)]
    SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE [$(DatabaseName)];
END

GO
PRINT N'Creating database $(DatabaseName)...'
GO
CREATE DATABASE [$(DatabaseName)]
    ON 
    PRIMARY(NAME = [$(DatabaseName)], FILENAME = N'$(DefaultDataPath)$(DefaultFilePrefix)_Primary.mdf')
    LOG ON (NAME = [$(DatabaseName)_log], FILENAME = N'$(DefaultLogPath)$(DefaultFilePrefix)_Primary.ldf') COLLATE SQL_Latin1_General_CP1_CI_AS
GO
IF EXISTS (SELECT 1
           FROM   [master].[dbo].[sysdatabases]
           WHERE  [name] = N'$(DatabaseName)')
    BEGIN
        ALTER DATABASE [$(DatabaseName)]
            SET AUTO_CLOSE OFF 
            WITH ROLLBACK IMMEDIATE;
    END


GO
USE [$(DatabaseName)];


GO
IF EXISTS (SELECT 1
           FROM   [master].[dbo].[sysdatabases]
           WHERE  [name] = N'$(DatabaseName)')
    BEGIN
        ALTER DATABASE [$(DatabaseName)]
            SET ANSI_NULLS ON,
                ANSI_PADDING ON,
                ANSI_WARNINGS ON,
                ARITHABORT ON,
                CONCAT_NULL_YIELDS_NULL ON,
                NUMERIC_ROUNDABORT OFF,
                QUOTED_IDENTIFIER ON,
                ANSI_NULL_DEFAULT ON,
                CURSOR_DEFAULT LOCAL,
                CURSOR_CLOSE_ON_COMMIT OFF,
                AUTO_CREATE_STATISTICS ON,
                AUTO_SHRINK OFF,
                AUTO_UPDATE_STATISTICS ON,
                RECURSIVE_TRIGGERS OFF 
            WITH ROLLBACK IMMEDIATE;
    END


GO
IF EXISTS (SELECT 1
           FROM   [master].[dbo].[sysdatabases]
           WHERE  [name] = N'$(DatabaseName)')
    BEGIN
        ALTER DATABASE [$(DatabaseName)]
            SET ALLOW_SNAPSHOT_ISOLATION OFF;
    END


GO
IF EXISTS (SELECT 1
           FROM   [master].[dbo].[sysdatabases]
           WHERE  [name] = N'$(DatabaseName)')
    BEGIN
        ALTER DATABASE [$(DatabaseName)]
            SET READ_COMMITTED_SNAPSHOT OFF 
            WITH ROLLBACK IMMEDIATE;
    END


GO
IF EXISTS (SELECT 1
           FROM   [master].[dbo].[sysdatabases]
           WHERE  [name] = N'$(DatabaseName)')
    BEGIN
        ALTER DATABASE [$(DatabaseName)]
            SET AUTO_UPDATE_STATISTICS_ASYNC OFF,
                PAGE_VERIFY NONE,
                DATE_CORRELATION_OPTIMIZATION OFF,
                DISABLE_BROKER,
                PARAMETERIZATION SIMPLE,
                SUPPLEMENTAL_LOGGING OFF 
            WITH ROLLBACK IMMEDIATE;
    END


GO
IF IS_SRVROLEMEMBER(N'sysadmin') = 1
    BEGIN
        IF EXISTS (SELECT 1
                   FROM   [master].[dbo].[sysdatabases]
                   WHERE  [name] = N'$(DatabaseName)')
            BEGIN
                EXECUTE sp_executesql N'ALTER DATABASE [$(DatabaseName)]
    SET TRUSTWORTHY OFF,
        DB_CHAINING OFF 
    WITH ROLLBACK IMMEDIATE';
            END
    END
ELSE
    BEGIN
        PRINT N'The database settings cannot be modified. You must be a SysAdmin to apply these settings.';
    END


GO
IF IS_SRVROLEMEMBER(N'sysadmin') = 1
    BEGIN
        IF EXISTS (SELECT 1
                   FROM   [master].[dbo].[sysdatabases]
                   WHERE  [name] = N'$(DatabaseName)')
            BEGIN
                EXECUTE sp_executesql N'ALTER DATABASE [$(DatabaseName)]
    SET HONOR_BROKER_PRIORITY OFF 
    WITH ROLLBACK IMMEDIATE';
            END
    END
ELSE
    BEGIN
        PRINT N'The database settings cannot be modified. You must be a SysAdmin to apply these settings.';
    END


GO
ALTER DATABASE [$(DatabaseName)]
    SET TARGET_RECOVERY_TIME = 0 SECONDS 
    WITH ROLLBACK IMMEDIATE;


GO
IF EXISTS (SELECT 1
           FROM   [master].[dbo].[sysdatabases]
           WHERE  [name] = N'$(DatabaseName)')
    BEGIN
        ALTER DATABASE [$(DatabaseName)]
            SET FILESTREAM(NON_TRANSACTED_ACCESS = OFF),
                CONTAINMENT = NONE 
            WITH ROLLBACK IMMEDIATE;
    END


GO
IF EXISTS (SELECT 1
           FROM   [master].[dbo].[sysdatabases]
           WHERE  [name] = N'$(DatabaseName)')
    BEGIN
        ALTER DATABASE [$(DatabaseName)]
            SET AUTO_CREATE_STATISTICS ON(INCREMENTAL = OFF),
                MEMORY_OPTIMIZED_ELEVATE_TO_SNAPSHOT = OFF,
                DELAYED_DURABILITY = DISABLED 
            WITH ROLLBACK IMMEDIATE;
    END


GO
IF EXISTS (SELECT 1
           FROM   [master].[dbo].[sysdatabases]
           WHERE  [name] = N'$(DatabaseName)')
    BEGIN
        ALTER DATABASE [$(DatabaseName)]
            SET QUERY_STORE (QUERY_CAPTURE_MODE = ALL, DATA_FLUSH_INTERVAL_SECONDS = 900, INTERVAL_LENGTH_MINUTES = 60, MAX_PLANS_PER_QUERY = 200, CLEANUP_POLICY = (STALE_QUERY_THRESHOLD_DAYS = 367), MAX_STORAGE_SIZE_MB = 100) 
            WITH ROLLBACK IMMEDIATE;
    END


GO
IF EXISTS (SELECT 1
           FROM   [master].[dbo].[sysdatabases]
           WHERE  [name] = N'$(DatabaseName)')
    BEGIN
        ALTER DATABASE [$(DatabaseName)]
            SET QUERY_STORE = OFF 
            WITH ROLLBACK IMMEDIATE;
    END


GO
IF EXISTS (SELECT 1
           FROM   [master].[dbo].[sysdatabases]
           WHERE  [name] = N'$(DatabaseName)')
    BEGIN
        ALTER DATABASE SCOPED CONFIGURATION SET MAXDOP = 0;
        ALTER DATABASE SCOPED CONFIGURATION FOR SECONDARY SET MAXDOP = PRIMARY;
        ALTER DATABASE SCOPED CONFIGURATION SET LEGACY_CARDINALITY_ESTIMATION = OFF;
        ALTER DATABASE SCOPED CONFIGURATION FOR SECONDARY SET LEGACY_CARDINALITY_ESTIMATION = PRIMARY;
        ALTER DATABASE SCOPED CONFIGURATION SET PARAMETER_SNIFFING = ON;
        ALTER DATABASE SCOPED CONFIGURATION FOR SECONDARY SET PARAMETER_SNIFFING = PRIMARY;
        ALTER DATABASE SCOPED CONFIGURATION SET QUERY_OPTIMIZER_HOTFIXES = OFF;
        ALTER DATABASE SCOPED CONFIGURATION FOR SECONDARY SET QUERY_OPTIMIZER_HOTFIXES = PRIMARY;
    END


GO
IF EXISTS (SELECT 1
           FROM   [master].[dbo].[sysdatabases]
           WHERE  [name] = N'$(DatabaseName)')
    BEGIN
        ALTER DATABASE [$(DatabaseName)]
            SET TEMPORAL_HISTORY_RETENTION ON 
            WITH ROLLBACK IMMEDIATE;
    END


GO
IF fulltextserviceproperty(N'IsFulltextInstalled') = 1
    EXECUTE sp_fulltext_database 'enable';


GO
PRINT N'Creating Table [dbo].[Blocks]...';


GO
CREATE TABLE [dbo].[Blocks] (
    [ID]                   INT      IDENTITY (1, 1) NOT NULL,
    [OrderLocationId]      INT      NOT NULL,
    [BlockNumber]          TINYINT  NOT NULL,
    [SeedTrayAmount]       SMALLINT NOT NULL,
    [NumberWithinTheBlock] TINYINT  NOT NULL,
    CONSTRAINT [PK_Blocks] PRIMARY KEY CLUSTERED ([ID] ASC)
);


GO
PRINT N'Creating Table [dbo].[Clients]...';


GO
CREATE TABLE [dbo].[Clients] (
    [ID]             SMALLINT      IDENTITY (1, 1) NOT NULL,
    [Name]           NVARCHAR (50) NOT NULL,
    [NickName]       NVARCHAR (50) NULL,
    [PhoneNumber]    NVARCHAR (20) NULL,
    [OtherNumber]    NVARCHAR (20) NULL,
    [OrganizationId] SMALLINT      NOT NULL,
    CONSTRAINT [PK_Clients] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [UQ_Clients_Name] UNIQUE NONCLUSTERED ([Name] ASC)
);


GO
PRINT N'Creating Table [dbo].[DeliveryDetails]...';


GO
CREATE TABLE [dbo].[DeliveryDetails] (
    [ID]                      INT      IDENTITY (1, 1) NOT NULL,
    [BlockId]                 INT      NOT NULL,
    [DeliveryDate]            DATE     NOT NULL,
    [SeedTrayAmountDelivered] SMALLINT NOT NULL,
    CONSTRAINT [PK_DeliveryDetails] PRIMARY KEY CLUSTERED ([ID] ASC)
);


GO
PRINT N'Creating Table [dbo].[GreenHouses]...';


GO
CREATE TABLE [dbo].[GreenHouses] (
    [ID]             TINYINT        IDENTITY (0, 1) NOT NULL,
    [Name]           NVARCHAR (50)  NOT NULL,
    [Description]    NVARCHAR (700) NULL,
    [Width]          NUMERIC (4, 2) NULL,
    [Length]         NUMERIC (4, 2) NULL,
    [GreenHouseArea] NUMERIC (5, 2) NULL,
    [SeedTrayArea]   NUMERIC (5, 2) NOT NULL,
    [AmountOfBlocks] TINYINT        NOT NULL,
    [Active]         BIT            NOT NULL,
    CONSTRAINT [PK_GreenHouses] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [UQ_GreenHouses_Name] UNIQUE NONCLUSTERED ([Name] ASC)
);


GO
PRINT N'Creating Table [dbo].[Municipalities]...';


GO
CREATE TABLE [dbo].[Municipalities] (
    [ID]         SMALLINT      IDENTITY (1, 1) NOT NULL,
    [Name]       NVARCHAR (50) NOT NULL,
    [ProvinceId] TINYINT       NOT NULL,
    CONSTRAINT [PK_Municipalities] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [UQ_Municipalities_Name] UNIQUE NONCLUSTERED ([Name] ASC)
);


GO
PRINT N'Creating Table [dbo].[OrderDetails]...';


GO
CREATE TABLE [dbo].[OrderDetails] (
    [ID]          SMALLINT       IDENTITY (1, 1) NOT NULL,
    [OrderId]     SMALLINT       NOT NULL,
    [SeedsSource] NVARCHAR (50)  NOT NULL,
    [Germination] TINYINT        NULL,
    [Description] NVARCHAR (700) NULL,
    CONSTRAINT [PK_OrderDetails] PRIMARY KEY CLUSTERED ([ID] ASC)
);


GO
PRINT N'Creating Table [dbo].[OrderLocations]...';


GO
CREATE TABLE [dbo].[OrderLocations] (
    [ID]                   INT      IDENTITY (1, 1) NOT NULL,
    [GreenHouseId]         TINYINT  NOT NULL,
    [SeedTrayId]           TINYINT  NOT NULL,
    [OrderId]              SMALLINT NOT NULL,
    [SeedTrayAmount]       SMALLINT NOT NULL,
    [SeedlingAmount]       INT      NOT NULL,
    [EstimateSowDate]      DATE     NULL,
    [EstimateDeliveryDate] DATE     NULL,
    [RealSowDate]          DATE     NULL,
    [RealDeliveryDate]     DATE     NULL,
    CONSTRAINT [PK_OrderLocations] PRIMARY KEY CLUSTERED ([ID] ASC)
);


GO
PRINT N'Creating Table [dbo].[Orders]...';


GO
CREATE TABLE [dbo].[Orders] (
    [ID]                         SMALLINT IDENTITY (1, 1) NOT NULL,
    [ClientId]                   SMALLINT NOT NULL,
    [ProductId]                  TINYINT  NOT NULL,
    [AmountOfWishedSeedlings]    INT      NOT NULL,
    [AmountOfAlgorithmSeedlings] INT      NOT NULL,
    [WishDate]                   DATE     NOT NULL,
    [DateOfRequest]              DATE     NOT NULL,
    [EstimateSowDate]            DATE     NOT NULL,
    [EstimateDeliveryDate]       DATE     NOT NULL,
    [RealSowDate]                DATE     NULL,
    [RealDeliveryDate]           DATE     NULL,
    [Sown]                       BIT      NOT NULL,
    [Delivered]                  BIT      NOT NULL,
    CONSTRAINT [PK_Orders] PRIMARY KEY CLUSTERED ([ID] ASC)
);


GO
PRINT N'Creating Table [dbo].[Organizations]...';


GO
CREATE TABLE [dbo].[Organizations] (
    [ID]                   SMALLINT      IDENTITY (1, 1) NOT NULL,
    [Name]                 NVARCHAR (50) NOT NULL,
    [MunicipalityId]       SMALLINT      NOT NULL,
    [TypeOfOrganizationId] TINYINT       NOT NULL,
    CONSTRAINT [PK_Organizations] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [UQ_Organizations_Name] UNIQUE NONCLUSTERED ([Name] ASC)
);


GO
PRINT N'Creating Table [dbo].[Products]...';


GO
CREATE TABLE [dbo].[Products] (
    [ID]       TINYINT       IDENTITY (1, 1) NOT NULL,
    [SpecieId] TINYINT       NOT NULL,
    [Variety]  NVARCHAR (50) NOT NULL,
    CONSTRAINT [PK_Products] PRIMARY KEY CLUSTERED ([ID] ASC)
);


GO
PRINT N'Creating Table [dbo].[Provinces]...';


GO
CREATE TABLE [dbo].[Provinces] (
    [ID]   TINYINT       IDENTITY (1, 1) NOT NULL,
    [Name] NVARCHAR (50) NOT NULL,
    CONSTRAINT [PK_Provinces] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [UQ_Provinces_Name] UNIQUE NONCLUSTERED ([Name] ASC)
);


GO
PRINT N'Creating Table [dbo].[SeedTrays]...';


GO
CREATE TABLE [dbo].[SeedTrays] (
    [ID]              TINYINT        IDENTITY (1, 1) NOT NULL,
    [Name]            NVARCHAR (50)  NOT NULL,
    [TotalAlveolus]   SMALLINT       NOT NULL,
    [AlveolusLength]  TINYINT        NULL,
    [AlveolusWidth]   TINYINT        NULL,
    [TrayLength]      NUMERIC (3, 2) NULL,
    [TrayWidth]       NUMERIC (3, 2) NULL,
    [TrayArea]        NUMERIC (5, 4) NULL,
    [LogicalTrayArea] NUMERIC (5, 4) NOT NULL,
    [TotalAmount]     SMALLINT       NOT NULL,
    [Material]        NVARCHAR (20)  NULL,
    [Active]          BIT            NOT NULL,
    [Selected]        BIT            NOT NULL,
    CONSTRAINT [PK_SeedTrays] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [UQ_SeedTrays_Name] UNIQUE NONCLUSTERED ([Name] ASC)
);


GO
PRINT N'Creating Table [dbo].[Species]...';


GO
CREATE TABLE [dbo].[Species] (
    [ID]                      TINYINT        IDENTITY (1, 1) NOT NULL,
    [Name]                    NVARCHAR (50)  NOT NULL,
    [ProductionDays]          TINYINT        NOT NULL,
    [WeightOf1000Seeds]       NUMERIC (7, 3) NULL,
    [AmountOfSeedsPerHectare] INT            NOT NULL,
    [WeightOfSeedsPerHectare] NUMERIC (6, 2) NOT NULL,
    CONSTRAINT [PK_Species] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [UQ_Species_Name] UNIQUE NONCLUSTERED ([Name] ASC)
);


GO
PRINT N'Creating Table [dbo].[TypesOfOrganization]...';


GO
CREATE TABLE [dbo].[TypesOfOrganization] (
    [ID]   TINYINT       IDENTITY (1, 1) NOT NULL,
    [Name] NVARCHAR (50) NOT NULL,
    CONSTRAINT [PK_TypesOfOrganization] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [UQ_TypesOfOrganization_Name] UNIQUE NONCLUSTERED ([Name] ASC)
);


GO
PRINT N'Creating Foreign Key [dbo].[FK_Blocks_OrderLocationId]...';


GO
ALTER TABLE [dbo].[Blocks]
    ADD CONSTRAINT [FK_Blocks_OrderLocationId] FOREIGN KEY ([OrderLocationId]) REFERENCES [dbo].[OrderLocations] ([ID]) ON DELETE CASCADE;


GO
PRINT N'Creating Foreign Key [dbo].[FK_Clients_OrganizationId]...';


GO
ALTER TABLE [dbo].[Clients]
    ADD CONSTRAINT [FK_Clients_OrganizationId] FOREIGN KEY ([OrganizationId]) REFERENCES [dbo].[Organizations] ([ID]);


GO
PRINT N'Creating Foreign Key [dbo].[FK_DeliveryDetails_BlockId]...';


GO
ALTER TABLE [dbo].[DeliveryDetails]
    ADD CONSTRAINT [FK_DeliveryDetails_BlockId] FOREIGN KEY ([BlockId]) REFERENCES [dbo].[Blocks] ([ID]) ON DELETE CASCADE;


GO
PRINT N'Creating Foreign Key [dbo].[FK_Municipalities_ProvinceId]...';


GO
ALTER TABLE [dbo].[Municipalities]
    ADD CONSTRAINT [FK_Municipalities_ProvinceId] FOREIGN KEY ([ProvinceId]) REFERENCES [dbo].[Provinces] ([ID]);


GO
PRINT N'Creating Foreign Key [dbo].[FK_OrderDetails_OrderId]...';


GO
ALTER TABLE [dbo].[OrderDetails]
    ADD CONSTRAINT [FK_OrderDetails_OrderId] FOREIGN KEY ([OrderId]) REFERENCES [dbo].[Orders] ([ID]) ON DELETE CASCADE;


GO
PRINT N'Creating Foreign Key [dbo].[FK_OrderLocations_SeedTrayId]...';


GO
ALTER TABLE [dbo].[OrderLocations]
    ADD CONSTRAINT [FK_OrderLocations_SeedTrayId] FOREIGN KEY ([SeedTrayId]) REFERENCES [dbo].[SeedTrays] ([ID]);


GO
PRINT N'Creating Foreign Key [dbo].[FK_OrderLocations_GreenHouseId]...';


GO
ALTER TABLE [dbo].[OrderLocations]
    ADD CONSTRAINT [FK_OrderLocations_GreenHouseId] FOREIGN KEY ([GreenHouseId]) REFERENCES [dbo].[GreenHouses] ([ID]);


GO
PRINT N'Creating Foreign Key [dbo].[FK_Orders_ClientId]...';


GO
ALTER TABLE [dbo].[Orders]
    ADD CONSTRAINT [FK_Orders_ClientId] FOREIGN KEY ([ClientId]) REFERENCES [dbo].[Clients] ([ID]);


GO
PRINT N'Creating Foreign Key [dbo].[FK_Orders_ProductId]...';


GO
ALTER TABLE [dbo].[Orders]
    ADD CONSTRAINT [FK_Orders_ProductId] FOREIGN KEY ([ProductId]) REFERENCES [dbo].[Products] ([ID]);


GO
PRINT N'Creating Foreign Key [dbo].[FK_Organizations_MunicipalityId]...';


GO
ALTER TABLE [dbo].[Organizations]
    ADD CONSTRAINT [FK_Organizations_MunicipalityId] FOREIGN KEY ([MunicipalityId]) REFERENCES [dbo].[Municipalities] ([ID]);


GO
PRINT N'Creating Foreign Key [dbo].[FK_Organizations_TypesOfOrganizationId]...';


GO
ALTER TABLE [dbo].[Organizations]
    ADD CONSTRAINT [FK_Organizations_TypesOfOrganizationId] FOREIGN KEY ([TypeOfOrganizationId]) REFERENCES [dbo].[TypesOfOrganization] ([ID]);


GO
PRINT N'Creating Foreign Key [dbo].[FK_Products_SpecieId]...';


GO
ALTER TABLE [dbo].[Products]
    ADD CONSTRAINT [FK_Products_SpecieId] FOREIGN KEY ([SpecieId]) REFERENCES [dbo].[Species] ([ID]);


GO
PRINT N'Creating Check Constraint [dbo].[CK_Blocks_SeedTrayAmount]...';


GO
ALTER TABLE [dbo].[Blocks]
    ADD CONSTRAINT [CK_Blocks_SeedTrayAmount] CHECK ([SeedTrayAmount] > -1);


GO
PRINT N'Creating Check Constraint [dbo].[CK_Blocks_BlockNumber]...';


GO
ALTER TABLE [dbo].[Blocks]
    ADD CONSTRAINT [CK_Blocks_BlockNumber] CHECK ([BlockNumber] > 0);


GO
PRINT N'Creating Check Constraint [dbo].[CK_Blocks_NumberWithinTheBlock]...';


GO
ALTER TABLE [dbo].[Blocks]
    ADD CONSTRAINT [CK_Blocks_NumberWithinTheBlock] CHECK ([NumberWithinTheBlock] >= 0);


GO
PRINT N'Creating Check Constraint [dbo].[CK_DeliveryDetails_SeedTrayAmountDelivered]...';


GO
ALTER TABLE [dbo].[DeliveryDetails]
    ADD CONSTRAINT [CK_DeliveryDetails_SeedTrayAmountDelivered] CHECK ([SeedTrayAmountDelivered] > 0);


GO
PRINT N'Creating Check Constraint [dbo].[CK_GreenHouses_Width]...';


GO
ALTER TABLE [dbo].[GreenHouses]
    ADD CONSTRAINT [CK_GreenHouses_Width] CHECK ([Width] IS NULL OR ([Width] > 0 AND [Width] < 200));


GO
PRINT N'Creating Check Constraint [dbo].[CK_GreenHouses_Length]...';


GO
ALTER TABLE [dbo].[GreenHouses]
    ADD CONSTRAINT [CK_GreenHouses_Length] CHECK ([Length] IS NULL OR ([Length] > 0 AND [Length] < 200));


GO
PRINT N'Creating Check Constraint [dbo].[CK_GreenHouses_GreenHouseArea]...';


GO
ALTER TABLE [dbo].[GreenHouses]
    ADD CONSTRAINT [CK_GreenHouses_GreenHouseArea] CHECK ([GreenHouseArea] IS NULL OR ([GreenHouseArea] > 0));


GO
PRINT N'Creating Check Constraint [dbo].[CK_GreenHouses_SeedTrayArea]...';


GO
ALTER TABLE [dbo].[GreenHouses]
    ADD CONSTRAINT [CK_GreenHouses_SeedTrayArea] CHECK ([SeedTrayArea] > 0);


GO
PRINT N'Creating Check Constraint [dbo].[CK_GreenHouses_AmountOfBlocks]...';


GO
ALTER TABLE [dbo].[GreenHouses]
    ADD CONSTRAINT [CK_GreenHouses_AmountOfBlocks] CHECK ([AmountOfBlocks] > 0 AND [AmountOfBlocks] < 10);


GO
PRINT N'Creating Check Constraint [dbo].[CK_OrderDetails_Germination]...';


GO
ALTER TABLE [dbo].[OrderDetails]
    ADD CONSTRAINT [CK_OrderDetails_Germination] CHECK ([Germination] > 0 AND [Germination] < 100);


GO
PRINT N'Creating Check Constraint [dbo].[CK_OrderLocations_SeedTrayAmount]...';


GO
ALTER TABLE [dbo].[OrderLocations]
    ADD CONSTRAINT [CK_OrderLocations_SeedTrayAmount] CHECK ([SeedTrayAmount] > -1);


GO
PRINT N'Creating Check Constraint [dbo].[CK_OrderLocations_SeedlingAmount]...';


GO
ALTER TABLE [dbo].[OrderLocations]
    ADD CONSTRAINT [CK_OrderLocations_SeedlingAmount] CHECK ([SeedlingAmount] > -1);


GO
PRINT N'Creating Check Constraint [dbo].[CK_Orders_AmountofWishedSeedlings]...';


GO
ALTER TABLE [dbo].[Orders]
    ADD CONSTRAINT [CK_Orders_AmountofWishedSeedlings] CHECK ([AmountofWishedSeedlings] > 0);


GO
PRINT N'Creating Check Constraint [dbo].[CK_Orders_AmountofAlgorithmSeedlings]...';


GO
ALTER TABLE [dbo].[Orders]
    ADD CONSTRAINT [CK_Orders_AmountofAlgorithmSeedlings] CHECK ([AmountofAlgorithmSeedlings] > 0);


GO
PRINT N'Creating Check Constraint [dbo].[CK_SeedTrays_TotalAlveolus]...';


GO
ALTER TABLE [dbo].[SeedTrays]
    ADD CONSTRAINT [CK_SeedTrays_TotalAlveolus] CHECK ([TotalAlveolus] > 0 AND [TotalAlveolus] < 400);


GO
PRINT N'Creating Check Constraint [dbo].[CK_SeedTrays_AlveolusLength]...';


GO
ALTER TABLE [dbo].[SeedTrays]
    ADD CONSTRAINT [CK_SeedTrays_AlveolusLength] CHECK ([AlveolusLength] IS NULL OR ([AlveolusLength] > 0 AND [AlveolusLength] < 50));


GO
PRINT N'Creating Check Constraint [dbo].[CK_SeedTrays_AlveolusWidth]...';


GO
ALTER TABLE [dbo].[SeedTrays]
    ADD CONSTRAINT [CK_SeedTrays_AlveolusWidth] CHECK ([AlveolusWidth] IS NULL OR ([AlveolusWidth] > 0 AND [AlveolusWidth] < 50));


GO
PRINT N'Creating Check Constraint [dbo].[CK_SeedTrays_TrayLength]...';


GO
ALTER TABLE [dbo].[SeedTrays]
    ADD CONSTRAINT [CK_SeedTrays_TrayLength] CHECK ([TrayLength] IS NULL OR ([TrayLength] > 0 AND [TrayLength] < 1.5));


GO
PRINT N'Creating Check Constraint [dbo].[CK_SeedTrays_TrayWidth]...';


GO
ALTER TABLE [dbo].[SeedTrays]
    ADD CONSTRAINT [CK_SeedTrays_TrayWidth] CHECK ([TrayWidth] IS NULL OR ([TrayWidth] > 0 AND [TrayWidth] < 1.5));


GO
PRINT N'Creating Check Constraint [dbo].[CK_SeedTrays_TrayArea]...';


GO
ALTER TABLE [dbo].[SeedTrays]
    ADD CONSTRAINT [CK_SeedTrays_TrayArea] CHECK ([TrayArea] IS NULL OR ([TrayArea] > 0 AND [TrayArea] < 2.25));


GO
PRINT N'Creating Check Constraint [dbo].[CK_SeedTrays_LogicalTrayArea]...';


GO
ALTER TABLE [dbo].[SeedTrays]
    ADD CONSTRAINT [CK_SeedTrays_LogicalTrayArea] CHECK (([LogicalTrayArea] >= [TrayArea]) AND [LogicalTrayArea] < 4);


GO
PRINT N'Creating Check Constraint [dbo].[CK_SeedTrays_TotalAmount]...';


GO
ALTER TABLE [dbo].[SeedTrays]
    ADD CONSTRAINT [CK_SeedTrays_TotalAmount] CHECK ([TotalAmount] > 0 AND [TotalAmount] < 10000);


GO
PRINT N'Creating Check Constraint [dbo].[CK_Species_ProductionDays]...';


GO
ALTER TABLE [dbo].[Species]
    ADD CONSTRAINT [CK_Species_ProductionDays] CHECK ([ProductionDays] > 0 AND [ProductionDays] < 100);


GO
PRINT N'Creating Check Constraint [dbo].[CK_Species_WeightOf1000Seeds]...';


GO
ALTER TABLE [dbo].[Species]
    ADD CONSTRAINT [CK_Species_WeightOf1000Seeds] CHECK ([WeightOf1000Seeds] > 0 AND [WeightOf1000Seeds] < 2000);


GO
PRINT N'Creating Check Constraint [dbo].[CK_Species_AmountOfSeedsPerHectare]...';


GO
ALTER TABLE [dbo].[Species]
    ADD CONSTRAINT [CK_Species_AmountOfSeedsPerHectare] CHECK ([AmountOfSeedsPerHectare] > 0);


GO
PRINT N'Creating Check Constraint [dbo].[CK_Species_WeightOfSeedsPerHectare]...';


GO
ALTER TABLE [dbo].[Species]
    ADD CONSTRAINT [CK_Species_WeightOfSeedsPerHectare] CHECK ([WeightOfSeedsPerHectare] > 0);


GO
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
GO

GO
DECLARE @VarDecimalSupported AS BIT;

SELECT @VarDecimalSupported = 0;

IF ((ServerProperty(N'EngineEdition') = 3)
    AND (((@@microsoftversion / power(2, 24) = 9)
          AND (@@microsoftversion & 0xffff >= 3024))
         OR ((@@microsoftversion / power(2, 24) = 10)
             AND (@@microsoftversion & 0xffff >= 1600))))
    SELECT @VarDecimalSupported = 1;

IF (@VarDecimalSupported > 0)
    BEGIN
        EXECUTE sp_db_vardecimal_storage_format N'$(DatabaseName)', 'ON';
    END


GO
PRINT N'Update complete.';


GO
