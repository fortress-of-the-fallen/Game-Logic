namespace Game.Application.Service.Models.Res.Base
{
    public class ResultRes<T> : ExecutionRes
    {
        public T Data { get; set; }

        public static ResultRes<T> Fail(string errorCode)
        {
            return new ResultRes<T>
            {
                ErrorCode = errorCode,
                Data = default,
            };
        }

        public static ResultRes<T> Get(string errorCode, T data)
        {
            return new ResultRes<T>
            {
                ErrorCode = errorCode,
                Data = data,
            };
        }
    }
}