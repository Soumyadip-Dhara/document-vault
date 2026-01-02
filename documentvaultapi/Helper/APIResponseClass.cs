// Helper class for API responses
using documentvaultapi.Enum;

namespace documentvaultapi.Helper
{
    public class APIResponseClass<T>
    {
        public T? result { get; set; }
        public APIResponseStatus apiResponseStatus { get; set; }
        public string message { get; set; }
    }
    //public class APIResponseClass
    //{
    //    public bool Success { get; set; }
    //    public string Message { get; set; }
    //    public object Data { get; set; }
    //}
}
