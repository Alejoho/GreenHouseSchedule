--SowScheduleDB

--if object_id('SowScheduleDB') is not null drop database SowScheduleDB;
use "master"
go

drop database if exists "SowScheduleDB";

go
create database "SowScheduleDB";

go
use "SowScheduleDB";

--------------------------------------------------------------------------

go
drop table if exists "Provinces";
go
create table "Provinces"(
	"ID" tinyint identity(1,1),
	"Name" nvarchar(50) not null,
	CONSTRAINT [PK_Provinces] primary key ("ID"),
	CONSTRAINT [UC_Provinces_Name] UNIQUE ("Name")
);

go 
drop table if exists "Municipalities";
go
create table "Municipalities"(
	"ID" smallint identity(1,1),
	"Name" nvarchar(50) not null,
	"ProvinceId" tinyint not null,
	constraint [PK_Municipalities] primary key ("ID"),
	constraint [FK_Municipalities_ProvinceId] foreign key ("ProvinceId") 
	references "Provinces" ("ID")
);

go
drop table if exists "TypesOfOrganization";
go
create table "TypesOfOrganization"(
	"ID" tinyint identity(1,1),
	"Name" nvarchar(50) not null,
	CONSTRAINT [PK_TypesOfOrganization] primary key ("ID"),
	CONSTRAINT [UC_TypesOfOrganization_Name] UNIQUE ("Name")
);

go 
drop table if exists "Organizations";
go
create table "Organizations"(
	"ID" smallint identity(1,1),
	"Name" nvarchar(50) not null,
	"MunicipalitiesId" smallint not null,
	"TypeOfOrganizationId" tinyint not null,
	CONSTRAINT [PK_Organizations] primary key ("ID"),
	CONSTRAINT [UC_Organizations_Name] UNIQUE ("Name"),
	CONSTRAINT [FK_Organizations_MunicipalityId] foreign key ("MunicipalitiesId") 
	references "Municipalities" ("ID"),
	CONSTRAINT [FK_Organizations_TypesOfOrganizationId] foreign key ("TypeOfOrganizationId") 
	references "TypesOfOrganization" ("ID")
);

go 
drop table if exists "Clients";
go
create table "Clients"(
	"ID" smallint identity(1,1),
	"Name" nvarchar(50) not null,
	"NickName" nvarchar(50),
	"PhoneNumber" nvarchar(20),
	"OtherNumber" nvarchar(20),
	"OrganizationId" smallint not null,
	CONSTRAINT [PK_Clients] primary key ("ID"),
	CONSTRAINT [UC_Clients_Name] UNIQUE ("Name"),
	CONSTRAINT [FK_Clients_OrganizationId] foreign key ("OrganizationId") 
	references "Organizations" ("ID")
);

--------------------------------------------------------------

go
drop table if exists "Species";
go
create table "Species"(
	"ID" tinyint identity(1,1), 
	"Name" nvarchar(50) not null,
	"ProductionDays" tinyint not null,
	"WeightOf1000Seeds" numeric(7,3),
	"AmountOfSeedsPerHectare" int not null,
	"WeightOfSeedsPerHectare" numeric(7,3) not null,
	CONSTRAINT [PK_Species] primary key ("ID"),
	CONSTRAINT [UC_Species_Name] UNIQUE ("Name"),
	CONSTRAINT [CK_Species_ProductionDays] check ("ProductionDays" > 0 and "ProductionDays" < 100),
	CONSTRAINT [CK_Species_WeightOf1000Seeds] check ("WeightOf1000Seeds" > 0 and "WeightOf1000Seeds" < 2000),
	CONSTRAINT [CK_Species_AmountOfSeedsPerHectare] check ("AmountOfSeedsPerHectare" > 0),
	CONSTRAINT [CK_Species_WeightOfSeedsPerHectare] check ("WeightOfSeedsPerHectare" > 0)
);

go
drop table if exists "Products";
go
create table "Products"(
	"ID" tinyint identity(1,1),
	"SpecieId" tinyint not null,
	"Variety" nvarchar(50) not null,
	CONSTRAINT [PK_Products] primary key ("ID"),
	CONSTRAINT [FK_Products_SpecieId] foreign key ("SpecieId") 
	references "Species" ("ID")
);

