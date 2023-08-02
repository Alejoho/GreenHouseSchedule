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
	"ProvinceID" tinyint identity(1,1),
	"Name" nvarchar(50) not null,
	CONSTRAINT PK_Provinces primary key ("ProvinceID"),
	CONSTRAINT UC_Name_Provinces UNIQUE ("Name")
);

go 
drop table if exists "Municipalities";
go
create table "Municipalities"(
	"MunicipalityID" smallint identity(1,1),
	"Name" nvarchar(50) not null,
	"IdProvince" tinyint not null,
	constraint PK_ID_Municipalities primary key ("MunicipalityID"),
	constraint FK_Municipalities_Provinces foreign key ("IdProvince") references "Provinces" ("ProvinceID")
);

go
drop table if exists "TypesOfOrganization";
go
create table "TypesOfOrganization"(
	"TypeOfOrganizationID" tinyint identity(1,1),
	"Name" nvarchar(50) not null,
	CONSTRAINT PK_ID_TypesOfOrganization primary key ("TypeOfOrganizationID"),
	CONSTRAINT UC_Name_TypesOfOrganization UNIQUE ("Name")
);

go 
drop table if exists "Organizations";
go
create table "Organizations"(
	"OrganizationID" smallint identity(1,1),
	"Name" nvarchar(50) not null,
	"IdMunicipalities" smallint not null,
	"IdTypeOfOrganization" tinyint not null,
	CONSTRAINT PK_ID_Organizations primary key ("OrganizationID"),
	CONSTRAINT UC_Name_Organizations UNIQUE ("Name"),
	CONSTRAINT FK_Organizations_Municipalities foreign key ("IdMunicipalities") references "Municipalities" ("MunicipalityID"),
	CONSTRAINT FK_Organizations_TypesOfOrganization foreign key ("IdTypeOfOrganization") references "TypesOfOrganization" ("TypeOfOrganizationID")
);

go 
drop table if exists "Clients";
go
create table "Clients"(
	"ClientID" smallint identity(1,1),
	"Name" nvarchar(50) not null,
	"NickName" nvarchar(50),
	"PhoneNumber" nvarchar(20),
	"OtherNumber" nvarchar(20),
	"IdOrganization" smallint not null,
	CONSTRAINT PK_ID_Clients primary key ("ClientID"),
	CONSTRAINT UC_Name_Clients UNIQUE ("Name"),
	CONSTRAINT FK_Clients_Organizations foreign key ("IdOrganization") references "Organizations" ("OrganizationID")
);

--------------------------------------------------------------

go
drop table if exists "Species";
go
create table "Species"(
	"SpecieID" tinyint identity(1,1), 
	"Name" nvarchar(50) not null,
	"ProductionDays" tinyint not null,
	"WeightOf1000Seeds" numeric(7,3),
	"AmountOfSeedsPerHectare" int not null,
	"WeightOfSeedsPerHectare" numeric(7,3) not null,
	CONSTRAINT PK_ID_Species primary key ("SpecieID"),
	CONSTRAINT UC_Name_Species UNIQUE ("Name"),
	CONSTRAINT CK_ProductionDays_Species check ("ProductionDays" > 0 and "ProductionDays" < 100),
	CONSTRAINT CK_WeightOf1000Seeds_Species check ("WeightOf1000Seeds" > 0 and "WeightOf1000Seeds" < 2000),
	CONSTRAINT CK_AmountOfSeedsPerHectare_Species check ("AmountOfSeedsPerHectare" > 0),
	CONSTRAINT CK_WeightOfSeedsPerHectare_Species check ("WeightOfSeedsPerHectare" > 0)
);

go
drop table if exists "Products";
go
create table "Products"(
	"ProductID" tinyint identity(1,1),
	"IdSpecie" tinyint not null,
	"Variety" nvarchar(50) not null,
	CONSTRAINT PK_ID_Products primary key ("ProductID"),
	CONSTRAINT Fk_Species foreign key ("IdSpecie") references "Species" ("SpecieID")
);

