﻿using Application.DTOs;
using Domain.Entities;

namespace Application.ApplicationServices.Interfaces;

public interface IAuth0ManagementService
{
    Task<ManagementToken> GetToken();
}