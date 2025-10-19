﻿namespace ProductService.Application.Common.Interfaces
{
    public interface ICacheableQuery
    {
        string CacheKey { get; }
        TimeSpan? Expiration { get; }
    }
}
