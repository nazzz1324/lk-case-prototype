using Account.Domain.Dictionaries;
using Account.Domain.Enum;

namespace Account.Domain.Result
{
    /// <summary>
    /// Исключение-результат, содержащее всю информацию об ошибке
    /// Аналог BaseResult, но для throw
    /// </summary>
    public class ExceptionResult : Exception
    {
        public ErrorCodes ErrorCode { get; }
        public int HttpStatusCode { get; }

        public ExceptionResult(ErrorCodes errorCode, string message)
            : base(message)
        {
            ErrorCode = errorCode;
            HttpStatusCode = ErrorDictionary.GetHttpStatus(errorCode);
        }
        public BaseResult<T> ToBaseResult<T>() where T : class
        {
            return new BaseResult<T>
            {
                ErrorCode = (int)ErrorCode,
                Message = Message
            };
        }

        public BaseResult ToBaseResult()
        {
            return new BaseResult
            {
                ErrorCode = (int)ErrorCode,
                Message = Message
            };
        }
    }
}