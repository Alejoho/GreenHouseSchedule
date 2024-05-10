using Domain.Models;
using Domain.ValuableObjects;
using System.Collections;

namespace Domain
{
    /// <summary>
    /// This class executes the iteration through the calendar from the present date to the future
    /// and checks the disponibility of resources, with the purpose of placing an order.
    /// </summary>
    public class DateIteratorAndResourceChecker
    {

        #region Fields

        /// <summary>
        /// Represents the main seedbed
        /// </summary>
        private SeedBedStatus _seedBedStatus;

        /// <summary>
        /// This is an auxiliar seedbed to realize some confirmation without edit the main seedbed.
        /// </summary>
        private SeedBedStatus _auxiliarSeedBedStatus;

        /// <summary>
        /// The order that its required to place in the seedbed.
        /// </summary>
        private OrderModel _orderInProcess;

        /// <summary>
        /// A linked list with the diferent permutations of the available amounts of seedtrays.
        /// </summary>
        private LinkedList<SeedTrayPermutation> _seedTrayPermutations;

        /// <summary>
        /// An array list with the seedtray permutations that will be deleted becuase didn't fullfil the conditions.
        /// </summary>
        private ArrayList _seedTrayPermutationsToDelete;

        #endregion


        #region Constructors

        /// <summary>
        /// This is a constructor for unit test case scenario.
        /// </summary>
        /// <param name="seedBedStatus">The seedbed loaded until the present day.</param>
        /// <param name="pOrderInProcess">The order that to place in the seedbed.</param>
        /// <param name="testing">Some variable to diferentiate this ctor.</param>
        public DateIteratorAndResourceChecker(SeedBedStatus seedBedStatus, OrderModel orderInProcess = null, bool testing = true)
        {
            _seedBedStatus = new SeedBedStatus(seedBedStatus);
            _auxiliarSeedBedStatus = new SeedBedStatus(_seedBedStatus);

            if (orderInProcess != null)
            {
                _orderInProcess = orderInProcess;
            }

            _seedTrayPermutations = new LinkedList<SeedTrayPermutation>();
            _seedTrayPermutationsToDelete = new ArrayList();
        }

        /// <summary>
        /// Initializes a new instance of <c>DateIteratorAndResourceChecker</c>
        /// </summary>
        /// <param name="seedBedStatus">The seedbed loaded until the present day.</param>
        /// <param name="pOrderInProcess">The order that to place in the seedbed.</param>
        public DateIteratorAndResourceChecker(SeedBedStatus seedBedStatus, OrderModel pOrderInProcess)
        {
            _seedBedStatus = seedBedStatus;
            _orderInProcess = pOrderInProcess;
            _seedTrayPermutations = new LinkedList<SeedTrayPermutation>();
            _seedTrayPermutationsToDelete = new ArrayList();
        }

        #endregion


        #region Methods to avance in the time

        /// <summary>
        /// Advances the state of the seedbed until the request date of a new order.
        /// </summary>
        private void DayByDayToRequestDate()
        {
            //CHECK - al crear la instancia de esta clase el status se mueve directo al dia de la orden que se desea agregar
            //y realiza el trabajo de ese dia sin agregar la orden. Pero al pasar al metodo LookForAvailability se va 
            //a trabajar de nuevo el dia requerido por la orden, esto trae consigo que se rewstablezca el potencial de
            //siembra del dia, lo que es un error.

            //TODO - Maybe change this loop to a while.
            do
            {
                DoTheWorkOfThisDay();

                CheckForNegattive();

                SeedBedStatus.IteratorDate = SeedBedStatus.IteratorDate.AddDays(1);

            } while (SeedBedStatus.IteratorDate <= _orderInProcess.EstimateSowDate);

            SeedBedStatus.IteratorDate = SeedBedStatus.IteratorDate.AddDays(-1);
        }

        private struct NegDate
        {
            internal decimal value;
            internal DateOnly date;

            public NegDate()
            {
                value = 10000;
            }
        }

        private NegDate[] negattiveGreenHouse = { new NegDate()
                , new NegDate(), new NegDate(), new NegDate()
                , new NegDate(), new NegDate(), new NegDate()
                , new NegDate() };
        private NegDate[] negattiveSeedTrays = { new NegDate()
                , new NegDate(), new NegDate(), new NegDate()
                , new NegDate(), new NegDate(), new NegDate() };

