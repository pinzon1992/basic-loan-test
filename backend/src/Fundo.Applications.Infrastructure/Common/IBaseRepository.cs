using Fundo.Applications.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Fundo.Applications.Infrastructure.Common
{
    public interface IBaseRepository<TEntity> where TEntity : BaseEntity
    {
        Task<List<TEntity>> GetAllAsync();
        Task<TEntity?> GetByIdAsync(Guid id);
        Task<TEntity> AddAsync(TEntity entity);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> SaveChangesAsync();

    }
}
