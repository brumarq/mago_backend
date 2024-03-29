SET IDENTITY_INSERT [dbo].[Field] ON 

INSERT [dbo].[Field] ([id], [created_at], [updated_at], [name], [unit_id], [device_type_id], [loggable]) VALUES (1, CAST(N'2024-01-12T11:54:43.107' AS DateTime), CAST(N'2024-01-12T11:54:43.107' AS DateTime), N'Test Field', 1, 1, 1)
INSERT [dbo].[Field] ([id], [created_at], [updated_at], [name], [unit_id], [device_type_id], [loggable]) VALUES (2, CAST(N'2024-01-12T12:09:24.717' AS DateTime), CAST(N'2024-01-12T12:09:24.717' AS DateTime), N'Test Field v2', 1, 1, 1)
SET IDENTITY_INSERT [dbo].[Field] OFF
GO
SET IDENTITY_INSERT [dbo].[MonthlyAverage] ON 

INSERT [dbo].[MonthlyAverage] ([id], [created_at], [updated_at], [average_value], [min_value], [max_value], [device_id], [field_id], [reference_date]) VALUES (1, CAST(N'2024-01-12T13:02:25.347' AS DateTime), CAST(N'2024-01-12T13:02:25.347' AS DateTime), 30, 15, 50, 1, 1, CAST(N'2024-01-01' AS Date))
INSERT [dbo].[MonthlyAverage] ([id], [created_at], [updated_at], [average_value], [min_value], [max_value], [device_id], [field_id], [reference_date]) VALUES (2, CAST(N'2024-01-12T13:03:12.767' AS DateTime), CAST(N'2024-01-12T13:03:12.767' AS DateTime), 30, 15, 50, 1, 1, CAST(N'2024-02-01' AS Date))
SET IDENTITY_INSERT [dbo].[MonthlyAverage] OFF
GO
SET IDENTITY_INSERT [dbo].[WeeklyAverage] ON 

INSERT [dbo].[WeeklyAverage] ([id], [created_at], [updated_at], [average_value], [min_value], [max_value], [device_id], [field_id], [reference_date]) VALUES (1, CAST(N'2024-01-12T13:02:12.530' AS DateTime), CAST(N'2024-01-12T13:02:12.530' AS DateTime), 30, 15, 50, 1, 1, CAST(N'2024-01-01' AS Date))
INSERT [dbo].[WeeklyAverage] ([id], [created_at], [updated_at], [average_value], [min_value], [max_value], [device_id], [field_id], [reference_date]) VALUES (2, CAST(N'2024-01-12T13:03:03.750' AS DateTime), CAST(N'2024-01-12T13:03:03.750' AS DateTime), 30, 15, 50, 1, 1, CAST(N'2024-01-08' AS Date))
SET IDENTITY_INSERT [dbo].[WeeklyAverage] OFF
GO
SET IDENTITY_INSERT [dbo].[YearlyAverage] ON 

INSERT [dbo].[YearlyAverage] ([id], [created_at], [updated_at], [average_value], [min_value], [max_value], [device_id], [field_id], [reference_date]) VALUES (1, CAST(N'2024-01-12T13:02:30.267' AS DateTime), CAST(N'2024-01-12T13:02:30.267' AS DateTime), 30, 15, 50, 1, 1, CAST(N'2024-01-01' AS Date))
INSERT [dbo].[YearlyAverage] ([id], [created_at], [updated_at], [average_value], [min_value], [max_value], [device_id], [field_id], [reference_date]) VALUES (2, CAST(N'2024-01-12T13:03:21.297' AS DateTime), CAST(N'2024-01-12T13:03:21.297' AS DateTime), 30, 15, 50, 1, 1, CAST(N'2025-01-01' AS Date))
SET IDENTITY_INSERT [dbo].[YearlyAverage] OFF
GO
SET IDENTITY_INSERT [dbo].[LogCollectionType] ON 

INSERT [dbo].[LogCollectionType] ([id], [created_at], [updated_at]) VALUES (1, CAST(N'2024-01-12T11:58:56.693' AS DateTime), CAST(N'2024-01-12T11:58:56.693' AS DateTime))
SET IDENTITY_INSERT [dbo].[LogCollectionType] OFF
GO
SET IDENTITY_INSERT [dbo].[LogCollection] ON 

INSERT [dbo].[LogCollection] ([id], [created_at], [updated_at], [device_id], [log_collection_type_id]) VALUES (1, CAST(N'2024-01-12T11:59:24.960' AS DateTime), CAST(N'2024-01-12T11:59:24.960' AS DateTime), 1, 1)
SET IDENTITY_INSERT [dbo].[LogCollection] OFF
GO
SET IDENTITY_INSERT [dbo].[LogValue] ON 

INSERT [dbo].[LogValue] ([id], [created_at], [updated_at], [value], [field_id], [log_collection_id]) VALUES (1, CAST(N'2024-01-12T12:09:30.863' AS DateTime), CAST(N'2024-01-12T12:09:30.863' AS DateTime), 50, 1, 1)
INSERT [dbo].[LogValue] ([id], [created_at], [updated_at], [value], [field_id], [log_collection_id]) VALUES (2, CAST(N'2024-01-12T12:09:34.123' AS DateTime), CAST(N'2024-01-12T12:09:34.123' AS DateTime), 50, 2, 1)
SET IDENTITY_INSERT [dbo].[LogValue] OFF
GO
