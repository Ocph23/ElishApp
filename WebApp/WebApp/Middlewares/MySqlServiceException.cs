using System;
using System.Runtime.Serialization;
using MySql.Data.MySqlClient;

namespace WebApp.Middlewares
{
    [Serializable]
    internal class MySqlServiceException : Exception
    {

        public MySqlServiceException()
        {
        }

        public MySqlServiceException(Exception ex) : base(ex.Message)
        {
            var mysqlError = (MySqlException)ex.InnerException;
            if (mysqlError != null)
            {
                if (mysqlError.Number == 1062)
                {
                    throw new SystemException("Data Sudah Ada !");
                }
            }
        }

        public MySqlServiceException(string message) : base(message)
        {

        }

        public MySqlServiceException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected MySqlServiceException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}