        private void CheckForNegattive()
        {
            SeedBedStatus.GreenHouses.ForEach(greenHouse =>
            {
                if (greenHouse.SeedTrayAvailableArea < negattiveGreenHouse[greenHouse.ID - 1].value)
                {
                    negattiveGreenHouse[greenHouse.ID - 1].value = greenHouse.SeedTrayAvailableArea;
                    negattiveGreenHouse[greenHouse.ID - 1].date = SeedBedStatus.IteratorDate;
                }
            });

            SeedBedStatus.SeedTrays.ForEach(seedTray =>
            {
                if (seedTray.FreeAmount < negattiveSeedTrays[seedTray.ID - 1].value)
                {
                    negattiveSeedTrays[seedTray.ID - 1].value = seedTray.FreeAmount;
                    negattiveSeedTrays[seedTray.ID - 1].date = SeedBedStatus.IteratorDate;
                }
            });
        }

        /// <summary>
        /// Do all tasks that should be done on one day.
        /// </summary>
        private void DoTheWorkOfThisDay()
        {
            RestartPotentialOfSowSeedTrayPerDay();
            ImplementEstimateRelease();
            ImplementEstimateReservation();
            SeedBedStatus.UpdateObjects();
            SeedBedStatus.ClearArrayLists();
        }

        /// <summary>
        /// Marks as free, the area and the seedtrays used by the order locations that will be delivered on the iterator date.
        /// </summary>
        private void ImplementEstimateRelease()
        {
            int outer = 0;
            foreach (OrderModel order in SeedBedStatus.Orders)
            {
                foreach (OrderLocationModel orderLocation in order.OrderLocations)
                {
                    if (orderLocation.EstimateDeliveryDate == SeedBedStatus.IteratorDate)
                    {
                        outer += orderLocation.SeedTrayAmount;
                        SeedBedStatus.ReleaseSeedTray(orderLocation.SeedTrayAmount, orderLocation.SeedTrayType);
                        SeedBedStatus.ReleaseArea(orderLocation.SeedTrayAmount, orderLocation.SeedTrayType, orderLocation.GreenHouse);
                        order.SeedlingAmount -= orderLocation.SeedlingAmount;
                        SeedBedStatus.OrderLocationsToDelete.Add(orderLocation);
                    }
                }
                if (order.SeedlingAmount == 0)
                {
                    SeedBedStatus.OrdersToDelete.Add(order);
                }
            }
        }

