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
	('Santiago de Cuba'),
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
insert into "GreenHouses" values
	('    -', 'This is a ghost house to put 
	the dont sown order locations', 1, 1, 1, 1, 1, 0);
