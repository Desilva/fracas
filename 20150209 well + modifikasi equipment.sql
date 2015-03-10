use [star_energi_geo]
GO
alter table [dbo].[equipment_daily_report_table]
add id_equipment int NULL
GO
alter table [dbo].[equipments]
add pnid_tag_num varchar(255) NULL
GO
/****** Object:  Table [dbo].[daily_log_wells]    Script Date: 2/9/2015 5:43:58 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[daily_log_wells](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[name] [varchar](255) NULL,
	[is_delete] [bit] NULL,
 CONSTRAINT [PK_daily_log_wells] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO