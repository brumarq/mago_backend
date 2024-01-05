using Application.ApplicationServices;
using Application.ApplicationServices.Authentization.Interfaces;
using Application.ApplicationServices.Authorization.Interfaces;
using Application.ApplicationServices.Interfaces;
using Application.DTOs.Device;
using Application.DTOs.Metrics;
using Application.DTOs.Misc;
using Application.Exceptions;
using Domain.Enums;
using Microsoft.Extensions.Configuration;
using Moq;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;

namespace Application.Test
{
    [TestFixture]
    public class DeviceMetricsServiceTest
    {
        private Mock<IDeviceService> _mockDeviceService;
        private Mock<IMetricsService> _mockMetricsService;
        private Mock<IAggregatedLogsService> _mockAggregatedLogsService;
        private Mock<IUnitService> _mockUnitService;

        private Mock<IHttpClientFactory> _mockHttpClientFactory;
        private Mock<IConfiguration> _mockConfiguration;
        private Mock<HttpClient> _mockHttpClient;

        private Mock<IAuthenticationService> _mockAuthenticationService;
        private Mock<IAuthorizationService> _mockAuthorizationService;

        private DeviceMetricsService _deviceMetricsService;


        [SetUp]
        public void Setup()
        {
            _mockDeviceService = new Mock<IDeviceService>();
            _mockMetricsService = new Mock<IMetricsService>();
            _mockAggregatedLogsService = new Mock<IAggregatedLogsService>();
            _mockUnitService = new Mock<IUnitService>();

            _mockHttpClientFactory = new Mock<IHttpClientFactory>();

            _mockConfiguration = new Mock<IConfiguration>();

            _mockAuthenticationService = new Mock<IAuthenticationService>();
            _mockAuthorizationService = new Mock<IAuthorizationService>();

            

            _deviceMetricsService = new DeviceMetricsService(
                _mockMetricsService.Object,
                _mockAggregatedLogsService.Object,
                _mockDeviceService.Object,
                _mockUnitService.Object
            );
        }

