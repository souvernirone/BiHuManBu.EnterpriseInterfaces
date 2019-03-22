using BiHuManBu.ExternalInterfaces.Models.IRepository;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Repository
{
    public class EfRepositoryBase<TEntity> : IRepositoryBase<TEntity> where TEntity : class
    {

        public DbContext Context;

        public EfRepositoryBase(DbContext context)
        {
            Context = context;
        }

        public virtual DbSet<TEntity> Table { get { return Context.Set<TEntity>(); } }

        public bool Any(Expression<Func<TEntity, bool>> predicate)
        {
            return GetAll().Any(predicate);
        }

        public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await GetAll().AnyAsync(predicate);
        }

        public int Count()
        {
            return GetAll().Count();
        }

        public int Count(Expression<Func<TEntity, bool>> predicate)
        {
            return GetAll().Where(predicate).Count();
        }

        public void Delete(TEntity entity)
        {
            AttachIfNot(entity);
            Table.Remove(entity);
        }

        public void Delete(Expression<Func<TEntity, bool>> predicate)
        {
            foreach (var entity in GetAll().Where(predicate).ToList())
            {
                Delete(entity);
            }
        }

        public void Delete<TEntity1>(TEntity1 entity) where TEntity1 : class
        {
            AttachIfNot(entity);
            Context.Set<TEntity1>().Remove(entity);
        }

        public TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            return GetAll().FirstOrDefault(predicate);
        }

        public async Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await Context.Set<TEntity>().FirstOrDefaultAsync(predicate);
        }

        public IQueryable<TEntity> GetAll()
        {
            return Table;
        }

        public IQueryable<TEntity1> GetAll<TEntity1>() where TEntity1 : class
        {
            return Context.Set<TEntity1>();
        }

        public List<TEntity> GetAllList()
        {
            return GetAll().ToList();
        }

        public List<TEntity> GetList(Expression<Func<TEntity, bool>> predicate)
        {
            return GetAll().Where(predicate).ToList();
        }

        public Task<List<TEntity>> GetListAsync()
        {
            return Task.FromResult(GetAllList());
        }

        public Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return Task.FromResult(GetList(predicate));
        }

        public DbContext GetDbContext()
        {
            return Context;
        }

        public void Insert(TEntity entity)
        {
            Table.Add(entity);
        }

        public void Insert<TEntity1>(TEntity1 entity) where TEntity1 : class
        {
            Context.Set<TEntity1>().Add(entity);
        }

        public async Task InsertAsync(TEntity entity)
        {
            await Task.FromResult(Table.Add(entity));
        }

        public async  Task InsertAsync(List<TEntity> listEntity)
        {
            foreach (var item in listEntity)
            {
                await InsertAsync(item);
            }
        }

        public long LongCount()
        {
            return GetAll().LongCount();
        }

        public long LongCount(Expression<Func<TEntity, bool>> predicate)
        {
            return GetAll().Where(predicate).LongCount();
        }

        public Task<long> LongCountAsync()
        {
            return Task.FromResult(LongCount());
        }

        public Task<long> LongCountAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return Task.FromResult(LongCount(predicate));
        }

        public int SaveChanges()
        {
            return Context.SaveChanges();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await Context.SaveChangesAsync();
        }

        public TEntity Single(Expression<Func<TEntity, bool>> predicate)
        {
            return GetAll().Single(predicate);
        }

        public Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return Task.FromResult(Single(predicate));
        }

        public void Update(TEntity entity)
        {
            AttachIfNot(entity);
            Context.Entry(entity).State = EntityState.Modified;
        }

        public void Update<TEntity1>(TEntity1 entity) where TEntity1 : class
        {
            AttachIfNot(entity);
            Context.Entry(entity).State = EntityState.Modified;
        }

        protected virtual void AttachIfNot(TEntity entity)
        {
            if (!Table.Local.Contains(entity))
            {
                Table.Attach(entity);
            }
        }

        protected virtual void AttachIfNot<TEntity1>(TEntity1 entity) where TEntity1 : class
        {
            if (!Context.Set<TEntity1>().Local.Contains(entity))
            {
                Context.Set<TEntity1>().Attach(entity);
            }
        }

        public void Insert(List<TEntity> listEntity)
        {
            listEntity.ForEach(o => Insert(o));
        }
    }
}
