namespace Domain.Models
{
    /// <summary>
    /// Represents a product that is grown in the seedbed.
    /// </summary>
    public class ProductModel
    {
        private readonly byte _ID;
        private readonly string _variety;
        private readonly string _specie;
        private readonly short _productionInterval;

        /// <summary>
        /// Initializes a new instance of <c>ProductModel</c>.
        /// </summary>
        /// <param name="pID">The single ID of a product.</param>
        /// <param name="pVariety">The variety name of the product.</param>
        /// <param name="pSpecie">The species name of the product.</param>
        /// <param name="pProductionInterval">The days that the product takes to be ready to deliver.</param>
        public ProductModel(byte pID, string pVariety, string pSpecie, short pProductionInterval)
        {
            _ID = pID;
            _variety = pVariety;
            _specie = pSpecie;
            _productionInterval = pProductionInterval;
        }

        /// <summary>
        /// Initializes a new duplicate instance of <c>ProductModel</c> from another instance.
        /// </summary>
        /// <param name="pProductModelOriginal">The instance of the original <c>ProductModel</c>.</param>
        public ProductModel(ProductModel pProductModelOriginal)
        {
            this._ID = pProductModelOriginal.ID;
            this._variety = pProductModelOriginal.Variety;
            this._specie = pProductModelOriginal.Specie;
            this._productionInterval = pProductModelOriginal.ProductionInterval;
        }
        /// <value>
        /// Gets the ID of the product
        /// </value>        
        public byte ID => _ID;
        /// <value>
        /// Gets the name of the variety
        /// </value>
        public string Variety => _variety;
        /// <value>
        /// Gets the name of the species
        /// </value>
        public string Specie => _specie;
        /// <value>
        /// Gets the days that a ProductModel takes to be ready to deliver
        /// </value>
        public short ProductionInterval => _productionInterval;
    }
}