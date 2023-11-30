using System.Net.Http.Headers;
using Application.ApplicationServices.Interfaces;
using Application.DTOs;
using AutoMapper;
using Bogus;
using Domain.Entities;
using Infrastructure.Repositories.Interfaces;
using Microsoft.Extensions.Logging;

using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Application.ApplicationServices;

public class UserService : IUserService
{
    private readonly IMapper _mapper;
    private readonly IRepository<User> _userRepository;
    private readonly ILogger<UserService> _logger;
    private readonly IAuth0ManagementService _auth0ManagementService;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly IAuth0Service _auth0Service;

    public UserService(IMapper mapper, IRepository<User> userRepository, IAuth0Service auth0Service, ILogger<UserService> logger, IAuth0ManagementService auth0ManagementService, IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _mapper = mapper;
        _userRepository = userRepository;
        _logger = logger;
        _auth0ManagementService = auth0ManagementService;
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _auth0Service = auth0Service;
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
            var token = await _auth0ManagementService.GetToken();
            if (token.Token.IsNullOrEmpty())
            {
                _logger.LogError("Something went wrong with the retrieval of the Management token!");
                throw new Exception();
            }
            
            using var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Token);

            var response = await client.GetAsync($"https://dev-izvg6e0c4usamzex.eu.auth0.com/api/v2/users/{id}");
            if (!response.IsSuccessStatusCode)
            {
                // handle error
            }

            var user = await response.Content.ReadFromJsonAsync<User>();
            
            
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