--------------------------------------------------------

go
drop table if exists "SeedTrays";
go
create table "SeedTrays"(
	"ID" tinyint identity(1,1),
	"Name" nvarchar(50) not null,
	"TotalAlveolus" smallint not null,
	"AlveolusLength" tinyint,
	"AlveolusWidth" tinyint,
	"TrayLength" numeric(3,2),
	"TrayWidth" numeric(3,2),
	"TrayArea" numeric(5,4),
	"LogicalTrayArea" numeric(5,4) not null,
	"TotalAmount" smallint not null,
	"Material" nvarchar(20),
	"Preference" tinyint not null,
	"Active" bit not null,
	CONSTRAINT [PK_SeedTrays] primary key ("ID"),
	CONSTRAINT [UC_SeedTrays_Name] UNIQUE ("Name"),
	CONSTRAINT [UC_SeedTrays_Preference] UNIQUE ("Preference"),
	CONSTRAINT [CK_SeedTrays_TotalAlveolus] CHECK ("TotalAlveolus" > 0 and "TotalAlveolus" < 400),
	CONSTRAINT [CK_SeedTrays_AlveolusLength] CHECK ("AlveolusLength" > 0 and "AlveolusLength" < 50),
	CONSTRAINT [CK_SeedTrays_AlveolusWidth] CHECK ("AlveolusWidth" > 0 and "AlveolusWidth" < 50),
	CONSTRAINT [CK_SeedTrays_TrayLength] CHECK ("TrayLength" > 0 and "TrayLength" < 1.5),
	CONSTRAINT [CK_SeedTrays_TrayWidth] CHECK ("TrayWidth" > 0 and "TrayWidth" < 1.5),
	CONSTRAINT [CK_SeedTrays_TrayArea] CHECK ("TrayArea" > 0 and "TrayArea" < 0.5),
	CONSTRAINT [CK_SeedTrays_LogicalTrayArea] CHECK ("LogicalTrayArea" >= "TrayArea"),
	CONSTRAINT [CK_SeedTrays_TotalAmount] CHECK ("TotalAmount" > 0),
	CONSTRAINT [CK_SeedTrays_Preference] CHECK ("Preference" > 0)

);


go 
drop table if exists "GreenHouses";
go
create table "GreenHouses"(
	"ID" tinyint identity(1,1),
	"Name" nvarchar(50) not null,
	"Description" nvarchar(max),
	"Width" numeric(4,2),
	"Length" numeric(4,2),
	"GreenHouseArea" numeric(5,2),
	"SeedTrayArea" numeric(5,2) not null,
	"AmountOfBlocks" tinyint not null,
	"Active" bit not null,
	CONSTRAINT [PK_GreenHouses] primary key ("ID"),
	CONSTRAINT [UC_GreenHouses_Name] UNIQUE ("Name"),
	CONSTRAINT [CK_GreenHouses_Width] CHECK ("Width" > 0 and "Width" < 200),
	CONSTRAINT [CK_GreenHouses_Length] CHECK ("Length" > 0 and "Length" < 200),
	CONSTRAINT [CK_GreenHouses_GreenHouseArea] CHECK ("GreenHouseArea" > 0),
	CONSTRAINT [CK_GreenHouses_SeedTrayArea] CHECK ("SeedTrayArea" > 0),
	CONSTRAINT [CK_GreenHouses_AmountOfBlocks] CHECK ("AmountOfBlocks" > 0 and "AmountOfBlocks" < 10)
);


