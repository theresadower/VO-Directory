USE [VORegTAP]
GO

/****** Object:  Table [dbo].[ResumptionInformation]    Script Date: 11/2/2016 11:19:37 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[ResumptionInformation](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[tokenValue] [varchar](500) NOT NULL,
	[expirationDate] [datetime] NOT NULL,
	[fromDate] [datetime] NULL,
	[untilDate] [datetime] NULL,
	[metadataPrefix] [varchar](100) NOT NULL,
	[setName] [varchar](500) NULL,
	[startIdx] [int] NOT NULL,
	[completeListSize] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

