# Premium Living Furniture WinForms ERP - Customer Name Added

## Update in this ZIP

- Sales Orders page now shows `CustomerName`, so staff can see who bought the order.
- Sales Order Items page now shows `CustomerName` beside `SalesOrderId` and `OrderNo`.
- Search on Sales Orders and Sales Order Items now supports customer name.
- Previous fixes are retained:
  - Delivery page supports FromAddress, ToAddress, RouteNotes.
  - Delivery print uses one delivery note per page to avoid overlap.
  - Sales Order Items shows shared SalesOrderId for multi-product orders.
  - Signature images are included under Signatures/.

## Run

1. Extract the ZIP to a fresh folder.
2. Open `PremiumLivingFurnitureWinForms.csproj` in Visual Studio.
3. Restore NuGet packages.
4. Clean Solution.
5. Rebuild Solution.
6. Start.
