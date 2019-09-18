using System;
using GatefailBot.Database.Models;

namespace GatefailBot.Database.BatchQueries
{
    public class BatchQueryItem : BaseEntity
    {
        public Guid UniqueId { get; set; }

        public ulong IdToQuery { get; set; }
    }
}