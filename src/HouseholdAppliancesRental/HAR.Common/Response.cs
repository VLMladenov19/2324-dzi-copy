namespace HAR.Common
{
    /// <summary>
    /// Generic response object for encapsulating an operations outcome
    /// </summary>
    public class Response
    {
        public bool IsSuccessful { get; set; }

        public string? Message { get; set; }

        public string? Details { get; set; }
    }
}
