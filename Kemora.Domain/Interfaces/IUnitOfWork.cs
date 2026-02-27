using System;
using System.Threading.Tasks;

namespace Kemora.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        Task<int> CommitAsync();
    }
}
