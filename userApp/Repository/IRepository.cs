using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using userApp.Models;

namespace userApp.Repository
{
    public interface IRepository<T> where T : BaseEntity
    {
        void Add(T item);
        void Remove(Guid id);
        void Update(T item);
        T FindByID(Guid id);
        IEnumerable<T> FindAll();
    }
}
