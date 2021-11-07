using System;
using System.Globalization;

namespace WebApi.Helpers.Auth
{
    // custom exception class for throwing application specific exceptions 
    // that can be caught and handled within the application
    public class AppException : Exception
    {
        private int statusCode = 400;
        private string title = null;
        private string type = null;

        public int StatusCode
        {
            get { return statusCode; }
            set { statusCode = value; }
        }

        public string Title
        {
            get { return title; }
            set { title = value; }
        }

        public string Type
        {
            get { return type; }
            set { type = value; }
        }

        public AppException(string message, int statusCode, string type, string title) : base(message)
        {
            this.statusCode = statusCode;
            this.type = type;
            this.title = title;
        }

        public AppException(string message, int statusCode, string type, string title, params object[] args)
            : base(String.Format(CultureInfo.CurrentCulture, message, args))
        {
            this.statusCode = statusCode;
            this.type = type;
            this.title = title;
        }
    }
}