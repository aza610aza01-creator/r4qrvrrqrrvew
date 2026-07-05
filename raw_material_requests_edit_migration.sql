USE plf_system;

CREATE TABLE IF NOT EXISTS RawMaterialRequests
(
    Id INT AUTO_INCREMENT PRIMARY KEY,
    RequestNo VARCHAR(50) UNIQUE,
    StockId INT NULL,
    RequestedByUserId INT NULL,
    Quantity INT NOT NULL DEFAULT 1,
    RequiredDate DATE NULL,
    Priority VARCHAR(50),
    Status VARCHAR(50),
    Purpose VARCHAR(255),
    CreatedDate DATE,
    INDEX IX_RawMaterialRequests_StockId(StockId),
    INDEX IX_RawMaterialRequests_UserId(RequestedByUserId)
) ENGINE=InnoDB;

SELECT r.Id, r.RequestNo, st.ItemCode, st.ItemName, u.Username AS RequestedBy,
       r.Quantity, r.RequiredDate, r.Priority, r.Status, r.Purpose, r.CreatedDate
FROM RawMaterialRequests r
LEFT JOIN Stock st ON r.StockId = st.Id
LEFT JOIN Users u ON r.RequestedByUserId = u.Id
ORDER BY r.Id DESC;
