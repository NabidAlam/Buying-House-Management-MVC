--25 April,2018(Added by Tazbirul)
------------Starts Here-----------

---------------------ProdSize--------------------------------

--Remove Product Category TypeId
Alter table [dbo].[ProdSize] drop [FK1_ProdSize];
Alter table [dbo].[ProdSize] alter column [ProdCatTypeId] int null;
Alter table [dbo].[ProdSize] drop column [ProdCatTypeId];

--Add Department
Alter table [dbo].[ProdSize] add [ProdDepartmentId]  int;
UPDATE [dbo].[ProdSize] SET [ProdDepartmentId] = 1 WHERE [ProdDepartmentId] IS NULL;
Alter table [dbo].[ProdSize] ADD CONSTRAINT FK1_ProdSize FOREIGN KEY ([ProdDepartmentId]) REFERENCES [dbo].[ProdDepartment](Id);
------------------------------------------------------

---------------------ProdCategory---------------------
--Add Buyer
Alter table [dbo].[ProdCategory] add  [BuyerInfoId] int;
UPDATE [dbo].[ProdCategory] SET [BuyerInfoId] = 1 WHERE [BuyerInfoId] IS NULL;
Alter table [dbo].[ProdCategory] ADD CONSTRAINT FK1_ProdCategory FOREIGN KEY ([BuyerInfoId]) REFERENCES [dbo].[BuyerInfo](Id);
------------------------------------------------------

--------------------SeasonInfo------------------------
--Add Department
--Alter table [dbo].[SeasonInfo] add  [ProdDepartmentId] int;
--UPDATE [dbo].[SeasonInfo] SET [ProdDepartmentId] = 1 WHERE [ProdDepartmentId] IS NULL;
--Alter table [dbo].[SeasonInfo] ADD CONSTRAINT FK1_SeasonInfo FOREIGN KEY ([ProdDepartmentId]) REFERENCES [dbo].[ProdDepartment](Id);


-----------------------------------------------------

--------------------ProdColor------------------------
--Add Season
Alter table [dbo].[ProdColor] add  [SeasonInfoId] int;
UPDATE [dbo].[ProdColor] SET [SeasonInfoId] = 1 WHERE [SeasonInfoId] IS NULL;
Alter table [dbo].[ProdColor] ADD CONSTRAINT FK1_ProdColor FOREIGN KEY ([SeasonInfoId]) REFERENCES [dbo].[SeasonInfo](Id);

-----------------------------------------------------


--26 April,2018(Added by Tazbirul)

---------------------DestinationPort----------------
--Add Buyer
Alter table [dbo].[DestinationPort] add  [BuyerInfoId] int;
UPDATE [dbo].[DestinationPort] SET [BuyerInfoId] = 1 WHERE [BuyerInfoId] IS NULL;
Alter table [dbo].[DestinationPort] ADD CONSTRAINT FK1_DestinationPort FOREIGN KEY ([BuyerInfoId]) REFERENCES [dbo].[BuyerInfo](Id);
----------------------------------------------------



--5 May,2018(Added by Tazbirul)

--------------------SeasonInfo------------------------
Alter table [dbo].[SeasonInfo] drop  FK1_SeasonInfo
Alter table [dbo].[SeasonInfo] drop column [ProdDepartmentId]

Alter table [dbo].[SeasonInfo] add  [BuyerInfoId] int;
UPDATE [dbo].[SeasonInfo] SET [BuyerInfoId] = 1 WHERE [BuyerInfoId] IS NULL;
Alter table [dbo].[SeasonInfo] ADD CONSTRAINT FK1_SeasonInfo FOREIGN KEY ([BuyerInfoId]) REFERENCES [dbo].[BuyerInfo](Id);


--------------------ProdColor------------------------
--Add Department
Alter table [dbo].[ProdColor] add  [ProdDepartmentId] int;
UPDATE [dbo].[ProdColor] SET [ProdDepartmentId] = 1 WHERE [ProdDepartmentId] IS NULL;
Alter table [dbo].[ProdColor] ADD CONSTRAINT FK2_ProdColor FOREIGN KEY ([ProdDepartmentId]) REFERENCES [dbo].ProdDepartment(Id);

-----------------------------------------------------


--12 May,2018(Added by Tazbirul)
--------------------BuyerOrderDet------------------------
Alter Table [dbo].[BuyerOrderDet] alter column [SupplierId] int not null
-----------------------------------------------------

