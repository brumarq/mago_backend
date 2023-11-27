using Application.ApplicationServices.Interfaces;
using Application.DTOs;
using Application.Exceptions;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Repositories.Interfaces;

namespace Application.ApplicationServices
{
    public class NotificationService : INotificationService
    {
        private readonly IMapper _mapper;
        private readonly IRepository<Status> _notificationRepository;
        private readonly IRepository<UserOnStatusType> _userOnStatusTypeRepository;

        public NotificationService(IMapper mapper, IRepository<Status> notificationRepository, IRepository<UserOnStatusType> userOnStatusTypeRepository)
        {
            _mapper = mapper;
            _notificationRepository = notificationRepository;
            _userOnStatusTypeRepository = userOnStatusTypeRepository;
        }

        public async Task<CreateNotificationDTO> CreateNotificationAsync(CreateNotificationDTO createNotificationDTO)
        {
            Status newNotification = new Status
            {
                Message = createNotificationDTO.Message,
                DeviceId = createNotificationDTO.DeviceID,
                Timestamp = DateTime.Now,
                StatusType = new StatusType { Id = createNotificationDTO.StatusTypeID }
            };

            await _notificationRepository.CreateAsync(newNotification);

            return _mapper.Map<CreateNotificationDTO>(newNotification);
        }

        public async Task<IEnumerable<NotificationResponseDTO>> GetAllNotificationsAsync()
        {
            var notifications = await _notificationRepository.GetAllAsync();

            return _mapper.Map<IEnumerable<NotificationResponseDTO>>(notifications);
        }

        public async Task<NotificationResponseDTO> GetNotificationByIdAsync(int id)
        {
            var notifications = await _notificationRepository.GetAllAsync();

            var notification = notifications.FirstOrDefault(n => n.Id == id);

            if (notification == null)
                throw new BadRequestException("Notification was not found...");

            return _mapper.Map<NotificationResponseDTO>(notification);
        }

        public async Task<IEnumerable<NotificationResponseDTO>> GetNotificationsByDeviceIdAsync(int deviceId)
        {
            var notifications = await _notificationRepository.GetAllAsync();

            var notification = notifications.Where(n => n.DeviceId == deviceId);

            return _mapper.Map<IEnumerable<NotificationResponseDTO>>(notification);
        }

        public async Task<IEnumerable<NotificationResponseDTO>> GetNotificationsForUserOnStatusTypeByUserIdAsync(int userId)
        {
            var userOnStatusTypes = await _userOnStatusTypeRepository.GetAllAsync();
            var notifications = await _notificationRepository.GetAllAsync();

            var resultTuples = userOnStatusTypes
                .Where(ust => ust.UserId == userId)
                .Select(ust => Tuple.Create(ust.DeviceId, ust.StatusType.Id))
                .ToList();

            var filteredNotifications = notifications
                .Where(s => resultTuples.Any(tuple => tuple.Item1 == s.DeviceId && tuple.Item2 == s.StatusType.Id))
                .ToList();

            return _mapper.Map<IEnumerable<NotificationResponseDTO>>(filteredNotifications);
        }
    }
}

