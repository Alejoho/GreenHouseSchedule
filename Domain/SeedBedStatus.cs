using DataAccess.Contracts;
using DataAccess.Repositories;
using Domain.Models;
using Domain.Processors;
using SupportLayer;
using System.Collections;
using System.Configuration;

namespace Domain
{
    //NEXT - implement a new logic to remove the strict place of the order location in the designate house.
    //When a store a new order I don't specify in what greenhouse to put it, that information is going to be
    //specify later in the unsteeve window.

    //TODO - Make some clean up of this class

    //TODO - I think it'd be good to change the evaluation of availability of sow seedtray per day from seedtray to 
    //seedlings, because diferent types of seedtrays change the amount of seedtray that can be sown in one day
    // -----(mientras que) amount of seedling remaings the same.

    /// <summary>
    /// Represents the amount of resources in the seed bed 
    /// at any given time.
    /// </summary>    
    public class SeedBedStatus
    {
        #region Fields      
        private DateOnly _iteratorDate;
        private DateOnly _presentDate;
        private List<GreenHouseModel> _greenHouses;
        private List<SeedTrayModel> _seedTrays;
        private LinkedList<OrderModel> _orders;
        private LinkedList<OrderLocationModel> _orderLocations;
        private List<DeliveryDetailModel> _deliveryDetails;
        //private DateTime _daysOfProduction;
        private readonly int _maxAmountOfSeedTrayToSowPerDay;
        private int _remainingAmountOfSeedTrayToSowPerDay;
        private readonly int _minimumLimitOfSeedTrayToSow;

        private readonly decimal _generalTotalArea;
        private decimal _generalUsedArea;
        private decimal _generalAvailableArea;

        private IGreenHouseRepository _greenHouseRepository;
        private ISeedTrayRepository _seedTrayRepository;

        private IOrderProcessor _orderProcessor;
        private IOrderLocationProcessor _orderLocationProcessor;
        private IDeliveryDetailProcessor _deliveryDetailProcessor;

        private ArrayList _ordersToDelete;
        private ArrayList _orderLocationsToDelete;
        private ArrayList _deliveryDetailsToDelete;
        private ArrayList _orderLocationsToAdd;


        #endregion

        #region Constructors

        /// <summary>
        /// This is a constructor for a unit test scenario.
        /// </summary>
        /// <param name="presentDate">The date to use as the present date</param>
        /// <param name="greenHouseRepo">The <c>GreenHouseRepository</c> or a Mock of it</param>
        /// <param name="seedTrayRepo">The <c>SeedTrayRepository</c> or a Mock of it</param>
        /// <param name="orderProcessor">The <c>OrderProcessor</c> or a Mock of it</param>
        /// <param name="orderLocationProcessor">The <c>OrderLocationProcessor</c> or a Mock of it</param>
        /// <param name="deliveryDetailProcessor">The <c>DeliveryDetailProcessor</c> or a Mock of it</param>
        /// <param name="working">A boolean to specify if the object needs to be fully working</param>
        internal SeedBedStatus(DateOnly? presentDate = null,
            IGreenHouseRepository greenHouseRepo = null,
            ISeedTrayRepository seedTrayRepo = null,
            IOrderProcessor orderProcessor = null,
            IOrderLocationProcessor orderLocationProcessor = null,
            IDeliveryDetailProcessor deliveryDetailProcessor = null,
            bool working = false)
        {
            if (presentDate != null)
            {
                _presentDate = (DateOnly)presentDate;
                _iteratorDate = ((DateOnly)presentDate).AddDays(-90);
            }

            _maxAmountOfSeedTrayToSowPerDay = 500;
            _minimumLimitOfSeedTrayToSow = 50;
            _remainingAmountOfSeedTrayToSowPerDay = _maxAmountOfSeedTrayToSowPerDay;

            _ordersToDelete = new ArrayList();
            _orderLocationsToDelete = new ArrayList();
            DeliveryDetailsToDelete = new ArrayList();
            _orderLocationsToAdd = new ArrayList();

            if (greenHouseRepo != null)
            {
                _greenHouseRepository = greenHouseRepo;
                _greenHouses = GetGreenHouses();

            }
            if (seedTrayRepo != null)
            {
                _seedTrayRepository = seedTrayRepo;
                _seedTrays = GetSeedTrays();
            }
            if (orderProcessor != null)
            {
                _orderProcessor = orderProcessor;
                _orders = GetMajorityDataOfOrders();
            }
            if (orderLocationProcessor != null)
            {
                _orderLocationProcessor = orderLocationProcessor;
                _orderLocations = GetOrderLocations();
            }
            if (deliveryDetailProcessor != null)
            {
                _deliveryDetailProcessor = deliveryDetailProcessor;
                _deliveryDetails = GetDeliveryDetails();
            }

            if (orderLocationProcessor != null && deliveryDetailProcessor != null)
            {
                FillDeliveryDetails();
            }
            if (orderProcessor != null && orderLocationProcessor != null)
            {
                FillOrderLocations();
            }

            if (orderProcessor != null
                && orderLocationProcessor != null
                && deliveryDetailProcessor != null
                && greenHouseRepo != null
                && seedTrayRepo != null
                && working == true)
            {
                DayByDayToCurrentDate();

                _generalAvailableArea = this.GreenHouses.Where(x => x.Active == true).Sum(x => x.SeedTrayAvailableArea);
                _generalUsedArea = this.GreenHouses.Where(x => x.Active == true).Sum(x => x.SeedTrayUsedArea);
            }
        }

