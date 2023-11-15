namespace SupportLayer.Models
{
    public partial class Municipality
    {
        public string ProvinceName
        {
            get { return Province.Name; }
        }
    }
}
