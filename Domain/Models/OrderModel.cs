namespace Domain.Models
{
    /// <summary>
    /// Represents an order to be sown in the seedbed.
    /// </summary>
    /// <remarks>
    /// Includes all the orderlocations that belongs to an order.
    /// </remarks>
    public class OrderModel
    {
        private readonly int _ID;
        private readonly ClientModel _client;
        private readonly ProductModel _product;
        private int _seedlingAmount;
        private readonly DateOnly _requestDate;
        private DateOnly? _estimateSowDate;
        private DateOnly? _estimateDeliveryDate;
        private DateOnly? _realSowDate;
        private DateOnly? _realDeliveryDate;
        private LinkedList<OrderLocationModel> _orderLocations;
        private bool _complete;

        /// <summary>
        /// Initializes a new instance of <c>OrderModel</c>. Tiene un cliente <paramref name="pClient"/>
        /// </summary>
        /// <param name="pID">The single ID of the order.</param>
        /// <param name="pClient">The instance of the <c>ClientModel</c> that owns this order.</param>
        /// <param name="pProduct">The instance of the <c>ProductModel</c> for which this order is made up.</param>
        /// <param name="pSeedlingAmount">The amount of seedling.</param>
        /// <param name="pRequestDate">The date at which this order is made up.</param>
        /// <param name="pEstimateSowDate">The estimate date on which the order must be sown.</param>
        /// <param name="pEstimateDeliveryDate">The estimate date on which the order must be delivered</param>
        /// <param name="pRealSowDate">The real date on which the order is started to sow.</param>
        /// <param name="pRealDeliveryDate">The real date on which the order is started to deliver.</param>
        /// <param name="pSown">This indicates whether this order is completely sown.</param>
        public OrderModel(int pID, ClientModel pClient, ProductModel pProduct, int pSeedlingAmount, DateOnly pRequestDate, DateOnly pEstimateSowDate, DateOnly? pEstimateDeliveryDate, DateOnly? pRealSowDate, DateOnly? pRealDeliveryDate, bool pComplete)
        {
            _ID = pID;
            _client = pClient;
            _product = pProduct;
            _seedlingAmount = pSeedlingAmount;
            _requestDate = pRequestDate;
            _estimateSowDate = pEstimateSowDate;
            _estimateDeliveryDate = pEstimateDeliveryDate;
            _realSowDate = pRealSowDate;
            _realDeliveryDate = pRealDeliveryDate;
            _orderLocations = new LinkedList<OrderLocationModel>();
            _complete = pComplete;
        }

        /// <summary>
        /// Initializes a new duplicate instance of <c>OrderModel</c> from another instance.
        /// </summary>
        /// <param name="pOrderModelOriginal">The instance of the original <c>OrderModel</c>.</param>
        public OrderModel(OrderModel pOrderModelOriginal, int newId = 0)
        {
            if (newId == 0)
            {
                this._ID = pOrderModelOriginal.ID;
            }
            else
            {
                this._ID = newId;
            }
            this._client = new ClientModel(pOrderModelOriginal.Client);
            this._product = new ProductModel(pOrderModelOriginal.Product);
            this._seedlingAmount = pOrderModelOriginal.SeedlingAmount;
            this._requestDate = pOrderModelOriginal.RequestDate;
            this._estimateSowDate = pOrderModelOriginal.EstimateSowDate;
            this._estimateDeliveryDate = pOrderModelOriginal.EstimateDeliveryDate;
            this._realSowDate = pOrderModelOriginal.RealSowDate;
            this._realDeliveryDate = pOrderModelOriginal.RealDeliveryDate;
            this._orderLocations = new LinkedList<OrderLocationModel>();
            this._complete = pOrderModelOriginal.Complete;

            foreach (OrderLocationModel orderLocationModel in pOrderModelOriginal.OrderLocations)
            {
                this._orderLocations.AddLast(new OrderLocationModel(orderLocationModel));
            }
        }

        internal void AdvanceEstimateSowDateOneDay()
        {
            this._estimateSowDate = this.EstimateSowDate?.AddDays(1);
        }

        internal void GoBackEstimateSowDateOneDay()
        {
            this._estimateSowDate = this.EstimateSowDate?.AddDays(-1);
        }

        internal void SetEstimateDates(DateOnly estimateSowDate, DateOnly estimateDeliveryDate)
        {
            this._estimateSowDate = estimateSowDate;
            this._estimateDeliveryDate = estimateDeliveryDate;
        }

        /// <value>
        /// Gets the ID of the order.
        /// </value>
        public int ID => _ID;

        /// <value>
        /// Gets the instance of the <c>ClientModel</c> that owns this order
        /// </value>
        public ClientModel Client => _client;

        /// <value>
        /// Gets the instance of the <c>ProductModel</c> for which this order is made up.
        /// </value>
        public ProductModel Product => _product;

        /// <value>
        /// Gets or sets the amount of seedling
        /// </value>
        public int SeedlingAmount { get => _seedlingAmount; set => _seedlingAmount = value; }

        /// <value>
        /// Gets the date on which this order is made up.
        /// </value>
        public DateOnly RequestDate => _requestDate;

        /// <value>
        /// Gets the estimate date on which the order must be sown.
        /// </value>
        public DateOnly? EstimateSowDate => _estimateSowDate;

        /// <value>
        /// Gets estimate date on which the order must be delivered.
        /// </value>
        public DateOnly? EstimateDeliveryDate => _estimateDeliveryDate;

        /// <value>
        /// Gets the real date on which the order is started to sow.
        /// </value>
        public DateOnly? RealSowDate { get => _realSowDate; set => _realSowDate = value; }

        /// <value>
        /// Gets the real date on which the order is started to deliver.
        /// </value>
        public DateOnly? RealDeliveryDate { get => _realDeliveryDate; set => _realDeliveryDate = value; }

        /// <value>
        /// Gets or sets the <c>Linkedlist</c> of order locations that belongs to this order.
        /// </value>        
        public LinkedList<OrderLocationModel> OrderLocations { get => _orderLocations; set => _orderLocations = value; }

        /// <value>
        /// Gets or sets a bool type that indicates whether this order is completely sown.
        /// </value>
        public bool Complete { get => _complete; set => _complete = value; }
    }
}