        //TODO - Maybe implement a restriction to avoid sow seedtrays on sundays.
        /// <summary>
        /// Marks as used, the area and the seedtrays used by the order locations that will be sown on the iterator date.
        /// </summary>
        private void ImplementEstimateReservation()
        {
            var ordersToSow = SeedBedStatus.Orders
                .Where(order => order.EstimateSowDate <= SeedBedStatus.IteratorDate && order.Complete == false)
                .OrderBy(x => x.EstimateSowDate)
                .ThenBy(x => x.RequestDate);

            foreach (OrderModel order in ordersToSow)
            {
                if (SeedBedStatus.RemainingAmountOfSeedTrayToSowPerDay > 0)
                {
                    foreach (OrderLocationModel orderLocation in order.OrderLocations)
                    {
                        if (orderLocation.Sown == false && SeedBedStatus.RemainingAmountOfSeedTrayToSowPerDay > 0)
                        {
                            if ((SeedBedStatus.RemainingAmountOfSeedTrayToSowPerDay - orderLocation.SeedTrayAmount) >= 0)
                            {
                                orderLocation.SowDate = SeedBedStatus.IteratorDate;
                                orderLocation.EstimateDeliveryDate = SeedBedStatus.IteratorDate.AddDays(order.Product.ProductionInterval);
                                SeedBedStatus.ReserveSeedTray(orderLocation.SeedTrayAmount, orderLocation.SeedTrayType);
                                SeedBedStatus.ReserveArea(orderLocation.SeedTrayAmount, orderLocation.SeedTrayType, orderLocation.GreenHouse);
                                SeedBedStatus.RemainingAmountOfSeedTrayToSowPerDay -= orderLocation.SeedTrayAmount;
                                orderLocation.Sown = true;
                            }
                            else if (SeedBedStatus.RemainingAmountOfSeedTrayToSowPerDay < orderLocation.SeedTrayAmount
                                && SeedBedStatus.RemainingAmountOfSeedTrayToSowPerDay > SeedBedStatus.MinimumLimitOfSeedTrayToSow)
                            {
                                int newID = SeedBedStatus.OrderLocations.Max(x => x.ID) + 1;

                                OrderLocationModel newOrderLocation = new OrderLocationModel(orderLocation, newID);

                                newOrderLocation.SowDate = SeedBedStatus.IteratorDate;
                                newOrderLocation.EstimateDeliveryDate = SeedBedStatus.IteratorDate.AddDays(order.Product.ProductionInterval);
                                newOrderLocation.SeedTrayAmount = SeedBedStatus.RemainingAmountOfSeedTrayToSowPerDay;

                                int alveolus = _seedBedStatus.SeedTrays
                                    .First(x => x.ID == orderLocation.SeedTrayType)
                                    .AlveolusQuantity;

                                newOrderLocation.SeedlingAmount = newOrderLocation.SeedTrayAmount * alveolus;

                                SeedBedStatus.ReserveSeedTray(newOrderLocation.SeedTrayAmount, newOrderLocation.SeedTrayType);
                                SeedBedStatus.ReserveArea(newOrderLocation.SeedTrayAmount, newOrderLocation.SeedTrayType, newOrderLocation.GreenHouse);

                                newOrderLocation.Sown = true;

                                SeedBedStatus.OrderLocationsToAdd.Add(newOrderLocation);

                                orderLocation.SeedTrayAmount -= newOrderLocation.SeedTrayAmount;
                                orderLocation.SeedlingAmount = orderLocation.SeedTrayAmount * alveolus;

                                SeedBedStatus.RemainingAmountOfSeedTrayToSowPerDay = 0;
                            }
                        }
                    }

                    if (order.OrderLocations.Where(x => x.Sown == false).Count() == 0)
                    {
                        order.Complete = true;
                    }
                }
                else
                {
                    break;
                }
            }
        }

        /// <summary>
        /// Restarts the remaining amount of sow seed tray per day to its initial value.
        /// </summary>
        private void RestartPotentialOfSowSeedTrayPerDay()
        {
            SeedBedStatus.RemainingAmountOfSeedTrayToSowPerDay = SeedBedStatus.MaxAmountOfSeedTrayToSowPerDay;
        }

        //public void AssignOrderInProcess(OrderModel pOrder)
        //{
        //    OrderInProcess = pOrder;
        //}

        #endregion


        #region Methods to Look For Availability

        /// <summary>
        /// Evaluates day by day from the request day whether is there resources to place the new order and if that order doesn't 
        /// displace the following orders.
        /// </summary>
        public void LookForAvailability()
        {
            //CHECK - al crear la instancia de esta clase el status se mueve directo al dia de la orden que se desea agregar
            //y realiza el trabajo de ese dia sin agregar la orden. Pero al pasar al metodo LookForAvailability se va 
            //a trabajar de nuevo el dia requerido por la orden, esto trae consigo que se rewstablezca el potencial de
            //siembra del dia, lo que es un error.

            GreenHouseModel tempGreenHouse = new GreenHouseModel(-1, "TempGreenHouse", 0, 0, true);
            SeedBedStatus.GreenHouses.Add(tempGreenHouse);

            do
            {
                DayByDayToRequestDate();
                _orderInProcess.RequestDate.AddDays(1);
            } while (AreThereResources() == false && DoesItDisplaceFollowingOrders() == true);
            _orderInProcess.RequestDate.AddDays(-1);

            SeedBedStatus.GreenHouses.Remove(tempGreenHouse);
        }

        /// <summary>
        /// Evaluates whether are there work force, area and seedtrays on the current day.
        /// </summary>
        /// <returns>Returns true if there are work force and available resources otherwise false.</returns>
        private bool AreThereResources()
        {
            return WorkForceResource() && SeedTrayAndAreaResources();
        }

