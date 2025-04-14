namespace ApsWebApp.Models
{
    public class AppSettings
    {
        public string Secret { get; set; }
        public string PasswordHashKey { get; set; }
        public string Accounting { get; set; }
        public string Warehouse { get; set; }
        public string BankAccountName { get; set; }
        public string BankAccountNumber { get; set; }

    }
}