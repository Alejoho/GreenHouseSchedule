using DataAccess.Contracts;
using DataAccess.Repositories;
using Domain.Models;
using Domain.Processors;
using SupportLayer;
using SupportLayer.Models;
using System.Collections;
using System.Configuration;

namespace Domain
{
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
        private readonly int _amountOfSowSeedTrayPerDay;
        private int _remainingAmountOfSowSeedTrayPerDay;

        private IGreenHouseRepository _greenHouseRepository;
        private ISeedTrayRepository _seedTrayRepository;
        private OrderRepository _orderRepository;
        private OrderLocationRepository _orderLocationRepository;
        private DeliveryDetailRepository _deliveryDetailRepository;

        private IOrderProcessor _orderProcessor;
        private IOrderLocationProcessor _orderLocationProcessor;
        private DeliveryDetailProcessor _deliveryDetailProcessor;

        private ArrayList _ordersToDelete;
        private ArrayList _orderLocationsToDelete;
        private ArrayList _orderLocationsToAdd;


        #endregion
        public SeedBedStatus(string a)
        {
            
        }
        public SeedBedStatus(IGreenHouseRepository greenHouseRepo = null, 
            ISeedTrayRepository seedTrayRepo = null,
            IOrderProcessor orderProcessor = null,
            IOrderLocationProcessor orderLocationProcessor = null)
        {
            _greenHouseRepository = greenHouseRepo;
            _seedTrayRepository = seedTrayRepo;
            _orderProcessor = orderProcessor;
            _orderLocationProcessor = orderLocationProcessor;
        }

