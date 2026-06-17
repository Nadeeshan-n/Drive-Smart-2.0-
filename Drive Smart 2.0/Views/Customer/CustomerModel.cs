namespace Drive_Smart_2._0.Views.Customer.Database
{
    /// <summary>
    /// Represents a single customer record stored in the SQLite database.
    /// </summary>
    public class CustomerModel
    {
        public int    CustomerID     { get; set; }   // Auto-incremented PK
        public string CustomerName   { get; set; }   // Full name
        public string ContactNumber  { get; set; }   // 10-digit phone
        public string Address        { get; set; }   // Residential address
        public string Gender         { get; set; }   // "Male" | "Female"
        public string EmailAddress   { get; set; }   // Email
        public string NICNumber      { get; set; }   // 12-digit national ID
        public string DrivingLicense { get; set; }   // License number
        public string CreatedAt      { get; set; }   // Auto-set by SQLite
    }
}
