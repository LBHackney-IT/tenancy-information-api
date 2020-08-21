namespace TenancyInformationApi.V1.Domain
{
    public class Tenancy
    {
        public string TenancyReference { get; set; }
        public string CommencementOfTenancyDate { get; set; }
        public string EndOfTenancyDate { get; set; }
        public float? CurrentBalance { get; set; }
        public bool Present { get; set; }
        public bool Terminated { get; set; }
        public string PaymentReference { get; set; }
        public string HouseholdReference { get; set; }
        public string PropertyReference { get; set; }
        public float? Service { get; set; }
        public float? OtherCharge { get; set; }
        public string Tenure { get; set; }
        public string Agreement { get; set; }
    }
}
