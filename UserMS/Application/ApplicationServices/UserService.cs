using Application.ApplicationServices.Interfaces;
using Application.DTOs;
using AutoMapper;
using Bogus;
using Domain.Entities;
using Infrastructure.Repositories.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.ApplicationServices;

public class UserService : IUserService
{
    private readonly IMapper _mapper;
    private readonly IRepository<User> _userRepository;
    private readonly ILogger<UserService> _logger;


    public UserService(IMapper mapper, IRepository<User> userRepository, ILogger<UserService> logger)
    {
        _mapper = mapper;
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<CreateUserDTO> CreateUserAsync(CreateUserDTO createUserDTO)
    {
        if (createUserDTO == null || string.IsNullOrEmpty(createUserDTO.Name))
        {
            _logger.LogWarning("CreateUserAsync called with invalid data");
            throw new ArgumentException("Invalid user data.");
        }
        
        try
        {
            var newUser = new User
            {
                Name = createUserDTO.Name,
                SysAdmin = createUserDTO.SysAdmin,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
            };

            await _userRepository.CreateAsync(newUser);
            _logger.LogInformation("User created with ID {UserId}", newUser.Id);
            return _mapper.Map<CreateUserDTO>(newUser);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating user");
            throw;
        }
    }

    public async Task<bool> DeleteUserAsync(int id)
    {
        try
        {
            var userToDelete = await _userRepository.GetByConditionAsync(u => u.Id == id);

            if (userToDelete == null)
            {
                _logger.LogWarning("Attempted to delete a user with ID {UserId} that does not exist.", id);
                return false;
            }

            await _userRepository.DeleteAsync(id);
            _logger.LogInformation("User with ID {UserId} deleted successfully.", id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting user with ID {UserId}", id);
            throw;
        }
    }

    public async Task<IEnumerable<UserResponseDTO>> GetAllUsersAsync()
    {
        try
        {
            var users = await _userRepository.GetAllAsync();

            if (users == null || !users.Any())
            {
                _logger.LogInformation("No users found.");
                return new List<UserResponseDTO>();
            }

            _logger.LogInformation("Retrieved {Count} users.", users.Count());
            return _mapper.Map<IEnumerable<UserResponseDTO>>(users);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving all users.");
            throw;
        }
    }


    public async Task<UserResponseDTO> GetUserByIdAsync(int id)
    {
        try
        {
            var user = await _userRepository.GetByConditionAsync(u => u.Id == id);

            if (user == null)
            {
                _logger.LogWarning("User with ID {UserId} not found.", id);
                return null;
            }

            _logger.LogInformation("Retrieved user with ID {UserId}.", id);
            return _mapper.Map<UserResponseDTO>(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving user with ID {UserId}", id);
            throw;
        }
    }

    public async Task<UserResponseDTO> UpdateUserAsync(int id, CreateUserDTO createUserDTO)
    {
        if (createUserDTO == null || string.IsNullOrEmpty(createUserDTO.Name))
        {
            _logger.LogWarning("UpdateUserAsync called with invalid data for user ID {UserId}", id);
            throw new ArgumentException("Invalid user data.");
        }

        try
        {
            var userToUpdate = await _userRepository.GetByConditionAsync(u => u.Id == id);

            if (userToUpdate == null)
            {
                _logger.LogWarning("Attempted to update a user with ID {UserId} that does not exist.", id);
                return null;
            }

            userToUpdate.Name = createUserDTO.Name ?? userToUpdate.Name;
            userToUpdate.SysAdmin = createUserDTO.SysAdmin;
            userToUpdate.UpdatedAt = DateTime.Now;

            var updateResult = await _userRepository.UpdateAsync(userToUpdate);

            if (updateResult.HasValue && updateResult.Value)
            {
                _logger.LogInformation("User with ID {UserId} updated successfully.", id);
                return _mapper.Map<UserResponseDTO>(userToUpdate);
            }
            else
            {
                _logger.LogWarning("Failed to update user with ID {UserId}.", id);
                return null;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating user with ID {UserId}", id);
            throw;
        }
    }


}