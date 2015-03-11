USE [star_energy_geo]
GO
/****** Object:  Table [dbo].[workflow_node]    Script Date: 03/11/2015 15:25:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[workflow_node](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[id_report] [int] NOT NULL,
	[report_type] [varchar](100) NOT NULL,
	[node_name] [varchar](255) NOT NULL,
	[status] [tinyint] NOT NULL,
 CONSTRAINT [PK_workflow_node] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Default [DF_incident_report_workflow_status]    Script Date: 03/11/2015 15:25:32 ******/
ALTER TABLE [dbo].[workflow_node] ADD  CONSTRAINT [DF_incident_report_workflow_status]  DEFAULT ((0)) FOR [status]
GO