        /// <summary>
        /// Initialize a new instance of <c>SeedBedStatus</c>
        /// </summary>
        public SeedBedStatus()
        {
            int daysToMoveBack;
            int.TryParse(ConfigurationManager.AppSettings[ConfigurationNames.RegressionDays], out daysToMoveBack);

            _presentDate = DateOnly.FromDateTime(DateTime.Now);

            _iteratorDate = _presentDate.AddDays(-daysToMoveBack);

            int seedTraysPerDay;
            int.TryParse(ConfigurationManager.AppSettings[ConfigurationNames.DailySowingPotential], out seedTraysPerDay);
            _maxAmountOfSeedTrayToSowPerDay = seedTraysPerDay;
            _remainingAmountOfSeedTrayToSowPerDay = _maxAmountOfSeedTrayToSowPerDay;

            int minimumLimitOfSow;
            int.TryParse(ConfigurationManager.AppSettings["MinimumLimitOfSow"], out minimumLimitOfSow);
            _minimumLimitOfSeedTrayToSow = minimumLimitOfSow;

            _greenHouseRepository = new GreenHouseRepository();
            _seedTrayRepository = new SeedTrayRepository();

            _orderProcessor = new OrderProcessor();
            _orderLocationProcessor = new OrderLocationProcessor();
            _deliveryDetailProcessor = new DeliveryDetailProcessor();

            _ordersToDelete = new ArrayList();
            _orderLocationsToDelete = new ArrayList();
            _deliveryDetailsToDelete = new ArrayList();
            _orderLocationsToAdd = new ArrayList();

            _greenHouses = GetGreenHouses();

            _generalTotalArea = this.GreenHouses.Where(x => x.Active == true).Sum(x => x.SeedTrayTotalArea);

            _seedTrays = GetSeedTrays();
            _orders = GetMajorityDataOfOrders();
            _orderLocations = GetOrderLocations();
            _deliveryDetails = GetDeliveryDetails();

            FillDeliveryDetails();

            FillOrderLocations();

            DayByDayToCurrentDate();

            _generalAvailableArea = this.GreenHouses.Where(x => x.Active == true).Sum(x => x.SeedTrayAvailableArea);
            _generalUsedArea = this.GreenHouses.Where(x => x.Active == true).Sum(x => x.SeedTrayUsedArea);
        }

        /// <summary>
        /// Initializes a new duplicate instance of <c>SeedBedStatus</c> from another instance.
        /// </summary>
        /// <param name="pOriginalSeedBedStatus">The instance of the original <c>SeedBedStatus</c></param>

