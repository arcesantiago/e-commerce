﻿namespace OrderService.Domain.Common
{
    public class BaseDomainModel
    {
        public int Id { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
    }
}