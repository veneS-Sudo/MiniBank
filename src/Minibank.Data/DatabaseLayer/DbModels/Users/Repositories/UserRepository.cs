using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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

        public async Task<User> GetByIdAsync(string id, CancellationToken cancellationToken)
        {
            var userEntity = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(user => user.Id == id, cancellationToken);
            if (userEntity == null)
            {
                throw new ObjectNotFoundException($"не существует пользователя c id: {id}");
            }

            return _mapper.Map<User>(userEntity);
        }

        public async Task<List<User>> GetAllUsersAsync(CancellationToken cancellationToken)
        {
            return await _context.Users
                    .AsNoTracking()
                    .Select(userEntity => _mapper.Map<User>(userEntity))
                    .ToListAsync(cancellationToken);
        }

        public async Task<User> CreateUserAsync(User user, CancellationToken cancellationToken)
        {
            var userEntity = _mapper.Map<UserEntity>(user);
            userEntity.Id = Guid.NewGuid().ToString();
            
            var createUser = await _context.Users.AddAsync(userEntity, cancellationToken);
            
            return _mapper.Map<User>(createUser.Entity);
        }

        public async Task<bool> UpdateUserAsync(User user, CancellationToken cancellationToken)
        {
            var userEntity = await _context.Users.FindAsync(new object[] { user.Id }, cancellationToken);
            if (userEntity == null)
            {
                throw new ObjectNotFoundException($"не существует пользователя c id: {user.Id}");
            }
            
            userEntity.Login = user.Login;
            userEntity.Email = user.Email;

            return true;
        }

        public async Task<bool> DeleteUserAsync(string id, CancellationToken cancellationToken)
        {
            var userEntity = await _context.Users.FindAsync(new object[] {id}, cancellationToken);
            if (userEntity == null)
            {
                throw new ObjectNotFoundException($"не существует пользователя c id: {id}");
            }
            
            var deleteUser = _context.Users.Remove(userEntity);
            return deleteUser.State == EntityState.Deleted;
        }

        public async Task<bool> IsExistAsync(string id, CancellationToken cancellationToken)
        {
            return await _context.Users
                .AsNoTracking()
                .AnyAsync(userEntity => userEntity.Id == id, cancellationToken);
        }
    }
}