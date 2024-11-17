namespace Common.Models
{
    public class Employee : PostgresDocument
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public decimal Contacts { get; set; }
        public decimal? Salary { get; set; }
        public string Function { get; set; }
    }
}