        public SeedBedStatus(SeedBedStatus pOriginalSeedBedStatus)
        {
            this._iteratorDate = pOriginalSeedBedStatus.IteratorDate;
            this._presentDate = pOriginalSeedBedStatus._presentDate;

            this._greenHouses = new List<GreenHouseModel>();
            this._seedTrays = new List<SeedTrayModel>();
            this._orders = new LinkedList<OrderModel>();
            this._orderLocations = new LinkedList<OrderLocationModel>();
            this._deliveryDetails = new List<DeliveryDetailModel>();

            //CHECK - I don't remeber what this variable is for.
            //this._daysOfProduction = pOriginalSeedBedStatus.DaysOfProduction;
            this._maxAmountOfSeedTrayToSowPerDay = pOriginalSeedBedStatus.MaxAmountOfSeedTrayToSowPerDay;
            this._remainingAmountOfSeedTrayToSowPerDay = pOriginalSeedBedStatus.RemainingAmountOfSeedTrayToSowPerDay;
            this._minimumLimitOfSeedTrayToSow = pOriginalSeedBedStatus._minimumLimitOfSeedTrayToSow;

            this._generalTotalArea = pOriginalSeedBedStatus._generalTotalArea;
            this._generalUsedArea = pOriginalSeedBedStatus._generalUsedArea;
            this._generalAvailableArea = pOriginalSeedBedStatus._generalAvailableArea;

            this._greenHouseRepository = new GreenHouseRepository();
            this._seedTrayRepository = new SeedTrayRepository();

            _orderProcessor = new OrderProcessor();
            _orderLocationProcessor = new OrderLocationProcessor();
            _deliveryDetailProcessor = new DeliveryDetailProcessor();

            this._ordersToDelete = new ArrayList();
            this._orderLocationsToDelete = new ArrayList();
            this._deliveryDetailsToDelete = new ArrayList();
            this._orderLocationsToAdd = new ArrayList();

            foreach (GreenHouseModel greenHouseModel in pOriginalSeedBedStatus.GreenHouses)
            {
                this._greenHouses.Add(new GreenHouseModel(greenHouseModel));
            }

            foreach (SeedTrayModel seedTrayModel in pOriginalSeedBedStatus.SeedTrays)
            {
                this._seedTrays.Add(new SeedTrayModel(seedTrayModel));
            }

            foreach (OrderModel orderModel in pOriginalSeedBedStatus.Orders)
            {
                OrderModel newOrder = new OrderModel(orderModel);

                this._orders.AddLast(newOrder);

                foreach (OrderLocationModel orderLocationModel in newOrder.OrderLocations)
                {
                    this._orderLocations.AddLast(orderLocationModel);

                    foreach (DeliveryDetailModel deliveryDetailModel in orderLocationModel.DeliveryDetails)
                    {
                        this._deliveryDetails.Add(deliveryDetailModel);
                    }
                }
            }
        }

        #endregion


        #region Methods to Fill Data of SeedBedStatus

        /// <summary>
        /// Gets all the green houses of the seedbed.
        /// </summary>
        /// <returns>Returns a <c>List<GreenHouseModel>.</c></returns>

        private List<GreenHouseModel> GetGreenHouses()
        {
            var greenHouses = _greenHouseRepository.GetAll();
            List<GreenHouseModel> greenHousesModelList = new List<GreenHouseModel>();
            foreach (var greenHouse in greenHouses)
            {
                greenHousesModelList.Add(new GreenHouseModel(
                    greenHouse.Id,
                    greenHouse.Name,
                    greenHouse.SeedTrayArea,
                    greenHouse.AmountOfBlocks,
                    greenHouse.Active
                    ));
            }
            return greenHousesModelList;
        }

        /// <summary>
        /// Gets all the seedtrays of the seedbed.
        /// </summary>
        /// <returns>Returns a <c>List<SeedTrayModel>.</returns>