--------------------BuyerOrderMas------------------------
Alter Table [dbo].[BuyerOrderMas] alter column [BuyerInfoId] int not null
-----------------------------------------------------




---------------Modified Date 24/05/2018(Tazbirul)-------------
--==============InvoiceCommFactDet============================

alter table [dbo].[InvoiceCommFactDet] drop FK2_InvoiceCommFactDet
alter table [dbo].[InvoiceCommFactDet] drop UK_InvoiceCommFactDet
alter table [dbo].[InvoiceCommFactDet] drop column ExFactoryShipDetId
Alter Table [dbo].[InvoiceCommFactDet] add  ExFactoryDetId int not null
Alter Table [dbo].[InvoiceCommFactDet] add constraint FK2_InvoiceCommFactDet foreign key (ExFactoryDetId) references ExFactoryDet(Id)

--=============================================================

--=================DocSubmissionMas============================
ALTER TABLE DocSubmissionMas alter column FDBCNo nvarchar(50) null;
alter table DocSubmissionMas drop FK2_DocSubmissionMas
alter table DocSubmissionMas drop column InvoiceCommMasId;

alter table DocSubmissionMas add MasterLCInfoMasId int null;
Alter Table DocSubmissionMas add constraint FK2_DocSubmissionMas foreign key (MasterLCInfoMasId) references MasterLCInfoMas(Id)
alter table DocSubmissionMas alter column [CourierInfoId] int null;



---------------Modified Date 29/05/2018(Tazbirul)-----------
--==============DocSubmissionDet============================

Alter table DocSubmissionDet add BuyerOrderMasId int  null;
Alter Table DocSubmissionDet add constraint FK3_DocSubmissionDet foreign key (BuyerOrderMasId) references BuyerOrderMas(Id);
Alter table DocSubmissionDet alter column [InvoiceCommMasId] int null;




--==========================================================
--=======================Table Script=======================
---------------Modified Date 02/06/2018(Tazbirul)-----------


--=======================FDD Payment========================

CREATE TABLE FDDPayments(
  Id			int IDENTITY(1,1) NOT NULL,
  SupplierId	int NOT NULL,
  DocSubmissionFactDetId	int NOT NULL,
  FDDNo		nvarchar(200),
  FDDDate		Datetime,
  FDDAmount   	decimal(12,2) NOT NULL
  CONSTRAINT PK_FDDPayment PRIMARY KEY (Id),
  CONSTRAINT FK1_FDDPayment FOREIGN KEY (SupplierId) REFERENCES Supplier(Id),
  CONSTRAINT FK2_FDDPayment FOREIGN KEY (DocSubmissionFactDetId) REFERENCES DocSubmissionFactDet(Id)
)
	
GO


--=======================TT Payment========================

CREATE TABLE TTPayments(
  Id			int IDENTITY(1,1) NOT NULL,
  SupplierId	int NOT NULL,
  DocSubmissionDetId	int NOT NULL,
  FDDNo		nvarchar(200),
  FDDDate		Datetime,
  FDDAmount   	decimal(12,2) NOT NULL
  CONSTRAINT PK_TTPayment PRIMARY KEY (Id),
  CONSTRAINT FK1_TTPayment FOREIGN KEY (SupplierId) REFERENCES Supplier(Id),
  CONSTRAINT FK2_TTPayment FOREIGN KEY (DocSubmissionDetId) REFERENCES DocSubmissionDet(Id)
)
	
GO


CREATE TABLE ProceedRealizationMas(
 Id							 int  Not Null,
 PaymentTypeId               int,
 BuyerInfoId				 int  Not Null,
 ProceedDate                 Datetime,
 --MasterLCInfoMasId		     int  Not Null,
 DocSubmissionMasId          int  Not Null,
 CONSTRAINT PK_ProceedRealizationMas PRIMARY KEY (Id),
 CONSTRAINT FK1_ProceedRealizationMas FOREIGN KEY (BuyerInfoId) REFERENCES BuyerInfo(Id),
 --CONSTRAINT FK2_ProceedRealizationMas FOREIGN KEY (MasterLCInfoMasId) REFERENCES MasterLCInfoMas(Id),
 CONSTRAINT FK2_ProceedRealizationMas FOREIGN KEY (DocSubmissionMasId) REFERENCES DocSubmissionMas(Id)
)


