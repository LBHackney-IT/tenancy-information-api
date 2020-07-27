namespace TenancyInformationApi.V1.Boundary.Response
{
    public class TenancyInformationResponse
    {
        public string TenancyReference { get; set; }
        public string CommencementOfTenancyDate { get; set; }
        public string EndOfTenancyDate { get; set; }
        public string CurrentBalance { get; set; }
        public string Present { get; set; }
        public string Terminated { get; set; }
        public string PaymentReference { get; set; }
        public string HouseholdReference { get; set; }
        public string PropertyReference { get; set; }
        public string TenureType { get; set; }
        public string AgreementType { get; set; }
        public string Service { get; set; }
        public string OtherCharge { get; set; }
    }
}
