using Npgsql;
using System;
using System.Runtime.Serialization;

namespace ApsWebApp
{
    [Serializable]
    internal class NpgsqlServiceException : Exception
    {

        public NpgsqlServiceException()
        {
        }

        public NpgsqlServiceException(Exception ex) : base(ex.Message)
        {
            var mysqlError = (NpgsqlException)ex.InnerException;
            if (mysqlError != null)
            {
                if (mysqlError.ErrorCode == 1062)
                {
                    throw new SystemException("Data Sudah Ada !");
                }
            }
        }

        public NpgsqlServiceException(string message) : base(message)
        {

        }

        public NpgsqlServiceException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected NpgsqlServiceException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}