CREATE TABLE ProceedRealizationDet(
 Id							 int  Not Null,
 ProceedRealizationMasId     int  Not Null,
 ProceedQty                  Numeric(12,2) Not Null,
 CONSTRAINT PK_ProceedRealizationDet PRIMARY KEY (Id),
 CONSTRAINT FK_ProceedRealizationDet FOREIGN KEY (ProceedRealizationMasId) REFERENCES ProceedRealizationMas(Id)
)



Alter Table [dbo].[ProceedRealizationDet] add DocSubmissionDetId  int not null
Alter Table [dbo].[ProceedRealizationDet] add constraint FK2_ProceedRealizationDet foreign key (DocSubmissionDetId) references DocSubmissionDet(Id)

--Alter table ProceedRealizationDet add DocSubmissionDetId int null;
--Alter Table ProceedRealizationDet add constraint FK2_ProceedRealizationDet foreign key (DocSubmissionDetId) references DocSubmissionDet(Id)

Alter Table TTPayments drop constraint FK2_TTPayment; 
Alter Table TTPayments drop column DocSubmissionDetId; 


Alter table TTPayments add ProceedRealizationDetId int not null;
Alter Table TTPayments add constraint FK2_TTPayments foreign key (ProceedRealizationDetId) references ProceedRealizationDet(Id)




--Modified by Tazbirul(10/6/2018)

Alter table [dbo].[FDDPayments] drop [FK2_FDDPayment]
Alter Table [dbo].[FDDPayments]  Drop Column [DocSubmissionFactDetId]

Alter Table [dbo].[FDDPayments] add  ProceedRealizationDetId int not null
Alter table [dbo].[FDDPayments] add constraint FK2_FDDPayment foreign key (ProceedRealizationDetId) references ProceedRealizationDet(Id)

Alter Table [dbo].[FDDPayments] add  DocSubmissionFactDetId int not null
Alter table [dbo].[FDDPayments] add constraint FK3_FDDPayment foreign key (DocSubmissionFactDetId) references DocSubmissionFactDet(Id)


--Modified by Tazbirul(12/6/2018)
Alter Table TTPayments add  DocSubmissionFactDetId int not null
Alter table TTPayments add constraint FK3_TTPayments foreign key (DocSubmissionFactDetId) references DocSubmissionFactDet(Id)



--Aded by Tazbirul(20/6/2018)
CREATE TABLE Brands(
 Id			int IDENTITY(1,1) NOT NULL,
 Name			  nvarchar(200) Not Null,
 BuyerInfoId      int  Not Null,
 CONSTRAINT PK_Brand PRIMARY KEY (Id),
 CONSTRAINT FK_Brand FOREIGN KEY (BuyerInfoId) REFERENCES BuyerInfo(Id)
)

Alter Table ProdDepartment drop [FK1_ProdDepartment]
Alter Table ProdDepartment drop [UK_ProdDepartment]
Alter Table ProdDepartment drop column [BuyerInfoId]
Alter Table ProdDepartment add  BrandId int not null
Alter table ProdDepartment add constraint FK1_ProdDepartment foreign key (BrandId) references Brands(Id)

Alter Table BuyerOrderMas add  BrandId int not null
Alter table BuyerOrderMas add constraint FK5_BuyerOrderMas foreign key (BrandId) references Brands(Id)


--Modified by Tazbirul(27/6/2018)
Alter Table DestinationPort Drop UK_DestinationPort

Alter Table ProdCategory Drop UK_ProdCategory

--Modified by Tazbirul(7/4/2018)
Alter Table SeasonInfo Drop UK_SeasonInfo

Alter Table ProdColor Drop [UK_ProdColor]




--==========================================================================
--==============================New Update==================================

--Added by Tazbirul(7/8/2018)
Alter table [dbo].[FactoryOrderDet] alter column [FOBUnitPrice] decimal(12,2) null;


--Add 
ALTER TABLE [dbo].[BuyerOrderMas] ADD FobType int null;
ALTER TABLE [dbo].[BuyerOrderMas] ADD DeliveryOn int null;

ALTER TABLE [dbo].[BuyerOrderDet] ADD FabricSupplierId int null;

ALTER TABLE [dbo].[BuyerOrderDet] ADD CONSTRAINT FK8_BuyerOrderDet FOREIGN KEY (FabricSupplierId)  REFERENCES FabricSupplier(Id);

ALTER TABLE [dbo].[ShipmentSummDet] ADD RdlFOB decimal(18, 2)  null;
ALTER TABLE [dbo].[ShipmentSummDet] ADD ShipmentMode int null;

