CREATE TABLE InvoiceCommDetDet(
  Id			int IDENTITY(1,1) NOT NULL,
  InvoiceCommDetId	int NOT NULL,
  ExFactoryShipDetId	int NOT NULL,
  ShipQty	int NOT NULL,
  CONSTRAINT PK_InvoiceCommDetDet PRIMARY KEY (Id),
  CONSTRAINT FK1_InvoiceCommDetDet FOREIGN KEY (InvoiceCommDetId) REFERENCES InvoiceCommDet(Id),
  CONSTRAINT FK2_InvoiceCommDetDet FOREIGN KEY (ExFactoryShipDetId) REFERENCES ExFactoryShipDet(Id)
);
	
GO

Alter Table InvoiceCommMas add  SplitFlag int null;
Alter Table InvoiceCommDetDet add  DiscountFlag bit null;

--update InvoiceCommDetDet set  DiscountFlag = 0

Alter Table InvoiceCommMas add  SplitFlag int null;


--17/10/2018
Alter Table FDDPayments drop FK3_FDDPayment;
Alter Table FDDPayments drop column DocSubmissionFactDetId;
Alter Table FDDPayments add DocSubmissionFactNO nvarchar(100);


CREATE TABLE FDDPaymentDet(
  Id			int IDENTITY(1,1) NOT NULL,
  FDDPaymentId	int NOT NULL,
  DocSubmissionFactDetId	int NOT NULL,
  CONSTRAINT PK_FDDPaymentDet PRIMARY KEY (Id),
  CONSTRAINT FK1_FDDPaymentDet FOREIGN KEY ( FDDPaymentId) REFERENCES  FDDPayments(Id),
  CONSTRAINT FK2_FDDPaymentDet FOREIGN KEY ( DocSubmissionFactDetId) REFERENCES  DocSubmissionFactDet(Id)
);
	
GO
