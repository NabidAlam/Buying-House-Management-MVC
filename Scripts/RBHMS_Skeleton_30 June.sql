USE [master]
GO
/****** Object:  Database [RBHMS]    Script Date: 6/30/2018 11:41:33 AM ******/
CREATE DATABASE [RBHMS]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'RBHMS', FILENAME = N'C:\Program Files (x86)\Microsoft SQL Server\MSSQL12.SQLEXPRESS\MSSQL\DATA\RBHMS.mdf' , SIZE = 4288KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'RBHMS_log', FILENAME = N'C:\Program Files (x86)\Microsoft SQL Server\MSSQL12.SQLEXPRESS\MSSQL\DATA\RBHMS_log.ldf' , SIZE = 832KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO
ALTER DATABASE [RBHMS] SET COMPATIBILITY_LEVEL = 120
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [RBHMS].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [RBHMS] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [RBHMS] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [RBHMS] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [RBHMS] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [RBHMS] SET ARITHABORT OFF 
GO
ALTER DATABASE [RBHMS] SET AUTO_CLOSE ON 
GO
ALTER DATABASE [RBHMS] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [RBHMS] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [RBHMS] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [RBHMS] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [RBHMS] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [RBHMS] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [RBHMS] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [RBHMS] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [RBHMS] SET  DISABLE_BROKER 
GO
ALTER DATABASE [RBHMS] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [RBHMS] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [RBHMS] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [RBHMS] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [RBHMS] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [RBHMS] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [RBHMS] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [RBHMS] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [RBHMS] SET  MULTI_USER 
GO
ALTER DATABASE [RBHMS] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [RBHMS] SET DB_CHAINING OFF 
GO
ALTER DATABASE [RBHMS] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [RBHMS] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO
ALTER DATABASE [RBHMS] SET DELAYED_DURABILITY = DISABLED 
GO
USE [RBHMS]
GO
/****** Object:  Table [dbo].[AIT]    Script Date: 6/30/2018 11:41:33 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AIT](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[AITPercent] [decimal](12, 2) NOT NULL,
 CONSTRAINT [PK_AIT] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[BankBranch]    Script Date: 6/30/2018 11:41:33 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BankBranch](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[BranchName] [nvarchar](50) NULL,
	[Phone] [nvarchar](50) NULL,
	[ContactPerson] [nvarchar](50) NULL,
	[SwiftCode] [nvarchar](50) NULL,
	[IsForeign] [bit] NOT NULL,
	[IsAuth] [bit] NOT NULL,
	[OpBy] [int] NOT NULL,
	[OpOn] [datetime] NOT NULL,
	[AuthBy] [int] NULL,
	[AuthOn] [datetime] NULL,
 CONSTRAINT [PK_BankBranch] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[BankCharge]    Script Date: 6/30/2018 11:41:33 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BankCharge](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PaymentTypeId] [int] NULL,
	[Charge] [decimal](12, 2) NOT NULL,
 CONSTRAINT [PK_BankCharge] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Brands]    Script Date: 6/30/2018 11:41:33 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Brands](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](200) NOT NULL,
	[BuyerInfoId] [int] NOT NULL,
 CONSTRAINT [PK_Brand] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[BuyerInfo]    Script Date: 6/30/2018 11:41:33 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BuyerInfo](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[NameShort] [nvarchar](20) NULL,
	[Address] [nvarchar](256) NULL,
	[ContactPerson] [nvarchar](50) NULL,
	[ContactPhone] [nvarchar](50) NULL,
	[CountryInfoId] [int] NULL,
	[MerchandiserId] [int] NULL,
	[MiddlePartyId] [int] NULL,
	[BankBranchId] [int] NULL,
	[PaymentTypeId] [int] NULL,
	[Tenor] [int] NULL,
	[PaymentTerm] [int] NULL,
	[BuyerGroup] [nvarchar](20) NULL,
	[IsAuth] [bit] NOT NULL,
	[OpBy] [int] NOT NULL,
	[OpOn] [datetime] NOT NULL,
	[AuthBy] [int] NULL,
	[AuthOn] [datetime] NULL,
 CONSTRAINT [PK_BuyerInfo] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UK_BuyerInfo] UNIQUE NONCLUSTERED 
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[BuyerOrderDet]    Script Date: 6/30/2018 11:41:33 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BuyerOrderDet](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[BuyerOrderMasId] [int] NOT NULL,
	[ProdCatTypeId] [int] NULL,
	[StyleNo] [nvarchar](50) NULL,
	[ProdSizeId] [int] NULL,
	[FabricItemId] [int] NULL,
	[ProdColorId] [int] NULL,
	[Quantity] [int] NULL,
	[UnitPrice] [decimal](12, 2) NULL,
	[SupplierId] [int] NOT NULL,
	[PictureFilename] [nvarchar](256) NULL,
	[IsLocked] [bit] NOT NULL,
	[ExFactoryDate] [date] NULL,
	[IsShipClosed] [bit] NULL,
 CONSTRAINT [PK_BuyerOrderDet] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[BuyerOrderMas]    Script Date: 6/30/2018 11:41:33 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BuyerOrderMas](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[OrderRefNo] [nvarchar](50) NOT NULL,
	[OrderDate] [date] NULL,
	[BuyerInfoId] [int] NOT NULL,
	[ProdDepartmentId] [int] NULL,
	[SeasonInfoId] [int] NULL,
	[FabSupplierId] [int] NULL,
	[IsAuth] [bit] NOT NULL,
	[OpBy] [int] NOT NULL,
	[OpOn] [datetime] NOT NULL,
	[AuthBy] [int] NULL,
	[AuthOn] [datetime] NULL,
	[IsLocked] [bit] NOT NULL,
	[BrandId] [int] NOT NULL,
 CONSTRAINT [PK_BuyerOrderMas] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UK_BuyerOrderMas] UNIQUE NONCLUSTERED 
(
	[OrderRefNo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[CommissionDistDet]    Script Date: 6/30/2018 11:41:33 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CommissionDistDet](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CommissionDistMasId] [int] NOT NULL,
	[FactoryOrderDetId] [int] NOT NULL,
	[OverseasCommPer] [float] NOT NULL,
	[OverseasCommValue] [decimal](12, 2) NOT NULL,
	[OthersCommPer] [float] NOT NULL,
	[OthersCommValue] [decimal](12, 2) NOT NULL,
	[CompCommValue] [decimal](12, 2) NOT NULL,
 CONSTRAINT [PK_CommmissionDistDet] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[CommissionDistMas]    Script Date: 6/30/2018 11:41:33 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CommissionDistMas](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[BuyerOrderMasId] [int] NOT NULL,
	[IsAuth] [bit] NOT NULL,
	[OpBy] [int] NOT NULL,
	[OpOn] [datetime] NOT NULL,
	[AuthBy] [int] NULL,
	[AuthOn] [datetime] NULL,
 CONSTRAINT [PK_CommmissionDistMas] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[CommissionDistTempDet]    Script Date: 6/30/2018 11:41:33 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CommissionDistTempDet](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CommissionDistTempMasId] [int] NOT NULL,
	[MinRange] [float] NOT NULL,
	[MaxRange] [float] NOT NULL,
	[OverseasComm] [float] NOT NULL,
	[OthersComm] [float] NOT NULL,
	[Remarks] [nvarchar](100) NULL,
 CONSTRAINT [PK_CommissionDistTempDet] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[CommissionDistTempMas]    Script Date: 6/30/2018 11:41:33 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CommissionDistTempMas](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[BuyerInfoId] [int] NOT NULL,
	[ProdDepartmentId] [int] NOT NULL,
	[CalcType] [int] NOT NULL,
	[IsAuth] [bit] NOT NULL,
	[OpBy] [int] NOT NULL,
	[OpOn] [datetime] NOT NULL,
	[AuthBy] [int] NULL,
	[AuthOn] [datetime] NULL,
 CONSTRAINT [PK_CommissionDistTempMas] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[CommissionRealization]    Script Date: 6/30/2018 11:41:33 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CommissionRealization](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ProceedRealizationMasId] [int] NOT NULL,
	[ExchangeRate] [decimal](12, 4) NOT NULL,
	[AITAmount] [decimal](12, 2) NOT NULL,
	[BankCharge] [decimal](12, 2) NOT NULL,
	[RealizationDate] [datetime] NOT NULL,
 CONSTRAINT [PK_CommissionRealization] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[CompanyInformations]    Script Date: 6/30/2018 11:41:33 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CompanyInformations](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Address] [nvarchar](256) NULL,
	[Phone] [nvarchar](50) NULL,
	[Web] [nvarchar](50) NULL,
	[Email] [nvarchar](50) NULL,
	[ContactPerson] [nvarchar](50) NULL,
	[IsAuth] [bit] NOT NULL,
	[OpBy] [int] NOT NULL,
	[OpOn] [datetime] NOT NULL,
	[AuthBy] [int] NULL,
	[AuthOn] [datetime] NULL,
 CONSTRAINT [PK_CompanyInformations] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[CompanyResources]    Script Date: 6/30/2018 11:41:33 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CompanyResources](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Position] [nvarchar](100) NULL,
	[DOJ] [datetime] NULL,
	[DOB] [datetime] NULL,
	[Phone] [nvarchar](50) NULL,
	[Address] [nvarchar](100) NULL,
	[Status] [nvarchar](1) NOT NULL,
	[IsAuth] [bit] NOT NULL,
	[OpBy] [int] NOT NULL,
	[OpOn] [datetime] NOT NULL,
	[AuthBy] [int] NULL,
	[AuthOn] [datetime] NULL,
 CONSTRAINT [PK_CompanyResources] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[CountryInfo]    Script Date: 6/30/2018 11:41:33 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CountryInfo](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_CountryInfo] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UK_CountryInfo] UNIQUE NONCLUSTERED 
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[CourierInfo]    Script Date: 6/30/2018 11:41:33 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CourierInfo](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[IsAuth] [bit] NOT NULL,
	[OpBy] [int] NOT NULL,
	[OpOn] [datetime] NOT NULL,
	[AuthBy] [int] NULL,
	[AuthOn] [datetime] NULL,
 CONSTRAINT [PK_CourierInfo] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UK_CourierInfo] UNIQUE NONCLUSTERED 
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[DestinationPort]    Script Date: 6/30/2018 11:41:33 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DestinationPort](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Description] [nvarchar](256) NULL,
	[IsAuth] [bit] NOT NULL,
	[OpBy] [int] NOT NULL,
	[OpOn] [datetime] NOT NULL,
	[AuthBy] [int] NULL,
	[AuthOn] [datetime] NULL,
	[BuyerInfoId] [int] NULL,
 CONSTRAINT [PK_DestinationPort] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[DocSubmissionDet]    Script Date: 6/30/2018 11:41:33 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DocSubmissionDet](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[DocSubmissionMasId] [int] NOT NULL,
	[InvoiceCommMasId] [int] NULL,
	[BuyerOrderMasId] [int] NULL,
 CONSTRAINT [PK_DocSubmissionDet] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UK_DocSubmissionDet] UNIQUE NONCLUSTERED 
(
	[DocSubmissionMasId] ASC,
	[InvoiceCommMasId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[DocSubmissionFactDet]    Script Date: 6/30/2018 11:41:33 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DocSubmissionFactDet](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[DocSubmissionDetId] [int] NOT NULL,
	[InvoiceCommDetId] [int] NULL,
	[FactFDBCNo] [nvarchar](50) NULL,
	[InvoiceCommFactDetId] [int] NULL,
 CONSTRAINT [PK_DocSubmissionFactDet] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[DocSubmissionMas]    Script Date: 6/30/2018 11:41:33 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DocSubmissionMas](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PaymentTypeId] [int] NOT NULL,
	[BuyerInfoId] [int] NOT NULL,
	[SubmissionDate] [date] NOT NULL,
	[FDBCNo] [nvarchar](50) NULL,
	[FDBCValue] [decimal](18, 0) NULL,
	[FDBCDate] [datetime] NULL,
	[AWBNo] [nvarchar](50) NULL,
	[AWBDate] [date] NULL,
	[CourierInfoId] [int] NULL,
	[IsAuth] [bit] NOT NULL,
	[OpBy] [int] NOT NULL,
	[OpOn] [datetime] NOT NULL,
	[AuthBy] [int] NULL,
	[AuthOn] [datetime] NULL,
	[MasterLCInfoMasId] [int] NULL,
 CONSTRAINT [PK_DocSubmissionMas] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ExFactoryDet]    Script Date: 6/30/2018 11:41:33 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ExFactoryDet](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ExFactoryMasId] [int] NOT NULL,
	[BuyerOrderMasId] [int] NOT NULL,
	[ShipQuantity] [int] NOT NULL,
 CONSTRAINT [PK_ExFactoryDet] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UK_ExFactoryDet] UNIQUE NONCLUSTERED 
(
	[ExFactoryMasId] ASC,
	[BuyerOrderMasId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ExFactoryMas]    Script Date: 6/30/2018 11:41:33 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ExFactoryMas](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ExFactoryDate] [date] NOT NULL,
	[SupplierId] [int] NOT NULL,
	[BuyerInfoId] [int] NOT NULL,
	[IsAuth] [bit] NOT NULL,
	[OpBy] [int] NOT NULL,
	[OpOn] [datetime] NOT NULL,
	[AuthBy] [int] NULL,
	[AuthOn] [datetime] NULL,
 CONSTRAINT [PK_ExFactoryMas] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UK_ExFactoryMas] UNIQUE NONCLUSTERED 
(
	[ExFactoryDate] ASC,
	[SupplierId] ASC,
	[BuyerInfoId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ExFactoryShipDet]    Script Date: 6/30/2018 11:41:33 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ExFactoryShipDet](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ExFactoryDetId] [int] NOT NULL,
	[BuyerOrderDetId] [int] NOT NULL,
	[ShipmentSummDetId] [int] NOT NULL,
	[ShipQuantity] [int] NOT NULL,
	[Remarks] [nvarchar](50) NULL,
	[IsShipClosed] [bit] NOT NULL,
 CONSTRAINT [PK_ExFactoryShipDet] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UK_ExFactoryShipDet] UNIQUE NONCLUSTERED 
(
	[ShipmentSummDetId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[FabricItem]    Script Date: 6/30/2018 11:41:33 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FabricItem](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Description] [nvarchar](256) NULL,
	[IsAuth] [bit] NOT NULL,
	[OpBy] [int] NOT NULL,
	[OpOn] [datetime] NOT NULL,
	[AuthBy] [int] NULL,
	[AuthOn] [datetime] NULL,
 CONSTRAINT [PK_FabricItem] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UK_FabricItem] UNIQUE NONCLUSTERED 
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[FactoryOrderDet]    Script Date: 6/30/2018 11:41:33 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FactoryOrderDet](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[BuyerOrderDetId] [int] NOT NULL,
	[FOBUnitPrice] [decimal](12, 2) NOT NULL,
	[Status] [nvarchar](10) NULL,
	[IsLocked] [bit] NOT NULL,
	[FactoryOrderMasId] [int] NOT NULL,
	[TransferPrice] [decimal](12, 2) NULL,
 CONSTRAINT [PK_FactoryOrderDet] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[FactoryOrderMas]    Script Date: 6/30/2018 11:41:33 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FactoryOrderMas](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[BuyerOrderMasId] [int] NOT NULL,
	[SalesContractNo] [nvarchar](50) NOT NULL,
	[SalesContractDate] [date] NOT NULL,
	[SupplierId] [int] NOT NULL,
	[IsAuth] [bit] NOT NULL,
	[OpBy] [int] NOT NULL,
	[OpOn] [datetime] NOT NULL,
	[AuthBy] [int] NULL,
	[AuthOn] [datetime] NULL,
	[IsLocked] [bit] NOT NULL,
 CONSTRAINT [PK_FactoryOrderMas] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[FDDPayments]    Script Date: 6/30/2018 11:41:33 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FDDPayments](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SupplierId] [int] NOT NULL,
	[FDDNo] [nvarchar](200) NULL,
	[FDDDate] [datetime] NULL,
	[FDDAmount] [decimal](12, 2) NOT NULL,
	[DocSubmissionFactDetId] [int] NOT NULL,
	[ProceedRealizationMasId] [int] NOT NULL,
 CONSTRAINT [PK_FDDPayment] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[InvoiceCommDet]    Script Date: 6/30/2018 11:41:33 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[InvoiceCommDet](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[InvoiceCommMasId] [int] NOT NULL,
	[InvoiceCommFactMasId] [int] NOT NULL,
 CONSTRAINT [PK_InvoiceCommDet] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UK_InvoiceCommDet] UNIQUE NONCLUSTERED 
(
	[InvoiceCommFactMasId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[InvoiceCommFactDet]    Script Date: 6/30/2018 11:41:33 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[InvoiceCommFactDet](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[InvoiceCommFactMasId] [int] NOT NULL,
	[ExFactoryDetId] [int] NOT NULL,
 CONSTRAINT [PK_InvoiceCommFactDet] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[InvoiceCommFactMas]    Script Date: 6/30/2018 11:41:33 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[InvoiceCommFactMas](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SupplierId] [int] NOT NULL,
	[BuyerInfoId] [int] NOT NULL,
	[InvoiceNoFact] [nvarchar](50) NOT NULL,
	[IssueDate] [date] NOT NULL,
	[IsAuth] [bit] NOT NULL,
	[OpBy] [int] NOT NULL,
	[OpOn] [datetime] NOT NULL,
	[AuthBy] [int] NULL,
	[AuthOn] [datetime] NULL,
 CONSTRAINT [PK_InvoiceCommFactMas] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UK_InvoiceCommFactMas] UNIQUE NONCLUSTERED 
(
	[InvoiceNoFact] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[InvoiceCommMas]    Script Date: 6/30/2018 11:41:33 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[InvoiceCommMas](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[IssueDate] [date] NOT NULL,
	[BuyerInfoId] [int] NOT NULL,
	[InvoiceNo] [nvarchar](50) NOT NULL,
	[PaymentTypeId] [int] NOT NULL,
	[MasterLCInfoMasId] [int] NULL,
	[TTNo] [nvarchar](50) NULL,
	[IsAuth] [bit] NOT NULL,
	[OpBy] [int] NOT NULL,
	[OpOn] [datetime] NOT NULL,
	[AuthBy] [int] NULL,
	[AuthOn] [datetime] NULL,
 CONSTRAINT [PK_InvoiceCommMas] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UK_InvoiceCommMas] UNIQUE NONCLUSTERED 
(
	[InvoiceNo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[LCAmendInfo]    Script Date: 6/30/2018 11:41:33 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[LCAmendInfo](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[MasterLCInfoMasId] [int] NOT NULL,
	[AmendDate] [date] NOT NULL,
	[AmendLCNo] [varchar](50) NOT NULL,
	[AmendLCRecvDate] [date] NULL,
	[AmendLatestShipDate] [date] NULL,
	[AmendQuantity] [int] NULL,
	[AmendTotalValue] [decimal](12, 2) NOT NULL,
	[AmendLCExpiryDate] [date] NULL,
	[AmendPaymentTerm] [int] NULL,
	[AmendTenor] [int] NULL,
 CONSTRAINT [PK_LCAmendInfo] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[LCTransferDet]    Script Date: 6/30/2018 11:41:33 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LCTransferDet](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[LCTransferMasId] [int] NOT NULL,
	[FactoryOrderMasId] [int] NOT NULL,
	[TransferDate] [date] NOT NULL,
 CONSTRAINT [PK_LCTransferDet] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UK_LCTransferDet] UNIQUE NONCLUSTERED 
(
	[LCTransferMasId] ASC,
	[FactoryOrderMasId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[LCTransferMas]    Script Date: 6/30/2018 11:41:33 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LCTransferMas](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[MasterLCInfoMasId] [int] NOT NULL,
	[IsAuth] [bit] NOT NULL,
	[OpBy] [int] NOT NULL,
	[OpOn] [datetime] NOT NULL,
	[AuthBy] [int] NULL,
	[AuthOn] [datetime] NULL,
 CONSTRAINT [PK_LCTransferMas] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[MasterLCInfoDet]    Script Date: 6/30/2018 11:41:33 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MasterLCInfoDet](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[MasterLCInfoMasId] [int] NOT NULL,
	[BuyerOrderMasId] [int] NOT NULL,
	[PINo] [nvarchar](50) NULL,
	[PIDate] [date] NULL,
 CONSTRAINT [PK_MasterLCInfoDet] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UK_MasterLCInfoDet] UNIQUE NONCLUSTERED 
(
	[MasterLCInfoMasId] ASC,
	[BuyerOrderMasId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[MasterLCInfoMas]    Script Date: 6/30/2018 11:41:33 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MasterLCInfoMas](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[LCNo] [nvarchar](50) NOT NULL,
	[LCDate] [date] NULL,
	[LCExpiryDate] [date] NULL,
	[PaymentTerm] [int] NULL,
	[Tenor] [int] NULL,
	[LCReceiveDate] [date] NULL,
	[LatestShipmentDate] [date] NULL,
	[Quantity] [int] NULL,
	[TotalValue] [decimal](12, 2) NOT NULL,
	[BuyerInfoId] [int] NULL,
	[BuyerBankId] [int] NULL,
	[IsAuth] [bit] NOT NULL,
	[OpBy] [int] NOT NULL,
	[OpOn] [datetime] NOT NULL,
	[AuthBy] [int] NULL,
	[AuthOn] [datetime] NULL,
 CONSTRAINT [PK_MasterLCInfoMas] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[MasterLCInfoOrderDet]    Script Date: 6/30/2018 11:41:33 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MasterLCInfoOrderDet](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[MasterLCInfoDetId] [int] NOT NULL,
	[BuyerOrderDetId] [int] NOT NULL,
 CONSTRAINT [PK_MasterLCInfoOrderDet] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UK_MasterLCInfoOrderDet] UNIQUE NONCLUSTERED 
(
	[MasterLCInfoDetId] ASC,
	[BuyerOrderDetId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[MiddleParty]    Script Date: 6/30/2018 11:41:33 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MiddleParty](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Address] [nvarchar](256) NULL,
	[IsAuth] [bit] NOT NULL,
	[OpBy] [int] NOT NULL,
	[OpOn] [datetime] NOT NULL,
	[AuthBy] [int] NULL,
	[AuthOn] [datetime] NULL,
 CONSTRAINT [PK_MiddleParty] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UK_MiddleParty] UNIQUE NONCLUSTERED 
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ProceedRealizationDet]    Script Date: 6/30/2018 11:41:33 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProceedRealizationDet](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ProceedRealizationMasId] [int] NOT NULL,
	[ProceedQty] [numeric](12, 2) NOT NULL,
	[DocSubmissionDetId] [int] NOT NULL,
 CONSTRAINT [PK_ProceedRealizationDet] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ProceedRealizationMas]    Script Date: 6/30/2018 11:41:33 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProceedRealizationMas](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PaymentTypeId] [int] NULL,
	[BuyerInfoId] [int] NOT NULL,
	[ProceedDate] [datetime] NULL,
	[DocSubmissionMasId] [int] NOT NULL,
 CONSTRAINT [PK_ProceedRealizationMas] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ProdCategory]    Script Date: 6/30/2018 11:41:33 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProdCategory](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Description] [nvarchar](256) NULL,
	[IsAuth] [bit] NOT NULL,
	[OpBy] [int] NOT NULL,
	[OpOn] [datetime] NOT NULL,
	[AuthBy] [int] NULL,
	[AuthOn] [datetime] NULL,
	[BuyerInfoId] [int] NULL,
 CONSTRAINT [PK_ProdCategory] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ProdCatType]    Script Date: 6/30/2018 11:41:33 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProdCatType](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ProdCategoryId] [int] NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Description] [nvarchar](256) NULL,
 CONSTRAINT [PK_ProdCatType] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ProdColor]    Script Date: 6/30/2018 11:41:33 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProdColor](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[IsAuth] [bit] NOT NULL,
	[OpBy] [int] NOT NULL,
	[OpOn] [datetime] NOT NULL,
	[AuthBy] [int] NULL,
	[AuthOn] [datetime] NULL,
	[SeasonInfoId] [int] NULL,
	[ProdDepartmentId] [int] NULL,
 CONSTRAINT [PK_ProdColor] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UK_ProdColor] UNIQUE NONCLUSTERED 
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ProdDepartment]    Script Date: 6/30/2018 11:41:33 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProdDepartment](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[BrandId] [int] NOT NULL,
 CONSTRAINT [PK_ProdDepartment] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ProdSize]    Script Date: 6/30/2018 11:41:33 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProdSize](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SizeRange] [nvarchar](50) NOT NULL,
	[IsAuth] [bit] NOT NULL,
	[OpBy] [int] NOT NULL,
	[OpOn] [datetime] NOT NULL,
	[AuthBy] [int] NULL,
	[AuthOn] [datetime] NULL,
	[ProdDepartmentId] [int] NULL,
 CONSTRAINT [PK_ProdSize] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[SeasonInfo]    Script Date: 6/30/2018 11:41:33 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SeasonInfo](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Description] [nvarchar](256) NULL,
	[IsAuth] [bit] NOT NULL,
	[OpBy] [int] NOT NULL,
	[OpOn] [datetime] NOT NULL,
	[AuthBy] [int] NULL,
	[AuthOn] [datetime] NULL,
	[BuyerInfoId] [int] NULL,
 CONSTRAINT [PK_SeasonInfo] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UK_SeasonInfo] UNIQUE NONCLUSTERED 
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ShipmentSummDet]    Script Date: 6/30/2018 11:41:33 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ShipmentSummDet](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[BuyerOrderDetId] [int] NOT NULL,
	[DelivSlno] [int] NOT NULL,
	[DelivQuantity] [int] NOT NULL,
	[ExFactoryDate] [date] NULL,
	[HandoverDate] [date] NULL,
	[DestinationPortId] [int] NULL,
	[ETD] [date] NULL,
	[BuyersPONo] [nvarchar](50) NULL,
	[Status] [nvarchar](10) NULL,
	[IsLocked] [bit] NOT NULL,
 CONSTRAINT [PK_ShipmentSummDet] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Supplier]    Script Date: 6/30/2018 11:41:33 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Supplier](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[ShortName] [nvarchar](10) NULL,
	[Address] [nvarchar](256) NULL,
	[Phone] [nvarchar](50) NULL,
	[ContactPerson] [nvarchar](50) NULL,
	[BankBranchId] [int] NULL,
	[AccNo] [nvarchar](50) NULL,
	[IsAuth] [bit] NOT NULL,
	[OpBy] [int] NOT NULL,
	[OpOn] [datetime] NOT NULL,
	[AuthBy] [int] NULL,
	[AuthOn] [datetime] NULL,
 CONSTRAINT [PK_Supplier] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[TTPayments]    Script Date: 6/30/2018 11:41:33 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TTPayments](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SupplierId] [int] NOT NULL,
	[FDDNo] [nvarchar](200) NULL,
	[FDDDate] [datetime] NULL,
	[FDDAmount] [decimal](12, 2) NOT NULL,
	[ProceedRealizationDetId] [int] NOT NULL,
	[DocSubmissionFactDetId] [int] NOT NULL,
 CONSTRAINT [PK_TTPayment] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[Brands]  WITH CHECK ADD  CONSTRAINT [FK_Brand] FOREIGN KEY([BuyerInfoId])
REFERENCES [dbo].[BuyerInfo] ([Id])
GO
ALTER TABLE [dbo].[Brands] CHECK CONSTRAINT [FK_Brand]
GO
ALTER TABLE [dbo].[BuyerInfo]  WITH CHECK ADD  CONSTRAINT [FK1_BuyerInfo] FOREIGN KEY([CountryInfoId])
REFERENCES [dbo].[CountryInfo] ([Id])
GO
ALTER TABLE [dbo].[BuyerInfo] CHECK CONSTRAINT [FK1_BuyerInfo]
GO
ALTER TABLE [dbo].[BuyerInfo]  WITH CHECK ADD  CONSTRAINT [FK2_BuyerInfo] FOREIGN KEY([MerchandiserId])
REFERENCES [dbo].[CompanyResources] ([Id])
GO
ALTER TABLE [dbo].[BuyerInfo] CHECK CONSTRAINT [FK2_BuyerInfo]
GO
ALTER TABLE [dbo].[BuyerInfo]  WITH CHECK ADD  CONSTRAINT [FK3_BuyerInfo] FOREIGN KEY([MiddlePartyId])
REFERENCES [dbo].[MiddleParty] ([Id])
GO
ALTER TABLE [dbo].[BuyerInfo] CHECK CONSTRAINT [FK3_BuyerInfo]
GO
ALTER TABLE [dbo].[BuyerInfo]  WITH CHECK ADD  CONSTRAINT [FK4_BuyerInfo] FOREIGN KEY([BankBranchId])
REFERENCES [dbo].[BankBranch] ([Id])
GO
ALTER TABLE [dbo].[BuyerInfo] CHECK CONSTRAINT [FK4_BuyerInfo]
GO
ALTER TABLE [dbo].[BuyerOrderDet]  WITH CHECK ADD  CONSTRAINT [FK1_BuyerOrderDet] FOREIGN KEY([BuyerOrderMasId])
REFERENCES [dbo].[BuyerOrderMas] ([Id])
GO
ALTER TABLE [dbo].[BuyerOrderDet] CHECK CONSTRAINT [FK1_BuyerOrderDet]
GO
ALTER TABLE [dbo].[BuyerOrderDet]  WITH CHECK ADD  CONSTRAINT [FK2_BuyerOrderDet] FOREIGN KEY([ProdCatTypeId])
REFERENCES [dbo].[ProdCatType] ([Id])
GO
ALTER TABLE [dbo].[BuyerOrderDet] CHECK CONSTRAINT [FK2_BuyerOrderDet]
GO
ALTER TABLE [dbo].[BuyerOrderDet]  WITH CHECK ADD  CONSTRAINT [FK3_BuyerOrderDet] FOREIGN KEY([ProdSizeId])
REFERENCES [dbo].[ProdSize] ([Id])
GO
ALTER TABLE [dbo].[BuyerOrderDet] CHECK CONSTRAINT [FK3_BuyerOrderDet]
GO
ALTER TABLE [dbo].[BuyerOrderDet]  WITH CHECK ADD  CONSTRAINT [FK4_BuyerOrderDet] FOREIGN KEY([FabricItemId])
REFERENCES [dbo].[FabricItem] ([Id])
GO
ALTER TABLE [dbo].[BuyerOrderDet] CHECK CONSTRAINT [FK4_BuyerOrderDet]
GO
ALTER TABLE [dbo].[BuyerOrderDet]  WITH CHECK ADD  CONSTRAINT [FK5_BuyerOrderDet] FOREIGN KEY([ProdColorId])
REFERENCES [dbo].[ProdColor] ([Id])
GO
ALTER TABLE [dbo].[BuyerOrderDet] CHECK CONSTRAINT [FK5_BuyerOrderDet]
GO
ALTER TABLE [dbo].[BuyerOrderDet]  WITH CHECK ADD  CONSTRAINT [FK6_BuyerOrderMas] FOREIGN KEY([SupplierId])
REFERENCES [dbo].[Supplier] ([Id])
GO
ALTER TABLE [dbo].[BuyerOrderDet] CHECK CONSTRAINT [FK6_BuyerOrderMas]
GO
ALTER TABLE [dbo].[BuyerOrderMas]  WITH CHECK ADD  CONSTRAINT [FK1_BuyerOrderMas] FOREIGN KEY([BuyerInfoId])
REFERENCES [dbo].[BuyerInfo] ([Id])
GO
ALTER TABLE [dbo].[BuyerOrderMas] CHECK CONSTRAINT [FK1_BuyerOrderMas]
GO
ALTER TABLE [dbo].[BuyerOrderMas]  WITH CHECK ADD  CONSTRAINT [FK2_BuyerOrderMas] FOREIGN KEY([ProdDepartmentId])
REFERENCES [dbo].[ProdDepartment] ([Id])
GO
ALTER TABLE [dbo].[BuyerOrderMas] CHECK CONSTRAINT [FK2_BuyerOrderMas]
GO
ALTER TABLE [dbo].[BuyerOrderMas]  WITH CHECK ADD  CONSTRAINT [FK3_BuyerOrderMas] FOREIGN KEY([SeasonInfoId])
REFERENCES [dbo].[SeasonInfo] ([Id])
GO
ALTER TABLE [dbo].[BuyerOrderMas] CHECK CONSTRAINT [FK3_BuyerOrderMas]
GO
ALTER TABLE [dbo].[BuyerOrderMas]  WITH CHECK ADD  CONSTRAINT [FK4_BuyerOrderMas] FOREIGN KEY([FabSupplierId])
REFERENCES [dbo].[Supplier] ([Id])
GO
ALTER TABLE [dbo].[BuyerOrderMas] CHECK CONSTRAINT [FK4_BuyerOrderMas]
GO
ALTER TABLE [dbo].[BuyerOrderMas]  WITH CHECK ADD  CONSTRAINT [FK5_BuyerOrderMas] FOREIGN KEY([BrandId])
REFERENCES [dbo].[Brands] ([Id])
GO
ALTER TABLE [dbo].[BuyerOrderMas] CHECK CONSTRAINT [FK5_BuyerOrderMas]
GO
ALTER TABLE [dbo].[CommissionDistDet]  WITH CHECK ADD  CONSTRAINT [FK1_CommissionDistDet] FOREIGN KEY([CommissionDistMasId])
REFERENCES [dbo].[CommissionDistMas] ([Id])
GO
ALTER TABLE [dbo].[CommissionDistDet] CHECK CONSTRAINT [FK1_CommissionDistDet]
GO
ALTER TABLE [dbo].[CommissionDistDet]  WITH CHECK ADD  CONSTRAINT [FK2_CommissionDistDet] FOREIGN KEY([FactoryOrderDetId])
REFERENCES [dbo].[FactoryOrderDet] ([Id])
GO
ALTER TABLE [dbo].[CommissionDistDet] CHECK CONSTRAINT [FK2_CommissionDistDet]
GO
ALTER TABLE [dbo].[CommissionDistMas]  WITH CHECK ADD  CONSTRAINT [FK1_CommissionDistMas] FOREIGN KEY([BuyerOrderMasId])
REFERENCES [dbo].[BuyerOrderMas] ([Id])
GO
ALTER TABLE [dbo].[CommissionDistMas] CHECK CONSTRAINT [FK1_CommissionDistMas]
GO
ALTER TABLE [dbo].[CommissionDistTempDet]  WITH CHECK ADD  CONSTRAINT [FK1_CommissionDistTempDet] FOREIGN KEY([CommissionDistTempMasId])
REFERENCES [dbo].[CommissionDistTempMas] ([Id])
GO
ALTER TABLE [dbo].[CommissionDistTempDet] CHECK CONSTRAINT [FK1_CommissionDistTempDet]
GO
ALTER TABLE [dbo].[CommissionDistTempMas]  WITH CHECK ADD  CONSTRAINT [FK1_CommissionDistTempMas] FOREIGN KEY([BuyerInfoId])
REFERENCES [dbo].[BuyerInfo] ([Id])
GO
ALTER TABLE [dbo].[CommissionDistTempMas] CHECK CONSTRAINT [FK1_CommissionDistTempMas]
GO
ALTER TABLE [dbo].[CommissionDistTempMas]  WITH CHECK ADD  CONSTRAINT [FK2_CommissionDistTempMas] FOREIGN KEY([ProdDepartmentId])
REFERENCES [dbo].[ProdDepartment] ([Id])
GO
ALTER TABLE [dbo].[CommissionDistTempMas] CHECK CONSTRAINT [FK2_CommissionDistTempMas]
GO
ALTER TABLE [dbo].[CommissionRealization]  WITH CHECK ADD  CONSTRAINT [FK1_CommissionRealization] FOREIGN KEY([ProceedRealizationMasId])
REFERENCES [dbo].[ProceedRealizationMas] ([Id])
GO
ALTER TABLE [dbo].[CommissionRealization] CHECK CONSTRAINT [FK1_CommissionRealization]
GO
ALTER TABLE [dbo].[DestinationPort]  WITH CHECK ADD  CONSTRAINT [FK1_DestinationPort] FOREIGN KEY([BuyerInfoId])
REFERENCES [dbo].[BuyerInfo] ([Id])
GO
ALTER TABLE [dbo].[DestinationPort] CHECK CONSTRAINT [FK1_DestinationPort]
GO
ALTER TABLE [dbo].[DocSubmissionDet]  WITH CHECK ADD  CONSTRAINT [FK1_DocSubmissionDet] FOREIGN KEY([DocSubmissionMasId])
REFERENCES [dbo].[DocSubmissionMas] ([Id])
GO
ALTER TABLE [dbo].[DocSubmissionDet] CHECK CONSTRAINT [FK1_DocSubmissionDet]
GO
ALTER TABLE [dbo].[DocSubmissionDet]  WITH CHECK ADD  CONSTRAINT [FK2_DocSubmissionDet] FOREIGN KEY([InvoiceCommMasId])
REFERENCES [dbo].[InvoiceCommMas] ([Id])
GO
ALTER TABLE [dbo].[DocSubmissionDet] CHECK CONSTRAINT [FK2_DocSubmissionDet]
GO
ALTER TABLE [dbo].[DocSubmissionDet]  WITH CHECK ADD  CONSTRAINT [FK3_DocSubmissionDet] FOREIGN KEY([BuyerOrderMasId])
REFERENCES [dbo].[BuyerOrderMas] ([Id])
GO
ALTER TABLE [dbo].[DocSubmissionDet] CHECK CONSTRAINT [FK3_DocSubmissionDet]
GO
ALTER TABLE [dbo].[DocSubmissionFactDet]  WITH CHECK ADD  CONSTRAINT [FK1_DocSubmissionFactDet] FOREIGN KEY([DocSubmissionDetId])
REFERENCES [dbo].[DocSubmissionDet] ([Id])
GO
ALTER TABLE [dbo].[DocSubmissionFactDet] CHECK CONSTRAINT [FK1_DocSubmissionFactDet]
GO
ALTER TABLE [dbo].[DocSubmissionFactDet]  WITH CHECK ADD  CONSTRAINT [FK2_DocSubmissionFactDet] FOREIGN KEY([InvoiceCommDetId])
REFERENCES [dbo].[InvoiceCommDet] ([Id])
GO
ALTER TABLE [dbo].[DocSubmissionFactDet] CHECK CONSTRAINT [FK2_DocSubmissionFactDet]
GO
ALTER TABLE [dbo].[DocSubmissionFactDet]  WITH CHECK ADD  CONSTRAINT [FK3_DocSubmissionFactDet] FOREIGN KEY([InvoiceCommFactDetId])
REFERENCES [dbo].[InvoiceCommFactDet] ([Id])
GO
ALTER TABLE [dbo].[DocSubmissionFactDet] CHECK CONSTRAINT [FK3_DocSubmissionFactDet]
GO
ALTER TABLE [dbo].[DocSubmissionMas]  WITH CHECK ADD  CONSTRAINT [FK1_DocSubmissionMas] FOREIGN KEY([BuyerInfoId])
REFERENCES [dbo].[BuyerInfo] ([Id])
GO
ALTER TABLE [dbo].[DocSubmissionMas] CHECK CONSTRAINT [FK1_DocSubmissionMas]
GO
ALTER TABLE [dbo].[DocSubmissionMas]  WITH CHECK ADD  CONSTRAINT [FK2_DocSubmissionMas] FOREIGN KEY([MasterLCInfoMasId])
REFERENCES [dbo].[MasterLCInfoMas] ([Id])
GO
ALTER TABLE [dbo].[DocSubmissionMas] CHECK CONSTRAINT [FK2_DocSubmissionMas]
GO
ALTER TABLE [dbo].[DocSubmissionMas]  WITH CHECK ADD  CONSTRAINT [FK3_DocSubmissionMas] FOREIGN KEY([CourierInfoId])
REFERENCES [dbo].[CourierInfo] ([Id])
GO
ALTER TABLE [dbo].[DocSubmissionMas] CHECK CONSTRAINT [FK3_DocSubmissionMas]
GO
ALTER TABLE [dbo].[ExFactoryDet]  WITH CHECK ADD  CONSTRAINT [FK1_ExFactoryDet] FOREIGN KEY([ExFactoryMasId])
REFERENCES [dbo].[ExFactoryMas] ([Id])
GO
ALTER TABLE [dbo].[ExFactoryDet] CHECK CONSTRAINT [FK1_ExFactoryDet]
GO
ALTER TABLE [dbo].[ExFactoryDet]  WITH CHECK ADD  CONSTRAINT [FK2_ExFactoryDet] FOREIGN KEY([BuyerOrderMasId])
REFERENCES [dbo].[BuyerOrderMas] ([Id])
GO
ALTER TABLE [dbo].[ExFactoryDet] CHECK CONSTRAINT [FK2_ExFactoryDet]
GO
ALTER TABLE [dbo].[ExFactoryMas]  WITH CHECK ADD  CONSTRAINT [FK1_ExFactoryMas] FOREIGN KEY([SupplierId])
REFERENCES [dbo].[Supplier] ([Id])
GO
ALTER TABLE [dbo].[ExFactoryMas] CHECK CONSTRAINT [FK1_ExFactoryMas]
GO
ALTER TABLE [dbo].[ExFactoryMas]  WITH CHECK ADD  CONSTRAINT [FK2_ExFactoryMas] FOREIGN KEY([BuyerInfoId])
REFERENCES [dbo].[BuyerInfo] ([Id])
GO
ALTER TABLE [dbo].[ExFactoryMas] CHECK CONSTRAINT [FK2_ExFactoryMas]
GO
ALTER TABLE [dbo].[ExFactoryShipDet]  WITH CHECK ADD  CONSTRAINT [FK1_ExFactoryShipDet] FOREIGN KEY([ExFactoryDetId])
REFERENCES [dbo].[ExFactoryDet] ([Id])
GO
ALTER TABLE [dbo].[ExFactoryShipDet] CHECK CONSTRAINT [FK1_ExFactoryShipDet]
GO
ALTER TABLE [dbo].[ExFactoryShipDet]  WITH CHECK ADD  CONSTRAINT [FK2_ExFactoryShipDet] FOREIGN KEY([BuyerOrderDetId])
REFERENCES [dbo].[BuyerOrderDet] ([Id])
GO
ALTER TABLE [dbo].[ExFactoryShipDet] CHECK CONSTRAINT [FK2_ExFactoryShipDet]
GO
ALTER TABLE [dbo].[ExFactoryShipDet]  WITH CHECK ADD  CONSTRAINT [FK3_ExFactoryShipDet] FOREIGN KEY([ShipmentSummDetId])
REFERENCES [dbo].[ShipmentSummDet] ([Id])
GO
ALTER TABLE [dbo].[ExFactoryShipDet] CHECK CONSTRAINT [FK3_ExFactoryShipDet]
GO
ALTER TABLE [dbo].[FactoryOrderDet]  WITH CHECK ADD  CONSTRAINT [FK1_FactoryOrderDet] FOREIGN KEY([BuyerOrderDetId])
REFERENCES [dbo].[BuyerOrderDet] ([Id])
GO
ALTER TABLE [dbo].[FactoryOrderDet] CHECK CONSTRAINT [FK1_FactoryOrderDet]
GO
ALTER TABLE [dbo].[FactoryOrderDet]  WITH CHECK ADD  CONSTRAINT [FK2_FactoryOrderDet] FOREIGN KEY([FactoryOrderMasId])
REFERENCES [dbo].[FactoryOrderMas] ([Id])
GO
ALTER TABLE [dbo].[FactoryOrderDet] CHECK CONSTRAINT [FK2_FactoryOrderDet]
GO
ALTER TABLE [dbo].[FactoryOrderMas]  WITH CHECK ADD  CONSTRAINT [FK1_FactoryOrderMas] FOREIGN KEY([BuyerOrderMasId])
REFERENCES [dbo].[BuyerOrderMas] ([Id])
GO
ALTER TABLE [dbo].[FactoryOrderMas] CHECK CONSTRAINT [FK1_FactoryOrderMas]
GO
ALTER TABLE [dbo].[FactoryOrderMas]  WITH CHECK ADD  CONSTRAINT [FK2_FactoryOrderMas] FOREIGN KEY([SupplierId])
REFERENCES [dbo].[Supplier] ([Id])
GO
ALTER TABLE [dbo].[FactoryOrderMas] CHECK CONSTRAINT [FK2_FactoryOrderMas]
GO
ALTER TABLE [dbo].[FDDPayments]  WITH CHECK ADD  CONSTRAINT [FK1_FDDPayment] FOREIGN KEY([SupplierId])
REFERENCES [dbo].[Supplier] ([Id])
GO
ALTER TABLE [dbo].[FDDPayments] CHECK CONSTRAINT [FK1_FDDPayment]
GO
ALTER TABLE [dbo].[FDDPayments]  WITH CHECK ADD  CONSTRAINT [FK2_FDDPayment] FOREIGN KEY([ProceedRealizationMasId])
REFERENCES [dbo].[ProceedRealizationMas] ([Id])
GO
ALTER TABLE [dbo].[FDDPayments] CHECK CONSTRAINT [FK2_FDDPayment]
GO
ALTER TABLE [dbo].[FDDPayments]  WITH CHECK ADD  CONSTRAINT [FK3_FDDPayment] FOREIGN KEY([DocSubmissionFactDetId])
REFERENCES [dbo].[DocSubmissionFactDet] ([Id])
GO
ALTER TABLE [dbo].[FDDPayments] CHECK CONSTRAINT [FK3_FDDPayment]
GO
ALTER TABLE [dbo].[InvoiceCommDet]  WITH CHECK ADD  CONSTRAINT [FK1_InvoiceCommDet] FOREIGN KEY([InvoiceCommMasId])
REFERENCES [dbo].[InvoiceCommMas] ([Id])
GO
ALTER TABLE [dbo].[InvoiceCommDet] CHECK CONSTRAINT [FK1_InvoiceCommDet]
GO
ALTER TABLE [dbo].[InvoiceCommDet]  WITH CHECK ADD  CONSTRAINT [FK2_InvoiceCommDet] FOREIGN KEY([InvoiceCommFactMasId])
REFERENCES [dbo].[InvoiceCommFactMas] ([Id])
GO
ALTER TABLE [dbo].[InvoiceCommDet] CHECK CONSTRAINT [FK2_InvoiceCommDet]
GO
ALTER TABLE [dbo].[InvoiceCommFactDet]  WITH CHECK ADD  CONSTRAINT [FK1_InvoiceCommFactDet] FOREIGN KEY([InvoiceCommFactMasId])
REFERENCES [dbo].[InvoiceCommFactMas] ([Id])
GO
ALTER TABLE [dbo].[InvoiceCommFactDet] CHECK CONSTRAINT [FK1_InvoiceCommFactDet]
GO
ALTER TABLE [dbo].[InvoiceCommFactDet]  WITH CHECK ADD  CONSTRAINT [FK2_InvoiceCommFactDet] FOREIGN KEY([ExFactoryDetId])
REFERENCES [dbo].[ExFactoryDet] ([Id])
GO
ALTER TABLE [dbo].[InvoiceCommFactDet] CHECK CONSTRAINT [FK2_InvoiceCommFactDet]
GO
ALTER TABLE [dbo].[InvoiceCommFactMas]  WITH CHECK ADD  CONSTRAINT [FK1_InvoiceCommFactMas] FOREIGN KEY([SupplierId])
REFERENCES [dbo].[Supplier] ([Id])
GO
ALTER TABLE [dbo].[InvoiceCommFactMas] CHECK CONSTRAINT [FK1_InvoiceCommFactMas]
GO
ALTER TABLE [dbo].[InvoiceCommFactMas]  WITH CHECK ADD  CONSTRAINT [FK2_InvoiceCommFactMas] FOREIGN KEY([BuyerInfoId])
REFERENCES [dbo].[BuyerInfo] ([Id])
GO
ALTER TABLE [dbo].[InvoiceCommFactMas] CHECK CONSTRAINT [FK2_InvoiceCommFactMas]
GO
ALTER TABLE [dbo].[InvoiceCommMas]  WITH CHECK ADD  CONSTRAINT [FK1_InvoiceCommMas] FOREIGN KEY([BuyerInfoId])
REFERENCES [dbo].[BuyerInfo] ([Id])
GO
ALTER TABLE [dbo].[InvoiceCommMas] CHECK CONSTRAINT [FK1_InvoiceCommMas]
GO
ALTER TABLE [dbo].[InvoiceCommMas]  WITH CHECK ADD  CONSTRAINT [FK2_InvoiceCommMas] FOREIGN KEY([MasterLCInfoMasId])
REFERENCES [dbo].[MasterLCInfoMas] ([Id])
GO
ALTER TABLE [dbo].[InvoiceCommMas] CHECK CONSTRAINT [FK2_InvoiceCommMas]
GO
ALTER TABLE [dbo].[LCAmendInfo]  WITH CHECK ADD  CONSTRAINT [FK1_LCAmendInfo] FOREIGN KEY([MasterLCInfoMasId])
REFERENCES [dbo].[MasterLCInfoMas] ([Id])
GO
ALTER TABLE [dbo].[LCAmendInfo] CHECK CONSTRAINT [FK1_LCAmendInfo]
GO
ALTER TABLE [dbo].[LCTransferDet]  WITH CHECK ADD  CONSTRAINT [FK1_LCTransferDet] FOREIGN KEY([LCTransferMasId])
REFERENCES [dbo].[LCTransferMas] ([Id])
GO
ALTER TABLE [dbo].[LCTransferDet] CHECK CONSTRAINT [FK1_LCTransferDet]
GO
ALTER TABLE [dbo].[LCTransferDet]  WITH CHECK ADD  CONSTRAINT [FK2_LCTransferDet] FOREIGN KEY([FactoryOrderMasId])
REFERENCES [dbo].[FactoryOrderMas] ([Id])
GO
ALTER TABLE [dbo].[LCTransferDet] CHECK CONSTRAINT [FK2_LCTransferDet]
GO
ALTER TABLE [dbo].[LCTransferMas]  WITH CHECK ADD  CONSTRAINT [FK1_LCTransferMas] FOREIGN KEY([MasterLCInfoMasId])
REFERENCES [dbo].[MasterLCInfoMas] ([Id])
GO
ALTER TABLE [dbo].[LCTransferMas] CHECK CONSTRAINT [FK1_LCTransferMas]
GO
ALTER TABLE [dbo].[MasterLCInfoDet]  WITH CHECK ADD  CONSTRAINT [FK1_MasterLCInfoDet] FOREIGN KEY([MasterLCInfoMasId])
REFERENCES [dbo].[MasterLCInfoMas] ([Id])
GO
ALTER TABLE [dbo].[MasterLCInfoDet] CHECK CONSTRAINT [FK1_MasterLCInfoDet]
GO
ALTER TABLE [dbo].[MasterLCInfoDet]  WITH CHECK ADD  CONSTRAINT [FK2_MasterLCInfoDet] FOREIGN KEY([BuyerOrderMasId])
REFERENCES [dbo].[BuyerOrderMas] ([Id])
GO
ALTER TABLE [dbo].[MasterLCInfoDet] CHECK CONSTRAINT [FK2_MasterLCInfoDet]
GO
ALTER TABLE [dbo].[MasterLCInfoMas]  WITH CHECK ADD  CONSTRAINT [FK1_MasterLCInfoMas] FOREIGN KEY([BuyerInfoId])
REFERENCES [dbo].[BuyerInfo] ([Id])
GO
ALTER TABLE [dbo].[MasterLCInfoMas] CHECK CONSTRAINT [FK1_MasterLCInfoMas]
GO
ALTER TABLE [dbo].[MasterLCInfoMas]  WITH CHECK ADD  CONSTRAINT [FK2_MasterLCInfoMas] FOREIGN KEY([BuyerBankId])
REFERENCES [dbo].[BankBranch] ([Id])
GO
ALTER TABLE [dbo].[MasterLCInfoMas] CHECK CONSTRAINT [FK2_MasterLCInfoMas]
GO
ALTER TABLE [dbo].[MasterLCInfoOrderDet]  WITH CHECK ADD  CONSTRAINT [FK1_MasterLCInfoOrderDet] FOREIGN KEY([MasterLCInfoDetId])
REFERENCES [dbo].[MasterLCInfoDet] ([Id])
GO
ALTER TABLE [dbo].[MasterLCInfoOrderDet] CHECK CONSTRAINT [FK1_MasterLCInfoOrderDet]
GO
ALTER TABLE [dbo].[MasterLCInfoOrderDet]  WITH CHECK ADD  CONSTRAINT [FK2_MasterLCInfoOrderDet] FOREIGN KEY([BuyerOrderDetId])
REFERENCES [dbo].[BuyerOrderDet] ([Id])
GO
ALTER TABLE [dbo].[MasterLCInfoOrderDet] CHECK CONSTRAINT [FK2_MasterLCInfoOrderDet]
GO
ALTER TABLE [dbo].[ProceedRealizationDet]  WITH CHECK ADD  CONSTRAINT [FK_ProceedRealizationDet] FOREIGN KEY([ProceedRealizationMasId])
REFERENCES [dbo].[ProceedRealizationMas] ([Id])
GO
ALTER TABLE [dbo].[ProceedRealizationDet] CHECK CONSTRAINT [FK_ProceedRealizationDet]
GO
ALTER TABLE [dbo].[ProceedRealizationDet]  WITH CHECK ADD  CONSTRAINT [FK2_ProceedRealizationDet] FOREIGN KEY([DocSubmissionDetId])
REFERENCES [dbo].[DocSubmissionDet] ([Id])
GO
ALTER TABLE [dbo].[ProceedRealizationDet] CHECK CONSTRAINT [FK2_ProceedRealizationDet]
GO
ALTER TABLE [dbo].[ProceedRealizationMas]  WITH CHECK ADD  CONSTRAINT [FK1_ProceedRealizationMas] FOREIGN KEY([BuyerInfoId])
REFERENCES [dbo].[BuyerInfo] ([Id])
GO
ALTER TABLE [dbo].[ProceedRealizationMas] CHECK CONSTRAINT [FK1_ProceedRealizationMas]
GO
ALTER TABLE [dbo].[ProceedRealizationMas]  WITH CHECK ADD  CONSTRAINT [FK2_ProceedRealizationMas] FOREIGN KEY([DocSubmissionMasId])
REFERENCES [dbo].[DocSubmissionMas] ([Id])
GO
ALTER TABLE [dbo].[ProceedRealizationMas] CHECK CONSTRAINT [FK2_ProceedRealizationMas]
GO
ALTER TABLE [dbo].[ProdCategory]  WITH CHECK ADD  CONSTRAINT [FK1_ProdCategory] FOREIGN KEY([BuyerInfoId])
REFERENCES [dbo].[BuyerInfo] ([Id])
GO
ALTER TABLE [dbo].[ProdCategory] CHECK CONSTRAINT [FK1_ProdCategory]
GO
ALTER TABLE [dbo].[ProdCatType]  WITH CHECK ADD  CONSTRAINT [FK1_ProdCatType] FOREIGN KEY([ProdCategoryId])
REFERENCES [dbo].[ProdCategory] ([Id])
GO
ALTER TABLE [dbo].[ProdCatType] CHECK CONSTRAINT [FK1_ProdCatType]
GO
ALTER TABLE [dbo].[ProdColor]  WITH CHECK ADD  CONSTRAINT [FK1_ProdColor] FOREIGN KEY([SeasonInfoId])
REFERENCES [dbo].[SeasonInfo] ([Id])
GO
ALTER TABLE [dbo].[ProdColor] CHECK CONSTRAINT [FK1_ProdColor]
GO
ALTER TABLE [dbo].[ProdColor]  WITH CHECK ADD  CONSTRAINT [FK2_ProdColor] FOREIGN KEY([ProdDepartmentId])
REFERENCES [dbo].[ProdDepartment] ([Id])
GO
ALTER TABLE [dbo].[ProdColor] CHECK CONSTRAINT [FK2_ProdColor]
GO
ALTER TABLE [dbo].[ProdDepartment]  WITH CHECK ADD  CONSTRAINT [FK1_ProdDepartment] FOREIGN KEY([BrandId])
REFERENCES [dbo].[Brands] ([Id])
GO
ALTER TABLE [dbo].[ProdDepartment] CHECK CONSTRAINT [FK1_ProdDepartment]
GO
ALTER TABLE [dbo].[ProdSize]  WITH CHECK ADD  CONSTRAINT [FK1_ProdSize] FOREIGN KEY([ProdDepartmentId])
REFERENCES [dbo].[ProdDepartment] ([Id])
GO
ALTER TABLE [dbo].[ProdSize] CHECK CONSTRAINT [FK1_ProdSize]
GO
ALTER TABLE [dbo].[SeasonInfo]  WITH CHECK ADD  CONSTRAINT [FK1_SeasonInfo] FOREIGN KEY([BuyerInfoId])
REFERENCES [dbo].[BuyerInfo] ([Id])
GO
ALTER TABLE [dbo].[SeasonInfo] CHECK CONSTRAINT [FK1_SeasonInfo]
GO
ALTER TABLE [dbo].[ShipmentSummDet]  WITH CHECK ADD  CONSTRAINT [FK1_ShipmentSummDet] FOREIGN KEY([BuyerOrderDetId])
REFERENCES [dbo].[BuyerOrderDet] ([Id])
GO
ALTER TABLE [dbo].[ShipmentSummDet] CHECK CONSTRAINT [FK1_ShipmentSummDet]
GO
ALTER TABLE [dbo].[ShipmentSummDet]  WITH CHECK ADD  CONSTRAINT [FK2_ShipmentSummDet] FOREIGN KEY([DestinationPortId])
REFERENCES [dbo].[DestinationPort] ([Id])
GO
ALTER TABLE [dbo].[ShipmentSummDet] CHECK CONSTRAINT [FK2_ShipmentSummDet]
GO
ALTER TABLE [dbo].[Supplier]  WITH CHECK ADD  CONSTRAINT [FK1_Supplier] FOREIGN KEY([BankBranchId])
REFERENCES [dbo].[BankBranch] ([Id])
GO
ALTER TABLE [dbo].[Supplier] CHECK CONSTRAINT [FK1_Supplier]
GO
ALTER TABLE [dbo].[TTPayments]  WITH CHECK ADD  CONSTRAINT [FK1_TTPayment] FOREIGN KEY([SupplierId])
REFERENCES [dbo].[Supplier] ([Id])
GO
ALTER TABLE [dbo].[TTPayments] CHECK CONSTRAINT [FK1_TTPayment]
GO
ALTER TABLE [dbo].[TTPayments]  WITH CHECK ADD  CONSTRAINT [FK2_TTPayments] FOREIGN KEY([ProceedRealizationDetId])
REFERENCES [dbo].[ProceedRealizationDet] ([Id])
GO
ALTER TABLE [dbo].[TTPayments] CHECK CONSTRAINT [FK2_TTPayments]
GO
ALTER TABLE [dbo].[TTPayments]  WITH CHECK ADD  CONSTRAINT [FK3_TTPayments] FOREIGN KEY([DocSubmissionFactDetId])
REFERENCES [dbo].[DocSubmissionFactDet] ([Id])
GO
ALTER TABLE [dbo].[TTPayments] CHECK CONSTRAINT [FK3_TTPayments]
GO
USE [master]
GO
ALTER DATABASE [RBHMS] SET  READ_WRITE 
GO