CREATE TABLE FabricSupplier(
 Id 			int IDENTITY(1,1) NOT NULL,
 Name        	nvarchar(100)  Not Null,
 CONSTRAINT PK_FabricSupplier PRIMARY KEY (Id)
)


CREATE TABLE FabricType(
 Id 			int IDENTITY(1,1) NOT NULL,
 Name        	nvarchar(100)  Not Null,
 CONSTRAINT PK_Units PRIMARY KEY (Id)
 )
 
 ALTER TABLE [dbo].[FabricItem] ADD FabricTypeId int null;
 ALTER TABLE [dbo].[FabricItem] ADD CONSTRAINT FK1_FabricItem FOREIGN KEY (FabricTypeId)  REFERENCES FabricType(Id); 


 ALTER TABLE [dbo].[BuyerOrderDet] ADD RDLTotal decimal(18, 2)  null;



 --Added by Tazbirul(8/8/2018)

 CREATE TABLE FactoryOrderDelivDet(
 Id 			int IDENTITY(1,1) NOT NULL,
 FactoryOrderDetId   int  Not Null,
 ExFactoryDate   datetime null,
 FactFOB decimal(12,2) not null,
 CONSTRAINT PK_FactoryOrderDelivDet PRIMARY KEY (Id),
 CONSTRAINT FK1_FactoryOrderDelivDet FOREIGN KEY (FactoryOrderDetId)  REFERENCES FactoryOrderDet(Id)
 )

  ALTER TABLE FactoryOrderDelivDet ADD FactTransferPrice decimal(18, 2)  null;
  ALTER TABLE FactoryOrderDelivDet ALTER COLUMN FactFOB decimal(18, 2)  NULL;
  ALTER TABLE FactoryOrderDelivDet ADD ShipmentSummDetId int  null;



--Added by Nabid 16/08/18------------------------------------------------------------------------------------
  
 -- alter table FactoryOrderDelivDet
 -- to accomodate 'DiscountedFOB decimal(18,2) NULL'
 
 
CREATE TABLE DiscountMas(
 Id							int IDENTITY(1,1)  Not Null,
 BuyerOrderDetId		    int  Not Null,
 DiscountDate               Datetime,
 IsAuth 					bit NOT NULL, -- 1
 OpBy 						int NOT NULL, --1
 OpOn 						datetime NOT NULL, --datenow
 AuthBy 					int NULL,
 AuthOn 					datetime NULL,
 CONSTRAINT PK_DiscountMas PRIMARY KEY (Id),
 CONSTRAINT FK1_DiscountMas FOREIGN KEY (BuyerOrderDetId) REFERENCES BuyerOrderDet(Id)
)


CREATE TABLE DiscountDet(
 Id							 int IDENTITY(1,1)  Not Null,
 DiscountMasId               int  Not Null, 
 FactoryOrderDelivDetId		 int  Not Null, 
 BuyerDiscount               Numeric(5,2),
 AdjustBuyerNow              bit,
 FactoryDiscount             Numeric(5,2),
 AdjustFactoryNow            bit,  

 CONSTRAINT PK_DiscountDet PRIMARY KEY (Id),
 CONSTRAINT FK1_DiscountDet FOREIGN KEY (DiscountMasId) REFERENCES DiscountMas(Id), 
 CONSTRAINT FK2_DiscountDet FOREIGN KEY (FactoryOrderDelivDetId) REFERENCES FactoryOrderDelivDet(Id)

)


--CREATE TABLE DiscountAdjustmentFactoryMas(
-- Id							 int IDENTITY(1,1)  Not Null,
-- SupplierId		             int  Not Null,
-- BuyerInfoId		         int  Not Null,
-- DateAdj                   Datetime NOT NULL,
-- BuyerOrderMasPrevId	     int  Not Null,
-- BuyerOrderMasAdjId		     int  Not Null, 
-- IsAuth 					bit NOT NULL, -- 1
-- OpBy 						int NOT NULL, --1
-- OpOn 						datetime NOT NULL, --datenow
-- AuthBy 					int NULL,
-- AuthOn 					datetime NULL,
-- CONSTRAINT PK_DiscountAdjustmentFactoryMas PRIMARY KEY (Id),
-- CONSTRAINT FK1_DiscountAdjustmentFactoryMas FOREIGN KEY (SupplierId) REFERENCES Supplier(Id),
-- CONSTRAINT FK2_DiscountAdjustmentFactoryMas FOREIGN KEY (BuyerInfoId) REFERENCES BuyerInfo(Id),
-- CONSTRAINT FK3_DiscountAdjustmentFactoryMas FOREIGN KEY (BuyerOrderMasPrevId) REFERENCES BuyerOrderMas(Id),
-- CONSTRAINT FK4_DiscountAdjustmentFactoryMas FOREIGN KEY (BuyerOrderMasAdjId) REFERENCES BuyerOrderMas(Id)

--)


