using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace ttapiOrderBookWatcher
{
    using TradingTechnologies.TTAPI;

    public class TTAPIEvents : IDisposable
    {
        private TradingFrameworkUtility TFU;
   

        private UniversalLoginTTAPI apiInstance = null;
        private WorkerDispatcher workDispatch = null;
        private bool disposed = false;
        private object m_lock = new object();

        private string username = string.Empty;
        private string password = string.Empty;


        /// <summary>
        /// Create and start the Dispatcher
        /// </summary>
        public void Start(TradingFrameworkUtility tfu)
        {
            TFU = tfu;
            username = Properties.Settings.Default.USER;
            password = Properties.Settings.Default.PASSWORD;

            TFU.debug("user is: {0}", username);
            // Attach a WorkerDispatcher to the current thread
            workDispatch = Dispatcher.AttachWorkerDispatcher();
            workDispatch.BeginInvoke(new Action(Init));
            workDispatch.Run();
        }

        /// <summary>
        /// Initialize TT API
        /// </summary>
        public void Init()
        {
            // Use "Universal Login" Login Mode
            TTAPI.UniversalLoginModeDelegate ulDelegate = new
            TTAPI.UniversalLoginModeDelegate(ttApiInitComplete);
            TTAPI.CreateUniversalLoginTTAPI(Dispatcher.Current, ulDelegate);
        }


        /// <summary>
        /// Event notification for status of TT API initialization
        /// </summary>
        public void ttApiInitComplete(UniversalLoginTTAPI api, Exception ex)
        {
            if (ex == null)
            {
                TFU.trace("TT API Initialization Succeeded");
                // Authenticate your credentials
                apiInstance = api;
                apiInstance.AuthenticationStatusUpdate += new
                EventHandler<AuthenticationStatusUpdateEventArgs>(
                apiInstance_AuthenticationStatusUpdate);
                apiInstance.Authenticate(username, password);
            
            }
            else
            {
                TFU.trace("TT API Initialization Failed: {0}", ex.Message);
       
            }
        }

        /// <summary>
        /// Event notification for status of authentication
        /// </summary>
        public void apiInstance_AuthenticationStatusUpdate(object sender,
            AuthenticationStatusUpdateEventArgs e)
        {
            if (e.Status.IsSuccess)
            {
                TFU.trace("TT API User Authentication: {0}", e.Status.AuthenticationResultCode);
                // Add code here to begin working with the TT API

            }
            else
            {
                TFU.trace("ERROR: TT API User Authentication: {0}", e.Status.AuthenticationResultCode);
            
            }
        }


        /// <summary>
        /// Shuts down the TT API
        /// </summary>
        public void Dispose()
        {

            lock (m_lock)
            {
                if (!disposed)
                {
                    // Shutdown the TT API
                    if (apiInstance != null)
                    {
                        apiInstance.Shutdown();
                        apiInstance = null;
                    }
                    // Shutdown the Dispatcher
                    if (workDispatch != null)
                    {
                        workDispatch.BeginInvokeShutdown();
                        workDispatch = null;
                    }
                    disposed = true;
                }
            }
        }

    }
}
