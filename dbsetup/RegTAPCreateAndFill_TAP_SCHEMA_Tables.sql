USE [VORegTAP]
GO
/****** Object:  Table [TAP_SCHEMA].[columns]    Script Date: 2/18/2016 4:23:26 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [TAP_SCHEMA].[columns](
	[table_name] [varchar](max) NULL,
	[column_name] [varchar](max) NULL,
	[description] [varchar](max) NULL,
	[unit] [varchar](max) NULL,
	[ucd] [varchar](max) NULL,
	[utype] [varchar](max) NULL,
	[datatype] [varchar](max) NULL,
	[size] [int] NULL,
	[principal] [int] NULL,
	[indexed] [int] NULL,
	[std] [int] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [TAP_SCHEMA].[key_columns]    Script Date: 2/18/2016 4:23:26 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [TAP_SCHEMA].[key_columns](
	[key_id] [varchar](max) NULL,
	[from_column] [varchar](max) NULL,
	[target_column] [varchar](max) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [TAP_SCHEMA].[keys]    Script Date: 2/18/2016 4:23:26 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [TAP_SCHEMA].[keys](
	[key_id] [varchar](max) NULL,
	[from_table] [varchar](max) NULL,
	[target_table] [varchar](max) NULL,
	[description] [varchar](max) NULL,
	[utype] [varchar](max) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [TAP_SCHEMA].[schemas]    Script Date: 2/18/2016 4:23:26 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [TAP_SCHEMA].[schemas](
	[schema_name] [varchar](max) NULL,
	[description] [varchar](max) NULL,
	[utype] [varchar](max) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [TAP_SCHEMA].[tables]    Script Date: 2/18/2016 4:23:26 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [TAP_SCHEMA].[tables](
	[schema_name] [varchar](max) NULL,
	[table_name] [varchar](max) NULL,
	[table_type] [varchar](max) NULL,
	[description] [varchar](max) NULL,
	[utype] [varchar](max) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
INSERT [TAP_SCHEMA].[columns] ([table_name], [column_name], [description], [unit], [ucd], [utype], [datatype], [size], [principal], [indexed], [std]) VALUES (N'rr.resource', N'ivoid', N'Unambiguous reference to the resource conforming to the IVOA standard for identifiers', NULL, NULL, N'xpath:identifier', N'char', NULL, 1, 1, NULL)
GO
INSERT [TAP_SCHEMA].[columns] ([table_name], [column_name], [description], [unit], [ucd], [utype], [datatype], [size], [principal], [indexed], [std]) VALUES (N'rr.resource', N'res_type', N'Resource type (something like vs:datacollection, vs:catalogservice, etc)', NULL, NULL, N'xpath:@xsi:type', N'char', NULL, NULL, NULL, NULL)
GO
INSERT [TAP_SCHEMA].[columns] ([table_name], [column_name], [description], [unit], [ucd], [utype], [datatype], [size], [principal], [indexed], [std]) VALUES (N'rr.resource', N'created', N'The UTC date and time this resource metadata description was created. This timestamp must not be in the future. This time is not required to be accurate; it should be at least accurate to the day. Any insignificant time fields should be set to zero.', NULL, NULL, N'xpath:@created', N'char', NULL, NULL, NULL, NULL)
GO
INSERT [TAP_SCHEMA].[columns] ([table_name], [column_name], [description], [unit], [ucd], [utype], [datatype], [size], [principal], [indexed], [std]) VALUES (N'rr.resource', N'creator_seq', N'	The creator(s) of the resource in the order given by the resource record author, separated by semicolons.', NULL, NULL, N'xpath:curation/creator/name
', N'char', NULL, NULL, NULL, NULL)
GO
INSERT [TAP_SCHEMA].[columns] ([table_name], [column_name], [description], [unit], [ucd], [utype], [datatype], [size], [principal], [indexed], [std]) VALUES (N'rr.resource', N'content_type', N'	A hash-separated list of natures or genres of the content of the resource.', NULL, NULL, N'xpath:content/type', N'char', NULL, NULL, NULL, NULL)
GO
INSERT [TAP_SCHEMA].[columns] ([table_name], [column_name], [description], [unit], [ucd], [utype], [datatype], [size], [principal], [indexed], [std]) VALUES (N'rr.resource', N'source_format', N'	The format of source_value. Recognized values include "bibcode", referring to a standard astronomical bibcode (http://cdsweb.u-strasbg.fr/simbad/refcode.html).', NULL, NULL, N'xpath:content/source/@format
', N'char', NULL, NULL, NULL, NULL)
GO
INSERT [TAP_SCHEMA].[columns] ([table_name], [column_name], [description], [unit], [ucd], [utype], [datatype], [size], [principal], [indexed], [std]) VALUES (N'rr.resource', N'source_value', N'A bibliographic reference from which the present resource is derived or extracted.', NULL, NULL, N'xpath:content/source
', N'char', NULL, NULL, NULL, NULL)
GO
INSERT [TAP_SCHEMA].[columns] ([table_name], [column_name], [description], [unit], [ucd], [utype], [datatype], [size], [principal], [indexed], [std]) VALUES (N'rr.resource', N'res_version', N'	Label associated with creation or availablilty of a version of a resource.', NULL, NULL, N'xpath:curation/version
', N'char', NULL, NULL, NULL, NULL)
GO
INSERT [TAP_SCHEMA].[columns] ([table_name], [column_name], [description], [unit], [ucd], [utype], [datatype], [size], [principal], [indexed], [std]) VALUES (N'rr.resource', N'region_of_regard', N'	A single numeric value representing the angle, given in decimal degrees, by which a positional query against this resource should be "blurred" in order to get an appropriate match.', NULL, NULL, N'xpath:coverage/regionOfRegard
', N'float', NULL, NULL, NULL, NULL)
GO
INSERT [TAP_SCHEMA].[columns] ([table_name], [column_name], [description], [unit], [ucd], [utype], [datatype], [size], [principal], [indexed], [std]) VALUES (N'rr.resource', N'waveband', N'	A hash-separated list of regions of the electro-magnetic spectrum that the resource''s spectral coverage overlaps with.', NULL, NULL, N'xpath:coverage/waveband
', N'char', NULL, NULL, NULL, NULL)
GO
INSERT [TAP_SCHEMA].[columns] ([table_name], [column_name], [description], [unit], [ucd], [utype], [datatype], [size], [principal], [indexed], [std]) VALUES (N'rr.resource', N'rights', N'Information about rights held in and over the resource (multiple values are separated by hashes).', NULL, NULL, N'xpath:rights
', N'char', NULL, NULL, NULL, NULL)
GO
INSERT [TAP_SCHEMA].[columns] ([table_name], [column_name], [description], [unit], [ucd], [utype], [datatype], [size], [principal], [indexed], [std]) VALUES (N'rr.intf_param', N'arraysize', N'The shape of the array that constitutes the value, e.g., 4, *, 4*, 5x4, or 5x*, as specified by VOTable.', NULL, NULL, N'
xpath:dataType/@arraysize
', N'char', NULL, NULL, NULL, NULL)
GO
INSERT [TAP_SCHEMA].[columns] ([table_name], [column_name], [description], [unit], [ucd], [utype], [datatype], [size], [principal], [indexed], [std]) VALUES (N'rr.intf_param', N'delim', N'The string that is used to delimit elements of an array value when arraysize is not ''1''.', NULL, NULL, N'xpath:dataType/@delim
', N'char', NULL, NULL, NULL, NULL)
GO
INSERT [TAP_SCHEMA].[columns] ([table_name], [column_name], [description], [unit], [ucd], [utype], [datatype], [size], [principal], [indexed], [std]) VALUES (N'rr.intf_param', N'param_use', N'An indication of whether this parameter is required to be provided for the application or service to work properly (one of required, optional, ignored, or NULL).', NULL, NULL, N'xpath:@use
', N'char', NULL, NULL, NULL, NULL)
GO
INSERT [TAP_SCHEMA].[columns] ([table_name], [column_name], [description], [unit], [ucd], [utype], [datatype], [size], [principal], [indexed], [std]) VALUES (N'rr.res_detail', N'detail_xpath', N'The xpath of the data item.', NULL, NULL, NULL, N'char', NULL, NULL, NULL, NULL)
GO
INSERT [TAP_SCHEMA].[columns] ([table_name], [column_name], [description], [unit], [ucd], [utype], [datatype], [size], [principal], [indexed], [std]) VALUES (N'rr.res_schema', N'ivoid', N'The parent resource.', NULL, NULL, N'xpath:/identifier', N'char', NULL, 1, 1, NULL)
GO
INSERT [TAP_SCHEMA].[columns] ([table_name], [column_name], [description], [unit], [ucd], [utype], [datatype], [size], [principal], [indexed], [std]) VALUES (N'rr.res_schema', N'schema_index', N'An arbitrary identifier for the res_schema rows belonging to a resource.', NULL, NULL, NULL, N'short', NULL, 1, NULL, NULL)
GO
INSERT [TAP_SCHEMA].[columns] ([table_name], [column_name], [description], [unit], [ucd], [utype], [datatype], [size], [principal], [indexed], [std]) VALUES (N'rr.res_schema', N'schema_description', N'A free text description of the tableset explaining in general how all of the tables are related.', NULL, NULL, N'xpath:description', N'char', NULL, NULL, NULL, NULL)
GO
INSERT [TAP_SCHEMA].[columns] ([table_name], [column_name], [description], [unit], [ucd], [utype], [datatype], [size], [principal], [indexed], [std]) VALUES (N'rr.res_schema', N'schema_name', N'A name for the set of tables.', NULL, NULL, N'xpath:name', N'char', NULL, NULL, NULL, NULL)
GO
INSERT [TAP_SCHEMA].[columns] ([table_name], [column_name], [description], [unit], [ucd], [utype], [datatype], [size], [principal], [indexed], [std]) VALUES (N'rr.res_schema', N'schema_title', N'A descriptibe, human-interpretable name for the table set', NULL, NULL, N'xpath:title', N'char', NULL, NULL, NULL, NULL)
GO
INSERT [TAP_SCHEMA].[columns] ([table_name], [column_name], [description], [unit], [ucd], [utype], [datatype], [size], [principal], [indexed], [std]) VALUES (N'rr.res_schema', N'schema_utype', N'An identifier for a concept in a data model that the data in this schema as a whole represent.', NULL, NULL, N'xpath:utype', N'char', NULL, NULL, NULL, NULL)
GO
INSERT [TAP_SCHEMA].[columns] ([table_name], [column_name], [description], [unit], [ucd], [utype], [datatype], [size], [principal], [indexed], [std]) VALUES (N'rr.res_subject', N'ivoid', N'The parent resource', NULL, NULL, N'xpath:/identifier', N'char', NULL, 1, 1, NULL)
GO
INSERT [TAP_SCHEMA].[columns] ([table_name], [column_name], [description], [unit], [ucd], [utype], [datatype], [size], [principal], [indexed], [std]) VALUES (N'rr.res_subject', N'res_subject', N'Topics, object types, or other descriptive keywords about the resource', NULL, NULL, N'xpath:subject', N'char', NULL, NULL, NULL, NULL)
GO
INSERT [TAP_SCHEMA].[columns] ([table_name], [column_name], [description], [unit], [ucd], [utype], [datatype], [size], [principal], [indexed], [std]) VALUES (N'rr.res_table', N'ivoid', N'The parent resource.', NULL, NULL, N'xpath:/identifier', N'char', NULL, 1, 1, NULL)
GO
INSERT [TAP_SCHEMA].[columns] ([table_name], [column_name], [description], [unit], [ucd], [utype], [datatype], [size], [principal], [indexed], [std]) VALUES (N'rr.res_table', N'schema_index', N'Index of the schema this table belongs to, if it belongs to a schema (otherwise NULL).', NULL, NULL, NULL, N'short', NULL, NULL, NULL, NULL)
GO
INSERT [TAP_SCHEMA].[columns] ([table_name], [column_name], [description], [unit], [ucd], [utype], [datatype], [size], [principal], [indexed], [std]) VALUES (N'rr.res_table', N'table_description', N'A free-text description of the table''s contents', NULL, NULL, N'xpath:description', N'char', NULL, NULL, NULL, NULL)
GO
INSERT [TAP_SCHEMA].[columns] ([table_name], [column_name], [description], [unit], [ucd], [utype], [datatype], [size], [principal], [indexed], [std]) VALUES (N'rr.res_table', N'table_name', N'	The fully qualified name of the table. This name should include all catalog or schema prefixes needed to distinguish it in a query.', NULL, NULL, N'xpath:name', N'char', NULL, NULL, NULL, NULL)
GO
INSERT [TAP_SCHEMA].[columns] ([table_name], [column_name], [description], [unit], [ucd], [utype], [datatype], [size], [principal], [indexed], [std]) VALUES (N'rr.res_table', N'table_index', N'An arbitrary identifier for the tables belonging to a resource.', NULL, NULL, NULL, N'short', NULL, 1, NULL, NULL)
GO
INSERT [TAP_SCHEMA].[columns] ([table_name], [column_name], [description], [unit], [ucd], [utype], [datatype], [size], [principal], [indexed], [std]) VALUES (N'rr.res_table', N'table_title', N'A descriptive, human-interpretable name for the table.', NULL, NULL, N'xpath:title', N'char', NULL, NULL, NULL, NULL)
GO
INSERT [TAP_SCHEMA].[columns] ([table_name], [column_name], [description], [unit], [ucd], [utype], [datatype], [size], [principal], [indexed], [std]) VALUES (N'rr.res_table', N'table_type', N'A name for the role this table plays. Recognized values include "output", indicating this table is output from a query; "base_table", indicating a table whose records represent the main subjects of its schema; and "view", indicating that the table represents a useful combination or subset of other tables. Other values are allowed.', NULL, NULL, N'xpath:@type', N'char', NULL, NULL, NULL, NULL)
GO
INSERT [TAP_SCHEMA].[columns] ([table_name], [column_name], [description], [unit], [ucd], [utype], [datatype], [size], [principal], [indexed], [std]) VALUES (N'rr.res_table', N'table_utype', N'	An identifier for a concept in a data model that the data in this table as a whole represent.', NULL, NULL, N'xpath:utype', N'char', NULL, NULL, NULL, NULL)
GO
INSERT [TAP_SCHEMA].[columns] ([table_name], [column_name], [description], [unit], [ucd], [utype], [datatype], [size], [principal], [indexed], [std]) VALUES (N'rr.table_column', N'ivoid', N'The parent resource.', NULL, NULL, N'xpath:/identifier
', N'char', NULL, 1, 1, NULL)
GO
INSERT [TAP_SCHEMA].[columns] ([table_name], [column_name], [description], [unit], [ucd], [utype], [datatype], [size], [principal], [indexed], [std]) VALUES (N'rr.table_column', N'table_index', NULL, NULL, NULL, NULL, N'short', NULL, 1, NULL, NULL)
GO
INSERT [TAP_SCHEMA].[columns] ([table_name], [column_name], [description], [unit], [ucd], [utype], [datatype], [size], [principal], [indexed], [std]) VALUES (N'rr.table_column', N'name', N'The name of the column.', NULL, NULL, N'xpath:name
', N'char', NULL, NULL, NULL, NULL)
GO
INSERT [TAP_SCHEMA].[columns] ([table_name], [column_name], [description], [unit], [ucd], [utype], [datatype], [size], [principal], [indexed], [std]) VALUES (N'rr.table_column', N'ucd', N'	A unified content descriptor that describes the scientific content of the parameter.', NULL, NULL, N'xpath:ucd', N'char', NULL, NULL, NULL, NULL)
GO
INSERT [TAP_SCHEMA].[columns] ([table_name], [column_name], [description], [unit], [ucd], [utype], [datatype], [size], [principal], [indexed], [std]) VALUES (N'rr.table_column', N'unit', N'The unit associated with all values in the column.', NULL, NULL, N'xpath:unit', N'char', NULL, NULL, NULL, NULL)
GO
INSERT [TAP_SCHEMA].[columns] ([table_name], [column_name], [description], [unit], [ucd], [utype], [datatype], [size], [principal], [indexed], [std]) VALUES (N'rr.table_column', N'utype', N'An identifier for a role in a data model that the data in this column represents.', NULL, NULL, N'xpath:utype', N'char', NULL, NULL, NULL, NULL)
GO
INSERT [TAP_SCHEMA].[columns] ([table_name], [column_name], [description], [unit], [ucd], [utype], [datatype], [size], [principal], [indexed], [std]) VALUES (N'rr.table_column', N'std', N'If 1, the meaning and use of this parameter is reserved and defined by a standard model. If 0, it represents a database-specific parameter that effectively extends beyond the standard.', NULL, NULL, N'xpath:@std', N'short', NULL, NULL, NULL, NULL)
GO
INSERT [TAP_SCHEMA].[columns] ([table_name], [column_name], [description], [unit], [ucd], [utype], [datatype], [size], [principal], [indexed], [std]) VALUES (N'rr.table_column', N'datatype', N'The type of the data contained in the column.', NULL, NULL, N'xpath:dataType', N'char', NULL, NULL, NULL, NULL)
GO
INSERT [TAP_SCHEMA].[columns] ([table_name], [column_name], [description], [unit], [ucd], [utype], [datatype], [size], [principal], [indexed], [std]) VALUES (N'rr.table_column', N'extended_schema', N'An identifier for the schema that the value given by the extended attribute is drawn from.', NULL, NULL, N'xpath:dataType/@extendedSchema', N'char', NULL, NULL, NULL, NULL)
GO
INSERT [TAP_SCHEMA].[columns] ([table_name], [column_name], [description], [unit], [ucd], [utype], [datatype], [size], [principal], [indexed], [std]) VALUES (N'rr.table_column', N'extended_type', N'A custom type for the values this column contains.', NULL, NULL, N'xpath:dataType/@extendedType', N'char', NULL, NULL, NULL, NULL)
GO
INSERT [TAP_SCHEMA].[columns] ([table_name], [column_name], [description], [unit], [ucd], [utype], [datatype], [size], [principal], [indexed], [std]) VALUES (N'rr.table_column', N'arraysize', N'	The shape of the array that constitutes the value, e.g., 4, *, 4*, 5x4, or 5x*, as specified by VOTable.', NULL, NULL, N'xpath:dataType/@arraysize', N'char', NULL, NULL, NULL, NULL)
GO
INSERT [TAP_SCHEMA].[columns] ([table_name], [column_name], [description], [unit], [ucd], [utype], [datatype], [size], [principal], [indexed], [std]) VALUES (N'rr.table_column', N'delim', N'The string that is used to delimit elements of an array value when arraysize is not ''1''.', NULL, NULL, N'xpath:dataType/@delim', N'char', NULL, NULL, NULL, NULL)
GO
INSERT [TAP_SCHEMA].[columns] ([table_name], [column_name], [description], [unit], [ucd], [utype], [datatype], [size], [principal], [indexed], [std]) VALUES (N'rr.table_column', N'type_system', N'The type system used, as a QName with a canonical prefix; this will ususally be one of vs:simpledatatype, vs:votabletype, and vs:taptype.', NULL, NULL, N'xpath:dataType/@xsi:type', N'char', NULL, NULL, NULL, NULL)
GO
INSERT [TAP_SCHEMA].[columns] ([table_name], [column_name], [description], [unit], [ucd], [utype], [datatype], [size], [principal], [indexed], [std]) VALUES (N'rr.table_column', N'flag', N'Hash-separated keywords representing traits of the column. Recognized values include "indexed", "primary", and "nullable".', NULL, NULL, N'xpath:flag', N'char', NULL, NULL, NULL, NULL)
GO
INSERT [TAP_SCHEMA].[columns] ([table_name], [column_name], [description], [unit], [ucd], [utype], [datatype], [size], [principal], [indexed], [std]) VALUES (N'rr.table_column', N'column_description', N'	A free-text description of the column''s contents.', NULL, NULL, N'xpath:description', N'char', NULL, NULL, NULL, NULL)
GO
INSERT [TAP_SCHEMA].[columns] ([table_name], [column_name], [description], [unit], [ucd], [utype], [datatype], [size], [principal], [indexed], [std]) VALUES (N'rr.validation', N'ivoid', N'The parent resource', NULL, NULL, N'xpath:/identifier', N'char', NULL, 1, 1, NULL)
GO
INSERT [TAP_SCHEMA].[columns] ([table_name], [column_name], [description], [unit], [ucd], [utype], [datatype], [size], [principal], [indexed], [std]) VALUES (N'rr.resource', N'short_name', N'A short name or abbreviation given to something. This name will be used where brief annotations for the resource name are required. Applications may use it to refer to the resource in a compact display. One word or a few letters is recommended. No more than sixteen characters are allowed.', NULL, NULL, N'xpath:shortName', N'char', NULL, NULL, NULL, NULL)
GO
INSERT [TAP_SCHEMA].[columns] ([table_name], [column_name], [description], [unit], [ucd], [utype], [datatype], [size], [principal], [indexed], [std]) VALUES (N'rr.resource', N'res_title', N'The full name given to the resource', NULL, NULL, N'xpath:title', N'unicodeChar', NULL, NULL, 1, NULL)
GO
INSERT [TAP_SCHEMA].[columns] ([table_name], [column_name], [description], [unit], [ucd], [utype], [datatype], [size], [principal], [indexed], [std]) VALUES (N'rr.resource', N'updated', N'The UTC date this resource metadata description was last updated. This timestamp must not be in the future. This time is not required to be accurate; it should be at least accurate to the day. Any insignificant time fields should be set to zero.', NULL, NULL, N'xpath:@updated', N'char', NULL, NULL, NULL, NULL)
GO
INSERT [TAP_SCHEMA].[columns] ([table_name], [column_name], [description], [unit], [ucd], [utype], [datatype], [size], [principal], [indexed], [std]) VALUES (N'rr.resource', N'content_level', N'A hash-separated list of content levels specifying the intended audience', NULL, NULL, N'xpath:content/contentLevel', N'char', NULL, NULL, NULL, NULL)
GO
INSERT [TAP_SCHEMA].[columns] ([table_name], [column_name], [description], [unit], [ucd], [utype], [datatype], [size], [principal], [indexed], [std]) VALUES (N'rr.resource', N'res_description', N'An account of the nature of the resource.', NULL, NULL, N'xpath:content/description', N'unicodeChar', NULL, NULL, 1, NULL)
GO
INSERT [TAP_SCHEMA].[columns] ([table_name], [column_name], [description], [unit], [ucd], [utype], [datatype], [size], [principal], [indexed], [std]) VALUES (N'rr.resource', N'reference_url', N'URL pointing to a human-readable document describing this resource.', NULL, NULL, N'xpath:content/referenceURL', N'char', NULL, NULL, NULL, NULL)
GO
INSERT [TAP_SCHEMA].[columns] ([table_name], [column_name], [description], [unit], [ucd], [utype], [datatype], [size], [principal], [indexed], [std]) VALUES (N'rr.capability', N'ivoid', N'The parent resource.', NULL, NULL, N'xpath:/identifier', N'char', NULL, 1, 1, NULL)
GO
INSERT [TAP_SCHEMA].[columns] ([table_name], [column_name], [description], [unit], [ucd], [utype], [datatype], [size], [principal], [indexed], [std]) VALUES (N'rr.capability', N'cap_index', N'An arbitrary identifier of this capability within the resource.', NULL, NULL, NULL, N'short', NULL, 1, 1, NULL)
GO
INSERT [TAP_SCHEMA].[columns] ([table_name], [column_name], [description], [unit], [ucd], [utype], [datatype], [size], [principal], [indexed], [std]) VALUES (N'rr.capability', N'cap_type', N'The type of capability covered here.', NULL, NULL, N'xpath:@xsi:type', N'char', NULL, NULL, NULL, NULL)
GO
INSERT [TAP_SCHEMA].[columns] ([table_name], [column_name], [description], [unit], [ucd], [utype], [datatype], [size], [principal], [indexed], [std]) VALUES (N'rr.capability', N'cap_description', N'A human-readable description of what this capability provides as part of the over-all service', NULL, NULL, N'xpath:description', N'unicodeChar', NULL, NULL, NULL, NULL)
GO
INSERT [TAP_SCHEMA].[columns] ([table_name], [column_name], [description], [unit], [ucd], [utype], [datatype], [size], [principal], [indexed], [std]) VALUES (N'rr.capability', N'standard_id', N'A URI for a standard this capability conforms to.', NULL, NULL, N'xpath:@standardID', N'char', NULL, NULL, NULL, NULL)
GO
INSERT [TAP_SCHEMA].[columns] ([table_name], [column_name], [description], [unit], [ucd], [utype], [datatype], [size], [principal], [indexed], [std]) VALUES (N'rr.res_date', N'ivoid', N'The parent resource.', NULL, NULL, N'xpath:/identifier', N'char', NULL, NULL, 1, NULL)
GO
INSERT [TAP_SCHEMA].[columns] ([table_name], [column_name], [description], [unit], [ucd], [utype], [datatype], [size], [principal], [indexed], [std]) VALUES (N'rr.res_date', N'date_value', N'A date associated with an event in the life cycle of the resource.', NULL, NULL, N'xpath:date', N'char', NULL, NULL, NULL, NULL)
GO
INSERT [TAP_SCHEMA].[columns] ([table_name], [column_name], [description], [unit], [ucd], [utype], [datatype], [size], [principal], [indexed], [std]) VALUES (N'rr.res_date', N'value_role', N'A string indicating what the date refers to, e.g., created, availability, updated.', NULL, NULL, N'xpath:date/@role', N'char', NULL, NULL, NULL, NULL)
GO
INSERT [TAP_SCHEMA].[columns] ([table_name], [column_name], [description], [unit], [ucd], [utype], [datatype], [size], [principal], [indexed], [std]) VALUES (N'rr.interface', N'ivoid', N'The parent resource.', NULL, NULL, N'xpath:/identifier', N'char', NULL, 1, 1, NULL)
GO
INSERT [TAP_SCHEMA].[columns] ([table_name], [column_name], [description], [unit], [ucd], [utype], [datatype], [size], [principal], [indexed], [std]) VALUES (N'rr.interface', N'cap_index', N'The index of the parent capability.', NULL, NULL, NULL, N'short', NULL, NULL, NULL, NULL)
GO
INSERT [TAP_SCHEMA].[columns] ([table_name], [column_name], [description], [unit], [ucd], [utype], [datatype], [size], [principal], [indexed], [std]) VALUES (N'rr.interface', N'intf_index', N'An arbitrary identifier for the interfaces of a resource.', NULL, NULL, NULL, N'short', NULL, 1, 1, NULL)
GO
INSERT [TAP_SCHEMA].[columns] ([table_name], [column_name], [description], [unit], [ucd], [utype], [datatype], [size], [principal], [indexed], [std]) VALUES (N'rr.interface', N'intf_type', N'The type of the interface (vr:webbrowser, vs:paramhttp, etc).', NULL, NULL, N'xpath:@xsi:type', N'char', NULL, NULL, NULL, NULL)
GO
INSERT [TAP_SCHEMA].[columns] ([table_name], [column_name], [description], [unit], [ucd], [utype], [datatype], [size], [principal], [indexed], [std]) VALUES (N'rr.interface', N'intf_role', N'An identifier for the role the interface plays in the particular capability. If the value is equal to "std" or begins with "std:", then the interface refers to a standard interface defined by the standard referred to by the capability''s standardID attribute.', NULL, NULL, N'xpath:@role', N'char', NULL, NULL, NULL, NULL)
GO
INSERT [TAP_SCHEMA].[columns] ([table_name], [column_name], [description], [unit], [ucd], [utype], [datatype], [size], [principal], [indexed], [std]) VALUES (N'rr.interface', N'std_version', N'The version of a standard interface specification that this interface complies with. When the interface is provided in the context of a Capability element, then the standard being refered to is the one identified by the Capability''s standardID element.', NULL, NULL, N'xpath:@version', N'char', NULL, NULL, NULL, NULL)
GO
INSERT [TAP_SCHEMA].[columns] ([table_name], [column_name], [description], [unit], [ucd], [utype], [datatype], [size], [principal], [indexed], [std]) VALUES (N'rr.interface', N'query_type', N'Hash-joined list of expected HTTP method (get or post) supported by the service.', NULL, NULL, N'xpath:queryType', N'char', NULL, NULL, NULL, NULL)
GO
INSERT [TAP_SCHEMA].[columns] ([table_name], [column_name], [description], [unit], [ucd], [utype], [datatype], [size], [principal], [indexed], [std]) VALUES (N'rr.interface', N'result_type', N'The MIME type of a document returned in the HTTP response.', NULL, NULL, N'xpath:resultType', N'char', NULL, NULL, NULL, NULL)
GO
INSERT [TAP_SCHEMA].[columns] ([table_name], [column_name], [description], [unit], [ucd], [utype], [datatype], [size], [principal], [indexed], [std]) VALUES (N'rr.interface', N'wsdl_url', N'The location of the WSDL that describes this Web Service. If NULL, the location can be assumed to be the accessURL with ''?wsdl'' appended.', NULL, NULL, N'xpath:wsdlURL', N'char', NULL, NULL, NULL, NULL)
GO
INSERT [TAP_SCHEMA].[columns] ([table_name], [column_name], [description], [unit], [ucd], [utype], [datatype], [size], [principal], [indexed], [std]) VALUES (N'rr.interface', N'url_use', N'A flag indicating whether this should be interpreted as a base URL (''base''), a full URL (''full''), or a URL to a directory that will produce a listing of files (''dir'').', NULL, NULL, N'xpath:accessURL/@use', N'char', NULL, NULL, NULL, NULL)
GO
INSERT [TAP_SCHEMA].[columns] ([table_name], [column_name], [description], [unit], [ucd], [utype], [datatype], [size], [principal], [indexed], [std]) VALUES (N'rr.interface', N'access_url', N'The URL at which the interface is found.', NULL, NULL, N'xpath:accessURL', N'char', NULL, NULL, NULL, NULL)
GO
INSERT [TAP_SCHEMA].[columns] ([table_name], [column_name], [description], [unit], [ucd], [utype], [datatype], [size], [principal], [indexed], [std]) VALUES (N'rr.validation', N'validated_by', N'	The IVOA ID of the registry or organisation that assigned the validation level.', NULL, NULL, N'xpath:validationLevel/@validatedBy', N'char', NULL, NULL, NULL, NULL)
GO
INSERT [TAP_SCHEMA].[columns] ([table_name], [column_name], [description], [unit], [ucd], [utype], [datatype], [size], [principal], [indexed], [std]) VALUES (N'rr.validation', N'cap_index', N'If non-NULL, the validation only refers to the capability referenced here.', NULL, NULL, NULL, N'short', NULL, NULL, NULL, NULL)
GO
INSERT [TAP_SCHEMA].[columns] ([table_name], [column_name], [description], [unit], [ucd], [utype], [datatype], [size], [principal], [indexed], [std]) VALUES (N'rr.intf_param', N'ivoid', N'The parent resource.', NULL, NULL, N'xpath:/identifier', N'char', NULL, NULL, NULL, NULL)
GO
INSERT [TAP_SCHEMA].[columns] ([table_name], [column_name], [description], [unit], [ucd], [utype], [datatype], [size], [principal], [indexed], [std]) VALUES (N'rr.intf_param', N'intf_index', N'The index of the interface this parameter belongs to.', NULL, NULL, NULL, N'short', NULL, NULL, NULL, NULL)
GO
INSERT [TAP_SCHEMA].[columns] ([table_name], [column_name], [description], [unit], [ucd], [utype], [datatype], [size], [principal], [indexed], [std]) VALUES (N'rr.intf_param', N'name', N'The name of the parameter.', NULL, NULL, N'xpath:name', N'char', NULL, NULL, NULL, NULL)
GO
INSERT [TAP_SCHEMA].[columns] ([table_name], [column_name], [description], [unit], [ucd], [utype], [datatype], [size], [principal], [indexed], [std]) VALUES (N'rr.intf_param', N'ucd', N'A unified content descriptor that describes the scientific content of the parameter.', NULL, NULL, N'xpath:ucd', N'char', NULL, NULL, NULL, NULL)
GO
INSERT [TAP_SCHEMA].[columns] ([table_name], [column_name], [description], [unit], [ucd], [utype], [datatype], [size], [principal], [indexed], [std]) VALUES (N'rr.intf_param', N'unit', N'The unit associated with all values in the parameter.', NULL, NULL, N'xpath:unit', N'char', NULL, NULL, NULL, NULL)
GO
INSERT [TAP_SCHEMA].[columns] ([table_name], [column_name], [description], [unit], [ucd], [utype], [datatype], [size], [principal], [indexed], [std]) VALUES (N'rr.intf_param', N'utype', N'An identifier for a role in a data model that the data in this parameter represents.', NULL, NULL, N'xpath:utype', N'char', NULL, NULL, NULL, NULL)
GO
INSERT [TAP_SCHEMA].[columns] ([table_name], [column_name], [description], [unit], [ucd], [utype], [datatype], [size], [principal], [indexed], [std]) VALUES (N'rr.intf_param', N'std', N'If 1, the meaning and use of this parameter is reserved and defined by a standard model. If 0, it represents a database-specific parameter that effectively extends beyond the standard.', NULL, NULL, N'xpath:@std', N'short', NULL, NULL, NULL, NULL)
GO
INSERT [TAP_SCHEMA].[columns] ([table_name], [column_name], [description], [unit], [ucd], [utype], [datatype], [size], [principal], [indexed], [std]) VALUES (N'rr.intf_param', N'datatype', N'The type of the data contained in the parameter.', NULL, NULL, N'xpath:dataType', N'char', NULL, NULL, NULL, NULL)
GO
INSERT [TAP_SCHEMA].[columns] ([table_name], [column_name], [description], [unit], [ucd], [utype], [datatype], [size], [principal], [indexed], [std]) VALUES (N'rr.intf_param', N'extended_schema', N'An identifier for the schema that the value given by the extended attribute is drawn from.', NULL, NULL, N'xpath:dataType/@extendedSchema', N'char', NULL, NULL, NULL, NULL)
GO
INSERT [TAP_SCHEMA].[columns] ([table_name], [column_name], [description], [unit], [ucd], [utype], [datatype], [size], [principal], [indexed], [std]) VALUES (N'rr.intf_param', N'extended_type', N'A custom type for the values this parameter contains.', NULL, NULL, N'xpath:dataType/@extendedType', N'char', NULL, NULL, NULL, NULL)
GO
INSERT [TAP_SCHEMA].[columns] ([table_name], [column_name], [description], [unit], [ucd], [utype], [datatype], [size], [principal], [indexed], [std]) VALUES (N'rr.intf_param', N'param_description', N'A free-text description of the parameter''s contents.', NULL, NULL, N'xpath:description', N'unicodeChar', NULL, NULL, NULL, NULL)
GO
INSERT [TAP_SCHEMA].[columns] ([table_name], [column_name], [description], [unit], [ucd], [utype], [datatype], [size], [principal], [indexed], [std]) VALUES (N'rr.relationship', N'ivoid', N'The parent resource.', NULL, NULL, N'xpath:/identifier', N'char', NULL, NULL, 1, NULL)
GO
INSERT [TAP_SCHEMA].[columns] ([table_name], [column_name], [description], [unit], [ucd], [utype], [datatype], [size], [principal], [indexed], [std]) VALUES (N'rr.relationship', N'relationship_type', N'The named type of relationship; this can be mirror-of, service-for, served-by, derived-from, related-to, or something user-defined.', NULL, NULL, N'xpath:relationshipType', N'char', NULL, NULL, NULL, NULL)
GO
INSERT [TAP_SCHEMA].[columns] ([table_name], [column_name], [description], [unit], [ucd], [utype], [datatype], [size], [principal], [indexed], [std]) VALUES (N'rr.relationship', N'related_id', N'The URI form of the IVOA identifier for the resource refered to.', NULL, NULL, N'xpath:relatedResource/@ivo-id', N'char', NULL, NULL, NULL, NULL)
GO
INSERT [TAP_SCHEMA].[columns] ([table_name], [column_name], [description], [unit], [ucd], [utype], [datatype], [size], [principal], [indexed], [std]) VALUES (N'rr.relationship', N'related_name', N'The name of resource that this resource is related to.', NULL, NULL, N'xpath:relatedResource', N'char', NULL, NULL, NULL, NULL)
GO
INSERT [TAP_SCHEMA].[columns] ([table_name], [column_name], [description], [unit], [ucd], [utype], [datatype], [size], [principal], [indexed], [std]) VALUES (N'rr.res_detail', N'ivoid', N'The parent resource.', NULL, NULL, N'xpath:/identifier', N'char', NULL, NULL, 1, NULL)
GO
INSERT [TAP_SCHEMA].[columns] ([table_name], [column_name], [description], [unit], [ucd], [utype], [datatype], [size], [principal], [indexed], [std]) VALUES (N'rr.res_detail', N'cap_index', N'The index of the parent capability', NULL, NULL, NULL, N'short', NULL, NULL, NULL, NULL)
GO
INSERT [TAP_SCHEMA].[columns] ([table_name], [column_name], [description], [unit], [ucd], [utype], [datatype], [size], [principal], [indexed], [std]) VALUES (N'rr.res_detail', N'detail_value', N'(Atomic) value of the member', NULL, NULL, NULL, N'unicodeChar', NULL, NULL, NULL, NULL)
GO
INSERT [TAP_SCHEMA].[columns] ([table_name], [column_name], [description], [unit], [ucd], [utype], [datatype], [size], [principal], [indexed], [std]) VALUES (N'rr.res_role', N'ivoid', N'The parent resource.', NULL, NULL, N'xpath:/identifier', N'char', NULL, NULL, 1, NULL)
GO
INSERT [TAP_SCHEMA].[columns] ([table_name], [column_name], [description], [unit], [ucd], [utype], [datatype], [size], [principal], [indexed], [std]) VALUES (N'rr.res_role', N'role_name', N'The real-world name or title of a person or organization', NULL, NULL, NULL, N'unicodeChar', NULL, NULL, NULL, NULL)
GO
INSERT [TAP_SCHEMA].[columns] ([table_name], [column_name], [description], [unit], [ucd], [utype], [datatype], [size], [principal], [indexed], [std]) VALUES (N'rr.res_role', N'role_ivoid', N'An IVOA identifier of a person or organization', NULL, NULL, NULL, N'char', NULL, NULL, NULL, NULL)
GO
INSERT [TAP_SCHEMA].[columns] ([table_name], [column_name], [description], [unit], [ucd], [utype], [datatype], [size], [principal], [indexed], [std]) VALUES (N'rr.res_role', N'street_address', N'A mailing address for a person or organization', NULL, NULL, NULL, N'unicodeChar', NULL, NULL, NULL, NULL)
GO
INSERT [TAP_SCHEMA].[columns] ([table_name], [column_name], [description], [unit], [ucd], [utype], [datatype], [size], [principal], [indexed], [std]) VALUES (N'rr.res_role', N'email', N'An email address the entity can be reached at', NULL, NULL, NULL, N'char', NULL, NULL, NULL, NULL)
GO
INSERT [TAP_SCHEMA].[columns] ([table_name], [column_name], [description], [unit], [ucd], [utype], [datatype], [size], [principal], [indexed], [std]) VALUES (N'rr.res_role', N'telephone', N'A telephone number the entity can be reached at', NULL, NULL, NULL, N'char', NULL, NULL, NULL, NULL)
GO
INSERT [TAP_SCHEMA].[columns] ([table_name], [column_name], [description], [unit], [ucd], [utype], [datatype], [size], [principal], [indexed], [std]) VALUES (N'rr.res_role', N'logo', N'URL pointing to a graphical logo, which may be used to help identify the entity', NULL, NULL, NULL, N'char', NULL, NULL, NULL, NULL)
GO
INSERT [TAP_SCHEMA].[columns] ([table_name], [column_name], [description], [unit], [ucd], [utype], [datatype], [size], [principal], [indexed], [std]) VALUES (N'rr.res_role', N'base_role', N'The role played by this entity; this is one of contact, publisher, and creator', NULL, NULL, NULL, N'char', NULL, NULL, NULL, NULL)
GO
INSERT [TAP_SCHEMA].[columns] ([table_name], [column_name], [description], [unit], [ucd], [utype], [datatype], [size], [principal], [indexed], [std]) VALUES (N'rr.validation', N'val_level', N'A numeric grade describing the quality of the resource description, when applicable, to be used to indicate the confidence an end-user can put in the resource as part of a VO application or research study.', NULL, NULL, N'xpath:validationLevel', N'char', NULL, NULL, NULL, NULL)
GO
INSERT [TAP_SCHEMA].[key_columns] ([key_id], [from_column], [target_column]) VALUES (N'rr.capability.ivoid', N'ivoid', N'ivoid')
GO
INSERT [TAP_SCHEMA].[key_columns] ([key_id], [from_column], [target_column]) VALUES (N'rr.validation.ivoid', N'ivoid', N'ivoid')
GO
INSERT [TAP_SCHEMA].[key_columns] ([key_id], [from_column], [target_column]) VALUES (N'rr.interface.ivoid', N'ivoid', N'ivoid')
GO
INSERT [TAP_SCHEMA].[key_columns] ([key_id], [from_column], [target_column]) VALUES (N'rr.res_date.ivoid', N'ivoid', N'ivoid')
GO
INSERT [TAP_SCHEMA].[key_columns] ([key_id], [from_column], [target_column]) VALUES (N'rr.intf_param.ivoid', N'ivoid', N'ivoid')
GO
INSERT [TAP_SCHEMA].[key_columns] ([key_id], [from_column], [target_column]) VALUES (N'rr.relationship.ivoid', N'ivoid', N'ivoid')
GO
INSERT [TAP_SCHEMA].[key_columns] ([key_id], [from_column], [target_column]) VALUES (N'rr.res_schema.ivoid', N'ivoid', N'ivoid')
GO
INSERT [TAP_SCHEMA].[key_columns] ([key_id], [from_column], [target_column]) VALUES (N'rr.res_subject.ivoid', N'ivoid', N'ivoid')
GO
INSERT [TAP_SCHEMA].[key_columns] ([key_id], [from_column], [target_column]) VALUES (N'rr.res_table.ivoid', N'ivoid', N'ivoid')
GO
INSERT [TAP_SCHEMA].[key_columns] ([key_id], [from_column], [target_column]) VALUES (N'rr.table_column.ivoid', N'ivoid', N'ivoid')
GO
INSERT [TAP_SCHEMA].[key_columns] ([key_id], [from_column], [target_column]) VALUES (N'rr.validation.ivoid', N'ivoid', N'ivoid')
GO
INSERT [TAP_SCHEMA].[key_columns] ([key_id], [from_column], [target_column]) VALUES (N'rr.res_detail.ivoid', N'ivoid', N'ivoid')
GO
INSERT [TAP_SCHEMA].[key_columns] ([key_id], [from_column], [target_column]) VALUES (N'rr.res_role.ivoid', N'ivoid', N'ivoid')
GO
INSERT [TAP_SCHEMA].[keys] ([key_id], [from_table], [target_table], [description], [utype]) VALUES (N'rr.capability.ivoid', N'rr.capability', N'rr.resource', NULL, NULL)
GO
INSERT [TAP_SCHEMA].[keys] ([key_id], [from_table], [target_table], [description], [utype]) VALUES (N'rr.validation.ivoid', N'rr.validation', N'rr.resource', NULL, NULL)
GO
INSERT [TAP_SCHEMA].[keys] ([key_id], [from_table], [target_table], [description], [utype]) VALUES (N'rr.interface.ivoid', N'rr.interface', N'rr.capability', NULL, NULL)
GO
INSERT [TAP_SCHEMA].[keys] ([key_id], [from_table], [target_table], [description], [utype]) VALUES (N'rr.res_date.ivoid', N'rr.res_date', N'rr.resource', NULL, NULL)
GO
INSERT [TAP_SCHEMA].[keys] ([key_id], [from_table], [target_table], [description], [utype]) VALUES (N'rr.intf_param.ivoid', N'rr.intf_param', N'rr.interface', NULL, NULL)
GO
INSERT [TAP_SCHEMA].[keys] ([key_id], [from_table], [target_table], [description], [utype]) VALUES (N'rr.relationship.ivoid', N'rr.relationship', N'rr.resource', NULL, NULL)
GO
INSERT [TAP_SCHEMA].[keys] ([key_id], [from_table], [target_table], [description], [utype]) VALUES (N'rr.res_schema.ivoid', N'rr.res_schema', N'rr.resource', NULL, NULL)
GO
INSERT [TAP_SCHEMA].[keys] ([key_id], [from_table], [target_table], [description], [utype]) VALUES (N'rr.res_subject.ivoid', N'rr.res_subject', N'rr.resource', NULL, NULL)
GO
INSERT [TAP_SCHEMA].[keys] ([key_id], [from_table], [target_table], [description], [utype]) VALUES (N'rr.res_table.ivoid', N'rr.res_table', N'rr.resource', NULL, NULL)
GO
INSERT [TAP_SCHEMA].[keys] ([key_id], [from_table], [target_table], [description], [utype]) VALUES (N'rr.table_column.ivoid', N'rr.table_column', N'rr.resource', NULL, NULL)
GO
INSERT [TAP_SCHEMA].[keys] ([key_id], [from_table], [target_table], [description], [utype]) VALUES (N'rr.res_detail.ivoid', N'rr.res_detail', N'rr.resource', NULL, NULL)
GO
INSERT [TAP_SCHEMA].[keys] ([key_id], [from_table], [target_table], [description], [utype]) VALUES (N'rr.res_role.ivoid', N'rr.res_role', N'rr.resource', NULL, NULL)
GO
INSERT [TAP_SCHEMA].[schemas] ([schema_name], [description], [utype]) VALUES (N'rr', N'Reference implementation of the Registry TAP schema', NULL)
GO
INSERT [TAP_SCHEMA].[schemas] ([schema_name], [description], [utype]) VALUES (N'tap_schema', N'schema information for TAP services', NULL)
GO
INSERT [TAP_SCHEMA].[tables] ([schema_name], [table_name], [table_type], [description], [utype]) VALUES (N'rr', N'rr.resource', N'table', N'The resources, i.e., services, data collections, organizations, etc., present in this registry.', N'xpath:/')
GO
INSERT [TAP_SCHEMA].[tables] ([schema_name], [table_name], [table_type], [description], [utype]) VALUES (N'tap_schema', N'tap_schema.schemas', N'table', NULL, NULL)
GO
INSERT [TAP_SCHEMA].[tables] ([schema_name], [table_name], [table_type], [description], [utype]) VALUES (N'tap_schema', N'tap_schema.tables', N'table', NULL, NULL)
GO
INSERT [TAP_SCHEMA].[tables] ([schema_name], [table_name], [table_type], [description], [utype]) VALUES (N'tap_schema', N'tap_schema.columns', N'table', NULL, NULL)
GO
INSERT [TAP_SCHEMA].[tables] ([schema_name], [table_name], [table_type], [description], [utype]) VALUES (N'rr', N'rr.capability', N'table', N'Pieces of behaviour of a resource.', N'xpath:/capability/')
GO
INSERT [TAP_SCHEMA].[tables] ([schema_name], [table_name], [table_type], [description], [utype]) VALUES (N'rr', N'rr.res_date', N'table', N'A date associated with an event in the life cycle of the resource. This could be creation or update. The role column can be used to clarify.', N'xpath:/curation/')
GO
INSERT [TAP_SCHEMA].[tables] ([schema_name], [table_name], [table_type], [description], [utype]) VALUES (N'rr', N'rr.interface', N'table', N'Information on access modes of a capability.', N'xpath:/capability/interface/')
GO
INSERT [TAP_SCHEMA].[tables] ([schema_name], [table_name], [table_type], [description], [utype]) VALUES (N'rr', N'rr.intf_param', N'table', N'Input parameters for services.', N'xpath:/capability/interface/param/')
GO
INSERT [TAP_SCHEMA].[tables] ([schema_name], [table_name], [table_type], [description], [utype]) VALUES (N'rr', N'rr.relationship', N'table', N'Relationships between resources, e.g., mirroring, derivation, but also providing access to data within a resource.', N'xpath:/content/relationship/')
GO
INSERT [TAP_SCHEMA].[tables] ([schema_name], [table_name], [table_type], [description], [utype]) VALUES (N'rr', N'rr.res_schema', N'table', N'
	Sets of tables related to resources.', N'xpath:/tableset/schema/
')
GO
INSERT [TAP_SCHEMA].[tables] ([schema_name], [table_name], [table_type], [description], [utype]) VALUES (N'rr', N'rr.res_subject', N'table', N'Topics, object types, or other descriptive keywords about the resource.', N'xpath:/content/
')
GO
INSERT [TAP_SCHEMA].[tables] ([schema_name], [table_name], [table_type], [description], [utype]) VALUES (N'rr', N'rr.res_table', N'table', N')table/

	(Relational) tables that are part of schemata or resources.', N'xpath:/(tableset/schema/ - )table/')
GO
INSERT [TAP_SCHEMA].[tables] ([schema_name], [table_name], [table_type], [description], [utype]) VALUES (N'rr', N'rr.table_column', N'table', N'Metadata on columns of a resource''s tables.', N'xpath:/(tableset/schema/ - )/table/column/')
GO
INSERT [TAP_SCHEMA].[tables] ([schema_name], [table_name], [table_type], [description], [utype]) VALUES (N'rr', N'rr.validation', N'table', N'Validation levels for resources and capabilities.', N'xpath:/(capability - )
')
GO
INSERT [TAP_SCHEMA].[tables] ([schema_name], [table_name], [table_type], [description], [utype]) VALUES (N'tap_schema', N'tap_schema.keys', N'table', NULL, NULL)
GO
INSERT [TAP_SCHEMA].[tables] ([schema_name], [table_name], [table_type], [description], [utype]) VALUES (N'tap_schema', N'tap_schema.key_columns', N'table', NULL, NULL)
GO
INSERT [TAP_SCHEMA].[tables] ([schema_name], [table_name], [table_type], [description], [utype]) VALUES (N'rr', N'rr.res_detail', N'table', N'XPath-value pairs for members of resource or capability and their derivations that are less used and/or from VOResource extensions. The pairs refer to a resource if cap_index is NULL, to the referenced capability otherwise.', NULL)
GO
INSERT [TAP_SCHEMA].[tables] ([schema_name], [table_name], [table_type], [description], [utype]) VALUES (N'rr', N'rr.res_role', N'table', N'Entities, i.e., persons or organizations, operating on resources: creators, contacts, publishers, contributors.', NULL)
GO
