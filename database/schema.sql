CREATE SCHEMA IF NOT EXISTS dbo;

CREATE TABLE dbo.tenagree(
	tag_ref char(11) NOT NULL PRIMARY KEY,
	prop_ref char(12) NULL,
	house_ref char(10) NULL,
	cot timestamp(0) NULL,
	eot timestamp(0) NULL,
	tenure char(3) NULL,
	present Boolean NOT NULL,
	terminated Boolean NOT NULL,
	service numeric(9, 2) NULL,
	other_charge numeric(9, 2) NULL,
	cur_bal numeric(9, 2) NULL,
	agr_type char(1) NULL,
	u_saff_rentacc char(20) NULL
);

CREATE TABLE dbo.lookup (
    lu_ref char(3) NOT NULL,
    lu_type char(3) NOT NULL,
    lu_desc varchar(80) DEFAULT ''
);

CREATE TABLE dbo.tenure (
    ten_type char(3) NOT NULL PRIMARY KEY,
    ten_desc char(15)
);
