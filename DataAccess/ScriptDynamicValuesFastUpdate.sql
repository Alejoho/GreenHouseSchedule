go 

DELETE FROM "DeliveryDetails";
DBCC CHECKIDENT ("DeliveryDetails", RESEED, 0)

DELETE FROM "Blocks";
DBCC CHECKIDENT ("Blocks", RESEED, 0)

DELETE FROM "OrderLocations";
DBCC CHECKIDENT ("OrderLocations", RESEED, 0)

DELETE FROM "Orders";
DBCC CHECKIDENT ("Orders", RESEED, 0)



go

DECLARE @currentDate DATETIME;

SET @currentDate = GETDATE();

insert into "Orders" values
	(1, 1, 40000, 48000, '5-8-2024', '2-15-2024', '4-10-2024', '5-8-2024', '4-10-2024', '5-8-2024', 1),
	(5, 2, 30000, 36000, '4-20-2024', '1-10-2024', '3-18-2024', '4-20-2024', '3-18-2024', '4-20-2024', 1),
	(3, 5, 60000, 72000, '6-3-2024', '4-21-2024', '5-4-2024', '6-3-2024', '5-4-2024', '6-3-2024', 1),
	(2, 8, 45000, 54000, '5-24-2024', '3-15-2024', '4-9-2024', '5-24-2024', '4-9-2024', '5-24-2024', 1),
	(4, 3, 18000, 21600, '4-30-2024', '1-11-2024', '3-28-2024', '4-30-2024', '3-28-2024', '4-30-2024', 1),
	(1, 6, 37000, 44400, DATEADD(DAY, 28, @currentDate), '5-17-2024', DATEADD(DAY, -2, @currentDate), DATEADD(DAY, 28, @currentDate), DATEADD(DAY, -2, @currentDate), null, 1),		
	(3, 8, 29000, 34800, DATEADD(DAY, 41, @currentDate), '3-21-2024', DATEADD(DAY, -4, @currentDate), DATEADD(DAY, 41, @currentDate), DATEADD(DAY, -3, @currentDate), null, 0),	
	(5, 7, 33800, 40560, DATEADD(DAY, 33, @currentDate), '3-27-2024', DATEADD(DAY, 3, @currentDate), DATEADD(DAY, 33, @currentDate), null, null, 0),
	(3, 4, 21000, 25200, DATEADD(DAY, 38, @currentDate), '5-29-2024', DATEADD(DAY, 8, @currentDate), DATEADD(DAY, 38, @currentDate), null, null, 0);


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
	(0, 3, 9, 63, 10080, DATEADD(DAY, 11, @currentDate), DATEADD(DAY, 41, @currentDate), null, null);



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
	(8, 3, 82, 13);



go
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
(13, '4-27-2023', 82);