--CREATE TABLE DiscountAdjustmentFactoryDet(
-- Id							            int IDENTITY(1,1) Not Null,
-- DiscountAdjustmentFactoryMasId		    int  Not Null,
-- AdjustmentAmt		                    Numeric(18,2) Not Null, 
-- FactoryOrderDelivDetId		 			int  Not Null, 
-- CONSTRAINT PK_DiscountAdjustmentFactoryDet PRIMARY KEY (Id),
-- CONSTRAINT FK1_DiscountAdjustmentFactoryDet FOREIGN KEY (DiscountAdjustmentFactoryMasId) REFERENCES DiscountAdjustmentFactoryMas(Id),
-- CONSTRAINT FK2_DiscountAdjustmentFactoryDet FOREIGN KEY (FactoryOrderDelivDetId) REFERENCES FactoryOrderDelivDet(Id)
--)


--CREATE TABLE DiscountAdjustmentBuyerMas(
-- Id							 int IDENTITY(1,1) Not Null,
-- BuyerInfoId		         int  Not Null,
-- SupplierId		             int  Not Null,
-- InvoiceCommMasPrevId	     int  Not Null,
-- InvoiceCommMasAdjId        int  Not Null,
-- DateAdj                     Datetime,
-- IsAuth 					bit NOT NULL, -- 1
-- OpBy 						int NOT NULL, --1
-- OpOn 						datetime NOT NULL, --datenow
-- AuthBy 					int NULL,
-- AuthOn 					datetime NULL,
-- CONSTRAINT PK_DiscountAdjustmentBuyerMas PRIMARY KEY (Id),
-- CONSTRAINT FK1_DiscountAdjustmentBuyerMas FOREIGN KEY (BuyerInfoId) REFERENCES BuyerInfo(Id),
-- CONSTRAINT FK2_DiscountAdjustmentBuyerMas FOREIGN KEY (SupplierId) REFERENCES Supplier(Id),
-- CONSTRAINT FK3_DiscountAdjustmentBuyerMas FOREIGN KEY (InvoiceCommMasPrevId) REFERENCES InvoiceCommMas(Id),
-- CONSTRAINT FK4_DiscountAdjustmentBuyerMas FOREIGN KEY (InvoiceCommMasAdjId) REFERENCES InvoiceCommMas(Id)

--)

--CREATE TABLE DiscountAdjustmentBuyerDet(
-- Id							                     int IDENTITY(1,1) Not Null,
-- AdjustmentAmt		                             Numeric(18,2) Not Null,
-- DiscountAdjustmentBuyerMasId		             int  Not Null,
-- ExFactoryShipDetId                                int Not Null, 
-- CONSTRAINT PK_DiscountAdjustmentBuyerDet PRIMARY KEY (Id),
-- CONSTRAINT FK1_DiscountAdjustmentBuyerDet FOREIGN KEY (DiscountAdjustmentBuyerMasId) REFERENCES DiscountAdjustmentBuyerMas(Id),
-- CONSTRAINT FK2_DiscountAdjustmentBuyerDet FOREIGN KEY (ExFactoryShipDetId) REFERENCES ExFactoryShipDet(Id)
--)

CREATE TABLE DiscountAdjustmentFactoryMas(
 Id							 int IDENTITY(1,1)  Not Null,
 SupplierId		             int  Not Null,
 BuyerInfoId		         int  Not Null,
 DateAdj                    Datetime NOT NULL,
 IsAuth 					bit NOT NULL, -- 1
 OpBy 						int NOT NULL, --1
 OpOn 						datetime NOT NULL, --datenow
 AuthBy 					int NULL,
 AuthOn 					datetime NULL,
 CONSTRAINT PK_DiscountAdjustmentFactoryMas PRIMARY KEY (Id),
 CONSTRAINT FK1_DiscountAdjustmentFactoryMas FOREIGN KEY (SupplierId) REFERENCES Supplier(Id),
 CONSTRAINT FK2_DiscountAdjustmentFactoryMas FOREIGN KEY (BuyerInfoId) REFERENCES BuyerInfo(Id),
)


