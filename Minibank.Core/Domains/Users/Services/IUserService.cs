﻿using System.Collections.Generic;

namespace Minibank.Core.Domains.Users.Services
{
    public interface IUserService
    {
        User GetById(string id);
        List<User> GetAllUsers();
        void CreateUser(User user);
        void UpdateUser(User user);
        void DeleteUser(string id);
    }
}