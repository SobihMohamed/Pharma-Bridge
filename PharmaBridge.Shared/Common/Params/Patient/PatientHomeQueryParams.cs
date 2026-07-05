namespace PharmaBridge.Shared.Common.Params.Patient
{
    public class PatientHomeQueryParams
    {
        private int _requestsCount = 5;
        public int RequestsCount
        {
            get => _requestsCount;
            set => _requestsCount = (value < 1) ? 1 : (value > 20) ? 20 : value;
        }

        private int _ordersCount = 5;
        public int OrdersCount
        {
            get => _ordersCount;
            set => _ordersCount = (value < 1) ? 1 : (value > 20) ? 20 : value;
        }
    }
}