--CREATE TABLE DiscountAdjustmentFactoryPrev(
-- Id							 		int IDENTITY(1,1) Not Null,
-- DiscountAdjustmentFactoryMasId		int  Not Null, 
-- BuyerOrderDetId	     		int  Not Null, 
-- CONSTRAINT PK_DiscountAdjustmentFactoryPrev PRIMARY KEY (Id), 
-- CONSTRAINT FK1_DiscountAdjustmentFactoryPrev FOREIGN KEY (DiscountAdjustmentFactoryMasId) REFERENCES DiscountAdjustmentFactoryMas(Id),
-- CONSTRAINT FK2_DiscountAdjustmentFactoryPrev FOREIGN KEY (BuyerOrderDetId) REFERENCES BuyerOrderDet(Id)

--)

CREATE TABLE DiscountAdjustmentFactoryPrev(
 Id							 		int IDENTITY(1,1) Not Null,
 DiscountAdjustmentFactoryMasId		int  Not Null, 
 BuyerOrderMasId	     		int  Not Null, 
 CONSTRAINT PK_DiscountAdjustmentFactoryPrev PRIMARY KEY (Id), 
 CONSTRAINT FK1_DiscountAdjustmentFactoryPrev FOREIGN KEY (DiscountAdjustmentFactoryMasId) REFERENCES DiscountAdjustmentFactoryMas(Id),
 CONSTRAINT FK2_DiscountAdjustmentFactoryPrev FOREIGN KEY (BuyerOrderMasId) REFERENCES BuyerOrderMas(Id)

)

--CREATE TABLE DiscountAdjustmentFactoryAdj( 
-- Id							 		int IDENTITY(1,1) Not Null,
-- DiscountAdjustmentFactoryMasId		int  Not Null, 
-- BuyerOrderDetId        		int  Not Null, 
-- CONSTRAINT PK_DiscountAdjustmentFactoryAdj PRIMARY KEY (Id),
-- CONSTRAINT FK1_DiscountAdjustmentFactoryAdj FOREIGN KEY (DiscountAdjustmentFactoryMasId) REFERENCES DiscountAdjustmentFactoryMas(Id),
-- CONSTRAINT FK2_DiscountAdjustmentFactoryAdj FOREIGN KEY (BuyerOrderDetId) REFERENCES BuyerOrderDet(Id)
--)

CREATE TABLE DiscountAdjustmentFactoryAdj( 
 Id							 		int IDENTITY(1,1) Not Null,
 DiscountAdjustmentFactoryMasId		int  Not Null, 
 BuyerOrderMasId        		int  Not Null, 
 CONSTRAINT PK_DiscountAdjustmentFactoryAdj PRIMARY KEY (Id),
 CONSTRAINT FK1_DiscountAdjustmentFactoryAdj FOREIGN KEY (DiscountAdjustmentFactoryMasId) REFERENCES DiscountAdjustmentFactoryMas(Id),
 CONSTRAINT FK2_DiscountAdjustmentFactoryAdj FOREIGN KEY (BuyerOrderMasId) REFERENCES BuyerOrderMas(Id)
)


CREATE TABLE DiscountAdjustmentFactoryDet(
 Id							            int IDENTITY(1,1) Not Null,
 DiscountAdjustmentFactoryAdjId		    int  Not Null,
 AdjustmentAmt		                    Numeric(18,2) Not Null, 
 FactoryOrderDelivDetId		 			int  Not Null, 
 CONSTRAINT PK_DiscountAdjustmentFactoryDet PRIMARY KEY (Id),
 CONSTRAINT FK1_DiscountAdjustmentFactoryDet FOREIGN KEY (DiscountAdjustmentFactoryAdjId) REFERENCES DiscountAdjustmentFactoryAdj(Id),
 CONSTRAINT FK2_DiscountAdjustmentFactoryDet FOREIGN KEY (FactoryOrderDelivDetId) REFERENCES FactoryOrderDelivDet(Id)
)

--====================New Update(Discount)==========================

--==========================18 august,2018==========================
Alter Table ExFactoryMas drop UK_ExFactoryMas;

--==========================19 august,2018==========================
Alter Table FactoryOrderDelivDet Add DiscountFOB decimal(18,2) null;
Alter table FactoryOrderDelivDet Add Constraint FK2_FactoryOrderDelivDet Foreign key (ShipmentSummDetId) references ShipmentSummDet(Id);


