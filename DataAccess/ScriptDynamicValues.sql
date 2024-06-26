go
use "SowScheduleDB";


go
insert into "Provinces" values
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

go
insert into "TypesOfOrganization" values
	('CCS'),
	('CPA'),
	('ECV'),
	('GU'),
	('UBPC'),
	('UM'),
	('N.I.');

go
insert into "Municipalities" values
	('Quivicán', 4),
	('Batabanó', 4),
	('San José de las Lajas', 4),
	('Melena del Sur', 4),
	('San Antonio de las Vegas', 4),
	('Mantanzas', 5),
	('Guira de Melena', 3),
	('Bejucal', 4),
	('San Antonio de los Baños', 3),
	('Buena Ventura', 4),
	('Bauta', 3),
	('Guines', 4),
	('Calvario', 2),
	('N.I.', 17);

go
insert into "Organizations" values
	('Mártires de Barbados', 1, 1),
	('Jose Luis Tacende', 14, 1),
	('Jose Antonio Echevería', 14, 1);

go
insert into "Clients" values
	('Alberto Martínez', '', '52368566', '', 1),
	('Yesdel Acosta Pérez', '', '55423113', '', 2),
	('Manuel Rodríguez', '', '', '47425145', 1),
	('Alberto Espinosa', '', '52556650', '', 3),
	('Jose Carlos', '', '53742114', '', 1);

--------------------------------------------------------------------------------------------

go 
insert into "Species" values
	('Tomate', 30, 750, 35000, 25.25),
	('Col', 30, 840, 30000, 25.20),
	('Pimiento', 45, 810, 33000, 26.73);

go
insert into "Products" values
	(1,'Vyta Mariela'),
	(1,'HA-3019'),
	(1,'N.I.'),
	(2,'KK-Cross'),
	(2,'Globe Master'),
	(2,'King of King Cross'),
	(2,'N.I. '),
	(3,'California'),
	(3,'N.I.');

----------------------------------------------------------------------------------

