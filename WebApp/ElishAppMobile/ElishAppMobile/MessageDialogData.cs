using System;
using System.Collections.Generic;
using System.Text;

namespace ElishAppMobile
{
    public class MessageDataCenter
    {
        public string Title { get; internal set; }
        public string Message { get; internal set; }
        public string? Ok { get; internal set; }
        public string? Cancel { get; internal set; }
    }
}
