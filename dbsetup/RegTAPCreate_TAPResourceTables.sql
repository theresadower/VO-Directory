USE [VORegTAP]
GO
/****** Object:  Table [rr].[capability]    Script Date: 2/18/2016 4:21:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING OFF
GO
CREATE TABLE [rr].[capability](
	[ivoid] [varchar](512) NOT NULL,
	[cap_index] [smallint] NOT NULL,
	[cap_type] [varchar](1024) NULL,
	[cap_description] [varchar](max) NULL,
	[standard_id] [varchar](1024) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [rr].[interface]    Script Date: 2/18/2016 4:21:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING OFF
GO
CREATE TABLE [rr].[interface](
	[ivoid] [varchar](512) NOT NULL,
	[cap_index] [smallint] NOT NULL,
	[intf_index] [smallint] NOT NULL,
	[intf_type] [varchar](1024) NULL,
	[intf_role] [varchar](1024) NULL,
	[std_version] [varchar](1024) NULL,
	[query_type] [varchar](1024) NULL,
	[result_type] [varchar](1024) NULL,
	[wsdl_url] [varchar](1024) NULL,
	[url_use] [varchar](1024) NOT NULL,
	[access_url] [varchar](1024) NOT NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [rr].[intf_param]    Script Date: 2/18/2016 4:21:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING OFF
GO
CREATE TABLE [rr].[intf_param](
	[ivoid] [varchar](512) NOT NULL,
	[cap_index] [smallint] NOT NULL,
	[intf_index] [smallint] NOT NULL,
	[name] [varchar](128) NOT NULL,
	[datatype] [varchar](1024) NULL,
	[param_description] [varchar](max) NULL,
	[ucd] [varchar](1024) NULL,
	[unit] [varchar](1024) NULL,
	[utype] [varchar](1024) NULL,
	[flag] [varchar](1024) NULL,
	[std] [smallint] NOT NULL,
	[extended_schema] [varchar](1024) NULL,
	[extended_type] [varchar](1024) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
SET ANSI_PADDING ON
ALTER TABLE [rr].[intf_param] ADD [arraysize] [varchar](1024) NULL
ALTER TABLE [rr].[intf_param] ADD [delim] [varchar](1024) NULL
ALTER TABLE [rr].[intf_param] ADD [param_use] [varchar](1024) NULL

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [rr].[relationship]    Script Date: 2/18/2016 4:21:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING OFF
GO
CREATE TABLE [rr].[relationship](
	[ivoid] [varchar](512) NOT NULL,
	[relationship_type] [varchar](1024) NOT NULL,
	[related_id] [varchar](1024) NULL,
	[related_name] [varchar](1024) NOT NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [rr].[res_date]    Script Date: 2/18/2016 4:21:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING OFF
GO
CREATE TABLE [rr].[res_date](
	[ivoid] [varchar](512) NOT NULL,
	[date_value] [date] NOT NULL,
	[value_role] [varchar](1024) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [rr].[res_detail]    Script Date: 2/18/2016 4:21:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING OFF
GO
CREATE TABLE [rr].[res_detail](
	[ivoid] [varchar](512) NOT NULL,
	[cap_index] [smallint] NULL,
	[detail_value] [varchar](max) NOT NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
SET ANSI_PADDING ON
ALTER TABLE [rr].[res_detail] ADD [detail_xpath] [varchar](1024) NULL

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [rr].[res_role]    Script Date: 2/18/2016 4:21:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING OFF
GO
CREATE TABLE [rr].[res_role](
	[ivoid] [varchar](512) NOT NULL,
	[role_name] [varchar](1024) NOT NULL,
	[role_ivoid] [varchar](512) NULL,
	[street_address] [varchar](1024) NULL,
	[email] [varchar](1024) NULL,
	[telephone] [varchar](128) NULL,
	[logo] [varchar](1024) NULL
) ON [PRIMARY]
SET ANSI_PADDING ON
ALTER TABLE [rr].[res_role] ADD [base_role] [varchar](1024) NULL

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [rr].[res_schema]    Script Date: 2/18/2016 4:21:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING OFF
GO
CREATE TABLE [rr].[res_schema](
	[ivoid] [varchar](512) NOT NULL,
	[schema_index] [smallint] NOT NULL,
	[schema_name] [varchar](1024) NULL,
	[schema_title] [varchar](1024) NULL,
	[schema_description] [varchar](max) NULL,
	[schema_utype] [varchar](1024) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [rr].[res_subject]    Script Date: 2/18/2016 4:21:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING OFF
GO
CREATE TABLE [rr].[res_subject](
	[ivoid] [varchar](512) NOT NULL,
	[res_subject] [varchar](1024) NOT NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [rr].[res_table]    Script Date: 2/18/2016 4:21:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING OFF
GO
CREATE TABLE [rr].[res_table](
	[schema_index] [smallint] NOT NULL,
	[ivoid] [varchar](512) NOT NULL,
	[table_description] [varchar](max) NULL,
	[table_name] [varchar](1024) NOT NULL,
	[table_index] [smallint] NOT NULL,
	[table_title] [varchar](1024) NULL,
	[table_type] [varchar](1024) NULL,
	[table_utype] [varchar](1024) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [rr].[resource]    Script Date: 2/18/2016 4:21:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING OFF
GO
CREATE TABLE [rr].[resource](
	[ivoid] [varchar](512) NOT NULL,
	[res_type] [varchar](32) NOT NULL,
	[created] [datetime2](7) NOT NULL,
	[updated] [datetime2](7) NOT NULL,
	[short_name] [varchar](24) NULL,
	[res_title] [varchar](512) NOT NULL,
	[content_level] [varchar](1024) NULL,
	[res_description] [varchar](max) NOT NULL,
	[reference_url] [varchar](1024) NOT NULL,
	[content_type] [varchar](1024) NULL,
	[source_format] [varchar](1024) NULL,
	[source_value] [varchar](1024) NULL,
	[res_version] [varchar](32) NULL,
	[region_of_regard] [real] NULL,
	[waveband] [varchar](128) NULL,
	[rights] [varchar](1024) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
SET ANSI_PADDING ON
ALTER TABLE [rr].[resource] ADD [creator_seq] [varchar](2048) NULL

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [rr].[table_column]    Script Date: 2/18/2016 4:21:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING OFF
GO
CREATE TABLE [rr].[table_column](
	[ivoid] [varchar](512) NOT NULL,
	[table_index] [smallint] NOT NULL,
	[name] [varchar](200) NOT NULL,
	[datatype] [varchar](1024) NULL,
	[column_description] [varchar](max) NULL,
	[ucd] [varchar](1024) NULL,
	[unit] [varchar](1024) NULL,
	[utype] [varchar](1024) NULL,
	[flag] [varchar](1024) NULL,
	[std] [smallint] NULL,
	[extended_schema] [varchar](1024) NULL,
	[extended_type] [varchar](1024) NULL,
	[arraysize] [varchar](1024) NULL,
	[delim] [varchar](1024) NULL,
	[type_system] [varchar](1024) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [rr].[validation]    Script Date: 2/18/2016 4:21:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING OFF
GO
CREATE TABLE [rr].[validation](
	[ivoid] [varchar](512) NOT NULL,
	[validated_by] [varchar](1024) NOT NULL,
	[val_level] [smallint] NOT NULL,
	[cap_index] [smallint] NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
