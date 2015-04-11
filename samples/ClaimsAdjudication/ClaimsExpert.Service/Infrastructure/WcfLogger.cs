using System;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using Common.Logging;

namespace NRules.Samples.ClaimsExpert.Service.Infrastructure
{
    public class WcfLogger : IErrorHandler
    {
        private static readonly ILog Log = LogManager.GetLogger<WcfLogger>();

        public bool HandleError(Exception error)
        {
            Log.Error("An unexpected has occurred.", error);
            return false;
        }

        public void ProvideFault(Exception error, MessageVersion version, ref Message fault)
        {
        }
    }
}