        private List<SeedTrayModel> GetSeedTrays()
        {
            var seedTrays = _seedTrayRepository.GetAll();
            List<SeedTrayModel> seedTrayModelList = new List<SeedTrayModel>();
            foreach (var seedTray in seedTrays)
            {
                seedTrayModelList.Add(new SeedTrayModel(
                    seedTray.Id,
                    seedTray.Name,
                    seedTray.LogicalTrayArea,
                    seedTray.TotalAlveolus,
                    seedTray.TotalAmount,
                    seedTray.Active
                    ));
            }
            return seedTrayModelList;
        }

        /// <summary>
        /// Gets all the orders with all their information but their order locations.
        /// </summary>
        /// <returns>Returns a <c> LinkedList<OrderModel></c>.</returns>
        private LinkedList<OrderModel> GetMajorityDataOfOrders()
        {
            var orderList = _orderProcessor.GetOrdersFromADateOn(_iteratorDate).ToList();

            LinkedList<OrderModel> orderModelLinkedList = new LinkedList<OrderModel>();

            foreach (var order in orderList)
            {
                orderModelLinkedList.AddLast(new OrderModel(
                    order.Id,
                    new ClientModel(order.Client.Id, order.Client.Name, order.Client.NickName),
                    new ProductModel(order.Product.Id, order.Product.Variety, order.Product.Specie.Name, order.Product.Specie.ProductionDays),
                    order.AmountOfAlgorithmSeedlings,
                    order.DateOfRequest,
                    order.EstimateSowDate,
                    order.EstimateDeliveryDate,
                    order.RealSowDate,
                    order.RealDeliveryDate,
                    order.Complete
                    ));
            }

            return orderModelLinkedList;
        }

        /// <summary>
        /// Gets all the order locations with all their information but their delivery details.
        /// </summary>
        /// <returns>Returns a <c>LinkedList<OrderLocationModel></c>.</returns>
        private LinkedList<OrderLocationModel> GetOrderLocations()
        {
            var orderLocations = _orderLocationProcessor
                .GetOrderLocationsFromADateOn(_iteratorDate)
                .ToList();

            LinkedList<OrderLocationModel> orderLocationModelLinkedList = new LinkedList<OrderLocationModel>();

            foreach (var orderLocation in orderLocations)
            {
                orderLocationModelLinkedList.AddLast(new OrderLocationModel(
                    orderLocation.Id,
                    orderLocation.GreenHouseId,
                    orderLocation.SeedTrayId,
                    orderLocation.OrderId,
                    orderLocation.SeedTrayAmount,
                    orderLocation.SeedlingAmount,
                    orderLocation.SowDate,
                    orderLocation.EstimateDeliveryDate,
                    orderLocation.RealDeliveryDate,
                    orderLocation.SowDate == null ? false : true
                    ));
            }

            return orderLocationModelLinkedList;
        }

        /// <summary>
        /// Gets all the delivery details.
        /// </summary>
        /// <returns>Returns a List<DeliveryDetailModel></returns>
        private List<DeliveryDetailModel> GetDeliveryDetails()
        {
            var deliveryDetails = _deliveryDetailProcessor
                .GetDeliveryDetailFromADateOn(_iteratorDate)
                .ToList();

            List<DeliveryDetailModel> deliveryDetailModelList = new List<DeliveryDetailModel>();

            foreach (var deliveryDetail in deliveryDetails)
            {
                deliveryDetailModelList.Add(new DeliveryDetailModel(
                    deliveryDetail.Id,
                    deliveryDetail.Block.OrderLocationId,
                    deliveryDetail.DeliveryDate,
                    deliveryDetail.SeedTrayAmountDelivered));
            }

            return deliveryDetailModelList;
        }

        /// <summary>
        /// Fills the DeliveryDetails property of each of the order locations passed in the LinkedList.
        /// </summary>
        /// <param name="pOrderLocationModelLinkedList">LinkedList<OrderLocationModel> to fill with their delivery details.</param>
        private void FillDeliveryDetails()
        {
            foreach (var orderLocation in _orderLocations)
            {
                orderLocation.DeliveryDetails =
                    (from deliveryDetailElement in _deliveryDetails
                     where deliveryDetailElement.OrderLocationID == orderLocation.ID
                     orderby deliveryDetailElement.DeliveryDate
                     select deliveryDetailElement).ToList();
            }
        }

