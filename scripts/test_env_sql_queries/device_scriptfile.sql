SET IDENTITY_INSERT [dbo].[DeviceType] ON 

INSERT [dbo].[DeviceType] ([Id], [Name], [CreatedAt], [UpdatedAt]) VALUES (1, N'Test Device Type', CAST(N'2024-01-08T15:40:03.6783721' AS DateTime2), CAST(N'2024-01-08T15:40:03.6783721' AS DateTime2))
SET IDENTITY_INSERT [dbo].[DeviceType] OFF
GO
SET IDENTITY_INSERT [dbo].[Device] ON 

INSERT [dbo].[Device] ([Id], [Name], [DeviceTypeId], [SendSettingsAtConn], [SendSettingsNow], [AuthId], [PwHash], [Salt], [CreatedAt], [UpdatedAt]) VALUES (1, N'Test Device', 1, 1, 1, N'string', NULL, NULL, CAST(N'2024-01-08T15:40:13.7480299' AS DateTime2), CAST(N'2024-01-08T15:40:13.7480294' AS DateTime2))
SET IDENTITY_INSERT [dbo].[Device] OFF
GO
SET IDENTITY_INSERT [dbo].[Unit] ON 

INSERT [dbo].[Unit] ([Id], [Name], [Symbol], [Factor], [Offset], [CreatedAt], [UpdatedAt]) VALUES (1, N'Kilometre per hour', N'km/h', 1, 0, CAST(N'2024-01-11T17:32:25.7133333' AS DateTime2), CAST(N'2024-01-11T17:32:25.7133333' AS DateTime2))
SET IDENTITY_INSERT [dbo].[Unit] OFF
GO
SET IDENTITY_INSERT [dbo].[UsersOnDevices] ON 

INSERT [dbo].[UsersOnDevices] ([Id], [UserId], [DeviceId], [CreatedAt], [UpdatedAt]) VALUES (1, N'auth0|659c1774cbcd7a0e1197baec', 1, CAST(N'2024-01-08T15:41:37.0529671' AS DateTime2), CAST(N'2024-01-08T15:41:37.0529670' AS DateTime2))
SET IDENTITY_INSERT [dbo].[UsersOnDevices] OFF
GO
