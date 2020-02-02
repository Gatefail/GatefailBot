using System;
using System.ComponentModel.DataAnnotations;

namespace GatefailBot.Infrastructure.Entities
{
    public class EntityBase
    {
        [Key]
        public ulong Id { get; set; }
    }
}