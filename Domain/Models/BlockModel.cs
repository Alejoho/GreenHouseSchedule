namespace Domain.Models
{
    /// <summary>
    /// Represents a group of seedtrays put together under the same order location.
    /// </summary>
    public class BlockModel
    {
        private readonly int _ID;
        private readonly int _orderLocationID;
        private readonly int _blockNumberInTheGreenHouse;
        private readonly int _seedTrayAmount;
        private int _numberInBlock;
        /// <summary>
        /// Initializes a new instance of <c>BlockModel</c>.
        /// </summary>
        /// <param name="pID">The single ID of the block.</param>
        /// <param name="pOrderLocationID">The single ID of the order location to wich the block belongs.</param>
        /// <param name="pNumberInTheGreenHouse">The block number of the greenhouse in wich the block is.</param>
        /// <param name="pSeedTrayAmount">The amount of seedtrays</param>
        /// <param name="pNumberInBlock">The number of the block begins from the front of the block of the greenhouse.</param>
        public BlockModel(int pID, int pOrderLocationID, int pNumberInTheGreenHouse, int pSeedTrayAmount, int pNumberInBlock)
        {
            _ID = pID;
            _orderLocationID = pOrderLocationID;
            _blockNumberInTheGreenHouse = pNumberInTheGreenHouse;
            _seedTrayAmount = pSeedTrayAmount;
            _numberInBlock = pNumberInBlock;
        }

        /// <summary>
        /// Initializes a new duplicate instance of <c>BlockModel</c> from another instance.
        /// </summary>
        /// <param name="pBlockModelOriginal">The instance of the original <c>BlockModel</c>.</param>
        public BlockModel(BlockModel pBlockModelOriginal)
        {
            this._ID = pBlockModelOriginal.ID;
            this._orderLocationID = pBlockModelOriginal.OrderLocationID;
            this._blockNumberInTheGreenHouse = pBlockModelOriginal.BlockNumberInTheGreenHouse;
            this._seedTrayAmount = pBlockModelOriginal.SeedTrayAmount;
            this._numberInBlock = pBlockModelOriginal.NumberInBlock;
        }

        /// <value>
        /// Gets the ID of the block.
        /// </value>
        public int ID => _ID;

        /// <value>
        /// Gets the ID of the order location to wich the block belongs.
        /// </value>
        public int OrderLocationID => _orderLocationID;

        /// <value>
        /// Gets the block number of the greenhouse in wich the block is.
        /// </value>
        public int BlockNumberInTheGreenHouse => _blockNumberInTheGreenHouse;

        /// <value>
        /// Gets the amount of seedtrays
        /// </value>
        public int SeedTrayAmount => _seedTrayAmount;

        /// <value>
        /// Gets the number of the block begins from the front of the block of the greenhouse.
        /// </value>
        public int NumberInBlock { get => _numberInBlock; set => _numberInBlock = value; }
    }
}
