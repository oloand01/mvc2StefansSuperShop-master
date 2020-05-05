USE [aspnet-StefanShopWeb-E3CED2D5-A95E-4B2D-A5F0-69A4F8C0D341]
GO

IF NOT EXISTS 
(
    SELECT * 
    FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE table_name = 'Products'
    AND column_name = 'FirstSalesDate'
)
BEGIN;
    ALTER TABLE Products 
    ADD FirstSalesDate datetime null;
END;
GO

UPDATE Products
SET FirstSalesDate = '2000-01-01'
WHERE FirstSalesDate is null
GO