        #region Constructors
        /// <summary>
        /// Initialize a new instance of <c>SeedBedStatus</c>
        /// </summary>
        public SeedBedStatus()
        {
            //OrderModel minee= new OrderModel();

            int daysToMoveBack;
            int.TryParse(ConfigurationManager.AppSettings[ConfigurationNames.RegressionDays], out daysToMoveBack);
            //_iteratorDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-daysToMoveBack));

            //_presentDate = DateOnly.FromDateTime(DateTime.Now);

            _presentDate = new DateOnly(2023, 6, 10);
            _iteratorDate = _presentDate.AddDays(-daysToMoveBack);

            int seedTraysPerDay;
            int.TryParse(ConfigurationManager.AppSettings[ConfigurationNames.DailySowingPotential], out seedTraysPerDay);
            _amountOfSowSeedTrayPerDay = seedTraysPerDay;

            _greenHouseRepository = new GreenHouseRepository();
            _seedTrayRepository = new SeedTrayRepository();
            _orderRepository = new OrderRepository();
            _orderLocationRepository = new OrderLocationRepository();
            _deliveryDetailRepository = new DeliveryDetailRepository();

            _orderProcessor = new OrderProcessor();
            _orderLocationProcessor = new OrderLocationProcessor();
            _deliveryDetailProcessor = new DeliveryDetailProcessor();

            //NEXT - I think these are useless intanciations
            _orderLocations = new LinkedList<OrderLocationModel>();
            _deliveryDetails = new List<DeliveryDetailModel>();


            _ordersToDelete = new ArrayList();
            _orderLocationsToDelete = new ArrayList();
            _orderLocationsToAdd = new ArrayList();


            _greenHouses = GetGreenHouses();
            _seedTrays = GetSeedTrays();
            _orders = GetMajorityDataOfOrders();
            //LATER - this method give me the to orderlocations of 3 months back from the 
            //present day. This include some order location that aren't in the orders
            //retrieve by the previous method. See if can I do somethig about it
            _orderLocations = GetOrderLocations();
            //LATER - this is the same situation than above.
            _deliveryDetails = GetDeliveryDetails();

            //NEXT - Create the tests for the sub methods related to this method.
            FillOrderLocations();

            DayByDayToCurrentDate();

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
            //this._daysOfProduction = pOriginalSeedBedStatus.DaysOfProduction;
            this._amountOfSowSeedTrayPerDay = pOriginalSeedBedStatus.AmountOfSowSeedTrayPerDay;
            this._remainingAmountOfSowSeedTrayPerDay = pOriginalSeedBedStatus.RemainingAmountOfSowSeedTrayPerDay;
            this._greenHouseRepository = new GreenHouseRepository();
            this._seedTrayRepository = new SeedTrayRepository();
            this._orderRepository = new OrderRepository();
            this._orderLocationRepository = new OrderLocationRepository();
            this._deliveryDetailRepository = new DeliveryDetailRepository();
            this._ordersToDelete = new ArrayList();
            this._orderLocationsToDelete = new ArrayList();
            this._orderLocationsToAdd = new ArrayList();

            foreach (GreenHouseModel greenHouseModel in pOriginalSeedBedStatus.GreenHouses)
            {
                //NEXT - Use the copy constructor in the lists inside each foreach
                this._greenHouses.Add(greenHouseModel);
            }

            foreach (SeedTrayModel seedTrayModel in pOriginalSeedBedStatus.SeedTrays)
            {
                this._seedTrays.Add(seedTrayModel);
            }

            foreach (OrderModel orderModel in pOriginalSeedBedStatus.Orders)
            {
                this._orders.AddLast(orderModel);
            }

            foreach (OrderLocationModel orderLocationModel in pOriginalSeedBedStatus.OrderLocations)
            {
                this._orderLocations.AddLast(orderLocationModel);
            }

            foreach (DeliveryDetailModel deliveryDetailModel in pOriginalSeedBedStatus.DeliveryDetails)
            {
                this._deliveryDetails.Add(deliveryDetailModel);
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
        /// Gets the order locations with their delivery details.
        /// </summary>
        private void FillOrderLocations()
        {
            LinkedList<OrderLocationModel> orderLocationModelLinkedList = GetOrderLocations();
            FillDeliveryDetails(orderLocationModelLinkedList);
            foreach (var order in _orders)
            {
                order.OrderLocations = new LinkedList<OrderLocationModel>(
                    (from orderLocationElement in orderLocationModelLinkedList
                     where orderLocationElement.OrderID == order.ID
                     orderby orderLocationElement.ID
                     select orderLocationElement));
                //order.OrderLocations = (LinkedList<OrderLocationModel>)
                //                        orderLocationModelLinkedList.
                //                        Where(orderLocationElement => orderLocationElement.OrderID == order.ID);
            }
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
        /// Fills the DeliveryDetails property of each of the order locations passed in the LinkedList.
        /// </summary>
        /// <param name="pOrderLocationModelLinkedList">LinkedList<OrderLocationModel> to fill with their delivery details.</param>
        private void FillDeliveryDetails(LinkedList<OrderLocationModel> pOrderLocationModelLinkedList)
        {
            List<DeliveryDetailModel> deliveryDetailList = GetDeliveryDetails();
            foreach (var orderLocation in pOrderLocationModelLinkedList)
            {
                orderLocation.DeliveryDetails =
                    (from deliveryDetailElement in deliveryDetailList
                     where deliveryDetailElement.OrderLocationID == orderLocation.ID
                     orderby deliveryDetailElement.DeliveryDate
                     select deliveryDetailElement).ToList();
                //orderLocation.DeliveryDetails = (List<DeliveryDetailModel>)
                //                                deliveryDetailList.
                //                                Where(n => n.OrderLocationID == orderLocation.ID);
            }
        }

        /// <summary>
        /// Gets all the delivery details.
        /// </summary>
        /// <returns>Returns a List<DeliveryDetailModel></returns>
        private List<DeliveryDetailModel> GetDeliveryDetails()
        {
            //NEXT - this is the next method to create its test
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
            } while (_iteratorDate <= _presentDate);
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
                        }
                    }
                    if (orderLocation.SeedTrayAmount == 0)
                    {
                        order.SeedlingAmount -= orderLocation.SeedlingAmount;
                        _orderLocationsToDelete.Add(orderLocation);
                    }
                }
                if (order.SeedlingAmount == 0)
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
                        _orderLocationsToDelete.Add(orderLocation);
                    }
                }
                if (order.SeedlingAmount == 0)
                {
                    _ordersToDelete.Add(order);
                }
            }
        }

        /// <summary>
        /// Eliminates the delivery details delivered on the day.
        /// </summary>
        private void RemoveDeliveryDetails()
        {
            DeliveryDetails.RemoveAll(deliveryDetail => deliveryDetail.DeliveryDate == _iteratorDate);
        }

        /// <summary>
        /// Eliminates the order locations that don't have left delivery details.
        /// </summary>
        private void RemoveOrderLocations()
        {
            for (int i = 0; i < _orderLocationsToDelete.Count; i++)
            {
                _orderLocations.Remove((OrderLocationModel)_orderLocationsToDelete[i]);
            }
        }

        /// <summary>
        /// Eliminates the orders that don't have left order locations.
        /// </summary>
        private void RemoveOrders()
        {
            for (int i = 0; i < _ordersToDelete.Count; i++)
            {
                _orders.Remove((OrderModel)_ordersToDelete[i]);
            }
        }

        /// <summary>
        /// Adds a new order location that were result of the split of another order location.
        /// </summary>
        private void AddOrderLocations()
        {
            //CHECK - here there is something wrong
            foreach (OrderLocationModel orderLocation in _orderLocationsToAdd)
            {
                //Extracts the order of the current orderlocation
                OrderModel order = _orders.First(order => order.ID == orderLocation.OrderID);
                //extracts the last OrderLocationModel of the linkedlist of the order
                LinkedListNode<OrderLocationModel> node = order.OrderLocations.Last;
                //Adds the current orderlocation to the linked list of the order
                order.OrderLocations.AddLast(orderLocation);

                //NEXT - Look up how to find an specific node in a linked list
                //LinkedListNode<OrderLocationModel> node = _orderLocations
                //    .Find(orderLocation => orderLocation.ID == node.Value.ID);

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
            _orderLocationsToAdd.Clear();
        }

        private int Test()
        {
            return 1;
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
            //decimal freeAvailableArea=_greenHouses.Sum(greenHouse=>greenHouse.SeedTrayAvailableArea);
            decimal freeAvailableArea = _greenHouses
                .Where(greenHouse => greenHouse.Active == true)
                .Select(greenHouse => greenHouse.SeedTrayAvailableArea)
                .Sum();
            //decimal freeAvailableArea = (from greenHouse in _greenHouses
            //                          select greenHouse.SeedTrayAvailableArea).Sum();
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
        internal bool ThereIsNonNegattiveValuesOfArea()
        {
            bool output = CalculateTotalAvailableArea() > 0 ? true : false;
            return output;
        }

        //internal void InsertNewOrder(OrderModel pNewOrderModel)
        //{

        //}


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
        /// I dont know what i need this for.
        /// </value>
        //public DateTime DaysOfProduction { get => _daysOfProduction; set => _daysOfProduction = value; }

        /// <value>
        /// Gets or sets the amount of seedtrays that can be sown in one day.
        /// </value>
        internal int AmountOfSowSeedTrayPerDay => _amountOfSowSeedTrayPerDay;

        /// <value>
        /// Gets or sets the remaining amount of seedtrays that can be sown in one day.
        /// </value>
        internal int RemainingAmountOfSowSeedTrayPerDay { get => _remainingAmountOfSowSeedTrayPerDay; set => _remainingAmountOfSowSeedTrayPerDay = value; }

        /// <value>
        /// Gets or sets an array list of orders marked to delete at the end of the day.
        /// </value>
        internal ArrayList OrdersToDelete { get => _ordersToDelete; set => _ordersToDelete = value; }

        /// <value>
        /// Gets or sets an array list of order locations marked to delete at the end of the day.
        /// </value>
        internal ArrayList OrderLocationsToDelete { get => _orderLocationsToDelete; set => _orderLocationsToDelete = value; }

        /// <value>
        /// Gets or sets an array list of orders marked to delete at the end of the day.
        /// </value>
        internal ArrayList OrderLocationsToAdd { get => _orderLocationsToAdd; set => _orderLocationsToAdd = value; }

        #endregion
    }
}