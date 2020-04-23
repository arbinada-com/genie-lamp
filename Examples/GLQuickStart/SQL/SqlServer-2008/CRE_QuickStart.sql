-- Genie Lamp Core (1.1.4798.27721)
-- Genie of SqlServer database (1.0.4798.27721)
-- Starter application (1.1.4798.27722)
-- This file was automatically generated at 2013-03-14 16:56:45
-- Do not modify it manually.

CREATE DATABASE [QuickStartDB]
;
GO

USE [QuickStartDB]
GO

-- Table of QuickStart.Customer
CREATE TABLE dbo.customer (
	id_customer int NOT NULL IDENTITY(1, 1),
	code nvarchar(10) NOT NULL,
	name nvarchar(100) NOT NULL,
	phone nvarchar(40) NULL,
	email nvarchar(255) NULL
)
GO
ALTER TABLE dbo.customer
	ADD CONSTRAINT pk_customer PRIMARY KEY (id_customer)
GO

ALTER TABLE dbo.customer
	ADD CONSTRAINT uc_customer_code1 UNIQUE (code)
GO

-- Table of QuickStart.Product
CREATE TABLE dbo.product (
	id_product int NOT NULL IDENTITY(1, 1),
	reference nvarchar(10) NOT NULL,
	name nvarchar(100) NOT NULL
)
GO
ALTER TABLE dbo.product
	ADD CONSTRAINT pk_product PRIMARY KEY (id_product)
GO

ALTER TABLE dbo.product
	ADD CONSTRAINT uc_product_reference1 UNIQUE (reference)
GO

-- Table of QuickStart.PurchaseOrder
CREATE TABLE dbo.purchase_order (
	id_purchase_order int NOT NULL IDENTITY(1, 1),
	number nvarchar(15) NOT NULL,
	issue_date date NOT NULL,
	id_customer int NOT NULL,
	validated bit DEFAULT 0 NOT NULL,
	shipment_date date NULL
)
GO
ALTER TABLE dbo.purchase_order
	ADD CONSTRAINT pk_purchase_order PRIMARY KEY (id_purchase_order)
GO

ALTER TABLE dbo.purchase_order
	ADD CONSTRAINT uc_purchase_order_number1 UNIQUE (number)
GO

-- Table of QuickStart.PurchaseOrderLine
CREATE TABLE dbo.purchase_order_line (
	id_purchase_order_line int NOT NULL IDENTITY(1, 1),
	id_purchase_order int NOT NULL,
	position smallint NOT NULL,
	id_product int NOT NULL,
	price numeric(18, 4) NOT NULL,
	quantity int NOT NULL
)
GO
ALTER TABLE dbo.purchase_order_line
	ADD CONSTRAINT pk_purchase_order_line PRIMARY KEY (id_purchase_order_line)
GO

ALTER TABLE dbo.purchase_order_line
	ADD CONSTRAINT uc_purchase_order_line_id_purchase_order_position1 UNIQUE (id_purchase_order,position)
GO

ALTER TABLE dbo.purchase_order
	ADD CONSTRAINT FK_purchase_order_customer_id_customer_21507 FOREIGN KEY
		(id_customer)
	REFERENCES dbo.customer
		(id_customer)
;
ALTER TABLE dbo.purchase_order_line
	ADD CONSTRAINT FK_purchase_order_line_purchase_order_id_purchase_order_33327 FOREIGN KEY
		(id_purchase_order)
	REFERENCES dbo.purchase_order
		(id_purchase_order)
;
ALTER TABLE dbo.purchase_order_line
	ADD CONSTRAINT FK_purchase_order_line_product_id_product_33282 FOREIGN KEY
		(id_product)
	REFERENCES dbo.product
		(id_product)
;
CREATE INDEX IX_purchase_order_id_customer_21507
	ON dbo.purchase_order (id_customer)
GO

CREATE INDEX IX_purchase_order_line_id_product_33282
	ON dbo.purchase_order_line (id_product)
GO