go
insert into "GreenHouses" values
	('    -', 'This is a ghost house to put 
	the dont sown order locations', 1, 1, 1, 1, 1, 0),
	('Casa 1', '', 11.58, 35, 405.3, 306.81, 4, 1),
	('Casa 2', '', 11.58, 34.6, 400.67, 306.72, 4, 1),
	('Casa 3', '', 11.32, 35, 396.2, 256.81, 4, 1),
	('Casa 4', '', 11.58, 35.2, 407.62, 308.77, 4, 1),
	('Casa 5', '', 11.58, 35, 405.3, 305.98, 4, 1),
	('Casa 6', '', 7, 30, 210, 170, 2, 1),
	('Casa 7', '', 7.3, 35, 255.5, 181.61, 2, 1),
	('Casa 8', '', 7.78, 35, 272.3, 181.61, 2, 1);

go
insert into "SeedTrays" values
	('264', 264, 22, 12, 0.62, 0.35, 0.217, 0.217, 2574, 'poliespuma',1,0),
	('260', 260, 20, 13, 0.72, 0.47, 0.3384, 0.3384, 2583, 'poliespuma',1,0),
	('160 negra', 160, 16, 10, 0.52, 0.33, 0.1716, 0.1716, 4722, 'polietileno',1,0),
	('150', 150, 15, 10, 0.56, 0.37, 0.2072, 0.2072, 900, 'polietileno',1,0),
	('160 blanca', 160, 16, 10, 0.52, 0.33, 0.1716, 0.1716, 100, 'poliespuma',1,0),
	('280', 280, 20, 14, 0.52, 0.33, 0.1716, 0.1716, 500, 'poliespuma',1,0);

--------------------------------------------------------------------------------------

go

DECLARE @currentDate DATETIME;

SET @currentDate = GETDATE();

insert into "Orders" values
	(1, 1, 40000, 48000, '5-8-2024', '2-15-2024', '4-10-2024', '5-8-2024', '4-10-2024', '5-8-2024', 1, 1),
	(5, 2, 30000, 36000, '4-20-2024', '1-10-2024', '3-18-2024', '4-20-2024', '3-18-2024', '4-20-2024', 1, 1),
	(3, 5, 60000, 72000, '6-3-2024', '4-21-2024', '5-4-2024', '6-3-2024', '5-4-2024', '6-3-2024', 1, 1),
	(2, 8, 45000, 54000, '5-24-2024', '3-15-2024', '4-9-2024', '5-24-2024', '4-9-2024', '5-24-2024', 1, 1),
	(4, 3, 18000, 21600, '4-30-2024', '1-11-2024', '3-28-2024', '4-30-2024', '3-28-2024', '4-30-2024', 1, 1),
	(1, 6, 37000, 44400, DATEADD(DAY, 28, @currentDate), DATEADD(DAY, -105, @currentDate), DATEADD(DAY, -2, @currentDate), DATEADD(DAY, 28, @currentDate), DATEADD(DAY, -2, @currentDate), null, 1, 0),		
	(3, 8, 29000, 34800, DATEADD(DAY, 41, @currentDate), DATEADD(DAY, -150, @currentDate), DATEADD(DAY, -4, @currentDate), DATEADD(DAY, 41, @currentDate), DATEADD(DAY, -3, @currentDate), null, 0, 0),	
	(5, 7, 33800, 40560, DATEADD(DAY, 33, @currentDate), DATEADD(DAY, -140, @currentDate), DATEADD(DAY, 3, @currentDate), DATEADD(DAY, 33, @currentDate), null, null, 0, 0),
	(3, 4, 21000, 25200, DATEADD(DAY, 38, @currentDate), DATEADD(DAY, -95, @currentDate), DATEADD(DAY, 8, @currentDate), DATEADD(DAY, 38, @currentDate), null, null, 0, 0),
	(2, 3, 75000, 90160, DATEADD(DAY, -5, @currentDate), DATEADD(DAY, -80, @currentDate), DATEADD(DAY, -35, @currentDate), DATEADD(DAY, -5, @currentDate), DATEADD(DAY, -35, @currentDate), DATEADD(DAY, -5, @currentDate), 1, 0),
	(1, 9, 83000, 99760, DATEADD(DAY, -1, @currentDate), DATEADD(DAY, -200, @currentDate), DATEADD(DAY, -46, @currentDate), DATEADD(DAY, -1, @currentDate), DATEADD(DAY, -46, @currentDate), null, 1, 0),
	(4, 5, 47250, 56700, DATEADD(DAY, 9, @currentDate), DATEADD(DAY, -185, @currentDate), DATEADD(DAY, -21, @currentDate), DATEADD(DAY, 9, @currentDate), DATEADD(DAY, -21, @currentDate), null, 1, 0),
	(2, 4, 46600, 56000, DATEADD(DAY, -5, @currentDate), DATEADD(DAY, -80, @currentDate), DATEADD(DAY, -35, @currentDate), DATEADD(DAY, -5, @currentDate), DATEADD(DAY, -35, @currentDate), DATEADD(DAY, -5, @currentDate), 1, 0);	


go

DECLARE @currentDate DATETIME;

SET @currentDate = GETDATE();

insert into "OrderLocations" values
	(1, 1, 1, 182, 48048, '4-10-2024', '5-8-2024', '4-10-2024', '5-8-2024'),
	(2, 1, 2, 76, 20064, '3-18-2024', '4-20-2024', '3-18-2024', '4-19-2024'),
	(3, 3, 2, 100, 16000, '3-19-2024', '4-21-2024', '3-19-2024', '4-19-2024'),
	(1, 2, 3, 180, 46800, '5-4-2024', '6-3-2024', '5-4-2024', '6-3-2024'),
	(3, 2, 3, 80, 20800, '5-5-2024', '6-4-2024', '5-5-2024', '6-4-2024'),
	(2, 3, 4, 130, 20800, '4-9-2024', '5-24-2024', '4-9-2024', '5-24-2024'),
	(1, 3, 4, 208, 33280, '4-9-2024', '5-24-2024', '4-9-2024', '5-24-2024'),
	(3, 1, 5, 82, 21648, '3-28-2024', '4-30-2024', '3-28-2024', '4-27-2024'),
	(0, 1, 6, 80, 21120, DATEADD(DAY, -2, @currentDate), DATEADD(DAY, 28, @currentDate), DATEADD(DAY, -2, @currentDate), null),
	(0, 2, 6, 35, 9100, DATEADD(DAY, -2, @currentDate), DATEADD(DAY, 28, @currentDate), DATEADD(DAY, -2, @currentDate), null),
	(0, 6, 6, 51, 14280, DATEADD(DAY, -1, @currentDate), DATEADD(DAY, 29, @currentDate), DATEADD(DAY, -1, @currentDate), null),	
	(0, 2, 7, 100, 26000, DATEADD(DAY, -4, @currentDate), DATEADD(DAY, 41, @currentDate), DATEADD(DAY, -3, @currentDate), null),
	(0, 2, 7, 34, 8840, DATEADD(DAY, -2, @currentDate), DATEADD(DAY, 43, @currentDate), null, null),	
	(0, 2, 8, 93, 24180, DATEADD(DAY, 3, @currentDate), DATEADD(DAY, 33, @currentDate), null, null),
	(0, 2, 8, 63, 16380, DATEADD(DAY, 5, @currentDate), DATEADD(DAY, 35, @currentDate), null, null),
	(0, 2, 9, 25, 6500, DATEADD(DAY, 8, @currentDate), DATEADD(DAY, 38, @currentDate), null, null),
	(0, 3, 9, 54, 8640, DATEADD(DAY, 8, @currentDate), DATEADD(DAY, 38, @currentDate), null, null),
	(0, 3, 9, 63, 10080, DATEADD(DAY, 11, @currentDate), DATEADD(DAY, 41, @currentDate), null, null),
	(2, 6, 10, 200, 56000, DATEADD(DAY, -35, @currentDate), DATEADD(DAY, -5, @currentDate), DATEADD(DAY, -35, @currentDate),DATEADD(DAY, -5, @currentDate)),
	(4, 6, 10, 122, 34160, DATEADD(DAY, -33, @currentDate), DATEADD(DAY, -3, @currentDate), DATEADD(DAY, -33, @currentDate),null),
	(1, 2, 11, 116, 30160, DATEADD(DAY, -46, @currentDate), DATEADD(DAY, -1, @currentDate), DATEADD(DAY, -46, @currentDate), null),
	(2, 5, 11, 125,  20000, DATEADD(DAY, -45, @currentDate), @currentDate, DATEADD(DAY, -45, @currentDate), null),
	(3, 5, 11, 310, 49600, DATEADD(DAY, -43, @currentDate), DATEADD(DAY, 2, @currentDate), DATEADD(DAY, -43, @currentDate), null),
	(7, 4, 12, 267, 40050, DATEADD(DAY, -21, @currentDate), DATEADD(DAY, 9, @currentDate), DATEADD(DAY, -21, @currentDate), null),
	(8, 4, 12, 111, 16650, DATEADD(DAY, -20, @currentDate), DATEADD(DAY, 10, @currentDate), DATEADD(DAY, -20, @currentDate), null),
	(2, 6, 13, 200, 56000, DATEADD(DAY, -35, @currentDate), DATEADD(DAY, -5, @currentDate), DATEADD(DAY, -35, @currentDate),DATEADD(DAY, -5, @currentDate));



go
insert into "Blocks" values
	(1, 1, 82, 1),
	(1, 1, 40, 2),
	(1, 2, 60, 3),
	(2, 3, 76, 4),
	(3, 4, 25, 5),
	(3, 4, 75, 6),
	(4, 1, 90, 7),
	(4, 4, 90, 8),
	(5, 2, 80, 9),
	(6, 4, 130, 10),
	(7, 4, 100, 11),
	(7, 4, 108, 12),
	(8, 3, 82, 13),
	(19, 1, 150, 0),
	(19, 2, 50, 0),
	(20, 2, 122, 0),
	(21, 1, 116, 0),
	(22, 2, 125, 0),
	(23, 3, 310, 0),
	(24, 2, 267, 0),
	(25, 1, 111, 0),
	(26, 1, 200, 0);
	--(9, 2, 80, 14),
	--(10, 1, 35, 15),
	--(11, 1, 51, 16),
	--(12, 4, 134, 17),
	--(13, 1, 50, 18),
	--(13, 2, 53, 19),
	--(13, 1, 24, 20),
	--(13, 3, 26, 21);




go

DECLARE @currentDate DATETIME;

SET @currentDate = GETDATE();

insert into "DeliveryDetails" values
(1, '5-8-2023', 42),
(1, '5-8-2023', 40),
(2, '5-8-2023', 40),
(3, '5-8-2023', 60),
(4, '4-19-2023', 76),
(5, '4-19-2023', 25),
(6, '4-19-2023',75),
(7, '6-3-2023', 90),
(8, '6-5-2023', 90),
(9, '6-4-2023',60),
(9, '6-5-2023',20),
(10, '5-24-2023', 130),
(11, '5-24-2023', 80),
(11, '5-29-2023', 20),
(12, '5-29-2023', 108),
(13, '4-27-2023', 82),
(14, DATEADD(DAY, -5, @currentDate) , 35);