        /// <summary>
        /// Evaluates whether is there work force.
        /// </summary>
        /// <returns>Returns true if there is work force otherwise false.</returns>
        private bool WorkForceResource()
        {
            //LATER - Maybe instead of comparing with 0 its better to compare with MinimumLimitOfSeedTrayToSow
            bool IsThereWorkForceOnThisDay = SeedBedStatus.RemainingAmountOfSeedTrayToSowPerDay > 0 ? true : false;
            return IsThereWorkForceOnThisDay;
        }

        /// <summary>
        /// Evaluates whether are there seedtrays and area available.
        /// </summary>
        /// <returns>Returns true if there are seedtrays and area otherwise false.</returns>
        private bool SeedTrayAndAreaResources()
        {
            bool output;
            _seedTrayPermutations.Clear();
            GenerateAndAddSimplePermutations();
            GenerateAndAddDoublePermutations();
            GenerateAndAddTriplePermutations();

            output = _seedTrayPermutations.Count > 0 ? true : false;

            return output;
        }

        /// <summary>
        /// Generates a simple permutation of seedtrays, evaluates whether are there resources for that permutation
        /// and adds it to the <c>SeedTrayPermutations</c> LinkedList. 
        /// </summary>
        private void GenerateAndAddSimplePermutations()
        {
            foreach (SeedTrayModel seedTrayModelLevel1 in SeedBedStatus.SeedTrays)
            {
                int estimateAmountOfSeedTrayLevel1 =
                    (int)Math.Ceiling(
                        (double)_orderInProcess.SeedlingAmount /
                        (double)seedTrayModelLevel1.AlveolusQuantity
                        );
                SeedTrayPermutation newSeedTrayPermutation =
                    new SeedTrayPermutation(seedTrayModelLevel1.ID, estimateAmountOfSeedTrayLevel1);

                if (AreThereFreeSeedTraysOfTheTypesInUse(newSeedTrayPermutation) == true &&
                    IsThereAreaForTheSeedTraysInUse(newSeedTrayPermutation) == true)
                {
                    _seedTrayPermutations.AddLast(newSeedTrayPermutation);
                }
            }
        }