        [Test]
        public async Task GetDeviceMetricsAsync_ShouldReturnDeviceMetrics()
        {
            var deviceId = 1;

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


            var expectedLogConnectionType = new LogCollectionTypeDTO
            {
                Id = 1,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            };

            var expectedUnit = new UnitResponseDTO
            {
                Id = 1,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Name = "Kelvin",
                Symbol = "K",
                Factor = 1,
                Offset = 0
            };

            // Mocking unit data
            _mockUnitService.Setup(x => x.GetUnitByIdAsync(It.IsAny<int>())).ReturnsAsync(expectedUnit);

            var expectedField = new FieldDTO
            {
                Id = 1,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Name = "Temperature",
                DeviceTypeId = expectedDeviceType.Id,
                UnitId = expectedUnit.Id,
            };

            var expectedLogCollection = new LogCollectionDTO
            {
                Id = 1,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                DeviceId = deviceId,
                LogCollectionType = expectedLogConnectionType
            };


            var expectedMetrics = new List<MetricsResponseDTO>
            {
                new MetricsResponseDTO
                {
                    Id = 1,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Value = 15,
                    Field = expectedField,
                    LogCollection = expectedLogCollection
                },
                new MetricsResponseDTO
                {
                    Id = 2,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Value = 30,
                    Field = expectedField,
                    LogCollection = expectedLogCollection
                }
            };

            _mockMetricsService.Setup(x => x.GetMetricsForDeviceAsync(deviceId)).ReturnsAsync(expectedMetrics);


            // Act
            var result = await _deviceMetricsService.GetDeviceMetricsAsync(deviceId);

            // Assert
            Assert.IsNotNull(result);
            Assert.That(expectedMetrics.Count, Is.EqualTo(result.Count()));

            // Use a foreach loop to iterate through the expected and actual results
            var actualMetrics = result.ToList();

            for (int i = 0; i < actualMetrics.Count; i++)
            {
                var expectedMetric = expectedMetrics[i];
                var actualMetric = actualMetrics[i];

                Assert.That(expectedMetric.Id, Is.EqualTo(actualMetric.Id));
                Assert.That(expectedMetric.CreatedAt, Is.EqualTo(actualMetric.CreatedAt));
                Assert.That(expectedMetric.UpdatedAt, Is.EqualTo(actualMetric.UpdatedAt));
                Assert.That(expectedMetric.Value, Is.EqualTo(actualMetric.Value));

                // Assert Field property
                Assert.That(expectedMetric.Field?.Id, Is.EqualTo(actualMetric?.Field?.Id));
                Assert.That(expectedMetric.Field?.CreatedAt, Is.EqualTo(actualMetric?.Field?.CreatedAt));
                Assert.That(expectedMetric.Field?.UpdatedAt, Is.EqualTo(actualMetric?.Field?.UpdatedAt));
                Assert.That(expectedMetric.Field?.Name, Is.EqualTo(actualMetric?.Field?.Name));

                // Add more assertions for the Field property as needed

                // Assert LogCollection property
                Assert.That(expectedMetric?.LogCollection?.Id, Is.EqualTo(actualMetric?.LogCollection?.Id));
                Assert.That(expectedMetric?.LogCollection?.CreatedAt, Is.EqualTo(actualMetric?.LogCollection?.CreatedAt));
                Assert.That(expectedMetric?.LogCollection?.UpdatedAt, Is.EqualTo(actualMetric?.LogCollection?.UpdatedAt));
                Assert.That(expectedMetric?.LogCollection?.LogCollectionType, Is.EqualTo(actualMetric?.LogCollection?.LogCollectionType));

                // Assert LogCollectionType property
                Assert.That(expectedMetric?.LogCollection?.LogCollectionType?.Id, Is.EqualTo(actualMetric?.LogCollection?.LogCollectionType?.Id));
                Assert.That(expectedMetric?.LogCollection?.LogCollectionType?.CreatedAt, Is.EqualTo(actualMetric?.LogCollection?.LogCollectionType?.CreatedAt));
                Assert.That(expectedMetric?.LogCollection?.LogCollectionType?.UpdatedAt, Is.EqualTo(actualMetric?.LogCollection?.LogCollectionType?.UpdatedAt));
            }
        }

/*        [Test]
        public void GetDeviceMetricsAsync_NotExistingDevice_ShouldThrowNotFoundException()
        {
            // Arrange
            var invalidDeviceId = 999;

            _mockAuthenticationService.Setup(x => x.IsLoggedInUser()).Returns(true);
            _mockAuthorizationService.Setup(x => x.IsCorrectUserOrAdmin(It.IsAny<string>(), It.IsAny<string>())).Returns(true);

            var fakeHttpMessageHandler = new FakeHttpMessageHandler();

            fakeHttpMessageHandler.SetupResponse($"https://metrics-microservice-service-mago-backend.apps.ocp4-inholland.joran-bergfeld.com/metrics/devices/{invalidDeviceId}",
                new HttpResponseMessage(HttpStatusCode.NotFound)
                {

                });

            var fakeHttpClient = new HttpClient(fakeHttpMessageHandler);
            _mockHttpClientFactory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(fakeHttpClient);

            var deviceService = new DeviceService(_mockConfiguration.Object, _mockHttpClientFactory.Object, _mockAuthenticationService.Object, _mockAuthorizationService.Object);
            // Act and Assert

            var exception = Assert.ThrowsAsync<NotFoundException>(
                async () => await deviceService.CheckDeviceExistence(invalidDeviceId)
            );
            Assert.That(exception?.Message, Is.EqualTo($"Device with id {invalidDeviceId} does not exist."));
        }*/

        // AGG LOGS TESTS
        [Test]
        public async Task GetDeviceAggregatedLogsAsync_ShouldReturnDeviceAggregatedLogs()
        {
            var deviceId = 1;
            var fieldId = 1;
            var startDate = "";
            var endDate = "";

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

            _mockAggregatedLogsService.Setup(x => x.GetAggregatedLogsAsync(AggregatedLogDateType.Weekly, deviceId, fieldId, startDate, endDate)).ReturnsAsync(expectedAggregatedLogs);

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
            var result = await _deviceMetricsService.GetDeviceAggregatedLogsAsync(
                AggregatedLogDateType.Weekly,
                deviceId,
                fieldId,
                startDate,
                endDate
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