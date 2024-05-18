namespace Domain.Models
{
    /// <summary>
    /// Represents a client
    /// </summary>
    public class ClientModel
    {
        private readonly short _ID;
        private readonly string _name;
        private readonly string _nickName;

        /// <summary>
        /// Initializes a new instance of <c>ClientModel</c>.
        /// </summary>
        /// <param name="pID">The single ID of the client.</param>
        /// <param name="pName">The full name of the client</param>
        /// <param name="pNickName">The nickname of the client</param>
        public ClientModel(short pID, string pName, string pNickName)
        {
            _ID = pID;
            _name = pName;
            _nickName = pNickName;
        }

        /// <summary>
        /// Initializes a new duplicate instance of <c>ClientModel</c> from another instance.
        /// </summary>
        /// <param name="pClientModelOriginal">The instance of the original <c>ClientModel</c>.</param>
        public ClientModel(ClientModel pClientModelOriginal)
        {
            this._ID = pClientModelOriginal.ID;
            this._name = pClientModelOriginal.Name;
            this._nickName = pClientModelOriginal.NickName;
        }

        /// <summary>
        /// Gets the ID of the client.
        /// </summary>
        public short ID => _ID;

        /// <summary>
        /// Gets the name of the client.
        /// </summary>
        public string Name => _name;

        /// <summary>
        /// Gets the nickname of the client.
        /// </summary>
        public string NickName => _nickName;
    }
}