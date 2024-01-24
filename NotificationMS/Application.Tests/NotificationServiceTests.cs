using Application.ApplicationServices;
using Application.ApplicationServices.Interfaces;
using Application.DTOs;
using Application.Exceptions;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Repositories.Interfaces;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Application.Tests
{
    [TestFixture]
    public class NotificationServiceTests
    {
        private Mock<IMapper> _mockMapper;
        private Mock<IRepository<Status>> _mockNotificationRepository;
        private Mock<IRepository<StatusType>> _mockStatusTypeRepository;
        private Mock<IRepository<NotificationTokenOnUser>> _mockNotificationTokenOnUserRepository;
        private INotificationService _notificationService;

        [SetUp]
        public void SetUp()
        {
            _mockMapper = new Mock<IMapper>();
            _mockNotificationRepository = new Mock<IRepository<Status>>();
            _mockStatusTypeRepository = new Mock<IRepository<StatusType>>();
            _mockNotificationTokenOnUserRepository = new Mock<IRepository<NotificationTokenOnUser>>();
            _notificationService = new NotificationService(_mockMapper.Object, _mockNotificationRepository.Object, _mockStatusTypeRepository.Object, _mockNotificationTokenOnUserRepository.Object);
        }


        #region CreateNotificationAsync
        [Test]
        [TestCase("device overheating", 1, 1)]
        [TestCase("device needs water", 32, 9)]
        [TestCase("Device exploded", 423, 424)]
        [TestCase("Device not found", 44, 43)]
        public async Task CreateNotificationAsync_ValidRequest_Should_Create_Notification_And_Return_ResponseDTO(string message, int deviceId, int statusTypeId)
        {
            var createNotificationDto = new CreateNotificationDTO
            {
                Message = message,
                DeviceID = deviceId,
                StatusTypeID = statusTypeId
            };

            var notification = new Status
            {
                Message = createNotificationDto.Message,
                DeviceId = createNotificationDto.DeviceID,
                Timestamp = DateTime.Now,
                StatusTypeId = createNotificationDto.StatusTypeID
            };

            var notificationResponseDTO = new NotificationResponseDTO
            {
                Id = 1,
                Message = notification.Message,
                DeviceID = notification.DeviceId,
                TimeStamp = DateTime.Now,
                StatusTypeId = notification.StatusTypeId
            };

            _mockNotificationRepository.Setup(repo => repo.CreateAsync(It.IsAny<Status>()))
                .Returns(Task.FromResult(notification));

            _mockMapper.Setup(mapper => mapper.Map<NotificationResponseDTO>(It.IsAny<Status>()))
                .Returns(notificationResponseDTO);


            var resultDto = await _notificationService.CreateNotificationAsync(createNotificationDto);

            Assert.That(resultDto, Is.Not.Null);
            Assert.That(resultDto.Message, Is.EqualTo(createNotificationDto.Message));
            Assert.That(resultDto.DeviceID, Is.EqualTo(createNotificationDto.DeviceID));
            Assert.That(resultDto.StatusTypeId, Is.EqualTo(createNotificationDto.StatusTypeID));
        }

        [Test]
        [TestCase("", 1, 1)]
        [TestCase(null, 1, 1)]
        [TestCase("Device on fire", 1, -2)]
        [TestCase("Device on fire", -2, 1)]
        public async Task CreateNotificationAsync_InvalidData_Throws_BadRequestException(string message, int deviceId, int statusTypeId)
        {
            var invalidDTO = new CreateNotificationDTO()
            {
                DeviceID = deviceId,
                StatusTypeID = statusTypeId,
                Message = message
            };

            var notification = new Status
            {
                Message = invalidDTO.Message,
                DeviceId = invalidDTO.DeviceID,
                Timestamp = DateTime.Now,
                StatusTypeId = invalidDTO.StatusTypeID,
                Id = 1
            };

            var notificationResponseDTO = new NotificationResponseDTO
            {
                Id = 1,
                Message = notification.Message,
                DeviceID = notification.DeviceId,
                TimeStamp = DateTime.Now,
                StatusTypeId = notification.StatusTypeId
            };

            _mockNotificationRepository.Setup(repo => repo.CreateAsync(notification))
                .Returns(Task.FromResult(notification));

            _mockMapper.Setup(mapper => mapper.Map<NotificationResponseDTO>(notification))
                .Returns(notificationResponseDTO);

            Assert.ThrowsAsync<BadRequestException>(async () => await _notificationService.CreateNotificationAsync(invalidDTO));
        }
        #endregion

        #region GetAllNotificationsAsync
        [Test]
        public async Task GetAllNotificationsAsync_Returns_NotificationResponseDTOs()
        {
            var fakeNotifications = new List<Status> {
            new Status
            {
                Message = "Device on fire",
                DeviceId = 1,
                Timestamp = DateTime.Now,
                StatusTypeId = 1
            },
            new Status
            {
                Message = "Device not working",
                DeviceId = 2,
                Timestamp = DateTime.Now,
                StatusTypeId = 2
            },
            new Status
            {
                Message = "Device disconnected",
                DeviceId = 3,
                Timestamp = DateTime.Now,
                StatusTypeId = 3
                }
            };

            var fakeNotificationResponseDTOs = new List<NotificationResponseDTO>
            {
                new NotificationResponseDTO
                {
                    Message = fakeNotifications[0].Message,
                    DeviceID = fakeNotifications[0].DeviceId,
                    TimeStamp = fakeNotifications[0].Timestamp,
                    StatusTypeId = fakeNotifications[0].StatusTypeId
                },
                new NotificationResponseDTO
                {
                    Message = fakeNotifications[1].Message,
                    DeviceID = fakeNotifications[1].DeviceId,
                    TimeStamp = fakeNotifications[1].Timestamp,
                    StatusTypeId = fakeNotifications[1].StatusTypeId
                },
                new NotificationResponseDTO
                {
                    Message = fakeNotifications[2].Message,
                    DeviceID = fakeNotifications[2].DeviceId,
                    TimeStamp = fakeNotifications[2].Timestamp,
                    StatusTypeId = fakeNotifications[2].StatusTypeId
                }
            };

            _mockNotificationRepository.Setup(repo => repo.GetAllPagedAsync(1, 100)).ReturnsAsync(fakeNotifications);
            _mockMapper.Setup(mapper => mapper.Map<IEnumerable<NotificationResponseDTO>>(fakeNotifications))
                       .Returns(fakeNotificationResponseDTOs);

            var result = await _notificationService.GetAllNotificationsPagedAsync(1, 100);
            
            Assert.IsNotNull(result);
            Assert.AreEqual(fakeNotifications.Count, fakeNotificationResponseDTOs.Count());
        }
#endregion

        #region GetNotificationByIdAsync
        [Test]
        public async Task GetNotificationByIdAsync_Notification_Not_Found_ThrowsNotFoundException()
        {
            var id = 99;
            _mockNotificationRepository.Setup(repo => repo.GetByConditionAsync(n => n.Id == id)).ReturnsAsync((Status)null);

            Assert.ThrowsAsync<NotFoundException>(async () => await _notificationService.GetNotificationByIdAsync(id));

        }

        [Test]
        [TestCase(1)]
        [TestCase(123)]
        [TestCase(54)]
        [TestCase(44)]
        public async Task GetNotificationByIdAsync_Valid_Id_Returns_NotificationResponseDto(int id)
        {
            var notification = new Status
            {
                Message = "Device on fire",
                DeviceId = 1,
                Timestamp = DateTime.Now,
                StatusTypeId = 1,
                Id = id
            };
            var responseDTO = new NotificationResponseDTO
            {
                Message = notification.Message,
                DeviceID = notification.Id,
                TimeStamp = notification.Timestamp,
                StatusTypeId = notification.StatusTypeId,
                Id = id
            };

            _mockNotificationRepository.Setup(repo => repo.GetByConditionAsync(n => n.Id == id))
                                       .ReturnsAsync(notification);
            _mockMapper.Setup(mapper => mapper.Map<NotificationResponseDTO>(notification))
                       .Returns(responseDTO);

            var result = await _notificationService.GetNotificationByIdAsync(id);

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<NotificationResponseDTO>(result);
            Assert.AreEqual(responseDTO.Message, result.Message);
            Assert.AreEqual(responseDTO.DeviceID, result.DeviceID);
            Assert.AreEqual(responseDTO.TimeStamp, result.TimeStamp);
            Assert.AreEqual(responseDTO.StatusTypeId, result.StatusTypeId);
            Assert.AreEqual(responseDTO.Id, result.Id);
        }
#endregion

        #region GetNotificationsByDeviceIdAsync
        [Test]
        public async Task GetNotificationsByDeviceIdAsync_Notification_Not_Found_ThrowsNotFoundException()
        {
            var id = 99;
            var emptyNotifications = new List<Status>();
            _mockNotificationRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(emptyNotifications);
                
            Assert.ThrowsAsync<NotFoundException>(async () => await _notificationService.GetNotificationsByDeviceIdPagedAsync(id, 1, 10));

        }
        [Test]
        [TestCase(1, 2)]
        [TestCase(123, 321)]
        [TestCase(54, 2)]
        [TestCase(44, 4)]
        public async Task GetNotificationByDeviceIdAsync_Valid_Id_Returns_NotificationResponseDto(int deviceId, int notDeviceId)
        {
            var notifications = new List<Status> {
            new Status
            {
                Message = "Device on fire",
                DeviceId = deviceId,
                Timestamp = DateTime.Now,
                StatusTypeId = 1,
                Id = 1
            },
            new Status
            {
                Message = "Device not working",
                DeviceId = deviceId,
                Timestamp = DateTime.Now,
                StatusTypeId = 2,
                Id = 2
            },
            new Status
            {
                Message = "Device disconnected",
                DeviceId = notDeviceId,
                Timestamp = DateTime.Now,
                StatusTypeId = 3,
                Id = 3
                }
            };

            var responseDTOs = new List<NotificationResponseDTO>
            {
                new NotificationResponseDTO
                {
                    Message = notifications[0].Message,
                    DeviceID = notifications[0].DeviceId,
                    TimeStamp = notifications[0].Timestamp,
                    StatusTypeId = notifications[0].StatusTypeId
                },
                new NotificationResponseDTO
                {
                    Message = notifications[1].Message,
                    DeviceID = notifications[1].DeviceId,
                    TimeStamp = notifications[1].Timestamp,
                    StatusTypeId = notifications[1].StatusTypeId
                },
                new NotificationResponseDTO
                {
                    Message = notifications[2].Message,
                    DeviceID = notifications[2].DeviceId,
                    TimeStamp = notifications[2].Timestamp,
                    StatusTypeId = notifications[2].StatusTypeId
                }
            };


            //_mockMapper.Setup(mapper => mapper.Map<List<NotificationResponseDTO>>(notifications.Where(n => n.DeviceId == deviceId)))
            //           .Returns(responseDTOs);

            _mockNotificationRepository.Setup(repo => repo.GetPagedListByConditionAsync(n => n.DeviceId == deviceId, 1, 10))
                .ReturnsAsync(notifications);

            _mockMapper.Setup(mapper => mapper.Map<List<NotificationResponseDTO>>(It.IsAny<Status>()))
                .Returns(responseDTOs);

            var result = await _notificationService.GetNotificationsByDeviceIdPagedAsync(deviceId, 1, 10);

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<IEnumerable<NotificationResponseDTO>>(result);
            foreach(NotificationResponseDTO notificationResponseDTO in result)
            {
                Assert.AreEqual(notificationResponseDTO.DeviceID, deviceId);

                var originalNotification = notifications.FirstOrDefault(n => n.Id == notificationResponseDTO.Id);

                Assert.IsNotNull(originalNotification);

                Assert.AreEqual(originalNotification.Message, notificationResponseDTO.Message);
                Assert.AreEqual(originalNotification.Timestamp, notificationResponseDTO.TimeStamp);
                Assert.AreEqual(originalNotification.StatusTypeId, notificationResponseDTO.StatusTypeId);
            }
        }

        #endregion

        #region GetStatusTypeById
        [Test]
        public async Task GetStatusTypeByIdAsync_Notification_Not_Found_ThrowsNotFoundException()
        {
            var id = 99;
            _mockStatusTypeRepository.Setup(repo => repo.GetByConditionAsync(n => n.Id == id)).ReturnsAsync((StatusType)null);

            Assert.ThrowsAsync<NotFoundException>(async () => await _notificationService.GetStatusTypeByIdAsync(id));

        }
        [Test]
        [TestCase(1)]
        [TestCase(123)]
        [TestCase(54)]
        [TestCase(44)]
        public async Task GetStatusTypeByIdAsyncc_Valid_Id_Returns_StatusTypeDTO(int id)
        {
            var statusType = new StatusType
            {
                Id = id,
                Name = "Warning",
            };
            var statusTypeDTO = new StatusTypeDTO
            {
                Id = statusType.Id,
                Name = statusType.Name
            };

            _mockStatusTypeRepository.Setup(repo => repo.GetByConditionAsync(n => n.Id == id))
                                       .ReturnsAsync(statusType);
            _mockMapper.Setup(mapper => mapper.Map<StatusTypeDTO>(statusType))
                       .Returns(statusTypeDTO);

            var result = await _notificationService.GetStatusTypeByIdAsync(id);

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<StatusTypeDTO>(result);
            Assert.AreEqual(statusTypeDTO.Id, result.Id);
            Assert.AreEqual(statusTypeDTO.Name, result.Name);
        }
        #endregion

        #region CreateStatusTypeAsync
        [Test]
        [TestCase("")]
        [TestCase(null)]
        public async Task CreateStatusTypeAsync_InvalidName_Throws_BadRequestException(string name)
        {
            CreateStatusTypeDTO createStatusTypeDTO = new CreateStatusTypeDTO
            {
                Name = name
            };
            Assert.ThrowsAsync<BadRequestException>(async () => await _notificationService.CreateStatusTypeAsync(createStatusTypeDTO));
        }

        [Test]
        [TestCase("Error")]
        [TestCase("Alert")]
        [TestCase("AAAAAAAAAAAAAAAAAAAAAAAAAA")]
        public async Task CreateStatusTypeAsync_Valid_Object_Returns_StatusTypeDTO(string name)
        {
            CreateStatusTypeDTO createStatusTypeDTO = new CreateStatusTypeDTO
            {
                Name = name
            };

            StatusType statusType = new StatusType()
            {
                Id = 1,
                Name = createStatusTypeDTO.Name
            };

            StatusTypeDTO statusTypeDTO = new StatusTypeDTO()
            {
                Id = statusType.Id,
                Name = statusType.Name
            };

            _mockStatusTypeRepository.Setup(repo => repo.CreateAsync(It.IsAny<StatusType>()))
                .Returns(Task.FromResult(statusType));

            _mockMapper.Setup(mapper => mapper.Map<StatusTypeDTO>(It.IsAny<StatusType>()))
                .Returns(statusTypeDTO);

            var resultDto = await _notificationService.CreateStatusTypeAsync(createStatusTypeDTO);

            Assert.IsNotNull(resultDto);
            Assert.That(resultDto.Name, Is.EqualTo(createStatusTypeDTO.Name));
            Assert.That(resultDto.Id, Is.EqualTo(statusType.Id));
        }
        #endregion

        #region DeleteStatusTypeAsync
        [Test]
        public async Task DeleteStatusTypeAsync_StatusTypeNotFound_ThrowsNotFoundException()
        {
            var id = 1;
            _mockStatusTypeRepository.Setup(repo => repo.GetByConditionAsync(It.IsAny<Expression<Func<StatusType, bool>>>()))
                .ReturnsAsync((StatusType)null);

            Assert.ThrowsAsync<NotFoundException>(async () => await _notificationService.DeleteStatusTypeAsync(id));
        }

        [Test]
        public async Task DeleteStatusTypeAsync_StatusTypeAssociatedWithNotifications_Throws_BadRequestException()
        {
            var id = 1;
            var statusType = new StatusType { Id = id, Name = "test" };

            _mockStatusTypeRepository.Setup(repo => repo.GetByConditionAsync(It.IsAny<Expression<Func<StatusType, bool>>>()))
                .ReturnsAsync(statusType);

            _mockNotificationRepository.Setup(repo => repo.GetByConditionAsync(It.IsAny<Expression<Func<Status, bool>>>()))
                .ReturnsAsync(new Status());

            Assert.ThrowsAsync<BadRequestException>(async () => await _notificationService.DeleteStatusTypeAsync(id));
        }

        [Test]
        public async Task DeleteStatusTypeAsync_ValidDeletion_CompletesSuccessfully()
        {
            var id = 1;
            var statusType = new StatusType { Id = id, Name = "test" };
            _mockStatusTypeRepository.Setup(repo => repo.GetByConditionAsync(It.IsAny<Expression<Func<StatusType, bool>>>()))
                .ReturnsAsync(statusType);

            _mockNotificationRepository.Setup(repo => repo.GetByConditionAsync(It.IsAny<Expression<Func<Status, bool>>>()))
                .ReturnsAsync((Status)null);

            await _notificationService.DeleteStatusTypeAsync(id);

            _mockStatusTypeRepository.Verify(repo => repo.DeleteAsync(id), Times.Once);
        }

        #endregion

        #region UpdateStatusTypeAsync
        [Test]
        public async Task UpdateStatusTypeAsync_StatusTypeNotFound_ThrowsNotFoundException()
        {
            var id = 1;
            CreateStatusTypeDTO statusTypeDTO = new CreateStatusTypeDTO { Name = "Error" };

            _mockStatusTypeRepository.Setup(repo => repo.GetByConditionAsync(It.IsAny<Expression<Func<StatusType, bool>>>()))
                                     .ReturnsAsync((StatusType)null);

            Assert.ThrowsAsync<NotFoundException>(async () => await _notificationService.UpdateStatusTypeAsync(id, statusTypeDTO));
        }

        [Test]
        public async Task UpdateStatusTypeAsync_ValidUpdate_ReturnsUpdatedStatusTypeDTO()
        {
            var id = 1;
            CreateStatusTypeDTO statusTypeDTO = new CreateStatusTypeDTO { Name = "New Error" };
            var statusType = new StatusType { Id = id, Name = "Old Alert" };
            var updatedStatusTypeDTO = new StatusTypeDTO { Id = id, Name = statusTypeDTO.Name };

            _mockStatusTypeRepository.Setup(repo => repo.GetByConditionAsync(It.IsAny<Expression<Func<StatusType, bool>>>()))
                                     .ReturnsAsync(statusType);
            _mockMapper.Setup(mapper => mapper.Map<StatusTypeDTO>(statusType)).Returns(updatedStatusTypeDTO);

            _mockStatusTypeRepository.Setup(repo => repo.UpdateAsync(statusType)).ReturnsAsync(true);


            var result = await _notificationService.UpdateStatusTypeAsync(id, statusTypeDTO);

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<StatusTypeDTO>(result);
            Assert.AreEqual(statusTypeDTO.Name, result.Name);

            _mockStatusTypeRepository.Verify(repo => repo.UpdateAsync(It.Is<StatusType>(st => st.Id == id && st.Name == statusTypeDTO.Name)), Times.Once);
        }
        #endregion
    }
}
