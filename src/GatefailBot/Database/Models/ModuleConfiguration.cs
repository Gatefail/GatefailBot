namespace GatefailBot.Database.Models
{
    public class ModuleConfiguration : BaseEntity
    {
        public string ModuleName { get; set; }
        public bool Activated { get; set; }
    }
}