﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ApplicationServices.Interfaces
{
    public interface IAuthorizationService
    {
        bool IsCorrectUserOrAdmin(string loggedInUserId, string userId);
    }
}