        /// <summary>
        /// Generates a double permutation of seedtrays and adds that permutation to the 
        /// <c>SeedTrayPermutations</c> LinkedList.
        /// </summary>
        private void GenerateAndAddDoublePermutations()
        {
            foreach (SeedTrayModel seedTrayModelLevel1 in SeedBedStatus.SeedTrays)
            {
                int estimateAmountOfSeedlingLevel1 =
                    seedTrayModelLevel1.FreeAmount * seedTrayModelLevel1.AlveolusQuantity;

                if (_orderInProcess.SeedlingAmount > estimateAmountOfSeedlingLevel1)
                {
                    var listOfSeedTraysLevel2 = SeedBedStatus.SeedTrays
                        .Where(seedTray => seedTray.ID != seedTrayModelLevel1.ID);

                    foreach (SeedTrayModel seedTrayModelLevel2 in listOfSeedTraysLevel2)
                    {
                        int estimateAmountOfSeedTrayLevel2 = (int)Math.Ceiling(
                            (double)(_orderInProcess.SeedlingAmount - estimateAmountOfSeedlingLevel1) /
                            (double)seedTrayModelLevel2.AlveolusQuantity);

                        SeedTrayPermutation newSeedTrayPermutation = new SeedTrayPermutation(
                            seedTrayModelLevel1.ID, seedTrayModelLevel1.FreeAmount,
                            seedTrayModelLevel2.ID, estimateAmountOfSeedTrayLevel2);

                        if (AreThereFreeSeedTraysOfTheTypesInUse(newSeedTrayPermutation) == true &&
                            IsThereAreaForTheSeedTraysInUse(newSeedTrayPermutation) == true)
                        {
                            _seedTrayPermutations.AddLast(newSeedTrayPermutation);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Generates a triple permutation of seedtrays and adds that permutation to the 
        /// <c>SeedTrayPermutations</c> LinkedList.
        /// </summary>
        private void GenerateAndAddTriplePermutations()
        {
            foreach (SeedTrayModel seedTrayModelLevel1 in SeedBedStatus.SeedTrays)
            {
                int estimateAmountOfSeedlingLevel1 = seedTrayModelLevel1.FreeAmount * seedTrayModelLevel1.AlveolusQuantity;

                if (_orderInProcess.SeedlingAmount > estimateAmountOfSeedlingLevel1)
                {
                    var listOfSeedTraysLevel2 = SeedBedStatus.SeedTrays
                        .Where(seedTray => seedTray.ID != seedTrayModelLevel1.ID);

                    foreach (SeedTrayModel seedTrayModelLevel2 in listOfSeedTraysLevel2)
                    {
                        int estimateAmountOfSeedlingLevel2 = seedTrayModelLevel2.FreeAmount * seedTrayModelLevel2.AlveolusQuantity;

                        if (_orderInProcess.SeedlingAmount > (estimateAmountOfSeedlingLevel1 + estimateAmountOfSeedlingLevel2))
                        {
                            var listOfSeedTraysLevel3 = listOfSeedTraysLevel2
                                .Where(seedTray => seedTray.ID != seedTrayModelLevel2.ID);

                            foreach (SeedTrayModel seedTrayModelLevel3 in listOfSeedTraysLevel3)
                            {
                                int estimateAmountOfSeedTrayLevel3 = (int)Math.Ceiling(
                                    (double)(_orderInProcess.SeedlingAmount - (estimateAmountOfSeedlingLevel1 + estimateAmountOfSeedlingLevel2)) /
                                    (double)seedTrayModelLevel2.AlveolusQuantity);

                                SeedTrayPermutation newSeedTrayPermutation = new SeedTrayPermutation(
                                    seedTrayModelLevel1.ID, seedTrayModelLevel1.FreeAmount,
                                    seedTrayModelLevel2.ID, seedTrayModelLevel2.FreeAmount,
                                    seedTrayModelLevel3.ID, estimateAmountOfSeedTrayLevel3);

                                if (AreThereFreeSeedTraysOfTheTypesInUse(newSeedTrayPermutation) == true &&
                                    IsThereAreaForTheSeedTraysInUse(newSeedTrayPermutation) == true)
                                {
                                    _seedTrayPermutations.AddLast(newSeedTrayPermutation);
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Evaluates whether are there free seedtrays to complete the passed <c>SeedTrayPermutation</c>.
        /// </summary>
        /// <param name="pSeedTrayPermutation">The <c>SeedTrayPermutation</c> to evaluate.</param>
        /// <returns>Returns true if there are seedtrays to complete the permutation otherwise false.</returns>
        private bool AreThereFreeSeedTraysOfTheTypesInUse(SeedTrayPermutation pSeedTrayPermutation)
        {
            bool output;
            bool firstSeedTrayComprobation = true;
            bool secondSeedTrayComprobation = true;
            bool thirdSeedTrayComprobation = true;

            SeedTrayModel? seedTrayModel = SeedBedStatus.SeedTrays
                .Find(seedTray => seedTray.ID == pSeedTrayPermutation.FirstSeedTrayID);
            firstSeedTrayComprobation = seedTrayModel.FreeAmount >= pSeedTrayPermutation.FirstAmount ? true : false;

            //LATER - Refactor these two methods. If I have a permutation with only a seedtray it have to iterate 
            //over all the seedtrays to find that the secondId and the thirdId are null. Instead of that include
            //the look up of the seedtray inside of the if.
            seedTrayModel = SeedBedStatus.SeedTrays
                .FirstOrDefault(seedTray => seedTray.ID == pSeedTrayPermutation.SecondSeedTrayID, null);

            if (seedTrayModel != null)
            {
                secondSeedTrayComprobation = seedTrayModel.FreeAmount >= pSeedTrayPermutation.SecondAmount ? true : false;
            }

            seedTrayModel = SeedBedStatus.SeedTrays
                .FirstOrDefault(seedTray => seedTray.ID == pSeedTrayPermutation.ThirdSeedTrayID, null);

            if (seedTrayModel != null)
            {
                thirdSeedTrayComprobation = seedTrayModel.FreeAmount >= pSeedTrayPermutation.ThirdAmount ? true : false;
            }
            output = firstSeedTrayComprobation && secondSeedTrayComprobation && thirdSeedTrayComprobation;

            return output;
        }

        /// <summary>
        /// Evaluates whether are there free area to place the passed <c>SeedTrayPermutation</c>.
        /// </summary>
        /// <param name="pSeedTrayPermutation">The <c>SeedTrayPermutation</c> to evaluate.</param>
        /// <returns>Returns true if there are area to place the permutation otherwise false.</returns>
        private bool IsThereAreaForTheSeedTraysInUse(SeedTrayPermutation pSeedTrayPermutation)
        {
            bool output;
            decimal areaUsedByTheSeedTrayTypes;
            decimal totalAvailableArea = _seedBedStatus.CalculateTotalAvailableArea();

            SeedTrayModel? seedTrayModel = SeedBedStatus.SeedTrays
                .Find(seedTray => seedTray.ID == pSeedTrayPermutation.FirstSeedTrayID);

            areaUsedByTheSeedTrayTypes = seedTrayModel.Area * pSeedTrayPermutation.FirstAmount;

            seedTrayModel = SeedBedStatus.SeedTrays
                .FirstOrDefault(seedTray => seedTray.ID == pSeedTrayPermutation.SecondSeedTrayID, null);

            if (seedTrayModel != null)
            {
                areaUsedByTheSeedTrayTypes += seedTrayModel.Area * pSeedTrayPermutation.SecondAmount;
            }

            seedTrayModel = SeedBedStatus.SeedTrays
                .FirstOrDefault(seedTray => seedTray.ID == pSeedTrayPermutation.ThirdSeedTrayID, null);

            if (seedTrayModel != null)
            {
                areaUsedByTheSeedTrayTypes += seedTrayModel.Area * pSeedTrayPermutation.ThirdAmount;
            }

            output = totalAvailableArea > areaUsedByTheSeedTrayTypes ? true : false;

            return output;
        }

        /// <summary>
        /// Evaluates if we put the new order whether displace following orders. 
        /// </summary>
        /// <returns>Returns true if the new order displace next orders otherwise false.</returns>
        private bool DoesItDisplaceFollowingOrders()
        {
            bool output = false;

            DateOnly limitDate = _seedBedStatus.IteratorDate.AddDays(_orderInProcess.Product.ProductionInterval);

            _auxiliarSeedBedStatus = new SeedBedStatus(_seedBedStatus);

            foreach (SeedTrayPermutation seedTrayPermutation in _seedTrayPermutations)
            {
                InsertOrderInProcessIntoSeedBedStatus(seedTrayPermutation);
                _seedBedStatus.IteratorDate = _seedBedStatus.IteratorDate.AddDays(-1);
                do
                {
                    _seedBedStatus.IteratorDate = _seedBedStatus.IteratorDate.AddDays(1);
                    DoTheWorkOfThisDay();
                } while (
                //CHECK - Here I evaluate if by putting the new order some resources get negattive numbers,
                //but I don't evaluate the limitation of the amount of seed trays to sow in a day.
                //tal vez chequeando que las ordernes no tenga un defase mayor a un terminado numero entre
                //la EstimateSowDate and the RealSowDate. No se digamos de una semana. Este es un valor que se puede 
                //poner en el app.config
                _seedBedStatus.IteratorDate <= limitDate &&
                _seedBedStatus.ThereAreNonNegattiveValuesOfSeedTray() &&
                _seedBedStatus.ThereAreNonNegattiveValuesOfArea()
                );

                if (_seedBedStatus.IteratorDate < limitDate)
                {
                    _seedTrayPermutationsToDelete.Add(seedTrayPermutation);
                }
            }

            RemoveSeedTrayPermutations();
            ClearSeedTrayPermutationToDeleteArrayList();

            if (_seedTrayPermutations.Count == 0)
            {
                output = true;
            }

            return output;
        }

        /// <summary>
        /// Inserts the new order with the especified seedtray permutation into the seedbed auxiliar.
        /// </summary>
        /// <param name="pSeedTrayPermutation">The <c>SeedTrayPermutation</c> to use with the new order.</param>
        private void InsertOrderInProcessIntoSeedBedStatus(SeedTrayPermutation pSeedTrayPermutation)
        {
            OrderModel newOrder = new OrderModel(_orderInProcess);
            OrderLocationModel newOrderLocation;

            int alveolus = (from seedTray in _seedBedStatus.SeedTrays
                                  where seedTray.ID == pSeedTrayPermutation.FirstSeedTrayID
                            select seedTray.AlveolusQuantity).FirstOrDefault(0);

            int seedlingAmount = alveolus * pSeedTrayPermutation.FirstAmount;

            newOrderLocation = new OrderLocationModel(
                _seedBedStatus.OrderLocations.Max(orderLocation => orderLocation.ID) + 1,
                pSeedTrayPermutation.FirstSeedTrayID,
                _orderInProcess.ID,
                pSeedTrayPermutation.FirstAmount,
                seedlingAmount);

            newOrder.OrderLocations.AddLast(newOrderLocation);

            _seedBedStatus.OrderLocations.AddLast(newOrderLocation);

            if (pSeedTrayPermutation.SecondSeedTrayID > 0)
            {
                alveolus = (from seedTray in _seedBedStatus.SeedTrays
                                  where seedTray.ID == pSeedTrayPermutation.SecondSeedTrayID
                            select seedTray.AlveolusQuantity).FirstOrDefault(0);

                seedlingAmount = alveolus * pSeedTrayPermutation.FirstAmount;

                newOrderLocation = new OrderLocationModel(
                    _seedBedStatus.OrderLocations.Max(orderLocation => orderLocation.ID) + 1,
                    pSeedTrayPermutation.SecondSeedTrayID,
                    _orderInProcess.ID,
                    pSeedTrayPermutation.SecondAmount,
                    seedlingAmount);

                newOrder.OrderLocations.AddLast(newOrderLocation);

                _seedBedStatus.OrderLocations.AddLast(newOrderLocation);
            }

            if (pSeedTrayPermutation.ThirdSeedTrayID != 0)
            {
                alveolus = (from seedTray in _seedBedStatus.SeedTrays
                                  where seedTray.ID == pSeedTrayPermutation.ThirdSeedTrayID
                            select seedTray.AlveolusQuantity).FirstOrDefault(0);

                seedlingAmount = alveolus * pSeedTrayPermutation.FirstAmount;

                newOrderLocation = new OrderLocationModel(
                    _seedBedStatus.OrderLocations.Max(orderLocation => orderLocation.ID) + 1,
                    pSeedTrayPermutation.ThirdSeedTrayID,
                    _orderInProcess.ID,
                    pSeedTrayPermutation.ThirdAmount,
                    seedlingAmount);

                newOrder.OrderLocations.AddLast(newOrderLocation);

                _seedBedStatus.OrderLocations.AddLast(newOrderLocation);
            }
            _seedBedStatus.Orders.AddLast(newOrder);
        }

        /// <summary>
        /// Deletes all the <c>SeedTrayPermutation</c> objects that are in the <c>SeedTrayPermutationsToDelete</c> ArrayList
        /// from the <c>SeedTrayPermutations</c> LinkedList.
        /// </summary>
        private void RemoveSeedTrayPermutations()
        {
            for (int i = 0; i < _seedTrayPermutationsToDelete.Count; i++)
            {
                _seedTrayPermutations.Remove((SeedTrayPermutation)_seedTrayPermutationsToDelete[i]);
            }
        }

        /// <summary>
        /// Removes all the <c>SeedTraysPermutation</c> objects that are in the <c>SeedTrayPermutationsToDelete</c> ArrayList.
        /// </summary>
        private void ClearSeedTrayPermutationToDeleteArrayList()
        {
            _seedTrayPermutationsToDelete.Clear();
        }


        #endregion


        #region Properties

        //LATER - Evaluar si quitar estas propiedades ya que no se usan fuera de la clase

        public SeedBedStatus SeedBedStatus { get => _seedBedStatus; set => _seedBedStatus = value; }

        //public SeedBedStatus SeedBedStatusAuxiliar { get => _seedBedStatusAuxiliar; set => _seedBedStatusAuxiliar = value; }

        //public OrderModel OrderInProcess { get => _orderInProcess; set => _orderInProcess = value; }

        public LinkedList<SeedTrayPermutation> SeedTrayPermutations { get => _seedTrayPermutations; set => _seedTrayPermutations = value; }

        //public ArrayList SeedTrayPermutationsToDelete { get => _seedTrayPermutationsToDelete; set => _seedTrayPermutationsToDelete = value; }

        #endregion
    }
}