        /// <summary>
        /// Gets the order locations with their delivery details.
        /// </summary>
        private void FillOrderLocations()
        {
            foreach (var order in _orders)
            {
                var orderLocationsToAdd = from orderLocationElement in _orderLocations
                                          where orderLocationElement.OrderID == order.ID
                                          orderby orderLocationElement.ID
                                          select orderLocationElement;

                foreach (var orderLocation in orderLocationsToAdd)
                {
                    order.OrderLocations.AddLast(orderLocation);
                }
            }
        }

        #endregion


        #region Methods to iterate until CurrentDate

        /// <summary>
        /// Advances day by day the status of the seedbed until it reaches to the present day.
        /// </summary>
        private void DayByDayToCurrentDate()
        {
            do
            {
                ImplementRelease();
                ImplementReservation();
                UpdateObjects();
                ClearArrayLists();
                _iteratorDate = _iteratorDate.AddDays(1);
            } while (_iteratorDate < _presentDate);
            ImplementDelayRelease();
            UpdateObjects();
            ClearArrayLists();
        }

        /// <summary>
        /// Marks as used, the area and the seedtrays used by the order locations that were sown on the iterator date.
        /// </summary>
        private void ImplementReservation()
        {
            foreach (OrderModel order in _orders)
            {
                foreach (OrderLocationModel orderLocation in order.OrderLocations)
                {
                    if (orderLocation.SowDate == _iteratorDate)
                    {
                        ReserveSeedTray(orderLocation.SeedTrayAmount, orderLocation.SeedTrayType);
                        ReserveArea(orderLocation.SeedTrayAmount, orderLocation.SeedTrayType, orderLocation.GreenHouse);
                    }
                }
            }
        }

        /// <summary>
        /// Marks as free, the area and the seedtrays used by the order locations that were delivered on the iterator date.
        /// </summary>
        private void ImplementRelease()
        {
            foreach (OrderModel order in _orders)
            {
                foreach (OrderLocationModel orderLocation in order.OrderLocations)
                {
                    foreach (DeliveryDetailModel deliveryDetail in orderLocation.DeliveryDetails)
                    {
                        if (deliveryDetail.DeliveryDate == _iteratorDate)
                        {
                            ReleaseSeedTray(deliveryDetail.SeedTrayAmountDelivered, orderLocation.SeedTrayType);
                            ReleaseArea(deliveryDetail.SeedTrayAmountDelivered, orderLocation.SeedTrayType, orderLocation.GreenHouse);
                            orderLocation.SeedTrayAmount -= deliveryDetail.SeedTrayAmountDelivered;
                            DeliveryDetailsToDelete.Add(deliveryDetail);
                        }
                    }
                    if (orderLocation.SeedTrayAmount == 0)
                    {
                        order.SeedlingAmount -= orderLocation.SeedlingAmount;
                        _orderLocationsToDelete.Add(orderLocation);
                    }
                }
                if (order.SeedlingAmount <= 0)
                {
                    _ordersToDelete.Add(order);
                }
            }
        }

        /// <summary>
        /// Marks as free the area and the seedtrays used by all the order locations left, in the seedbed, to be delivered.
        /// </summary>
        private void ImplementDelayRelease()
        {
            foreach (OrderModel order in _orders)
            {
                foreach (OrderLocationModel orderLocation in order.OrderLocations)
                {
                    if (orderLocation.EstimateDeliveryDate < _iteratorDate)
                    {
                        ReleaseSeedTray(orderLocation.SeedTrayAmount, orderLocation.SeedTrayType);
                        ReleaseArea(orderLocation.SeedTrayAmount, orderLocation.SeedTrayType, orderLocation.GreenHouse);
                        order.SeedlingAmount -= orderLocation.SeedlingAmount;
                        _deliveryDetailsToDelete.AddRange(orderLocation.DeliveryDetails);
                        _orderLocationsToDelete.Add(orderLocation);
                    }
                }
                if (order.SeedlingAmount <= 0)
                {
                    _ordersToDelete.Add(order);
                }
            }
        }

