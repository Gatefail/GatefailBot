namespace GatefailBot.Services.ResponseModels
{
    public class ToggleModuleResponse
    {
        public bool Success { get; }
        public string Reason { get; }

        public ToggleModuleResponse(bool success, string reason)
        {
            this.Success = success;
            this.Reason = reason;
        }

        public static ToggleModuleResponse ToggleModuleSuccess(string reason)
        {
            return new ToggleModuleResponse(true, reason);
        }

        public static ToggleModuleResponse ToggleModuleFailed(string reason)
        {
            return new ToggleModuleResponse(false, reason);
        }
    }
    
    
}