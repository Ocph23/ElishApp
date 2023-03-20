using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Text;

namespace ApsWebApp
{

    public interface IExceptionNotificationService
    {
        virtual void WriteLine(string value) { }
        event EventHandler<string> OnException;
    }
    public class ExceptionNotificationService : TextWriter, IExceptionNotificationService
    {
        private readonly TextWriter _decorated;
        public event EventHandler<string> OnException;
        public override Encoding Encoding => Encoding.UTF8;

        public ExceptionNotificationService()
        {
            _decorated = Console.Error;
            Console.SetError(this);
        }
        //THis is the method called by Blazor
        public override void WriteLine(string value)
        {
            OnException?.Invoke(this, value);
            _decorated.WriteLine(value);
        }
    }



    public static class ApiControllerErrorExtention
    {
        public static ObjectResult OnError(this ControllerBase controller, System.Exception ex)
        {
            var type = ex.GetType();
            if (type.Name == typeof(System.UnauthorizedAccessException).Name)
                return controller.Unauthorized(ex.Message);

            return controller.BadRequest(new ErrorMessage(ex.Message));
        }

        public static SystemException ThrowException(this Exception ex)
        {
            var type = ex.GetType();
            if (type.Name == typeof(System.UnauthorizedAccessException).Name)
                return new UnauthorizedAccessException(ex.Message, ex);
            return new SystemException(ex.Message,ex);
        }
    }


  
}