        /// <summary>
        /// Eliminates the delivery details delivered of the day, 
        /// from the general list and the order location list.
        /// </summary>
        private void RemoveDeliveryDetails()
        {
            for (int i = 0; i < DeliveryDetailsToDelete.Count; i++)
            {
                DeliveryDetailModel deliveryDetailToDelete = (DeliveryDetailModel)DeliveryDetailsToDelete[i];

                _deliveryDetails.Remove(deliveryDetailToDelete);

                OrderLocationModel orderLocation = _orderLocations
                    .First(x => x.ID == deliveryDetailToDelete.OrderLocationID);

                orderLocation.DeliveryDetails.Remove(deliveryDetailToDelete);
            }
        }

        /// <summary>
        /// Eliminates the order locations that don't have left delivery details, 
        /// from the general list and the order list.
        /// </summary>
        private void RemoveOrderLocations()
        {
            for (int i = 0; i < _orderLocationsToDelete.Count; i++)
            {
                OrderLocationModel orderLocationToDelete = (OrderLocationModel)_orderLocationsToDelete[i];

                _orderLocations.Remove(orderLocationToDelete);

                OrderModel order = _orders.First(x => x.ID == orderLocationToDelete.OrderID);

                order.OrderLocations.Remove(orderLocationToDelete);
            }
        }

        /// <summary>
        /// Eliminates the orders that don't have left order locations.
        /// </summary>
        private void RemoveOrders()
        {
            for (int i = 0; i < _ordersToDelete.Count; i++)
            {
                OrderModel orderToDelete = (OrderModel)_ordersToDelete[i];
                _orders.Remove(orderToDelete);
            }
        }

        /// <summary>
        /// Adds a new order location that were result of the split of another order location.
        /// </summary>
        private void AddOrderLocations()
        {
            foreach (OrderLocationModel orderLocation in _orderLocationsToAdd)
            {
                OrderModel order = _orders.First(order => order.ID == orderLocation.OrderID);

                LinkedListNode<OrderLocationModel> node = _orderLocations.Find(order.OrderLocations.Last());

                order.OrderLocations.AddLast(orderLocation);

                _orderLocations.AddAfter(node, orderLocation);
            }
        }

        /// <summary>
        /// Clears the <c>ArrayLists</c> of the diferents object that were deleted or added on the day.
        /// </summary>
        internal void ClearArrayLists()
        {
            _ordersToDelete.Clear();
            _orderLocationsToDelete.Clear();
            _deliveryDetailsToDelete.Clear();
            _orderLocationsToAdd.Clear();
        }

        #endregion


        #region Internal Methods

        /// <summary>
        /// Calculate the area used by a determined amount of seedtrays of one type 
        /// and marks as used that area in the specified greenhouse.
        /// </summary>
        /// <param name="pAmount">The amount of seedtrays.</param>
        /// <param name="pSeedTrayType">The ID of the seedtray type.</param>
        /// <param name="pGreenHouse"> The ID of the greenhouse.</param>
        internal void ReserveArea(int pAmount, int pSeedTrayType, int pGreenHouse)
        {
            GreenHouseModel currentGreenHouse = _greenHouses.First(n => n.ID == pGreenHouse);
            SeedTrayModel currentSeedTray = _seedTrays.First(n => n.ID == pSeedTrayType);
            decimal area = (currentSeedTray.Area * pAmount);
            currentGreenHouse.SeedTrayAvailableArea -= area;
            currentGreenHouse.SeedTrayUsedArea += area;
        }

