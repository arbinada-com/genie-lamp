-- Genie Lamp Core (1.1.4798.27721)
-- Genie of SqlServer database (1.0.4798.27721)
-- Starter application (1.1.4798.27722)
-- This file was automatically generated at 2013-03-14 16:56:45
-- Do not modify it manually.

IF NOT EXISTS(SELECT 1 FROM sys.databases WHERE name = N'QuickStartDB')
BEGIN
	CREATE DATABASE [QuickStartDB]
	;
END
GO

USE [QuickStartDB]
GO

-- Table of QuickStart.Customer
IF NOT EXISTS(SELECT 1 FROM sys.tables WHERE object_id = OBJECT_ID(N'dbo.customer'))
BEGIN
	CREATE TABLE dbo.customer (
		id_customer int NOT NULL IDENTITY(1, 1),
		code nvarchar(10) NOT NULL,
		name nvarchar(100) NOT NULL,
		phone nvarchar(40) NULL,
		email nvarchar(255) NULL
	)
END
GO
IF NOT EXISTS(SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.customer') AND name = 'id_customer')
BEGIN
	ALTER TABLE dbo.customer ADD 
		id_customer int NULL ;
END
GO
IF NOT EXISTS(SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.customer') AND name = 'code')
BEGIN
	ALTER TABLE dbo.customer ADD 
		code nvarchar(10) NULL ;
END
GO
IF NOT EXISTS(SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.customer') AND name = 'name')
BEGIN
	ALTER TABLE dbo.customer ADD 
		name nvarchar(100) NULL ;
END
GO
IF NOT EXISTS(SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.customer') AND name = 'phone')
BEGIN
	ALTER TABLE dbo.customer ADD 
		phone nvarchar(40) NULL ;
END
GO
IF NOT EXISTS(SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.customer') AND name = 'email')
BEGIN
	ALTER TABLE dbo.customer ADD 
		email nvarchar(255) NULL ;
END
GO

IF NOT EXISTS(SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'dbo.customer') AND is_primary_key = 1)
BEGIN
	ALTER TABLE dbo.customer
		ADD CONSTRAINT pk_customer PRIMARY KEY (id_customer)
END
GO

IF NOT EXISTS(SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'dbo.customer') AND is_unique_constraint = 1 AND is_primary_key = 0 AND name = N'uc_customer_code1')
BEGIN
	ALTER TABLE dbo.customer
		ADD CONSTRAINT uc_customer_code1 UNIQUE (code)
END
GO


-- Table of QuickStart.Product
IF NOT EXISTS(SELECT 1 FROM sys.tables WHERE object_id = OBJECT_ID(N'dbo.product'))
BEGIN
	CREATE TABLE dbo.product (
		id_product int NOT NULL IDENTITY(1, 1),
		reference nvarchar(10) NOT NULL,
		name nvarchar(100) NOT NULL
	)
END
GO
IF NOT EXISTS(SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.product') AND name = 'id_product')
BEGIN
	ALTER TABLE dbo.product ADD 
		id_product int NULL ;
END
GO
IF NOT EXISTS(SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.product') AND name = 'reference')
BEGIN
	ALTER TABLE dbo.product ADD 
		reference nvarchar(10) NULL ;
END
GO
IF NOT EXISTS(SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.product') AND name = 'name')
BEGIN
	ALTER TABLE dbo.product ADD 
		name nvarchar(100) NULL ;
END
GO

IF NOT EXISTS(SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'dbo.product') AND is_primary_key = 1)
BEGIN
	ALTER TABLE dbo.product
		ADD CONSTRAINT pk_product PRIMARY KEY (id_product)
END
GO

IF NOT EXISTS(SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'dbo.product') AND is_unique_constraint = 1 AND is_primary_key = 0 AND name = N'uc_product_reference1')
BEGIN
	ALTER TABLE dbo.product
		ADD CONSTRAINT uc_product_reference1 UNIQUE (reference)
END
GO


-- Table of QuickStart.PurchaseOrder
IF NOT EXISTS(SELECT 1 FROM sys.tables WHERE object_id = OBJECT_ID(N'dbo.purchase_order'))
BEGIN
	CREATE TABLE dbo.purchase_order (
		id_purchase_order int NOT NULL IDENTITY(1, 1),
		number nvarchar(15) NOT NULL,
		issue_date date NOT NULL,
		id_customer int NOT NULL,
		validated bit DEFAULT 0 NOT NULL,
		shipment_date date NULL
	)
END
GO
IF NOT EXISTS(SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.purchase_order') AND name = 'id_purchase_order')
BEGIN
	ALTER TABLE dbo.purchase_order ADD 
		id_purchase_order int NULL ;
END
GO
IF NOT EXISTS(SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.purchase_order') AND name = 'number')
BEGIN
	ALTER TABLE dbo.purchase_order ADD 
		number nvarchar(15) NULL ;
END
GO
IF NOT EXISTS(SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.purchase_order') AND name = 'issue_date')
BEGIN
	ALTER TABLE dbo.purchase_order ADD 
		issue_date date NULL ;
END
GO
IF NOT EXISTS(SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.purchase_order') AND name = 'id_customer')
BEGIN
	ALTER TABLE dbo.purchase_order ADD 
		id_customer int NULL ;
END
GO
IF NOT EXISTS(SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.purchase_order') AND name = 'validated')
BEGIN
	ALTER TABLE dbo.purchase_order ADD 
		validated bit NULL DEFAULT 0;
	EXEC sp_executesql N'UPDATE dbo.purchase_order SET validated = 0;';
	ALTER TABLE dbo.purchase_order ALTER COLUMN validated bit NOT NULL;
END
GO
IF NOT EXISTS(SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.purchase_order') AND name = 'shipment_date')
BEGIN
	ALTER TABLE dbo.purchase_order ADD 
		shipment_date date NULL ;
END
GO

IF NOT EXISTS(SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'dbo.purchase_order') AND is_primary_key = 1)
BEGIN
	ALTER TABLE dbo.purchase_order
		ADD CONSTRAINT pk_purchase_order PRIMARY KEY (id_purchase_order)
END
GO

IF NOT EXISTS(SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'dbo.purchase_order') AND is_unique_constraint = 1 AND is_primary_key = 0 AND name = N'uc_purchase_order_number1')
BEGIN
	ALTER TABLE dbo.purchase_order
		ADD CONSTRAINT uc_purchase_order_number1 UNIQUE (number)
END
GO


-- Table of QuickStart.PurchaseOrderLine
IF NOT EXISTS(SELECT 1 FROM sys.tables WHERE object_id = OBJECT_ID(N'dbo.purchase_order_line'))
BEGIN
	CREATE TABLE dbo.purchase_order_line (
		id_purchase_order_line int NOT NULL IDENTITY(1, 1),
		id_purchase_order int NOT NULL,
		position smallint NOT NULL,
		id_product int NOT NULL,
		price numeric(18, 4) NOT NULL,
		quantity int NOT NULL
	)
END
GO
IF NOT EXISTS(SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.purchase_order_line') AND name = 'id_purchase_order_line')
BEGIN
	ALTER TABLE dbo.purchase_order_line ADD 
		id_purchase_order_line int NULL ;
END
GO
IF NOT EXISTS(SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.purchase_order_line') AND name = 'id_purchase_order')
BEGIN
	ALTER TABLE dbo.purchase_order_line ADD 
		id_purchase_order int NULL ;
END
GO
IF NOT EXISTS(SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.purchase_order_line') AND name = 'position')
BEGIN
	ALTER TABLE dbo.purchase_order_line ADD 
		position smallint NULL ;
END
GO
IF NOT EXISTS(SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.purchase_order_line') AND name = 'id_product')
BEGIN
	ALTER TABLE dbo.purchase_order_line ADD 
		id_product int NULL ;
END
GO
IF NOT EXISTS(SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.purchase_order_line') AND name = 'price')
BEGIN
	ALTER TABLE dbo.purchase_order_line ADD 
		price numeric(18, 4) NULL ;
END
GO
IF NOT EXISTS(SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.purchase_order_line') AND name = 'quantity')
BEGIN
	ALTER TABLE dbo.purchase_order_line ADD 
		quantity int NULL ;
END
GO

IF NOT EXISTS(SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'dbo.purchase_order_line') AND is_primary_key = 1)
BEGIN
	ALTER TABLE dbo.purchase_order_line
		ADD CONSTRAINT pk_purchase_order_line PRIMARY KEY (id_purchase_order_line)
END
GO

IF NOT EXISTS(SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'dbo.purchase_order_line') AND is_unique_constraint = 1 AND is_primary_key = 0 AND name = N'uc_purchase_order_line_id_purchase_order_position1')
BEGIN
	ALTER TABLE dbo.purchase_order_line
		ADD CONSTRAINT uc_purchase_order_line_id_purchase_order_position1 UNIQUE (id_purchase_order,position)
END
GO


IF NOT EXISTS(SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_purchase_order_customer_id_customer_21507')
BEGIN
	ALTER TABLE dbo.purchase_order
		ADD CONSTRAINT FK_purchase_order_customer_id_customer_21507 FOREIGN KEY
			(id_customer)
		REFERENCES dbo.customer
			(id_customer)
END
GO
IF NOT EXISTS(SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_purchase_order_line_purchase_order_id_purchase_order_33327')
BEGIN
	ALTER TABLE dbo.purchase_order_line
		ADD CONSTRAINT FK_purchase_order_line_purchase_order_id_purchase_order_33327 FOREIGN KEY
			(id_purchase_order)
		REFERENCES dbo.purchase_order
			(id_purchase_order)
END
GO
IF NOT EXISTS(SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_purchase_order_line_product_id_product_33282')
BEGIN
	ALTER TABLE dbo.purchase_order_line
		ADD CONSTRAINT FK_purchase_order_line_product_id_product_33282 FOREIGN KEY
			(id_product)
		REFERENCES dbo.product
			(id_product)
END
GO
IF NOT EXISTS(SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'dbo.purchase_order') AND name = N'IX_purchase_order_id_customer_21507')
BEGIN
	CREATE INDEX IX_purchase_order_id_customer_21507
		ON dbo.purchase_order (id_customer)
END
GO

IF NOT EXISTS(SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'dbo.purchase_order_line') AND name = N'IX_purchase_order_line_id_product_33282')
BEGIN
	CREATE INDEX IX_purchase_order_line_id_product_33282
		ON dbo.purchase_order_line (id_product)
END
GO


