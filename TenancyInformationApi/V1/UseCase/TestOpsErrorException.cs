using System;

namespace TenancyInformationApi.V1.UseCase
{
    public class TestOpsErrorException : Exception
    {
        public TestOpsErrorException() { }

        public TestOpsErrorException(string message) : base(message)
        {
        }

        public TestOpsErrorException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
