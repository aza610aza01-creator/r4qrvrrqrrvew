USE plf_system;

CREATE TABLE IF NOT EXISTS Materials
(
    Id INT AUTO_INCREMENT PRIMARY KEY,
    MaterialName VARCHAR(150) NOT NULL,
    PhysicalQuantity INT NOT NULL DEFAULT 0,
    Unit VARCHAR(50) NOT NULL,
    SupplierId INT NULL,
    ProductId INT NULL,
    INDEX IX_Materials_SupplierId(SupplierId),
    INDEX IX_Materials_ProductId(ProductId),
    CONSTRAINT FK_Materials_Suppliers FOREIGN KEY(SupplierId) REFERENCES Suppliers(Id),
    CONSTRAINT FK_Materials_Products FOREIGN KEY(ProductId) REFERENCES Products(Id)
) ENGINE=InnoDB;

SET @supplier_col_exists := (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'Materials' AND COLUMN_NAME = 'SupplierId');
SET @supplier_col_sql := IF(@supplier_col_exists = 0, 'ALTER TABLE Materials ADD COLUMN SupplierId INT NULL AFTER Unit', 'SELECT ''SupplierId already exists''');
PREPARE stmt FROM @supplier_col_sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

SET @product_col_exists := (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'Materials' AND COLUMN_NAME = 'ProductId');
SET @product_col_sql := IF(@product_col_exists = 0, 'ALTER TABLE Materials ADD COLUMN ProductId INT NULL AFTER SupplierId', 'SELECT ''ProductId already exists''');
PREPARE stmt FROM @product_col_sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

UPDATE Materials SET SupplierId = 1, ProductId = 2 WHERE MaterialName LIKE '%Oak%' AND SupplierId IS NULL;
UPDATE Materials SET SupplierId = 2, ProductId = 2 WHERE MaterialName LIKE '%Steel%' AND SupplierId IS NULL;
UPDATE Materials SET SupplierId = 3, ProductId = 1 WHERE MaterialName LIKE '%Fabric%' AND SupplierId IS NULL;
UPDATE Materials SET SupplierId = 3, ProductId = 1 WHERE MaterialName LIKE '%Foam%' AND SupplierId IS NULL;

SELECT m.Id, m.MaterialName, m.PhysicalQuantity, m.Unit, s.SupplierName, p.ProductCode, p.ProductName
FROM Materials m
LEFT JOIN Suppliers s ON m.SupplierId = s.Id
LEFT JOIN Products p ON m.ProductId = p.Id
ORDER BY m.Id DESC;
