namespace Game.Application.Service.Models.Res.Base
{
    public class ResultRes<T> : ExecutionRes
    {
        public T Result { get; set; }

        public static ResultRes<T> Fail(string errorCode)
        {
            return new ResultRes<T>
            {
                ErrorCode = errorCode,
                Result = default,
            };
        }

        public static ResultRes<T> Get(string errorCode, T Result)
        {
            return new ResultRes<T>
            {
                ErrorCode = errorCode,
                Result = Result,
            };
        }
    }
}