using AutoMapper;
using Bogus;
using Model.DTOs.Users;
using Model.Entities.Users;
using Service.Interfaces;

namespace Service
{
    public class UserService : IUserService
    {
        private readonly IMapper _mapper;
        private static List<User> users = GenerateFakeUsers(10);

        public UserService(IMapper mapper)
        {
            _mapper = mapper;
        }

        public Task<CreateUserDTO> CreateUserAsync(CreateUserDTO createUserDTO)
        {
            User newUser = new User
            {
                Id = users.Count + 1,
                Name = createUserDTO.Name,
                Password = createUserDTO.Password,
                SysAdmin = createUserDTO.SysAdmin,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
            };

            users.Add(newUser);

            return Task.FromResult(_mapper.Map<CreateUserDTO>(newUser));
        }

        public Task<bool> DeleteUserAsync(int id)
        {
            User userToDelete = users.FirstOrDefault(u => u.Id == id)!;

            if (userToDelete == null)
                return Task.FromResult(false);

            users.Remove(userToDelete);
            return Task.FromResult(true);
        }

        public Task<IEnumerable<UserResponseDTO>> GetAllUsersAsync()
        {
            return Task.FromResult(_mapper.Map<IEnumerable<UserResponseDTO>>(users));
        }

        public Task<UserResponseDTO> GetUserByIdAsync(int id)
        {
            User user = users.FirstOrDefault(u => u.Id == id)!;

            if (user == null)
                return Task.FromResult<UserResponseDTO>(null);

            return Task.FromResult(_mapper.Map<UserResponseDTO>(user));
        }

        public Task<bool?> UpdateUserAsync(int id, CreateUserDTO createUserDTO)
        {
            User userToUpdate = users.FirstOrDefault(u => u.Id == id)!;

            if (userToUpdate == null)
                return Task.FromResult<bool?>(false);
           
            userToUpdate.Name = createUserDTO.Name ?? userToUpdate.Name;
            userToUpdate.SysAdmin = createUserDTO.SysAdmin;
            userToUpdate.Password = createUserDTO.Password;
            userToUpdate.UpdatedAt = DateTime.Now;

            return Task.FromResult<bool?>(true);
        }

        // seeder for static data
        private static List<User> GenerateFakeUsers(int count)
        {
            var faker = new Faker<User>()
                .RuleFor(u => u.Id, f => f.IndexFaker + 1)
                .RuleFor(u => u.Name, f => f.Person.FullName)
                .RuleFor(u => u.SysAdmin, f => f.Random.Bool())
                .RuleFor(u => u.Password, f => f.Internet.Password());

            return faker.Generate(count);
        }
    }
}

