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
        private readonly IRepository<StatusType> _statusTypeRepository;


        public NotificationService(IMapper mapper, IRepository<Status> notificationRepository,  IRepository<StatusType> statusTypeRepository)
        {
            _mapper = mapper;
            _notificationRepository = notificationRepository;
            _statusTypeRepository = statusTypeRepository;
        }
        

        public async Task<CreateNotificationDTO> CreateNotificationAsync(CreateNotificationDTO createNotificationDTO)
        {
            Status newNotification = new Status
            {
                Message = createNotificationDTO.Message,
                DeviceId = createNotificationDTO.DeviceID,
                Timestamp = DateTime.Now,
                StatusTypeId = createNotificationDTO.StatusTypeID // Set the foreign key proper
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

        public async Task<StatusTypeDTO> GetStatusTypeByIdAsync(int id)
        {
            var statusTypes = await _statusTypeRepository.GetAllAsync();

            var statusType = statusTypes.FirstOrDefault(n => n.Id == id);

            if (statusType == null)
                throw new NotFoundException("Status Type was not found...");

            return _mapper.Map<StatusTypeDTO>(statusType);
        }

        public async Task<StatusTypeDTO> CreateStatusTypeAsync(CreateStatusTypeDTO statusTypeDTO)
        {
            StatusType newStatusType = new StatusType
            {
                Name = statusTypeDTO.Name
            };

            await _statusTypeRepository.CreateAsync(newStatusType);

            return _mapper.Map<StatusTypeDTO>(newStatusType);
        }
        
        public async Task DeleteStatusTypeAsync(int id)
        {
            var statusType = await _statusTypeRepository.GetByConditionAsync(st => st.Id == id);

            if (statusType == null)
            {
                throw new NotFoundException("StatusType not found.");
            }

            // Optional: Add logic here to handle if the StatusType is associated with any Statuses

            await _statusTypeRepository.DeleteAsync(statusType.Id); // Adjust repository methods if needed
        }
        
        public async Task<StatusTypeDTO> UpdateStatusTypeAsync(int id, CreateStatusTypeDTO statusTypeDTO)
        {
            var statusType = await _statusTypeRepository.GetByConditionAsync(st => st.Id == id);

            if (statusType == null)
            {
                throw new NotFoundException("StatusType not found.");
            }

            // Update properties
            statusType.Name = statusTypeDTO.Name;
            // Add other property updates as necessary

            await _statusTypeRepository.UpdateAsync(statusType); // Adjust repository methods if needed

            return _mapper.Map<StatusTypeDTO>(statusType);
        }




    }
}

