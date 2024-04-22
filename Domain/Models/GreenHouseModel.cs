namespace Domain.Models
{
    /// <summary>
    /// Represents a greenhouse of the seedbed.
    /// </summary>
    public class GreenHouseModel
    {
        private readonly int _ID;
        private readonly string _name;
        private readonly decimal _seedTrayTotalArea;
        private decimal _seedTrayAvailableArea;
        private decimal _seedTrayUsedArea;
        private readonly int _amountOfBlocks;
        private readonly bool _active;

        /// <summary>
        /// Initializes a new instance of <c>GreenHouseModel</c>.
        /// </summary>
        /// <param name="pID">The single ID of the greenhouse.</param>
        /// <param name="pName">The name of the greenhouse.</param>
        /// <param name="pSeedTrayTotalArea">The physical area that is used to place the seedtrays</param>
        /// <param name="pAmountOfBlocks">The amount of blocks of the greenhouse</param>
        /// <param name="pActive">This indicates whether the greenhouse is in explotation.</param>
        public GreenHouseModel(int pID, string pName, decimal pSeedTrayTotalArea, int pAmountOfBlocks, bool pActive)
        {
            _ID = pID;
            _name = pName;
            _seedTrayTotalArea = pSeedTrayTotalArea;
            _seedTrayAvailableArea = pSeedTrayTotalArea;
            _seedTrayUsedArea = 0;
            _amountOfBlocks = pAmountOfBlocks;
            _active = pActive;
        }

        /// <summary>
        /// Initializes a new duplicate instance of <c>GreenHouseModel</c> from another instance.
        /// </summary>
        /// <param name="pGreenHouseModelOriginal">The instance of the original <c>GreenHouseModel</c>.</param>
        public GreenHouseModel(GreenHouseModel pGreenHouseModelOriginal)
        {
            this._ID = pGreenHouseModelOriginal.ID;
            this._name = pGreenHouseModelOriginal.Name;
            this._seedTrayTotalArea = pGreenHouseModelOriginal.SeedTrayTotalArea;
            this._seedTrayAvailableArea = pGreenHouseModelOriginal.SeedTrayAvailableArea;
            this._seedTrayUsedArea = pGreenHouseModelOriginal.SeedTrayUsedArea;
            this._amountOfBlocks = pGreenHouseModelOriginal.AmountOfBlocks;
            this._active = pGreenHouseModelOriginal.Active;
        }

        /// <Value>
        /// Gets the ID of the greenhouse.
        /// </Value>
        public int ID => _ID;

        /// <Value>
        /// Gets the name of the greenhouse.
        /// </Value>
        public string Name => _name;

        /// <Value>
        /// Gets or sets the physical area destined to place seedtrays.
        /// </Value>
        public decimal SeedTrayTotalArea => _seedTrayTotalArea;

        /// <Value>
        /// Gets or sets the physical area available to place seedtrays.
        /// </Value>
        public decimal SeedTrayAvailableArea { get => _seedTrayAvailableArea; set => _seedTrayAvailableArea = value; }

        /// <Value>
        /// Gets or sets the physical area in current use by seedtrays.
        /// </Value>
        public decimal SeedTrayUsedArea { get => _seedTrayUsedArea; set => _seedTrayUsedArea = value; }

        /// <Value>
        /// Gets or sets the amount of blocks of the greenhouse
        /// </Value>
        public int AmountOfBlocks => _amountOfBlocks;

        /// <Value>
        /// Gets a bool value that indicates whether the greenhouse is in explotation.
        /// </Value>
        public bool Active => _active;
    }
}