using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebClient.Middlewares
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
}

