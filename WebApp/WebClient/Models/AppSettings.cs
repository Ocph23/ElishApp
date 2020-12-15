namespace WebClient.Models
{
    public class AppSettings
    {
        public string Secret { get; set; }
        public string PasswordHashKey { get; set; }
        public string Accounting { get; set; }
        public string Warehouse { get; set; }
    }
}