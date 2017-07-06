USE [star_energi_geo]
GO
/****** Object:  Table [dbo].[bom_component]    Script Date: 7/6/2017 5:22:16 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[bom_component](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[id_bom] [int] NOT NULL,
	[id_component] [int] NOT NULL,
 CONSTRAINT [PK_bom_component] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[bom_equipment]    Script Date: 7/6/2017 5:22:16 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[bom_equipment](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[id_bom] [int] NOT NULL,
	[id_equipment] [int] NOT NULL,
 CONSTRAINT [PK_bom_equipment] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[build_of_material]    Script Date: 7/6/2017 5:22:16 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[build_of_material](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[functional_location] [varchar](100) NOT NULL,
	[no_keymap] [varchar](100) NOT NULL,
	[is_delete] [bit] NOT NULL CONSTRAINT [DF_build_of_material_is_delete]  DEFAULT ((0)),
 CONSTRAINT [PK_build_of_material] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
ALTER TABLE [dbo].[bom_component]  WITH CHECK ADD  CONSTRAINT [FK_bom_component_build_of_material] FOREIGN KEY([id_bom])
REFERENCES [dbo].[build_of_material] ([id])
GO
ALTER TABLE [dbo].[bom_component] CHECK CONSTRAINT [FK_bom_component_build_of_material]
GO
ALTER TABLE [dbo].[bom_component]  WITH CHECK ADD  CONSTRAINT [FK_bom_component_component] FOREIGN KEY([id_component])
REFERENCES [dbo].[component] ([id])
GO
ALTER TABLE [dbo].[bom_component] CHECK CONSTRAINT [FK_bom_component_component]
GO
ALTER TABLE [dbo].[bom_equipment]  WITH CHECK ADD  CONSTRAINT [FK_bom_equipment_build_of_material] FOREIGN KEY([id_bom])
REFERENCES [dbo].[build_of_material] ([id])
GO
ALTER TABLE [dbo].[bom_equipment] CHECK CONSTRAINT [FK_bom_equipment_build_of_material]
GO
ALTER TABLE [dbo].[bom_equipment]  WITH CHECK ADD  CONSTRAINT [FK_bom_equipment_equipments] FOREIGN KEY([id_equipment])
REFERENCES [dbo].[equipments] ([id])
GO
ALTER TABLE [dbo].[bom_equipment] CHECK CONSTRAINT [FK_bom_equipment_equipments]
GO
