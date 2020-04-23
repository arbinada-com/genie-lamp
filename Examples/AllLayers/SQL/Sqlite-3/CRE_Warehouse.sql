-- Genie Lamp Core (1.1.4594.29523)
-- Genie of Sqlite database (1.0.4594.29525)
-- Starter application (1.1.4594.29524)
-- This file was automatically generated at 2012-07-30 16:36:48
-- Do not modify it manually.


/*
Use following command to create database
sqlite3 "D:\Projects\GenieLamp\Sources\Tests\AllLayers\SQL\Sqlite-3\Warehouse.db" < "D:\Projects\GenieLamp\Sources\Tests\AllLayers\SQL\Sqlite-3\CRE_Warehouse.sql"
*/

-- PRAGMA foreign_keys = ON;

-- Table of Core.EntityType
CREATE TABLE IF NOT EXISTS entity_type (
	id_entity_type INTEGER NOT NULL CONSTRAINT pk_entity_type PRIMARY KEY AUTOINCREMENT,
	full_name NVARCHAR(255) NOT NULL,
	short_name NVARCHAR(64) NOT NULL,
	description NVARCHAR(1000) NULL
	, CONSTRAINT uc_entity_type_full_name UNIQUE (full_name)
)
;
-- Table of Core.EntityRegistry
CREATE TABLE IF NOT EXISTS entity_registry (
	id_entity_registry INTEGER NOT NULL CONSTRAINT pk_entity_registry PRIMARY KEY AUTOINCREMENT,
	id_entity_type INTEGER NULL REFERENCES entity_type(id_entity_type)
	-- , FOREIGN KEY FK1_entity_registry_id_entity_ REFERENCES entity_type(id_entity_type)
)
;
-- Table of Warehouse.ExampleOneToOne
CREATE TABLE IF NOT EXISTS example_one_to_one (
	id_example_one_to_one INTEGER NOT NULL CONSTRAINT pk_example_one_to_one PRIMARY KEY AUTOINCREMENT,
	name NVARCHAR(64) NOT NULL,
	version INTEGER NOT NULL,
	id_entity_registry INTEGER NULL REFERENCES entity_registry(id_entity_registry),
	created_by NVARCHAR(64) NULL,
	created_date DATETIME NULL,
	last_modified_by NVARCHAR(64) NULL,
	last_modified_date DATETIME NULL
	, CONSTRAINT uc_example_one_to_one_name UNIQUE (name)
	, CONSTRAINT uc_example_one_to_one_entity_r UNIQUE (id_entity_registry)
	-- , FOREIGN KEY FK1_example_one_to_one_entity_ REFERENCES entity_registry(id_entity_registry)
)
;
-- Table of Warehouse.ExampleOneToOneEx
CREATE TABLE IF NOT EXISTS example_one_to_one_ex (
	id_example_one_to_one_ex INTEGER NOT NULL CONSTRAINT pk_example_one_to_one_ex PRIMARY KEY AUTOINCREMENT,
	id_example_one_to_one INTEGER NOT NULL REFERENCES example_one_to_one(id_example_one_to_one),
	caption NVARCHAR(100) NULL,
	id_entity_registry INTEGER NULL REFERENCES entity_registry(id_entity_registry),
	version INTEGER NOT NULL,
	created_by NVARCHAR(64) NULL,
	created_date DATETIME NULL,
	last_modified_by NVARCHAR(64) NULL,
	last_modified_date DATETIME NULL
	, CONSTRAINT uc_example_one_to_one_ex_id_ex UNIQUE (id_example_one_to_one)
	, CONSTRAINT uc_example_one_to_one_ex_entit UNIQUE (id_entity_registry)
	-- , FOREIGN KEY FK1_example_one_to_one_ex_id_e REFERENCES example_one_to_one(id_example_one_to_one)
	-- , FOREIGN KEY FK2_example_one_to_one_ex_enti REFERENCES entity_registry(id_entity_registry)
)
;
-- Table of Warehouse.ProductType
CREATE TABLE IF NOT EXISTS product_type (
	id_product_type INTEGER NOT NULL CONSTRAINT pk_product_type PRIMARY KEY AUTOINCREMENT,
	code NVARCHAR(3) NOT NULL,
	name NVARCHAR(100) NULL,
	id_parent INTEGER NULL REFERENCES product_type(id_parent),
	id_entity_registry INTEGER NULL REFERENCES entity_registry(id_entity_registry),
	version INTEGER NOT NULL,
	created_by NVARCHAR(64) NULL,
	created_date DATETIME NULL,
	last_modified_by NVARCHAR(64) NULL,
	last_modified_date DATETIME NULL
	, CONSTRAINT uc_product_type_code UNIQUE (code)
	, CONSTRAINT uc_product_type_entity_registr UNIQUE (id_entity_registry)
	-- , FOREIGN KEY FK1_product_type_id_parent REFERENCES product_type(id_parent)
	-- , FOREIGN KEY FK2_product_type_entity_regist REFERENCES entity_registry(id_entity_registry)
)
;
-- Table of Warehouse.Product
CREATE TABLE IF NOT EXISTS product (
	id_product INTEGER NOT NULL CONSTRAINT pk_product PRIMARY KEY AUTOINCREMENT,
	ref_code NVARCHAR(10) NOT NULL,
	caption NVARCHAR(100) NULL,
	id_product_type INTEGER NULL REFERENCES product_type(id_product_type),
	id_entity_registry INTEGER NULL REFERENCES entity_registry(id_entity_registry),
	version INTEGER NOT NULL,
	created_by NVARCHAR(64) NULL,
	created_date DATETIME NULL,
	last_modified_by NVARCHAR(64) NULL,
	last_modified_date DATETIME NULL
	, CONSTRAINT uc_product_ref_code UNIQUE (ref_code)
	, CONSTRAINT uc_product_entity_registry_id UNIQUE (id_entity_registry)
	-- , FOREIGN KEY FK1_product_id_product_type REFERENCES product_type(id_product_type)
	-- , FOREIGN KEY FK2_product_entity_registry_id REFERENCES entity_registry(id_entity_registry)
)
;
-- Table of Warehouse.StoreType
CREATE TABLE IF NOT EXISTS store_type (
	id_store_type INTEGER NOT NULL CONSTRAINT pk_store_type PRIMARY KEY AUTOINCREMENT,
	name NVARCHAR(3) NOT NULL,
	caption NVARCHAR(100) NULL,
	id_entity_registry INTEGER NULL REFERENCES entity_registry(id_entity_registry),
	version INTEGER NOT NULL,
	created_by NVARCHAR(64) NULL,
	created_date DATETIME NULL,
	last_modified_by NVARCHAR(64) NULL,
	last_modified_date DATETIME NULL
	, CONSTRAINT uc_store_type_name UNIQUE (name)
	, CONSTRAINT uc_store_type_entity_registry_ UNIQUE (id_entity_registry)
	-- , FOREIGN KEY FK1_store_type_entity_registry REFERENCES entity_registry(id_entity_registry)
)
;
-- Table of Warehouse.Store
CREATE TABLE IF NOT EXISTS store (
	id_store INTEGER NOT NULL CONSTRAINT pk_store PRIMARY KEY AUTOINCREMENT,
	code NVARCHAR(15) NOT NULL,
	caption NVARCHAR(100) NULL,
	id_store_type INTEGER NULL REFERENCES store_type(id_store_type),
	id_entity_registry INTEGER NULL REFERENCES entity_registry(id_entity_registry),
	version INTEGER NOT NULL,
	created_by NVARCHAR(64) NULL,
	created_date DATETIME NULL,
	last_modified_by NVARCHAR(64) NULL,
	last_modified_date DATETIME NULL
	, CONSTRAINT uc_store_code UNIQUE (code)
	, CONSTRAINT uc_store_entity_registry_id UNIQUE (id_entity_registry)
	-- , FOREIGN KEY FK1_store_id_store_type REFERENCES store_type(id_store_type)
	-- , FOREIGN KEY FK2_store_entity_registry_id REFERENCES entity_registry(id_entity_registry)
)
;
-- Table of Warehouse.Contractor
CREATE TABLE IF NOT EXISTS contractor (
	id_contractor INTEGER NOT NULL CONSTRAINT pk_contractor PRIMARY KEY AUTOINCREMENT,
	name NVARCHAR(100) NULL,
	address NVARCHAR(255) NULL,
	phone NVARCHAR(20) NULL,
	email NVARCHAR(80) NULL,
	id_entity_registry INTEGER NULL REFERENCES entity_registry(id_entity_registry),
	version INTEGER NOT NULL,
	created_by NVARCHAR(64) NULL,
	created_date DATETIME NULL,
	last_modified_by NVARCHAR(64) NULL,
	last_modified_date DATETIME NULL
	, CONSTRAINT uc_contractor_name UNIQUE (name)
	, CONSTRAINT uc_contractor_phone UNIQUE (phone)
	, CONSTRAINT uc_contractor_entity_registry_ UNIQUE (id_entity_registry)
	-- , FOREIGN KEY FK1_contractor_entity_registry REFERENCES entity_registry(id_entity_registry)
)
;
-- Table of Warehouse.Partner
CREATE TABLE IF NOT EXISTS partner (
	id_partner INTEGER NOT NULL REFERENCES contractor(id_partner),
	since DATETIME NOT NULL
	, CONSTRAINT pk_partner PRIMARY KEY (id_partner)
	-- , FOREIGN KEY FK1_partner_id_partner REFERENCES contractor(id_partner)
)
;
-- Table of Warehouse.StoreDoc
CREATE TABLE IF NOT EXISTS store_doc (
	id_store_doc INTEGER NOT NULL CONSTRAINT pk_store_doc PRIMARY KEY AUTOINCREMENT,
	ref_num NVARCHAR(16) NOT NULL,
	created DATETIME DEFAULT current_timestamp NOT NULL,
	name NVARCHAR(100) NULL,
	id_entity_registry INTEGER NULL REFERENCES entity_registry(id_entity_registry),
	version INTEGER NOT NULL,
	created_by NVARCHAR(64) NULL,
	created_date DATETIME NULL,
	last_modified_by NVARCHAR(64) NULL,
	last_modified_date DATETIME NULL
	, CONSTRAINT uc_store_doc_ref_num UNIQUE (ref_num)
	, CONSTRAINT uc_store_doc_entity_registry_i UNIQUE (id_entity_registry)
	-- , FOREIGN KEY FK1_store_doc_entity_registry_ REFERENCES entity_registry(id_entity_registry)
)
;
-- Table of Warehouse.StoreDocLine
CREATE TABLE IF NOT EXISTS store_doc_line (
	id_store_doc INTEGER NOT NULL REFERENCES store_doc(id_store_doc),
	position INTEGER NOT NULL,
	quantity INTEGER NOT NULL,
	id_product INTEGER NOT NULL REFERENCES product(id_product),
	id_entity_registry INTEGER NULL REFERENCES entity_registry(id_entity_registry),
	version INTEGER NOT NULL,
	created_by NVARCHAR(64) NULL,
	created_date DATETIME NULL,
	last_modified_by NVARCHAR(64) NULL,
	last_modified_date DATETIME NULL
	, CONSTRAINT pk_store_doc_line PRIMARY KEY (id_store_doc, position)
	, CONSTRAINT uc_store_doc_line_entity_regis UNIQUE (id_entity_registry)
	-- , FOREIGN KEY FK1_store_doc_line_id_store_do REFERENCES store_doc(id_store_doc)
	-- , FOREIGN KEY FK2_store_doc_line_id_product REFERENCES product(id_product)
	-- , FOREIGN KEY FK3_store_doc_line_entity_regi REFERENCES entity_registry(id_entity_registry)
)
;
-- Table of Warehouse.StoreTransaction
CREATE TABLE IF NOT EXISTS store_transaction (
	id_store_transaction INTEGER NOT NULL CONSTRAINT pk_store_transaction PRIMARY KEY AUTOINCREMENT,
	tx_date DATETIME DEFAULT current_timestamp NOT NULL,
	direction TINYINT NOT NULL,
	state TINYINT NOT NULL,
	id_supplier INTEGER NOT NULL REFERENCES contractor(id_supplier),
	quantity INTEGER NOT NULL,
	id_store INTEGER NOT NULL REFERENCES store(id_store),
	id_product INTEGER NOT NULL REFERENCES product(id_product),
	id_customer INTEGER NOT NULL REFERENCES contractor(id_customer),
	id_entity_registry INTEGER NULL REFERENCES entity_registry(id_entity_registry),
	version INTEGER NOT NULL,
	created_by NVARCHAR(64) NULL,
	created_date DATETIME NULL,
	last_modified_by NVARCHAR(64) NULL,
	last_modified_date DATETIME NULL
	, CONSTRAINT uc_store_transaction_entity_re UNIQUE (id_entity_registry)
	-- , FOREIGN KEY FK1_store_transaction_id_suppl REFERENCES contractor(id_supplier)
	-- , FOREIGN KEY FK2_store_transaction_id_store REFERENCES store(id_store)
	-- , FOREIGN KEY FK3_store_transaction_id_produ REFERENCES product(id_product)
	-- , FOREIGN KEY FK4_store_transaction_id_custo REFERENCES contractor(id_customer)
	-- , FOREIGN KEY FK5_store_transaction_entity_r REFERENCES entity_registry(id_entity_registry)
)
;