        /// <summary>
        /// Calculate the area used by a determined amount of seedtrays of one type 
        /// and marks as free that area in the specified greenhouse.
        /// </summary>
        /// <param name="pAmount">The amount of seedtrays.</param>
        /// <param name="pSeedTrayType">The ID of the seedtray type.</param>
        /// <param name="pGreenHouse"> The ID of the greenhouse.</param>
        internal void ReleaseArea(int pAmount, int pSeedTrayType, int pGreenHouse)
        {
            GreenHouseModel currentGreenHouse = _greenHouses.First(n => n.ID == pGreenHouse);
            SeedTrayModel currentSeedTray = _seedTrays.First(n => n.ID == pSeedTrayType);
            decimal area = (currentSeedTray.Area * pAmount);
            currentGreenHouse.SeedTrayAvailableArea += area;
            currentGreenHouse.SeedTrayUsedArea -= area;
        }

        /// <summary>
        /// Calculate the area used by a determined amount of seedtrays of one type 
        /// and marks as used that area in the general area.
        /// </summary>
        /// <param name="pAmount">The amount of seedtrays.</param>
        /// <param name="pSeedTrayType">The ID of the seedtray type.</param>
        internal void ReserveArea(int pAmount, int pSeedTrayType)
        {
            SeedTrayModel currentSeedTray = _seedTrays.First(n => n.ID == pSeedTrayType);
            decimal area = (currentSeedTray.Area * pAmount);
            _generalAvailableArea -= area;
            _generalUsedArea += area;
        }

        /// <summary>
        /// Calculate the area used by a determined amount of seedtrays of one type 
        /// and marks as free that area in the general area.
        /// </summary>
        /// <param name="pAmount">The amount of seedtrays.</param>
        /// <param name="pSeedTrayType">The ID of the seedtray type.</param>
        internal void ReleaseArea(int pAmount, int pSeedTrayType)
        {
            SeedTrayModel currentSeedTray = _seedTrays.First(n => n.ID == pSeedTrayType);
            decimal area = (currentSeedTray.Area * pAmount);
            _generalAvailableArea += area;
            _generalUsedArea -= area;
        }

        /// <summary>
        /// Marks as used the given amount of seedtrays of the especified seedtray type stack.
        /// </summary>
        /// <param name="pAmount">The amount of seedtrays.</param>
        /// <param name="pSeedTrayType">The ID of the seedtray type.</param>
        internal void ReserveSeedTray(int pAmount, int pSeedTrayType)
        {
            SeedTrayModel currentSeedTray = _seedTrays.First(n => n.ID == pSeedTrayType);
            currentSeedTray.FreeAmount -= pAmount;
            currentSeedTray.UsedAmount += pAmount;
        }

        /// <summary>
        /// Marks as free the given amount of seedtrays of the especified seedtray type stack.
        /// </summary>
        /// <param name="pAmount">The amount of seedtrays.</param>
        /// <param name="pSeedTrayType">The ID of the seedtray type.</param>
        internal void ReleaseSeedTray(int pAmount, int pSeedTrayType)
        {
            SeedTrayModel currentSeedTray = _seedTrays.First(n => n.ID == pSeedTrayType);
            currentSeedTray.FreeAmount += pAmount;
            currentSeedTray.UsedAmount -= pAmount;
        }

        /// <summary>
        /// Adds or removes objects at the end of one day.
        /// </summary>
        /// <remarks>Those objects can be <c>DeliveyDetailModel</c>, <c>OrderLocationModel</c>
        /// or <c>OrderModel</c>.</remarks>
        internal void UpdateObjects()
        {
            RemoveDeliveryDetails();
            RemoveOrderLocations();
            RemoveOrders();
            AddOrderLocations();
        }

        /// <summary>
        /// Calculates the total available area in the seedbed.
        /// </summary>
        /// <returns>Returns a decimal value that represents the total available area in the seedbed.</returns>
        public decimal CalculateTotalAvailableArea()
        {
            decimal freeAvailableArea = _greenHouses
                .Where(greenHouse => greenHouse.Active == true)
                .Select(greenHouse => greenHouse.SeedTrayAvailableArea)
                .Sum();

            return freeAvailableArea;
        }

