using System;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Collections.Generic;
using System.ServiceModel.Channels;
using System.IO;
using System.ServiceModel.Web;
using System.Text;
using System.ServiceModel.Description;

namespace NoughtsAndCrossesMultiPlayer.Web
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.Single)]
    public class ChatService : IChatService, IPolicyRetriever
    {

        object syncRoot = new object();
        Dictionary<string, string> sessionIdTopic = new Dictionary<string, string>();

        Dictionary<string, Dictionary<string, INotification>> topicSessionIdCallbackChannel = new Dictionary<string, Dictionary<string, INotification>>();

        static TypedMessageConverter messageConverter = TypedMessageConverter.Create(
            typeof(NotificationData),
            NotificationData.NotificationAction,
            "http://schemas.datacontract.org/2004/07/NoughtsAndCrossesMultiPlayer.Web");

        static AsyncCallback onNotifyCompleted = new AsyncCallback(NotifyCompleted);

        public void Subscribe(string topic)
        {
            if (topic == null)
            {
                topic = string.Empty;
            }

            string sessionId = OperationContext.Current.Channel.SessionId;
            INotification callbackChannel = OperationContext.Current.GetCallbackChannel<INotification>();

            lock (syncRoot)
            {
                this.sessionIdTopic[sessionId] = topic;
                Dictionary<string, INotification> sessionIdCallbackChannel = null;
                if (!this.topicSessionIdCallbackChannel.TryGetValue(topic, out sessionIdCallbackChannel))
                {
                    sessionIdCallbackChannel = new Dictionary<string, INotification>();
                    this.topicSessionIdCallbackChannel[topic] = sessionIdCallbackChannel;
                }
                sessionIdCallbackChannel[sessionId] = callbackChannel;
            }
            OperationContext.Current.Channel.Faulted += new EventHandler(this.Unsubscribe);
            OperationContext.Current.Channel.Closed += new EventHandler(this.Unsubscribe);
        }

        Stream StringToStream(string result)
        {
            WebOperationContext.Current.OutgoingResponse.ContentType = "application/xml";
            return new MemoryStream(Encoding.UTF8.GetBytes(result));
        }

        public Stream GetSilverlightPolicy()
        {
            string result = @"<?xml version=""1.0"" encoding=""utf-8""?>
<access-policy>
    <cross-domain-access>
        <policy>
            <allow-from http-request-headers=""*"">
                <domain uri=""*""/>
            </allow-from>
            <grant-to>
                <resource path=""/"" include-subpaths=""true""/>
            </grant-to>
        </policy>
    </cross-domain-access>
</access-policy>";
            return StringToStream(result);
        }

        public Stream GetFlashPolicy()
        {
            string result = @"<?xml version=""1.0""?>
<!DOCTYPE cross-domain-policy SYSTEM ""http://www.macromedia.com/xml/dtds/cross-domain-policy.dtd"">
<cross-domain-policy>
    <allow-access-from domain=""*"" />
</cross-domain-policy>";
            return StringToStream(result);
        }

        public void Publish(string topic, string content)
        {
            if (topic == null)
            {
                topic = string.Empty;
            }

            List<INotification> clientsToNotify = null;
            lock (this.syncRoot)
            {
                Dictionary<string, INotification> sessionIdCallbackChannel = null;
                if (this.topicSessionIdCallbackChannel.TryGetValue(topic, out sessionIdCallbackChannel))
                {
                    clientsToNotify = new List<INotification>(
                        sessionIdCallbackChannel.Where(x => x.Key != OperationContext.Current.Channel.SessionId).Select(x => x.Value));
                }
            }

            if (clientsToNotify != null && clientsToNotify.Count > 0)
            {
                MessageBuffer notificationMessageBuffer = messageConverter.ToMessage(new NotificationData { Content = content }).CreateBufferedCopy(65536);
                foreach (INotification callbackChannel in clientsToNotify)
                {
                    try
                    {
                        callbackChannel.BeginNotify(notificationMessageBuffer.CreateMessage(), onNotifyCompleted, callbackChannel);
                    }
                    catch (CommunicationException)
                    {
                        // empty
                    }
                }
            }
        }

        static void NotifyCompleted(IAsyncResult result)
        {
            try
            {
                ((INotification)(result.AsyncState)).EndNotify(result);
            }
            catch (CommunicationException)
            {
                // empty
            }
            catch (TimeoutException)
            {
                // empty
            }
        }

        void Unsubscribe(object sender, EventArgs e)
        {
            IContextChannel channel = (IContextChannel)sender;
            if (channel != null && channel.SessionId != null)
            {
                lock (this.syncRoot)
                {
                    string topic = null;
                    if (this.sessionIdTopic.TryGetValue(channel.SessionId, out topic))
                    {
                        this.sessionIdTopic.Remove(channel.SessionId);
                        Dictionary<string, INotification> sessionIdCallbackChannel = null;
                        if (this.topicSessionIdCallbackChannel.TryGetValue(topic, out sessionIdCallbackChannel))
                        {
                            sessionIdCallbackChannel.Remove(channel.SessionId);
                            if (sessionIdCallbackChannel.Count == 0)
                            {
                                this.topicSessionIdCallbackChannel.Remove(topic);
                            }
                        }
                    }
                }
            }
        }
    }
}
