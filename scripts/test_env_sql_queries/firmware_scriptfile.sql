SET IDENTITY_INSERT [dbo].[file_sends] ON 

INSERT [dbo].[file_sends] ([id], [created_at], [updated_at], [update_status], [device_id], [file], [curr_part], [tot_parts]) VALUES (1, CAST(N'2024-01-09T20:48:34.7981235+00:00' AS DateTimeOffset), CAST(N'2024-01-09T20:48:34.7981235+00:00' AS DateTimeOffset), N'New', 1, N'string', 0, 0)
INSERT [dbo].[file_sends] ([id], [created_at], [updated_at], [update_status], [device_id], [file], [curr_part], [tot_parts]) VALUES (2, CAST(N'2024-01-09T23:03:45.0598484+00:00' AS DateTimeOffset), CAST(N'2024-01-09T23:03:45.0598484+00:00' AS DateTimeOffset), N'New', 1, N'New firmware', 0, 0)
INSERT [dbo].[file_sends] ([id], [created_at], [updated_at], [update_status], [device_id], [file], [curr_part], [tot_parts]) VALUES (3, CAST(N'2024-01-09T23:19:10.9501434+00:00' AS DateTimeOffset), CAST(N'2024-01-09T23:19:10.9501434+00:00' AS DateTimeOffset), N'New', 1, N'Come on', 0, 0)
SET IDENTITY_INSERT [dbo].[file_sends] OFF
GO