--------------------------------------------------------

go
drop table if exists "SeedTrays";
go
create table "SeedTrays"(
	"SeedTrayID" tinyint identity(1,1),
	"Name" nvarchar(50) not null,
	"TotalAlveolus" smallint not null,
	"AlveolusLength" tinyint,
	"AlveolusWidth" tinyint,
	"TrayLength" numeric(3,2),
	"TrayWidth" numeric(3,2),
	"TrayArea" numeric(5,4) not null,
	"TotalAmount" smallint not null,
	"Material" nvarchar(20),
	"Preference" tinyint not null,
	"Active" bit not null,
	CONSTRAINT PK_ID_SeedTrays primary key ("SeedTrayID"),
	CONSTRAINT UC_Name_SeedTrays UNIQUE ("Name"),
	CONSTRAINT UC_Preference_SeedTrays UNIQUE ("Preference"),
	CONSTRAINT CK_TotalAlveolus_SeedTrays CHECK ("TotalAlveolus" > 0 and "TotalAlveolus" < 400),
	CONSTRAINT CK_AlveolusLength_SeedTrays CHECK ("AlveolusLength" > 0 and "AlveolusLength" < 50),
	CONSTRAINT CK_AlveolusWidth_SeedTrays CHECK ("AlveolusWidth" > 0 and "AlveolusWidth" < 50),
	CONSTRAINT CK_TrayLength_SeedTrays CHECK ("TrayLength" > 0 and "TrayLength" < 1.5),
	CONSTRAINT CK_TrayWidth_SeedTrays CHECK ("TrayWidth" > 0 and "TrayWidth" < 1.5),
	CONSTRAINT CK_TrayArea_SeedTrays CHECK ("TrayArea" > 0 and "TrayArea" < 0.5),
	CONSTRAINT CK_TotalAmount_SeedTrays CHECK ("TotalAmount" > 0),
	CONSTRAINT CK_Preference_SeedTrays CHECK ("Preference" > 0)

);


go 
drop table if exists "GreenHouses";
go
create table "GreenHouses"(
	"GreenHouseID" tinyint identity(1,1),
	"Name" nvarchar(50) not null,
	"Description" nvarchar(max),
	"Width" numeric(4,2),
	"Lenght" numeric(4,2),
	"GreenHouseArea" numeric(5,2),
	"SeedTrayArea" numeric(5,2) not null,
	"AmountOfBlocks" tinyint not null,
	"Active" bit not null,
	CONSTRAINT PK_ID_GreenHouses primary key ("GreenHouseID"),
	CONSTRAINT UC_Name_GreenHouses UNIQUE ("Name"),
	CONSTRAINT CK_Width_GreenHouses CHECK ("Width" > 0 and "Width" < 200),
	CONSTRAINT CK_Lenght_GreenHouses CHECK ("Lenght" > 0 and "Lenght" < 200),
	CONSTRAINT CK_GreenHouseArea_GreenHouses CHECK ("GreenHouseArea" > 0),
	CONSTRAINT CK_SeedTrayArea_GreenHouses CHECK ("SeedTrayArea" > 0),
	CONSTRAINT CK_AmountOfBlocks_GreenHouses CHECK ("AmountOfBlocks" > 0 and "AmountOfBlocks" < 10)
);


go
drop table if exists "OrderLocations";
go
create table "OrderLocations"(
	"OrderLocationID" int identity(1,1),
	"IdGreenHouse" tinyint not null,
	"IdSeedTray" tinyint not null,
	"IdOrder" smallint not null,
	"SeedTrayAmount" smallint not null,
	"SeedlingAmount" int not null,
	"SowDate" date,
	"EstimateDeliveryDate" date,
	"RealDeliveryDate" date,
	CONSTRAINT PK_ID_OrderLocations primary key ("OrderLocationID"),
	CONSTRAINT FK_OrderLocations_SeedTrays foreign key ("IdSeedTray") references "SeedTrays" ("SeedTrayID"),
	CONSTRAINT FK_OrderLocations_GreenHouses foreign key ("IdGreenHouse") references "GreenHouses" ("GreenHouseID"),
	CONSTRAINT CK_SeedTrayAmount_OrderLocations CHECK ("SeedTrayAmount" > 0),
	CONSTRAINT CK_SeedlingAmount_OrderLocations CHECK ("SeedlingAmount" > 0)
);

