USE [VORegTAP]
GO

/*** Database creation script vor voresource 1.1. 03/19/2018

Creates harvester-related tables, only required if the VAO/STScI harvester service will be run against this database.

Related files:
	* VOResourceCreate - Basic table generation for minimal database schema and search.
    * RofRFillHarvesterTables - Snapshot of RofR contents for harvesting, to go in these tables.
***/


/****** Object:  Table [dbo].[HarvesterLog]    Script Date: 04/01/2013 16:12:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[HarvesterLog](
	[ServiceURL] [nvarchar](100) NOT NULL,
	[date] [datetime] NOT NULL,
	[type] [nchar](10) NOT NULL,
	[status] [int] NULL,
	[message] [text] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

GO
/****** Object:  Table [dbo].[Harvester]    Script Date: 3/18/2018  ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Harvester](
	[ServiceURL] [nvarchar](100) NOT NULL,
	[RegistryID] [nvarchar](100) NOT NULL,
	[DoHarvest] [bit] NOT NULL
) ON [PRIMARY]