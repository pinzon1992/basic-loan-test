using Fundo.Applications.Domain.Entities;
using Fundo.Applications.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Fundo.Applications.Infrastructure.Common
{
    public class BaseRepository<TEntity>(AppDbContext context) : IBaseRepository<TEntity> where TEntity : BaseEntity
    {
        protected AppDbContext Context => context;
        public async Task<TEntity> AddAsync(TEntity entity)
        {
            var createdEntity = await Context.Set<TEntity>().AddAsync(entity);
            return createdEntity.Entity;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var entity = await GetByIdAsync(id);
            if (entity == null)
            {
                return false;
            }
            Context.Set<TEntity>().Remove(entity);

            return true;
        }

        public virtual async Task<List<TEntity>> GetAllAsync()
        {
            var entities = await Context.Set<TEntity>().ToListAsync();
            return entities;
        }

        public virtual async Task<TEntity?> GetByIdAsync(Guid id)
        {
            var entity = await Context.Set<TEntity>().FirstOrDefaultAsync(x => x.Id == id);
            return entity;
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await Context.SaveChangesAsync() > 0;
        }
    }
}
