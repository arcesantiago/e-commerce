using MediatR;
using ProductService.Application.Common.Interfaces;
using ProductService.Application.Contracts.Infrastructure;

namespace ProductService.Application.Behaviours
{
    public class CachingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly ICacheService _cache;

        public CachingBehavior(ICacheService cache)
        {
            _cache = cache;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            if (request is not ICacheableQuery cacheable)
                return await next();

            var cached = await _cache.GetAsync<TResponse>(cacheable.CacheKey);
            if (cached is not null)
                return cached;

            var response = await next();

            await _cache.SetAsync(
                cacheable.CacheKey,
                response,
                cacheable.Expiration ?? TimeSpan.FromMinutes(5));

            return response;
        }
    }
}