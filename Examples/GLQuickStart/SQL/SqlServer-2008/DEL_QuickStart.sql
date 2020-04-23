-- Genie Lamp Core (1.1.4798.27721)
-- Genie of SqlServer database (1.0.4798.27721)
-- Starter application (1.1.4798.27722)
-- This file was automatically generated at 2013-03-14 16:56:45
-- Do not modify it manually.

ALTER TABLE dbo.purchase_order DROP CONSTRAINT FK_purchase_order_customer_id_customer_21507
GO
ALTER TABLE dbo.purchase_order_line DROP CONSTRAINT FK_purchase_order_line_purchase_order_id_purchase_order_33327
GO
ALTER TABLE dbo.purchase_order_line DROP CONSTRAINT FK_purchase_order_line_product_id_product_33282
GO

DROP TABLE dbo.customer
GO
DROP TABLE dbo.product
GO
DROP TABLE dbo.purchase_order
GO
DROP TABLE dbo.purchase_order_line
GO

