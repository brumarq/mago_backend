SET IDENTITY_INSERT [dbo].[StatusTypes] ON 

INSERT [dbo].[StatusTypes] ([Id], [CreatedAt], [UpdatedAt], [Name]) VALUES (1, CAST(N'2024-01-08T15:43:09.3468191' AS DateTime2), CAST(N'2024-01-08T15:43:09.3468191' AS DateTime2), N'Error')
SET IDENTITY_INSERT [dbo].[StatusTypes] OFF
GO
SET IDENTITY_INSERT [dbo].[Statuses] ON 

INSERT [dbo].[Statuses] ([Id], [Timestamp], [StatusTypeId], [Message], [DeviceId], [CreatedAt], [UpdatedAt]) VALUES (1, CAST(N'2024-01-08T15:43:41.7136270' AS DateTime2), 1, N'Temperature to high', 1, CAST(N'2024-01-08T15:43:41.7231699' AS DateTime2), CAST(N'2024-01-08T15:43:41.7231698' AS DateTime2))
INSERT [dbo].[Statuses] ([Id], [Timestamp], [StatusTypeId], [Message], [DeviceId], [CreatedAt], [UpdatedAt]) VALUES (2, CAST(N'2024-01-11T11:55:01.3580840' AS DateTime2), 1, N'Machine is down', 1, CAST(N'2024-01-11T11:55:01.3586469' AS DateTime2), CAST(N'2024-01-11T11:55:01.3586468' AS DateTime2))
SET IDENTITY_INSERT [dbo].[Statuses] OFF
GO
