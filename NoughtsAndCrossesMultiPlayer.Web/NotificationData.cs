using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;

namespace NoughtsAndCrossesMultiPlayer.Web
{
    [MessageContract]
    public class NotificationData
    {
        public const string NotificationAction = "http://tempuri.org/IChatService/Notify";

        [MessageBodyMember]
        public string Content { get; set; }
    }
}