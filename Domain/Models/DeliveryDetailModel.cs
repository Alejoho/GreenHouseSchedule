namespace Domain.Models
{
    /// <summary>
    /// Represents the details of a delivery to its client.
    /// </summary>
    public class DeliveryDetailModel
    {
        private readonly long _ID;
        private readonly int _orderLocationID;
        private readonly DateOnly _deliveryDate;
        private readonly int _seedTrayAmountDelivered;

        /// <summary>
        /// Initializes a new instance of <c>DeliveryDetailModel</c>.
        /// </summary>
        /// <param name="pID">The single ID of the delivery.</param>
        /// <param name="pOrderLocationID">The single ID of the order location to wich the delivery belongs.</param>
        /// <param name="pDeliveryDate">The real date on wich the delivery it is done.</param>
        /// <param name="pSeedTrayAmountDelivered">The amount of seedtrays delivered.</param>
        public DeliveryDetailModel(long pID, int pOrderLocationID, DateOnly pDeliveryDate, int pSeedTrayAmountDelivered)
        {
            _ID = pID;
            _orderLocationID = pOrderLocationID;
            _deliveryDate = pDeliveryDate;
            _seedTrayAmountDelivered = pSeedTrayAmountDelivered;
        }

        /// <summary>
        /// Initializes a new duplicate instance of <c>DeliveryDetailModel</c> from another instance.
        /// </summary>
        /// <param name="pDeliveryDetailModelOriginal">The instance of the original <c>DeliveryDetailModel</c>.</param>
        public DeliveryDetailModel(DeliveryDetailModel pDeliveryDetailModelOriginal)
        {
            this._ID = pDeliveryDetailModelOriginal.ID;
            this._orderLocationID = pDeliveryDetailModelOriginal.OrderLocationID;
            this._deliveryDate = pDeliveryDetailModelOriginal.DeliveryDate;
            this._seedTrayAmountDelivered = pDeliveryDetailModelOriginal.SeedTrayAmountDelivered;
        }

        public long ID => _ID;

        public int OrderLocationID => _orderLocationID;

        public DateOnly DeliveryDate => _deliveryDate;

        public int SeedTrayAmountDelivered => _seedTrayAmountDelivered;
    }
}