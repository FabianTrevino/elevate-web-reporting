using System.Collections.Generic;

namespace DM.WR.Data.Repository.Types
{
    public class ServerResult<T> where T : class
    {
        public ServerResult()
        {
            IsError = false;
            Data = null;
            Errors = null;
        }

        public ServerResult(List<string> errors)
        {
            IsError = true;
            Data = null;
            Errors = errors;
        }

        public ServerResult(T data)
        {
            IsError = false;
            Data = data;
            Errors = null;
        }

        public bool IsError { get; set; }
        public T Data { get; set; }
        public List<string> Errors { get; set; }
    }
}