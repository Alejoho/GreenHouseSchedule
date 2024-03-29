﻿using SupportLayer.Models;

namespace DataAccess.Contracts
{
    public interface IDeliveryDetailRepository : IGenericRepository<DeliveryDetail>
    {
        IEnumerable<DeliveryDetail> GetByADeliveryDateOn(DateOnly date);
    }
}
