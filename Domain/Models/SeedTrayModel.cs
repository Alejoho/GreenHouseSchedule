namespace Domain.Models
{
    /// <summary>
    /// Represents a type of seedtray with its characteristics.
    /// </summary>
    public class SeedTrayModel
    {
        private readonly int _ID;
        private readonly string _name;
        private readonly decimal _area;
        private readonly int _alveolusQuantity;
        private readonly int _totalAmount;
        private int _freeAmount;
        private int _usedAmount;
        private bool _active;
        /// <summary>
        /// Initializes a new instance of <c>SeedTrayModel</c>.
        /// </summary>
        /// <param name="pID">The single ID of the seedtray.</param>
        /// <param name="pName">The name of the seedtray.</param>
        /// <param name="pArea">The logic area that the seedtray takes up in the seedbed.</param>
        /// <param name="pAlveolusQuantity"> The total of alveolus of the seedtray.</param>
        /// <param name="pTotalAmount">The total amount of seedtrays of this type available for explotation.</param>
        /// <param name="pActive">This indicates whether the seedtray type is in explotation.</param>
        public SeedTrayModel(int pID, string pName, decimal pArea, int pAlveolusQuantity, int pTotalAmount, bool pActive)
        {
            _ID = pID;
            _name = pName;
            _area = pArea;
            _alveolusQuantity = pAlveolusQuantity;
            _totalAmount = pTotalAmount;
            _freeAmount = pTotalAmount;
            _usedAmount = 0;
            _active = pActive;
        }
        /// <summary>
        /// Initializes a new duplicate instance of <c>SeedTrayModel</c> from another instance.
        /// </summary>
        /// <param name="pSeedTrayModelOriginal">The instance of the original <c>SeedTrayModel</c>.</param>
        public SeedTrayModel(SeedTrayModel pSeedTrayModelOriginal)
        {
            this._ID = pSeedTrayModelOriginal.ID;
            this._name = pSeedTrayModelOriginal.Name;
            this._area = pSeedTrayModelOriginal.Area;
            this._alveolusQuantity = pSeedTrayModelOriginal.AlveolusQuantity;
            this._totalAmount = pSeedTrayModelOriginal.TotalAmount;
            this._freeAmount = pSeedTrayModelOriginal.TotalAmount;
            this._usedAmount = pSeedTrayModelOriginal.UsedAmount;
            this._active = pSeedTrayModelOriginal.Active;
        }
        
        /// <value>
        /// Gets the ID of the seedtray.
        /// </value>
        public int ID => _ID;

        /// <value>
        /// Gets the name of the seedtray type.
        /// </value>
        public string Name => _name;

        /// <value>
        /// Gets the logical area occupied by this seedtray in the seedbed.
        /// </value>
        public decimal Area => _area;

        /// <value>
        /// Gets the total quantity of alveolus.
        /// </value>
        public int AlveolusQuantity => _alveolusQuantity;

        /// <value>
        /// Gets the total amount in explotation of seedtrays.
        /// </value>
        public int TotalAmount => _totalAmount;

        /// <value>
        /// Gets or sets the amount available of seedtrays.
        /// </value>
        public int FreeAmount { get => _freeAmount; set => _freeAmount = value; }

        /// <value>
        /// Gets or sets the amount used of seedtrays.
        /// </value>
        public int UsedAmount { get => _usedAmount; set => _usedAmount = value; }

        /// <value>
        /// Gets a bool value that indicates whether the seedtray is in explotation.
        /// </value>
        public bool Active => _active;
    }
}