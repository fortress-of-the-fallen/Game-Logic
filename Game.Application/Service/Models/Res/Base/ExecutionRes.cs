namespace Game.Application.Service.Models.Res.Base
{
    public class ExecutionRes
    {
        public string ErrorCode { get; set; }

        public bool IsSuccess => string.IsNullOrEmpty(ErrorCode);
    }
}