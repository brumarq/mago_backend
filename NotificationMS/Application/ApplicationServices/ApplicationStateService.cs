using Application.ApplicationServices.Interfaces;
using Domain.Entities;
using Infrastructure.Repositories.Interfaces;

namespace Application.ApplicationServices;

public class ApplicationStateService : IApplicationStateService
{
    private readonly IRepository<BaseEntity> _repository;

    public ApplicationStateService(IRepository<BaseEntity> repository)
    {
        _repository = repository;
    }

    public async Task<bool> DbIsConnected()
    {
        return await _repository.IsDatabaseConnected();
    }
}