go
drop table if exists "OrderLocations";
go
create table "OrderLocations"(
	"ID" int identity(1,1),
	"GreenHouseId" tinyint not null,
	"SeedTrayId" tinyint not null,
	"OrderId" smallint not null,
	"SeedTrayAmount" smallint not null,
	"SeedlingAmount" int not null,
	"SowDate" date,
	"EstimateDeliveryDate" date,
	"RealDeliveryDate" date,
	CONSTRAINT [PK_OrderLocations] primary key ("ID"),
	CONSTRAINT [FK_OrderLocations_SeedTrayId] foreign key ("SeedTrayId") 
	references "SeedTrays" ("ID"),
	CONSTRAINT [FK_OrderLocations_GreenHouseId] foreign key ("GreenHouseId") 
	references "GreenHouses" ("ID"),
	CONSTRAINT [CK_OrderLocations_SeedTrayAmount] CHECK ("SeedTrayAmount" > 0),
	CONSTRAINT [CK_OrderLocations_SeedlingAmount] CHECK ("SeedlingAmount" > 0)
);

go
drop table if exists "Blocks";
go
create table "Blocks"(
	"ID" int identity(1,1),
	"OrderLocationId" int not null,
	"BlockNumber" tinyint not null,
	"SeedTrayAmount" smallint not null,
	"NumberWithinThBlock" tinyint not null,
	CONSTRAINT [PK_Blocks] primary key ("ID"),
	CONSTRAINT [FK_Blocks_OrderLocationId] foreign key ("OrderLocationId") 
	references "OrderLocations" ("ID"),
	CONSTRAINT [CK_Blocks_SeedTrayAmount] CHECK ("SeedTrayAmount" > 0),
	CONSTRAINT [CK_Blocks_BlockNumber] CHECK ("BlockNumber" > 0),
	CONSTRAINT [CK_Blocks_NumberWithinThBlock] CHECK ("NumberWithinThBlock" > 0)
);

go
drop table if exists "DeliveryDetails";
go
create table "DeliveryDetails"(
	"ID" int identity(1,1),
	"BlockId" int not null,
	"DeliveryDate" date not null,
	"SeedTrayAmountDelivered" smallint not null,
	CONSTRAINT [PK_DeliveryDetails] primary key ("ID"),
	CONSTRAINT [FK_DeliveryDetails_BlockId] foreign key ("BlockId") 
	references "Blocks" ("ID"),
	CONSTRAINT [CK_DeliveryDetails_SeedTrayAmountDelivered] CHECK ("SeedTrayAmountDelivered" > 0)
);

---------------------------------------------------------------------------------------

go
drop table if exists "Orders";
go
create table "Orders"(
	"ID" smallint identity(1,1),
	"ClientId" smallint not null,
	"ProductId" tinyint not null,
	"AmountofWishedSeedlings" int not null,
	"AmountofAlgorithmSeedlings" int not null,
	"WishDate" date not null,
	"DateOfRequest" date not null,
	"EstimateSowDate" date not null,
	"EstimateDeliveryDate" date not null,
	"RealSowDate" date,
	"RealDeliveryDate" date,
	"Complete" bit not null,
	CONSTRAINT [PK_Orders] primary key ("ID"),
	CONSTRAINT [FK_Orders_ClientId] foreign key ("ClientId") 
	references "Clients" ("ID"),
	CONSTRAINT [FK_Orders_ProductId] foreign key ("ProductId") 
	references "Products" ("ID"),
	CONSTRAINT [CK_Orders_AmountofWishedSeedlings] CHECK ("AmountofWishedSeedlings" > 0),
	CONSTRAINT [CK_Orders_AmountofAlgorithmSeedlings] CHECK ("AmountofAlgorithmSeedlings" > 0)
);


go
alter table "OrderLocations"
add CONSTRAINT [FK_OrderLocations_OrderId] foreign key ("OrderId") 
references "Orders" ("ID");


go
drop table if exists "OrderDetails";
go
create table "OrderDetails"(
	"ID" smallint identity(1,1),
	"OrderId" smallint not null,
	"SeedsSource" nvarchar(50) not null,
	"Germination" tinyint,
	"Description" nvarchar(max),
	CONSTRAINT [PK_OrderDetails] primary key ("ID"),
	CONSTRAINT [FK_OrderDetails_OrderId] foreign key ("OrderId") 
	references "Orders" ("ID"),
	CONSTRAINT [CK_OrderDetails_Germination] CHECK ("Germination" > 0 and "Germination" < 100)
);




