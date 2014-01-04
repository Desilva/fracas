USE [star_energi_geo]
GO
ALTER TABLE [dbo].[equipment_daily_report_table] DROP CONSTRAINT [FK_equipment_daily_report_table_equipment_daily_report]
GO
/****** Object:  Table [dbo].[safe_man_hours_incident]    Script Date: 11/22/2013 9:18:04 AM ******/
DROP TABLE [dbo].[safe_man_hours_incident]
GO
/****** Object:  Table [dbo].[safe_man_hours]    Script Date: 11/22/2013 9:18:05 AM ******/
DROP TABLE [dbo].[safe_man_hours]
GO
/****** Object:  Table [dbo].[monthly_she_contractor]    Script Date: 11/22/2013 9:18:05 AM ******/
DROP TABLE [dbo].[monthly_she_contractor]
GO
/****** Object:  Table [dbo].[monthly_project_she_report]    Script Date: 11/22/2013 9:18:05 AM ******/
DROP TABLE [dbo].[monthly_project_she_report]
GO
/****** Object:  Table [dbo].[monthly_project_outstanding_activity]    Script Date: 11/22/2013 9:18:05 AM ******/
DROP TABLE [dbo].[monthly_project_outstanding_activity]
GO
/****** Object:  Table [dbo].[monthly_project_activity]    Script Date: 11/22/2013 9:18:05 AM ******/
DROP TABLE [dbo].[monthly_project_activity]
GO
/****** Object:  Table [dbo].[equipment_daily_report_table]    Script Date: 11/22/2013 9:18:05 AM ******/
DROP TABLE [dbo].[equipment_daily_report_table]
GO
/****** Object:  Table [dbo].[equipment_daily_report_table]    Script Date: 11/22/2013 9:18:05 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[equipment_daily_report_table](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[id_equipment_daily_report] [int] NULL,
	[tag_id] [varchar](255) NULL,
	[description] [varchar](255) NULL,
	[barcode] [varchar](10) NULL,
	[min_limit] [varchar](20) NULL,
	[max_limit] [varchar](20) NULL,
	[unit] [varchar](20) NULL,
	[tag_value] [varchar](50) NULL,
	[date] [date] NULL,
	[time] [varchar](10) NULL,
	[name_operator] [varchar](255) NULL,
	[keterangan] [varchar](255) NULL,
 CONSTRAINT [PK_equipment_daily_report_table] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[monthly_project_activity]    Script Date: 11/22/2013 9:18:05 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[monthly_project_activity](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[id_monthly_project] [int] NULL,
	[activity] [varchar](255) NULL,
 CONSTRAINT [PK_monthly_project_activity] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[monthly_project_outstanding_activity]    Script Date: 11/22/2013 9:18:05 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[monthly_project_outstanding_activity](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[id_monthly_project] [int] NULL,
	[activity] [varchar](255) NULL,
 CONSTRAINT [PK_monthly_project_outstanding_activity] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[monthly_project_she_report]    Script Date: 11/22/2013 9:18:05 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[monthly_project_she_report](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[contractor_name] [varchar](255) NULL,
	[contractor_id] [int] NULL,
	[no_contract] [varchar](255) NULL,
	[month_year] [date] NULL,
	[project_name] [varchar](255) NULL,
	[project_location] [varchar](255) NULL,
	[project_manager] [varchar](255) NULL,
	[contract_supervisor] [varchar](255) NULL,
	[project_she_representative] [varchar](255) NULL,
	[se_she_representative] [varchar](255) NULL,
	[incident_minor_total] [int] NULL,
	[incident_minor_cost] [float] NULL,
	[incident_minor_ytd] [int] NULL,
	[incident_minor_ytd_cost] [float] NULL,
	[toolbox_meeting_total] [int] NULL,
	[toolbox_meeting_ytd] [int] NULL,
	[incident_moderate_total] [int] NULL,
	[incident_moderate_cost] [float] NULL,
	[incident_moderate_ytd] [int] NULL,
	[incident_moderate_ytd_cost] [float] NULL,
	[weekly_she_meeting_total] [int] NULL,
	[weekly_she_meeting_ytd] [int] NULL,
	[incident_serious_total] [int] NULL,
	[incident_serious_cost] [float] NULL,
	[incident_serious_ytd] [int] NULL,
	[incident_serious_ytd_cost] [float] NULL,
	[monthly_contr_mig_total] [int] NULL,
	[monthly_contr_mig_ytd] [int] NULL,
	[incident_major_total] [int] NULL,
	[incident_major_cost] [float] NULL,
	[incident_major_ytd] [int] NULL,
	[incident_major_ytd_cost] [float] NULL,
	[environmental_loss_total] [int] NULL,
	[environmental_loss_cost] [float] NULL,
	[environmental_loss_ytd] [int] NULL,
	[environmental_loss_ytd_cost] [float] NULL,
	[she_observation_card_total] [int] NULL,
	[she_observation_card_ytd] [int] NULL,
	[property_damage_total] [int] NULL,
	[property_damage_cost] [float] NULL,
	[property_damage_ytd] [int] NULL,
	[property_damage_ytd_cost] [float] NULL,
	[new_jsa_hira_total] [int] NULL,
	[new_jsa_hira_ytd] [int] NULL,
	[process_loss_total] [int] NULL,
	[process_loss_cost] [float] NULL,
	[process_loss_ytd] [int] NULL,
	[process_loss_ytd_cost] [float] NULL,
	[ptw_issued_total] [int] NULL,
	[ptw_issued_ytd] [int] NULL,
	[external_relation_total] [int] NULL,
	[external_relation_cost] [float] NULL,
	[external_relation_ytd] [int] NULL,
	[external_relation_ytd_cost] [float] NULL,
	[theft_crime_total] [int] NULL,
	[theft_crime_cost] [float] NULL,
	[theft_crime_ytd] [int] NULL,
	[theft_crime_ytd_cost] [float] NULL,
	[facility_inspection_total] [int] NULL,
	[facility_inspection_ytd] [int] NULL,
	[vehicular_total] [int] NULL,
	[vehicular_cost] [float] NULL,
	[vehicular_ytd] [int] NULL,
	[vehicular_ytd_cost] [float] NULL,
	[vehicular_inspection_total] [int] NULL,
	[vehicular_inspection_ytd] [int] NULL,
	[near_miss_total] [int] NULL,
	[near_miss_cost] [float] NULL,
	[near_miss_ytd] [int] NULL,
	[near_miss_ytd_cost] [float] NULL,
	[ppe_inspection_total] [int] NULL,
	[ppe_inspection_ytd] [int] NULL,
	[lifting_eq_inspection_total] [int] NULL,
	[lifting_eq_inspection_ytd] [int] NULL,
	[man_hours_mh] [int] NULL,
	[man_hours_ytd] [int] NULL,
	[fire_inspection_total] [int] NULL,
	[fire_inspection_ytd] [int] NULL,
	[days_mh] [int] NULL,
	[days_ytd] [int] NULL,
	[vehicle_emission_total] [int] NULL,
	[vehicle_emission_ytd] [int] NULL,
	[incident_frequency_rate_mh] [float] NULL,
	[incident_frequency_rate_ytd] [float] NULL,
	[welding_eq_inspection_total] [int] NULL,
	[welding_eq_inspection_ytd] [int] NULL,
	[incident_severity_rate_mh] [float] NULL,
	[incident_severity_rate_ytd] [float] NULL,
	[hde_inspection_total] [int] NULL,
	[hde_inspection_ytd] [int] NULL,
	[last_date_time_lti] [datetime] NULL,
	[light_vehicle_travel_mh] [int] NULL,
	[light_vehicle_travel_ytd] [int] NULL,
	[fire_emergency_total] [int] NULL,
	[fire_emergency_ytd] [int] NULL,
	[h2s_emergency_total] [int] NULL,
	[h2s_emergency_ytd] [int] NULL,
	[local_workers] [int] NULL,
	[local_lead] [int] NULL,
	[local_spv] [int] NULL,
	[local_total] [int] NULL,
	[environmental_spill_total] [int] NULL,
	[environmental_spill_ytd] [int] NULL,
	[non_local_workers] [int] NULL,
	[non_local_lead] [int] NULL,
	[non_local_spv] [int] NULL,
	[non_local_total] [int] NULL,
	[medical_evacuation_total] [int] NULL,
	[medical_evacuation_ytd] [int] NULL,
	[expatriates_workers] [int] NULL,
	[expatriates_lead] [int] NULL,
	[expatriates_spv] [int] NULL,
	[expatriates_total] [int] NULL,
	[fit_for_day_total] [int] NULL,
	[fit_for_day_ytd] [int] NULL,
	[domestic_waste_total] [int] NULL,
	[domestic_waste_ytd] [int] NULL,
	[clinic_visit_total] [int] NULL,
	[clinic_visit_ytd] [int] NULL,
	[hazardous_waste_total] [int] NULL,
	[hazardous_waste_ytd] [int] NULL,
	[no_work_illness_total] [int] NULL,
	[no_work_illness_ytd] [int] NULL,
	[new_msds_total] [int] NULL,
	[new_msds_ytd] [int] NULL,
	[ill_monitoring_total] [int] NULL,
	[ill_monitoring_ytd] [int] NULL,
	[lti_ytd] [int] NULL,
	[period_start] [date] NULL,
	[period_end] [date] NULL,
 CONSTRAINT [PK_monthly_project_she_report] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[monthly_she_contractor]    Script Date: 11/22/2013 9:18:05 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[monthly_she_contractor](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[name] [varchar](255) NULL,
 CONSTRAINT [PK_monthly_she_contractor] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[safe_man_hours]    Script Date: 11/22/2013 9:18:05 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[safe_man_hours](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[month] [date] NULL,
	[seg_shift] [int] NULL,
	[seg_non_shift] [int] NULL,
	[seg_work_hr_shift] [int] NULL,
	[seg_work_hr_non_shift] [int] NULL,
	[seg_total_work_hr_shift] [int] NULL,
	[seg_total_work_hr_non_shift] [int] NULL,
	[seg_total_work_hr] [int] NULL,
	[seg_total] [int] NULL,
	[cont_shift] [int] NULL,
	[cont_non_shift] [int] NULL,
	[cont_work_hr_shift] [int] NULL,
	[cont_work_hr_non_shift] [int] NULL,
	[cont_total_work_hr_shift] [int] NULL,
	[cont_total_work_hr_non_shift] [int] NULL,
	[cont_total_work_hr] [int] NULL,
	[cont_total] [int] NULL,
	[grand_total] [int] NULL,
	[work_hr_total] [int] NULL,
	[seg_mh_smh] [bigint] NULL,
	[cont_mh_smh] [bigint] NULL,
	[seg_emp_smh] [bigint] NULL,
	[cont_emp_smh] [bigint] NULL,
	[emp_total_smh] [bigint] NULL,
	[mh_total_smh] [bigint] NULL,
	[days_smh] [bigint] NULL,
	[seg_mh_ytd] [bigint] NULL,
	[cont_mh_ytd] [bigint] NULL,
	[seg_emp_ytd] [bigint] NULL,
	[cont_emp_ytd] [bigint] NULL,
	[emp_total_ytd] [bigint] NULL,
	[mh_total_ytd] [bigint] NULL,
 CONSTRAINT [PK_safe_man_hours] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[safe_man_hours_incident]    Script Date: 11/22/2013 9:18:05 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[safe_man_hours_incident](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[date] [date] NULL,
 CONSTRAINT [PK_safe_man_hours_incident] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[equipment_daily_report_table]  WITH CHECK ADD  CONSTRAINT [FK_equipment_daily_report_table_equipment_daily_report] FOREIGN KEY([id_equipment_daily_report])
REFERENCES [dbo].[equipment_daily_report] ([id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[equipment_daily_report_table] CHECK CONSTRAINT [FK_equipment_daily_report_table_equipment_daily_report]
GO
