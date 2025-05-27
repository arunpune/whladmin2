using System.Collections.Generic;
using System.Threading.Tasks;

namespace WHLAdmin.Common.Repositories;

public interface IReadOnlyRepository<T> where T : class
{
    Task<IEnumerable<T>> GetAll();
    Task<T> GetOne(T item);
}

public interface ICRUDRepository<T> : IReadOnlyRepository<T> where T : class
{
    Task<bool> Add(string correlationId, T item);
    Task<bool> Update(string correlationId, T item);
    Task<bool> Delete(string correlationId, T item);
}