--==========================26 august,2018==========================
--Alter Table FactoryOrderDelivDet Add DiscountFlag bit null;
--update FactoryOrderDelivDet set DiscountFlag=0;

--==========================29 august,2018==========================
Alter Table InvoiceCommDet Drop [FK2_InvoiceCommDet];
Alter Table InvoiceCommDet Drop [UK_InvoiceCommDet];
Alter Table InvoiceCommDet Add BuyerOrderMasId int null;
Alter table InvoiceCommDet add constraint FK3_InvoiceCommDet foreign key (BuyerOrderMasId) references BuyerOrderMas(Id)

--Alter Table InvoiceCommDet Add InvoiceCommFactMasId int null;
--Alter table InvoiceCommDet add constraint FK2_InvoiceCommDet foreign key (InvoiceCommFactMasId) references InvoiceCommFactMas(Id)
--Alter Table InvoiceCommDet Drop FK2_InvoiceCommDet;


--CREATE TABLE DiscountAdjustmentBuyerPrev(
-- Id							 		int IDENTITY(1,1) Not Null,
-- DiscountAdjustmentBuyerMasId		int  Not Null, 
-- BuyerOrderMasId  	     		    int  Not Null, 
-- CONSTRAINT PK_DiscountAdjustmentBuyerPrev PRIMARY KEY (Id), 
-- CONSTRAINT FK1_DiscountAdjustmentBuyerPrev FOREIGN KEY (DiscountAdjustmentBuyerMasId) REFERENCES DiscountAdjustmentBuyerMas(Id),
-- CONSTRAINT FK2_DiscountAdjustmentBuyerPrev FOREIGN KEY (BuyerOrderMasId) REFERENCES BuyerOrderMas(Id)

--)

--CREATE TABLE DiscountAdjustmentBuyerAdj( 
-- Id							 		int IDENTITY(1,1) Not Null,
-- DiscountAdjustmentBuyerMasId		int  Not Null, 
-- BuyerOrderMasId        		    int  Not Null, 
-- CONSTRAINT PK_DiscountAdjustmentBuyerAdj PRIMARY KEY (Id),
-- CONSTRAINT FK1_DiscountAdjustmentBuyerAdj FOREIGN KEY (DiscountAdjustmentBuyerMasId) REFERENCES DiscountAdjustmentBuyerMas(Id),
-- CONSTRAINT FK2_DiscountAdjustmentBuyerAdj FOREIGN KEY (BuyerOrderMasId) REFERENCES BuyerOrderMas(Id)
--)


CREATE TABLE FactoryCashAdjustment(
  Id				       int IDENTITY(1,1) NOT NULL,
  EntryDate                DateTime Not Null,
  SupplierId               int Not Null,
  FacAdjustDate            DateTime Not Null,
  FacReciptNo              nvarchar(100),
  FacAdjustAmount          decimal(18,2) Not Null,
  FacAdjustRemarks         nvarchar(500),
  IsAuth                   bit NOT NULL,
  OpBy                     int NOT NULL,
  OpOn                     datetime NOT NULL,
  AuthBy                   int NULL,
  AuthOn                   datetime     NULL,
  CONSTRAINT PK_FactoryCashAdjustment       PRIMARY KEY (Id),
  CONSTRAINT FK1_FactoryCashAdjustment      FOREIGN KEY (SupplierId)      REFERENCES Supplier(Id) 
       
)

Go

CREATE TABLE BuyerCashAdjustment(
  Id				       int IDENTITY(1,1) NOT NULL,
  EntryDate                DateTime Not Null,
  BuyerInfoId              int Not Null,
  BuyerAdjustDate          DateTime Not Null,
  BuyerReciptNo            nvarchar(100),
  BuyerAdjustAmount        decimal(18,2) Not Null,
  BuyerAdjustRemarks       nvarchar(500),
  IsAuth                   bit NOT NULL,
  OpBy                     int NOT NULL,
  OpOn                     datetime NOT NULL,
  AuthBy                   int NULL,
  AuthOn                   datetime     NULL,
  CONSTRAINT PK_BuyerCashAdjustment       PRIMARY KEY (Id),
  CONSTRAINT FK1_BuyerCashAdjustment      FOREIGN KEY (BuyerInfoId)      REFERENCES BuyerInfo(Id)        
)

Go

--==========================9 September,2018==========================
Alter Table Supplier Add OpeningBalance  decimal(18,2) null;
Alter Table Supplier Add BalanceDate  datetime null;

