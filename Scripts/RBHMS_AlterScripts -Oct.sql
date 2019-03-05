--2 October,2018(Added by Tazbirul)
------------Starts Here-----------

--Added by Tazbirul(2/10/2018)
Alter table [dbo].[FactoryOrderDelivDet] Add  Remarks nvarchar(50) null;

Alter table [dbo].[CommissionDistDet] Add  CheckFlag bit null;
Update [dbo].[CommissionDistDet] set CheckFlag = 1

--Added by Nabid 
Alter table FabricType add  ProdCategoryId int;
Alter table FabricType ADD CONSTRAINT FK1_FabricType FOREIGN KEY (ProdCategoryId) REFERENCES [dbo].[ProdCategory](Id);
UPDATE FabricType SET [ProdCategoryId] = 1;  