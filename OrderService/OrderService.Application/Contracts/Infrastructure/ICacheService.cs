﻿namespace OrderService.Application.Contracts.Infrastructure
{
    public interface ICacheService
    {
        Task<T?> GetAsync<T>(string key);
        Task SetAsync<T>(string key, T value, TimeSpan ttl);
        Task RemoveAsync(string key);
    }
}
