using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Minibank.Core.Domains.Users;
using Minibank.Core.Domains.Users.Repositories;
using Minibank.Core.Exceptions.FriendlyExceptions;
using Minibank.Data.DatabaseLayer.Context;

namespace Minibank.Data.DatabaseLayer.DbModels.Users.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly MinibankContext _context;
        private readonly IMapper _mapper;

        public UserRepository(MinibankContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<User> GetByIdAsync(string id)
        {
            var userEntity = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(user => user.Id == id);
            if (userEntity == null)
            {
                throw new ObjectNotFoundException($"не существует пользователя c id: {id}");
            }

            return _mapper.Map<User>(userEntity);
        }

        public Task<List<User>> GetAllUsersAsync()
        {
            return
                _context.Users
                    .AsNoTracking()
                    .Select(userEntity => _mapper.Map<User>(userEntity))
                    .ToListAsync();
        }

        public Task CreateUserAsync(User user)
        {
            var userEntity = _mapper.Map<UserEntity>(user);
            userEntity.Id = Guid.NewGuid().ToString();
            
            return _context.Users.AddAsync(userEntity).AsTask();
        }

        public async Task UpdateUserAsync(User user)
        {
            var userEntity = await _context.Users.FindAsync(user.Id);
            if (userEntity == null)
            {
                throw new ObjectNotFoundException($"не существует пользователя c id: {user.Id}");
            }
            
            userEntity.Login = user.Login;
            userEntity.Email = user.Email;
        }

        public async Task DeleteUserAsync(string id)
        {
            var userEntity = await _context.Users.FindAsync(id);
            if (userEntity == null)
            {
                throw new ObjectNotFoundException($"не существует пользователя c id: {id}");
            }
            
            _context.Users.Remove(userEntity);
        }

        public Task<bool> ExistsAsync(string id)
        {
            return _context.Users
                .AsNoTracking()
                .AnyAsync(userEntity => userEntity.Id == id);
        }
    }
}