using Application.ApplicationServices.Interfaces;
using Application.DTOs;
using AutoMapper;
using Bogus;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ApplicationServices
{
    public class NotificationService : INotificationService
    {
        private readonly IMapper _mapper;
        private static List<Status> notifications = GenerateFakeNotifications(20);
        private static List<UserOnStatusType> userOnStatusTypeList = GenerateFakeUserOnStatusTypes(20);

        public NotificationService(IMapper mapper)
        {
            _mapper = mapper;
        }

        public Task<CreateNotificationDTO> CreateNotificationAsync(CreateNotificationDTO createNotificationDTO)
        {
            Status newNotification = new Status
            {
                Id = notifications.Count + 1,
                Message = createNotificationDTO.Message,
                DeviceId = createNotificationDTO.DeviceID,
                Timestamp = DateTime.Now,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                StatusType = new StatusType { Id = createNotificationDTO.StatusTypeID }

            };

            notifications.Add(newNotification);

            return Task.FromResult(_mapper.Map<CreateNotificationDTO>(newNotification));
        }

        public Task<IEnumerable<NotificationResponseDTO>> GetAllNotificationsAsync()
        {
            return Task.FromResult(_mapper.Map<IEnumerable<NotificationResponseDTO>>(notifications));
        }

        public Task<NotificationResponseDTO> GetNotificationByIdAsync(int id)
        {
            Status notification = notifications.FirstOrDefault(n => n.Id == id)!;

            if (notification == null)
                return Task.FromResult<NotificationResponseDTO>(null);

            return Task.FromResult(_mapper.Map<NotificationResponseDTO>(notification));
        }

        public Task<IEnumerable<NotificationResponseDTO>> GetNotificationsByDeviceId(int deviceId)
        {
            IEnumerable<Status> notification = notifications.Where(n => n.DeviceId == deviceId);

            var notificationDtos = _mapper.Map<IEnumerable<NotificationResponseDTO>>(notification);

            return Task.FromResult(notificationDtos);
        }

        public Task<IEnumerable<NotificationResponseDTO>> GetNotificationsForUserOnStatusTypeByUserId(int userId)
        {
            var resultTuples = userOnStatusTypeList
                .Where(ust => ust.UserId == userId)
                .Select(ust => Tuple.Create(ust.DeviceId, ust.StatusType.Id))
                .ToList();

            IEnumerable<Status> filteredNotifications = notifications
                .Where(s => resultTuples.Any(tuple => tuple.Item1 == s.DeviceId && tuple.Item2 == s.StatusType.Id))
                .ToList();

            var notificationDtos = _mapper.Map<IEnumerable<NotificationResponseDTO>>(filteredNotifications);

            return Task.FromResult(notificationDtos);
        }

        private static List<Status> GenerateFakeNotifications(int count)
        {
            Random rnd = new Random();

            var statusTypeFaker = new Faker<StatusType>()
                .RuleFor(st => st.Id, f => rnd.Next(1, 8));

            var faker = new Faker<Status>()
                .RuleFor(n => n.Id, f => f.IndexFaker + 1)
                .RuleFor(n => n.Timestamp, f => f.Date.Recent(40))
                .RuleFor(n => n.Message, f => f.Lorem.Sentence())
                .RuleFor(n => n.StatusType, f => statusTypeFaker.Generate())
                .RuleFor(n => n.DeviceId, f => rnd.Next(1, 5));

            return faker.Generate(count);
        }

        private static List<UserOnStatusType> GenerateFakeUserOnStatusTypes(int count)
        {
            {
                Random rnd = new Random();

                var statusTypeFaker = new Faker<StatusType>()
                    .RuleFor(st => st.Id, f => rnd.Next(1, 8));

                var uniqueCombinations = new HashSet<(int, int, int)>(); // UserId, DeviceID, StatusType
                var userOnStatusTypes = new List<UserOnStatusType>();

                for (int i = 0; i < count; i++)
                {
                    UserOnStatusType newUserOnStatusType;
                    bool isUnique;

                    do
                    {
                        newUserOnStatusType = new Faker<UserOnStatusType>()
                            .RuleFor(u => u.Id, f => i + 1)
                            .RuleFor(u => u.UserId, f => rnd.Next(1, 10))
                            .RuleFor(u => u.DeviceId, f => rnd.Next(1, 4))
                            .RuleFor(u => u.StatusType, f => statusTypeFaker.Generate())
                            .Generate();

                        // Create a tuple for the combination
                        var combination = (newUserOnStatusType.UserId, newUserOnStatusType.DeviceId, newUserOnStatusType.StatusType.Id);
                        isUnique = uniqueCombinations.Add(combination);

                    } while (!isUnique);

                    userOnStatusTypes.Add(newUserOnStatusType);
                }

                return userOnStatusTypes;
            }
        }
    }
}

