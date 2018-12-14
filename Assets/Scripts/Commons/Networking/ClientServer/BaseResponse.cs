using System.Collections.Generic;

namespace nopact.Commons.Networking.ClientServer
{
    public class BaseResponse
    {
        protected Result result;
        protected Dictionary<string, string> validationErrors;

        public Result Result
        {
            get { return this.result; }
            set { this.result = value; }
        }

        public Dictionary<string, string> ValidationErrors
        {
            get { return this.validationErrors; }
            set { validationErrors = value; }
        }
    }

    public class Result
    {
        private int code;
        private string message;
        private string detail;

        public Result()
        {
        }


        public Result(int code, string message, string detail)
        {
            this.code = code;
            this.message = message;
            this.detail = detail;
        }

        public int Code
        {
            get { return this.code; }
            set { this.code = value; }
        }


        public string Message
        {
            get { return this.message; }
            set { this.message = value; }
        }

        public string Detail
        {
            get { return this.detail; }
            set { this.detail = value; }
        }
    }
}