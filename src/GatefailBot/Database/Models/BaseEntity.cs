using System.ComponentModel.DataAnnotations;

namespace GatefailBot.Database.Models
{
    public class BaseEntity
    {
        [Key] public string Id { get; set; }
    }
}