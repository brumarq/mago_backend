﻿using Application.ApplicationServices;
using Application.ApplicationServices.Authentization.Interfaces;
using Application.ApplicationServices.Authorization.Interfaces;
using Application.ApplicationServices.Interfaces;
using Application.DTOs.Device;
using Application.DTOs.Metrics;
using Application.DTOs.Misc;
using Domain.Enums;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Net;

namespace Application.Test
{
    [TestFixture]
    public class DeviceAggregatedLogsServiceTests
    {
        private Mock<IDeviceService> _mockDeviceService;
        private Mock<IAggregatedLogsService> _mockAggregatedLogsService;
        private Mock<IUnitService> _mockUnitService;

        private Mock<IHttpClientFactory> _mockHttpClientFactory;
        private Mock<IConfiguration> _mockConfiguration;
        private Mock<HttpClient> _mockHttpClient;

        private Mock<IAuthenticationService> _mockAuthenticationService;
        private Mock<IAuthorizationService> _mockAuthorizationService;

        private DeviceAggregatedLogsService _deviceAggregatedLogsService;


        [SetUp]
        public void Setup()
        {
            _mockDeviceService = new Mock<IDeviceService>();
            _mockAggregatedLogsService = new Mock<IAggregatedLogsService>();
            _mockUnitService = new Mock<IUnitService>();

            _mockHttpClientFactory = new Mock<IHttpClientFactory>();

            _mockConfiguration = new Mock<IConfiguration>();

            _mockAuthenticationService = new Mock<IAuthenticationService>();
            _mockAuthorizationService = new Mock<IAuthorizationService>();



            _deviceAggregatedLogsService = new DeviceAggregatedLogsService(
                _mockAggregatedLogsService.Object,
                _mockDeviceService.Object,
                _mockUnitService.Object
            );
        }

        [Test]
        public async Task GetDeviceAggregatedLogsAsync_ShouldReturnDeviceAggregatedLogs()
        {
            var deviceId = 1;
            var fieldId = 1;
            var startDate = "";
            var endDate = "";
            var pageNumber = 1;
            var pageSize = 50;

            var expectedDeviceType = new DeviceTypeResponseDTO
            {
                Id = 1,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Name = "Device Type 1",
            };


            var expectedDevice = new DeviceResponseDTO
            {
                Id = deviceId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Name = "Device 1",
                DeviceType = expectedDeviceType,
                AuthId = "Example Auth Id",
                SendSettingsAtConn = true,
                SendSettingsNow = true,
            };

            _mockDeviceService.Setup(x => x.GetDeviceByIdAsync(deviceId)).ReturnsAsync(expectedDevice);

            var expectedAggregatedLogs = new List<AggregatedLogsResponseDTO>
            {
                new AggregatedLogsResponseDTO
                {
                    AverageValue = 10.5f,
                    MinValue = 8.75f,
                    MaxValue = 15f,
                    DeviceId = 1,
                    Field = new FieldDTO
                    {
                        Id = fieldId,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        DeviceTypeId = 1,
                        Name = "Temperature",
                        UnitId = 1,
                        Loggable = true,
                    }
                },
                new AggregatedLogsResponseDTO
                {
                    AverageValue = 13.5f,
                    MinValue = 4.35f,
                    MaxValue = 40f,
                    DeviceId = 1,
                    Field = new FieldDTO
                    {
                        Id = fieldId,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        DeviceTypeId = 1,
                        Name = "Temperature",
                        UnitId = 1,
                        Loggable = true,
                    }
                },
            };

            _mockAggregatedLogsService.Setup(x => x.GetAggregatedLogsAsync(AggregatedLogDateType.Weekly, deviceId, fieldId, startDate, endDate, pageNumber,pageSize))
                .ReturnsAsync(expectedAggregatedLogs);

            var expectedUnit = new UnitResponseDTO
            {
                Id = 1,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Name = "Celsius",
                Symbol = "C",
                Factor = 1,
                Offset = 0,
            };

            _mockUnitService.Setup(x => x.GetUnitByIdAsync(It.IsAny<int>())).ReturnsAsync(expectedUnit);

            // Act
            var result = await _deviceAggregatedLogsService.GetDeviceAggregatedLogsAsync(
                AggregatedLogDateType.Weekly,
                deviceId,
                fieldId,
                startDate,
                endDate,
                pageNumber,
                pageSize
            );

            var resultList = result.ToList();

            // Assert
            Assert.IsNotNull(resultList);
            Assert.That(expectedAggregatedLogs.Count, Is.EqualTo(resultList.Count));

            // Use a foreach loop to iterate through the expected and actual results
            for (int i = 0; i < resultList.Count; i++)
            {
                var expectedLog = expectedAggregatedLogs[i];
                var actualLog = resultList[i];

                // Assert agg log details
                Assert.That(actualLog.AverageValue, Is.EqualTo(expectedLog.AverageValue));
                Assert.That(actualLog.MinValue, Is.EqualTo(expectedLog.MinValue));
                Assert.That(actualLog.MaxValue, Is.EqualTo(expectedLog.MaxValue));
                Assert.That(actualLog.Field?.Name, Is.EqualTo(expectedLog.Field?.Name));

                // Assert device details
                Assert.That(actualLog.Device?.Id, Is.EqualTo(expectedDevice.Id));
                Assert.That(actualLog.Device?.CreatedAt, Is.EqualTo(expectedDevice.CreatedAt));
                Assert.That(actualLog.Device?.UpdatedAt, Is.EqualTo(expectedDevice.UpdatedAt));
                Assert.That(actualLog.Device?.Name, Is.EqualTo(expectedDevice.Name));


                // Assert unit details
                Assert.That(actualLog.Field.Unit?.Id, Is.EqualTo(expectedUnit.Id));
                Assert.That(actualLog.Field.Unit?.CreatedAt, Is.EqualTo(expectedUnit.CreatedAt));
                Assert.That(actualLog.Field.Unit?.UpdatedAt, Is.EqualTo(expectedUnit.UpdatedAt));
                Assert.That(actualLog.Field.Unit?.Name, Is.EqualTo(expectedUnit.Name));
            }
        }

        private class FakeHttpMessageHandler : DelegatingHandler
        {
            private readonly Dictionary<string, HttpResponseMessage> _responses = new Dictionary<string, HttpResponseMessage>();

            public void SetupResponse(string url, HttpResponseMessage responseMessage)
            {
                _responses[url] = responseMessage;
            }

            protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                if (_responses.TryGetValue(request.RequestUri.ToString(), out var response))
                {
                    return await Task.FromResult(response);
                }
                return new HttpResponseMessage(HttpStatusCode.NotFound) { Content = new StringContent("Not Found") };
            }
        }
    }
}