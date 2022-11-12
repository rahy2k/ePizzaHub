
using ePizzaHub.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ePizzaHub.Repositories.Impelementations
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
       protected DbContext _db;

        public Repository(DbContext db)
        {
            _db = db;
        }
        public void Add(TEntity entity)
        {
            _db.Set<TEntity>().Add(entity);
        }

        public void Delete(object id)
        {
            TEntity entity = _db.Set<TEntity>().Find(id);
            if(entity != null)
                _db.Remove(entity);
        }

        public TEntity Find(object id)
        {
            return _db.Set<TEntity>().Find(id);
        }

        public IEnumerable<TEntity> GetAll()
        {
            return _db.Set<TEntity>().ToList();
         }

        public int SaveChanges()
        {
           return _db.SaveChanges();
        }

        public void Update(TEntity entity)
        {
            _db.Set<TEntity>().Update(entity);
        }
    }
}
