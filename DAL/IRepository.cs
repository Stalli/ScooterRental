using System.Collections.Generic;

namespace DAL
{
    public interface IRepository<T> where T : class, IEntity
    {
        List<T> GetAll();
        T Get(string id);
        void Add(string id, T entity);
        void Update(T entity);
        void Delete(string id);
    }
}
