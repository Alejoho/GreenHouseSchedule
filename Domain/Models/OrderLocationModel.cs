namespace Domain.Models
{
    /// <summary>
    /// Represents one order location from an OrderModel
    /// </summary>
    /// <remarks>
    /// One order can have more thant one order location
    /// </remarks>
    public class OrderLocationModel
    {
        private readonly int _ID;
        private readonly int _greenHouse;
        private readonly int _seedTrayType;
        private readonly int _orderID;
        private int _seedTrayAmount;
        private int _seedlingAmount;
        private DateOnly? _sowDate;
        private DateOnly? _estimateDeliveryDate;
        private DateOnly? _realDeliveryDate;
        private List<DeliveryDetailModel> _deliveryDetails;
        private bool _sown;

        /// <summary>
        /// Initializes a new instance of <c>OrderLocationModel</c>.
        /// </summary>
        /// <param name="pID">The single ID of the order location.</param>
        /// <param name="pGreenHouse">The single ID of the greenhouse in wich the order location is.</param>
        /// <param name="pSeedTrayType">The single ID of the seedtray type in wich the order location is sown.</param>
        /// <param name="pOrderID">The single ID of the order to wich the order location belongs.</param>
        /// <param name="pSeedTrayAmount">The amount of seedtrays used to sown the order location.</param>
        /// <param name="pSeedlingAmount">The total amount of seedling in the order location.</param>
        /// <param name="pSowDate">The date on which this order location is sown.</param>
        /// <param name="pEstimateDeliveryDate">The estimate date on which this order location is delivered.</param>
        /// <param name="pRealDeliveryDate">The real date on which this order location is delivered.</param>
        /// <param name="pSown">This indicates whether this order location is sown.</param>
        public OrderLocationModel(int pID, int pGreenHouse, int pSeedTrayType, int pOrderID, int pSeedTrayAmount, int pSeedlingAmount, DateOnly? pSowDate, DateOnly? pEstimateDeliveryDate, DateOnly? pRealDeliveryDate, bool pSown)
        {
            _ID = pID;
            _greenHouse = pGreenHouse;
            _seedTrayType = pSeedTrayType;
            _orderID = pOrderID;
            _seedTrayAmount = pSeedTrayAmount;
            _seedlingAmount = pSeedlingAmount;
            _sowDate = pSowDate;
            _estimateDeliveryDate = pEstimateDeliveryDate;
            _realDeliveryDate = pRealDeliveryDate;
            _deliveryDetails = new List<DeliveryDetailModel>();
            _sown = pSown;
        }

        /// <summary>
        /// Initializes a new instance of <c>OrderLocationModel</c> but without especify 
        /// the dates.
        /// </summary>
        /// <remarks>
        /// This constructor is used to create an instance of an <c>OrderLocationModel</c> for 
        /// the method <c>InsertOrderInProcessIntoSeedBedStatusAuxiliar</c> of the
        /// <c>DateIteratorAndResourceChecker</c> class.
        /// </remarks>
        /// <param name="pID">The single ID of an order location.</param>
        /// <param name="pSeedTrayType">The single ID of the seedtray type in wich the order location is sown.</param>
        /// <param name="pOrderID">The single ID of the order to wich the order location belongs.</param>
        /// <param name="pSeedTrayAmount">The amount of seedtrays used to sown the order location.</param>
        /// <param name="pSeedlingAmount">The total amount of seedling in the order location.</param>
        public OrderLocationModel(int pID, int pSeedTrayType, int pOrderID, int pSeedTrayAmount, int pSeedlingAmount)
        {
            _ID = pID;
            _greenHouse = -1;
            _seedTrayType = pSeedTrayType;
            _orderID = pOrderID;
            _seedTrayAmount = pSeedTrayAmount;
            _seedlingAmount = pSeedlingAmount;
            _sowDate = null;
            _estimateDeliveryDate = null;
            _realDeliveryDate = null;
            _deliveryDetails = new List<DeliveryDetailModel>();
            _sown = false;
        }

        /// <summary>
        /// Initializes a new duplicate instance of <c>OrderLocationModel</c> from another instance.
        /// </summary>
        /// <param name="pOrderLocationOriginal">The instance of the original <c>OrderLocationModel</c>.</param>
        public OrderLocationModel(OrderLocationModel pOrderLocationOriginal, int newId = 0)
        {
            if (newId == 0)
            {
                this._ID = pOrderLocationOriginal.ID;
            }
            else
            {
                this._ID = newId;
            }
            this._greenHouse = pOrderLocationOriginal.GreenHouse;
            this._seedTrayType = pOrderLocationOriginal.SeedTrayType;
            this._orderID = pOrderLocationOriginal.OrderID;
            this._seedTrayAmount = pOrderLocationOriginal.SeedTrayAmount;
            this._seedlingAmount = pOrderLocationOriginal.SeedlingAmount;
            this._sowDate = pOrderLocationOriginal.SowDate;
            this._estimateDeliveryDate = pOrderLocationOriginal.EstimateDeliveryDate;
            this._realDeliveryDate = pOrderLocationOriginal.RealDeliveryDate;
            this._deliveryDetails = new List<DeliveryDetailModel>();
            foreach (DeliveryDetailModel deliveryDetailModel in pOrderLocationOriginal.DeliveryDetails)
            {
                this._deliveryDetails.Add(new DeliveryDetailModel(deliveryDetailModel));
            }
            this._sown = pOrderLocationOriginal.Sown;
        }

        /// <value>
        /// Gets the ID of the order location
        /// </value>
        public int ID => _ID;

        /// <value>
        /// Gets the ID of the greenhouse in wich the order location is.
        /// </value>
        public int GreenHouse => _greenHouse;

        /// <value>
        /// Gets the ID of the seedtray type in wich the order location is sown.
        /// </value>
        public int SeedTrayType => _seedTrayType;

        /// <value>
        /// Gets the ID of the order to wich the order location belongs.
        /// </value>
        public int OrderID => _orderID;

        /// <value>
        /// Gets or sets the amount of seedtrays used to sown the order location.
        /// </value>
        public int SeedTrayAmount { get => _seedTrayAmount; set => _seedTrayAmount = value; }

        /// <value>
        /// Gets or sets the total amount of seedling in the order location.
        /// </value>
        public int SeedlingAmount { get => _seedlingAmount; set => _seedlingAmount = value; }

        /// <value>
        /// Gets or sets the date on which this order location is sown.
        /// </value>
        public DateOnly? SowDate { get => _sowDate; set => _sowDate = value; }

        /// <value>
        /// Gets or sets the estimate date on which this order location is delivered.
        /// </value>
        public DateOnly? EstimateDeliveryDate { get => _estimateDeliveryDate; set => _estimateDeliveryDate = value; }

        /// <value>
        /// Gets or sets the real date on which this order location is delivered.
        /// </value>
        public DateOnly? RealDeliveryDate { get => _realDeliveryDate; set => _realDeliveryDate = value; }

        /// <value>
        /// Gets or sets the <c>List</c> of delivery detatils that belongs to this order location.
        /// </value>
        public List<DeliveryDetailModel> DeliveryDetails { get => _deliveryDetails; set => _deliveryDetails = value; }

        /// <value>
        /// Gets or sets a bool type that indicates whether this order location is sown.
        /// </value>
        public bool Sown { get => _sown; set => _sown = value; }
    }
}