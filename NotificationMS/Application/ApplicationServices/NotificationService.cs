using Application.ApplicationServices.Interfaces;
using Application.DTOs;
using Application.Exceptions;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Repositories;
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
        

        public async Task<NotificationResponseDTO> CreateNotificationAsync(CreateNotificationDTO createNotificationDTO)
        {
            ValidateCreateNotificationDTO(createNotificationDTO);

            Status newNotification = new Status
            {
                Message = createNotificationDTO.Message,
                DeviceId = createNotificationDTO.DeviceID,
                Timestamp = DateTime.Now,
                StatusTypeId = createNotificationDTO.StatusTypeID
            };

            var responseDTO = await _notificationRepository.CreateAsync(newNotification);

            return _mapper.Map<NotificationResponseDTO>(responseDTO);
        }

        public async Task<IEnumerable<NotificationResponseDTO>> GetAllNotificationsPagedAsync(int pageNumber, int pageSize)
        {
            var notifications = await _notificationRepository.GetAllPagedAsync(pageNumber, pageSize);

            return _mapper.Map<IEnumerable<NotificationResponseDTO>>(notifications);
        }

        public async Task<NotificationResponseDTO> GetNotificationByIdAsync(int id)
        {

            var notification = await _notificationRepository.GetByConditionAsync(n => n.Id == id);

            if (notification == null)
                throw new NotFoundException("Notification was not found...");

            return _mapper.Map<NotificationResponseDTO>(notification);
        }
        public async Task<IEnumerable<NotificationResponseDTO>> GetNotificationsByDeviceIdPagedAsync(int deviceId, int pageNumber, int pageSize)
        {
            var notifications = await _notificationRepository.GetPagedListByConditionAsync(n => n.DeviceId == deviceId, pageNumber, pageSize);

            if (!notifications.Any())
                throw new NotFoundException("Notifications were not found...");

            return _mapper.Map<IEnumerable<NotificationResponseDTO>>(notifications);
        }

        public async Task<StatusTypeDTO> GetStatusTypeByIdAsync(int id)
        {
            var statusType = await _statusTypeRepository.GetByConditionAsync(s => s.Id == id);

            if (statusType == null)
                throw new NotFoundException("Status Type was not found...");

            return _mapper.Map<StatusTypeDTO>(statusType);
        }

        public async Task<StatusTypeDTO> CreateStatusTypeAsync(CreateStatusTypeDTO createStatusTypeDTO)
        {
            ValidateCreateStatusTypeDTO(createStatusTypeDTO);

            StatusType newStatusType = new StatusType
            {
                Name = createStatusTypeDTO.Name
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

            var notificationByStatusTypeId = await _notificationRepository.GetByConditionAsync(st => st.StatusTypeId == id);

            if (notificationByStatusTypeId != null)
            {
                throw new BadRequestException("StatusType is associated with notifications");
            }

            await _statusTypeRepository.DeleteAsync(statusType.Id);
        }
        
        public async Task<StatusTypeDTO> UpdateStatusTypeAsync(int id, CreateStatusTypeDTO statusTypeDTO)
        {
            var statusType = await _statusTypeRepository.GetByConditionAsync(st => st.Id == id);

            if (statusType == null)
            {
                throw new NotFoundException("StatusType not found.");
            }

            statusType.Name = statusTypeDTO.Name;

            await _statusTypeRepository.UpdateAsync(statusType);

            return _mapper.Map<StatusTypeDTO>(statusType);
        }

        private void ValidateCreateStatusTypeDTO(CreateStatusTypeDTO createStatusTypeDTO)
        {
            if (createStatusTypeDTO.Name == null || createStatusTypeDTO.Name == "")
                throw new BadRequestException("StatusType Name property is required to be filled out");
        }
        private void ValidateCreateNotificationDTO(CreateNotificationDTO createNotificationDTO)
        {
            switch (createNotificationDTO)
            {
                case null:
                    throw new BadRequestException("The object cannot be null.");

                case { Message: null } or { Message: "" }:
                    throw new BadRequestException("The 'Message' property is required to be filled out");

                case { DeviceID: <= 0 }:
                    throw new BadRequestException("The 'DeviceID' property cannot be negative or 0.");

                case { StatusTypeID: <= 0 }:
                    throw new BadRequestException("The 'StatusTypeID' property cannot be negative or 0.");


            }
        }


    }
}