Alter Table BuyerInfo Add OpeningBalance  decimal(18,2) null;
Alter Table BuyerInfo Add BalanceDate  datetime null;

Alter Table FabricItem drop UK_FabricItem;


--9 Sept 2018
ALTER TABLE [dbo].[ShipmentSummDet] add BuyerSlNo nvarchar(50) null;




--drop table DiscountAdjustmentBuyerDet;
--drop table DiscountAdjustmentBuyerAdj;
--drop table DiscountAdjustmentBuyerPrev;
--drop table DiscountAdjustmentBuyerMas;


CREATE TABLE DiscountAdjustmentBuyerMas(
 Id							 int IDENTITY(1,1) Not Null,
 BuyerInfoId		         int  Not Null,
 SupplierId		             int  Not Null, 
 DateAdj                     Datetime,
 IsAuth 					bit NOT NULL, -- 1
 OpBy 						int NOT NULL, --1
 OpOn 						datetime NOT NULL, --datenow
 AuthBy 					int NULL,
 AuthOn 					datetime NULL,
 CONSTRAINT PK_DiscountAdjustmentBuyerMas PRIMARY KEY (Id),
 CONSTRAINT FK1_DiscountAdjustmentBuyerMas FOREIGN KEY (BuyerInfoId) REFERENCES BuyerInfo(Id),
 CONSTRAINT FK2_DiscountAdjustmentBuyerMas FOREIGN KEY (SupplierId) REFERENCES Supplier(Id)
)

CREATE TABLE DiscountAdjustmentBuyerPrev(
 Id							 		int IDENTITY(1,1) Not Null,
 DiscountAdjustmentBuyerMasId		int  Not Null, 
 BuyerOrderMasId  	     		    int  Not Null, 
 CONSTRAINT PK_DiscountAdjustmentBuyerPrev PRIMARY KEY (Id), 
 CONSTRAINT FK1_DiscountAdjustmentBuyerPrev FOREIGN KEY (DiscountAdjustmentBuyerMasId) REFERENCES DiscountAdjustmentBuyerMas(Id),
 CONSTRAINT FK2_DiscountAdjustmentBuyerPrev FOREIGN KEY (BuyerOrderMasId) REFERENCES BuyerOrderMas(Id)

)

CREATE TABLE DiscountAdjustmentBuyerAdj( 
 Id							 		int IDENTITY(1,1) Not Null,
 DiscountAdjustmentBuyerMasId		int  Not Null, 
 BuyerOrderMasId        		    int  Not Null, 
 CONSTRAINT PK_DiscountAdjustmentBuyerAdj PRIMARY KEY (Id),
 CONSTRAINT FK1_DiscountAdjustmentBuyerAdj FOREIGN KEY (DiscountAdjustmentBuyerMasId) REFERENCES DiscountAdjustmentBuyerMas(Id),
 CONSTRAINT FK2_DiscountAdjustmentBuyerAdj FOREIGN KEY (BuyerOrderMasId) REFERENCES BuyerOrderMas(Id)
)


CREATE TABLE DiscountAdjustmentBuyerDet(
 Id							                     int IDENTITY(1,1) Not Null,
 AdjustmentAmt		                             Numeric(18,2) Not Null,
 DiscountAdjustmentBuyerAdjId		             int  Not Null,
 ExFactoryShipDetId                                int Not Null 
 CONSTRAINT PK_DiscountAdjustmentBuyerDet PRIMARY KEY (Id),
 CONSTRAINT FK1_DiscountAdjustmentBuyerDet FOREIGN KEY (DiscountAdjustmentBuyerAdjId) REFERENCES DiscountAdjustmentBuyerAdj(Id),
 CONSTRAINT FK2_DiscountAdjustmentBuyerDet FOREIGN KEY (ExFactoryShipDetId) REFERENCES ExFactoryShipDet(Id)
)



--=========================================================
--added by(19/09/2018)

Alter Table InvoiceCommFactDet Add InvoiceTotalAmt decimal(18,2) null;

Alter Table InvoiceCommDet Add InvoiceRDLTotalAmt decimal(18,2) null;


--==========================22 sep,2018==========================
--Alter Table DiscountDet Add DiscountFlag bit null;
--update DiscountDet set DiscountFlag=0;
--Alter Table DiscountDet drop column DiscountFlag;

Alter Table DiscountMas Add DiscountFlag bit null;
update DiscountMas set DiscountFlag=0;

--==========================26 sep,2018==========================
Alter table ExFactoryShipDet add ShipmentMode int;