using System.Linq;
using Functional.Maybe;
using System;

namespace MPE.Models.Interfaces
{
    public interface IRepository<T>
        where T : EntityAbstract
    {
        Maybe<T> Get(Guid id);
        void Save(T obj);
        void Delete(T obj);
        IQueryable<T> GetAll();
    }
}
