using System.Collections.Generic;

namespace TenancyInformationApi.V1.Boundary.Response
{
    public class TenancyInformationResponse
    {
        public string TenancyAgreementReference { get; set; }
        public string Address { get; set; }
        public string Postcode { get; set; }
        public string CommencementOfTenancyDate { get; set; }
        public string EndOfTenancyDate { get; set; }
        public string CurrentBalance { get; set; }
        public bool Present { get; set; }
        public bool Terminated { get; set; }
        public string PaymentReference { get; set; }
        public string HouseholdReference { get; set; }
        public string PropertyReference { get; set; }
        public string TenureType { get; set; }
        public string AgreementType { get; set; }
        public string Service { get; set; }
        public string OtherCharge { get; set; }
        public List<Resident> Residents { get; set; }
    }

    public class Resident
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DateOfBirth { get; set; }
        public int PersonNumber { get; set; }
    }
}
