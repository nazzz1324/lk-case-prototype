using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Account.Domain.Result
{
    public class BaseResult
    {
        public bool IsSuccess => Message == null;
        public string Message {  get; set; }
        public int? ErrorCode { get; set; }
    }
    public class BaseResult<T> : BaseResult
    {
        public BaseResult(string message, int errorCode, T data)
        {
            Message = message;
            ErrorCode = errorCode;
            Data = data;

        }
        public BaseResult() { }
        public T Data { get; set; }
    }
}