go
drop table if exists "Blocks";
go
create table "Blocks"(
	"BlockID" bigint identity(1,1),
	"IdOrderLocation" int not null,
	"BlockNumber" tinyint not null,
	"SeedTrayAmount" smallint not null,
	"NumberWithinThBlock" tinyint not null,
	CONSTRAINT PK_ID_Blocks primary key ("BlockID"),
	CONSTRAINT FK_OrderLocations_Blocks foreign key ("IdOrderLocation") references "OrderLocations" ("OrderLocationID"),
	CONSTRAINT CK_SeedTrayAmount_Blocks CHECK ("SeedTrayAmount" > 0),
	CONSTRAINT CK_BlockNumber_Blocks CHECK ("BlockNumber" > 0),
	CONSTRAINT CK_NumberWithinThBlock_Blocks CHECK ("NumberWithinThBlock" > 0)
);

go
drop table if exists "DeliveryDetails";
go
create table "DeliveryDetails"(
	"DeliveryDetailID" bigint identity(1,1),
	"IdBlock" bigint not null,
	"DeliveryDate" date not null,
	"SeedTrayAmountDelivered" smallint not null,
	CONSTRAINT PK_ID_DeliveryDetails primary key ("DeliveryDetailID"),
	CONSTRAINT FK_Blocks_DeliveryDetails foreign key ("IdBlock") references "Blocks" ("BlockID"),
	CONSTRAINT CK_SeedTrayAmountDelivered_DeliveryDetails CHECK ("SeedTrayAmountDelivered" > 0)
);

---------------------------------------------------------------------------------------

go
drop table if exists "Orders";
go
create table "Orders"(
	"OrderID" smallint identity(1,1),
	"IdClient" smallint not null,
	"IdProduct" tinyint not null,
	"AmountofWishedSeedlings" int not null,
	"AmountofAlgorithmSeedlings" int not null,
	"WishDate" date not null,
	"DateOfRequest" date not null,
	"EstimateSowDate" date not null,
	"EstimateDeliveryDate" date not null,
	"RealSowDate" date,
	"RealDeliveryDate" date,
	CONSTRAINT PK_ID_Orders primary key ("OrderID"),
	CONSTRAINT FK_Orders_Clients foreign key ("IdClient") references "Clients" ("ClientID"),
	CONSTRAINT FK_Orders_Products foreign key ("IdProduct") references "Products" ("ProductID"),
	CONSTRAINT CK_AmountofWishedSeedlings_Orders CHECK ("AmountofWishedSeedlings" > 0),
	CONSTRAINT CK_AmountofAlgorithmSeedlings_Orders CHECK ("AmountofAlgorithmSeedlings" > 0)
);


go
alter table "OrderLocations"
add CONSTRAINT FK_OrderLocations_Orders foreign key ("IdOrder") references "Orders" ("OrderID");


go
drop table if exists "OrderDetails";
go
create table "OrderDetails"(
	"OrderDetailID" smallint identity(1,1),
	"IdOrder" smallint not null,
	"SeedsSource" nvarchar(50) not null,
	"Germination" tinyint,
	"Description" nvarchar(max),
	CONSTRAINT PK_ID_OrderDetails primary key ("OrderDetailID"),
	CONSTRAINT FK_OrderDetails_Orders foreign key ("IdOrder") references "Orders" ("OrderID"),
	CONSTRAINT CK_Germination_OrderDetails CHECK ("Germination" > 0 and "Germination" < 100)
);




