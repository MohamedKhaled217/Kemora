using System;

namespace Kemora.Application.Interfaces
{
    public interface ICacheService
    {
        T? Get<T>(string key);
        void Set<T>(string key, T value, TimeSpan? absoluteExpireTime = null);
        void Remove(string key);
    }
}
