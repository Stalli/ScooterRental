using System.Collections.Generic;
using System.Linq;

namespace DAL
{
    public class MockRepository<T> : IRepository<T> where T : class, IEntity
    {
        public Dictionary<string, T> MainStorage { get; }

        public MockRepository()
        {
            MainStorage = new Dictionary<string, T>();
        }

        public void Add(string id, T entity)
        {
            MainStorage.Add(id, entity);
        }

        public void Delete(string id)
        {
            MainStorage.Remove(id);
        }

        public T Get(string id)
        {
            T result;
            return MainStorage.TryGetValue(id, out result) ? result : null;
        }

        public List<T> GetAll()
        {
            return MainStorage.Select(x => x.Value).ToList();
        }

        public void Update(T entity)
        {
            MainStorage[entity.Id] = entity;
        }
    }
}
