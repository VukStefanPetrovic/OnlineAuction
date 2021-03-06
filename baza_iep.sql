USE [master]
GO
/****** Object:  Database [baza_iep]    Script Date: 2.2.2019. 17.43.15 ******/
CREATE DATABASE [baza_iep]
GO
ALTER DATABASE [baza_iep] SET COMPATIBILITY_LEVEL = 140
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [baza_iep].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [baza_iep] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [baza_iep] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [baza_iep] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [baza_iep] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [baza_iep] SET ARITHABORT OFF 
GO
ALTER DATABASE [baza_iep] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [baza_iep] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [baza_iep] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [baza_iep] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [baza_iep] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [baza_iep] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [baza_iep] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [baza_iep] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [baza_iep] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [baza_iep] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [baza_iep] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [baza_iep] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [baza_iep] SET READ_COMMITTED_SNAPSHOT ON 
GO
ALTER DATABASE [baza_iep] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [baza_iep] SET  MULTI_USER 
GO
ALTER DATABASE [baza_iep] SET DB_CHAINING OFF 
GO
ALTER DATABASE [baza_iep] SET ENCRYPTION ON
GO
ALTER DATABASE [baza_iep] SET QUERY_STORE = ON
GO
ALTER DATABASE [baza_iep] SET QUERY_STORE (OPERATION_MODE = READ_WRITE, CLEANUP_POLICY = (STALE_QUERY_THRESHOLD_DAYS = 30), DATA_FLUSH_INTERVAL_SECONDS = 900, INTERVAL_LENGTH_MINUTES = 60, MAX_STORAGE_SIZE_MB = 100, QUERY_CAPTURE_MODE = ALL, SIZE_BASED_CLEANUP_MODE = AUTO)
GO
USE [baza_iep]
GO
/****** Object:  Table [dbo].[Auction]    Script Date: 2.2.2019. 17.43.18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Auction](
	[Name] [nvarchar](max) NOT NULL,
	[Duration] [int] NOT NULL,
	[Img] [varbinary](max) NOT NULL,
	[openingTime] [datetime] NULL,
	[closingTime] [datetime] NULL,
	[currentPrice] [decimal](10, 3) NULL,
	[startingPrice] [decimal](10, 3) NULL,
	[createdOn] [datetime] NULL,
	[State] [nvarchar](10) NULL,
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Lastbidder] [nvarchar](max) NULL,
	[LastbidderId] [int] NULL,
 CONSTRAINT [PK_Auction_1] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Bid]    Script Date: 2.2.2019. 17.43.18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Bid](
	[TokensNumber] [int] NULL,
	[BiddingTime] [date] NOT NULL,
	[idAuction] [int] NOT NULL,
	[idUser] [int] NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SystemParameters]    Script Date: 2.2.2019. 17.43.18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SystemParameters](
	[Currency] [decimal](10, 3) NULL,
	[Silver] [int] NULL,
	[Gold] [int] NULL,
	[Platnium] [int] NULL,
	[TokensValue] [decimal](10, 3) NULL,
	[ItemsPerPage] [int] NOT NULL,
	[Id] [int] IDENTITY(1,1) NOT NULL,
 CONSTRAINT [PK_SystemParameters_1] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TokenOrders]    Script Date: 2.2.2019. 17.43.18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TokenOrders](
	[TokensNumber] [int] NOT NULL,
	[Price] [decimal](10, 3) NULL,
	[State] [nvarchar](50) NULL,
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[IdUser] [int] NOT NULL,
	[OrderTime] [datetime] NULL,
 CONSTRAINT [PK_TokenOrders_1] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[User]    Script Date: 2.2.2019. 17.43.18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[User](
	[Firstname] [nvarchar](50) NOT NULL,
	[Lastname] [nvarchar](50) NOT NULL,
	[Email] [nvarchar](max) NOT NULL,
	[Password] [nvarchar](max) NOT NULL,
	[TokensNumber] [int] NULL,
	[Role] [nvarchar](10) NULL,
	[IdUser] [int] IDENTITY(1,1) NOT NULL,
 CONSTRAINT [PK_User_1] PRIMARY KEY CLUSTERED 
(
	[IdUser] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [dbo].[Bid]  WITH CHECK ADD  CONSTRAINT [FK_Bid_Auction] FOREIGN KEY([idAuction])
REFERENCES [dbo].[Auction] ([Id])
GO
ALTER TABLE [dbo].[Bid] CHECK CONSTRAINT [FK_Bid_Auction]
GO
ALTER TABLE [dbo].[Bid]  WITH CHECK ADD  CONSTRAINT [FK_Bid_User] FOREIGN KEY([idUser])
REFERENCES [dbo].[User] ([IdUser])
GO
ALTER TABLE [dbo].[Bid] CHECK CONSTRAINT [FK_Bid_User]
GO
ALTER TABLE [dbo].[TokenOrders]  WITH CHECK ADD  CONSTRAINT [FK_TokenOrders_User] FOREIGN KEY([IdUser])
REFERENCES [dbo].[User] ([IdUser])
GO
ALTER TABLE [dbo].[TokenOrders] CHECK CONSTRAINT [FK_TokenOrders_User]
GO
USE [master]
GO
ALTER DATABASE [baza_iep] SET  READ_WRITE 
GO
