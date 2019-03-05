CREATE TABLE UserDept(
Id				               int IDENTITY(1,1) NOT NULL,
DeptName			           nvarchar(50) NOT NULL,
CONSTRAINT PK_UserDept    PRIMARY KEY (Id),
)

GO

CREATE TABLE TimeActionMas(
Id				               int IDENTITY(1,1) NOT NULL,
TemplateName			       nvarchar(50) NOT NULL,
BuyerInfoId				       int NOT NULL,
UserDeptId                       int NOT NULL,
CompanyResourceId              int NOT NULL,
CONSTRAINT PK_TimeActionMas    PRIMARY KEY (Id),
CONSTRAINT FK1_TimeActionMas   FOREIGN KEY (BuyerInfoId) REFERENCES BuyerInfo(Id),
CONSTRAINT FK2_TimeActionMas   FOREIGN KEY (UserDeptId) REFERENCES UserDept(Id),
CONSTRAINT FK3_TimeActionMas   FOREIGN KEY (CompanyResourceId) REFERENCES CompanyResources(Id)
)

GO


CREATE TABLE TimeActionDet(
Id				                int IDENTITY(1,1) NOT NULL,
TimeActionMasId			        int NOT NULL,
ActivityName		            nvarchar(100) NOT NULL,
ActivityDays			        int,
Source                          int,
CONSTRAINT PK_TimeActionDet     PRIMARY KEY (Id),
CONSTRAINT FK1_TimeActionDet    FOREIGN KEY (TimeActionMasId) REFERENCES TimeActionMas(Id)
)

GO


CREATE TABLE ActionActivityMas(
Id				          		 int IDENTITY(1,1) NOT NULL,
FactoryOrderDelivDetId		      		 int NOT NULL,
TimeActionMasId			         int NOT NULL,
PlanFlag					     bit NOT NULL,
RevisedFlag						 bit NOT NULL,
CONSTRAINT PK_ActionActivityMas  PRIMARY KEY (Id),
CONSTRAINT FK1_ActionActivityMas FOREIGN KEY (FactoryOrderDelivDetId) REFERENCES FactoryOrderDelivDet(Id),
CONSTRAINT FK2_ActionActivityMas FOREIGN KEY (TimeActionMasId) REFERENCES TimeActionMas(Id)
)
GO


CREATE TABLE ActionActivityDet(
Id				              	 int IDENTITY(1,1) NOT NULL,
ActionActivityMasId		      	 int NOT NULL,
TimeActionDetId				  	 int NOT NULL,
PlanDate                      	 DateTime NOT NULL,
RevisedDate                    	 DateTime,
ActualDate                    	 DateTime,
Remarks                       	 nvarchar(100),
CONSTRAINT PK_ActionActivityDet  PRIMARY KEY (Id),
CONSTRAINT FK1_ActionActivityDet FOREIGN KEY (ActionActivityMasId) REFERENCES ActionActivityMas(Id),
CONSTRAINT FK2_ActionActivityDet FOREIGN KEY (TimeActionDetId) REFERENCES TimeActionDet(Id)
)

GO
