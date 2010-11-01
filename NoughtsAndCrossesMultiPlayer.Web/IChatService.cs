using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.IO;
using System.ServiceModel.Web;

namespace NoughtsAndCrossesMultiPlayer.Web
{
    [ServiceContract(CallbackContract = typeof(INotification))]
    public interface IChatService
    {
        [OperationContract(IsOneWay = true)]
        void Subscribe(string channelName);

        [OperationContract(IsOneWay = true)]
        void Publish(string channelName, string content);
    }

    /// <summary>
    /// Notify contract
    /// </summary>
    [ServiceContract]
    public interface INotification
    {
        [OperationContract(IsOneWay = true, AsyncPattern = true)]
        IAsyncResult BeginNotify(Message message, AsyncCallback callback, object state);
        void EndNotify(IAsyncResult result);
    }

    /// <summary>
    /// This is for getting policy
    /// </summary>
    [ServiceContract]
    public interface IPolicyRetriever
    {
        [OperationContract, WebGet(UriTemplate = "/clientaccesspolicy.xml")]
        Stream GetSilverlightPolicy();
        [OperationContract, WebGet(UriTemplate = "/crossdomain.xml")]
        Stream GetFlashPolicy();

    }

}
