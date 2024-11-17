using System.ComponentModel.DataAnnotations.Schema;

namespace Common.Models
{
    public abstract class PostgresDocument
    {
        public Guid Id { get; set; } = Guid.NewGuid(); 
        public DateTime LastChangedAt { get; set; }
    }
}