        /// <summary>
        /// Checks if any of the seedtray type stacks doesn't have negattive values.
        /// </summary>
        /// <returns>Returns true if there aren't neggative values, otherwise false.</returns>
        internal bool ThereAreNonNegattiveValuesOfSeedTray()
        {
            bool output = true;

            foreach (SeedTrayModel seedTray in _seedTrays)
            {
                if (seedTray.FreeAmount < 0)
                {
                    output = false;
                }
            }

            return output;
        }

        /// <summary>
        /// Checks if the total available area isn't negative.
        /// </summary>
        /// <returns>Returns true if the total available area isn't negative otherwise false.</returns>
        internal bool ThereAreNonNegattiveValuesOfArea()
        {
            bool output = _generalAvailableArea > 0 ? true : false;
            return output;
        }

        #endregion


        #region Properties

        /// <value>
        /// Gets or sets the date on which the algorithm is iterating right now.
        /// </value>
        public DateOnly IteratorDate { get => _iteratorDate; internal set => _iteratorDate = value; }

        /// <value>
        /// Gets or sets the present date.
        /// </value>
        public DateOnly PresentDate { get => _presentDate; internal set => _presentDate = value; }

        /// <value>
        /// Gets or sets the list of greenhouses.
        /// </value>
        public List<GreenHouseModel> GreenHouses { get => _greenHouses; set => _greenHouses = value; }

        /// <value>
        /// Gets or sets the list of seedtrays.
        /// </value>
        public List<SeedTrayModel> SeedTrays { get => _seedTrays; set => _seedTrays = value; }

        /// <value>
        /// Gets or sets the orders of the seedbed.
        /// </value>
        public LinkedList<OrderModel> Orders { get => _orders; set => _orders = value; }

        /// <value>
        /// Gets or sets the order locations of the orders
        /// </value>
        public LinkedList<OrderLocationModel> OrderLocations { get => _orderLocations; set => _orderLocations = value; }

        /// <value>
        /// Gets or sets the delivery details of the order locations
        /// </value>
        public List<DeliveryDetailModel> DeliveryDetails { get => _deliveryDetails; set => _deliveryDetails = value; }

        /// <value>
        /// I dont know what I need this for.
        /// </value>
        //public DateTime DaysOfProduction { get => _daysOfProduction; set => _daysOfProduction = value; }

        /// <value>
        /// Gets or sets the amount of seedtrays that can be sown in one day.
        /// </value>
        internal int MaxAmountOfSeedTrayToSowPerDay => _maxAmountOfSeedTrayToSowPerDay;

        /// <value>
        /// Gets or sets the remaining amount of seedtrays that can be sown in one day.
        /// </value>
        internal int RemainingAmountOfSeedTrayToSowPerDay { get => _remainingAmountOfSeedTrayToSowPerDay; set => _remainingAmountOfSeedTrayToSowPerDay = value; }

        /// <value>
        /// Gets the minimum limit of sow of seedtray of an order.
        /// </value>
        internal int MinimumLimitOfSeedTrayToSow { get => _minimumLimitOfSeedTrayToSow; }


        /// <value>
        /// Gets or sets an array list of orders marked to delete at the end of the day.
        /// </value>
        internal ArrayList OrdersToDelete { get => _ordersToDelete; set => _ordersToDelete = value; }

        /// <value>
        /// Gets or sets an array list of order locations marked to delete at the end of the day.
        /// </value>
        internal ArrayList OrderLocationsToDelete { get => _orderLocationsToDelete; set => _orderLocationsToDelete = value; }

        /// <summary>
        /// Gets or sets an array list of delivery details marked to delete at the end of the day.
        /// </summary>
        internal ArrayList DeliveryDetailsToDelete { get => _deliveryDetailsToDelete; set => _deliveryDetailsToDelete = value; }

        /// <value>
        /// Gets or sets an array list of orders marked to delete at the end of the day.
        /// </value>
        internal ArrayList OrderLocationsToAdd { get => _orderLocationsToAdd; set => _orderLocationsToAdd = value; }


        #endregion
    }
}