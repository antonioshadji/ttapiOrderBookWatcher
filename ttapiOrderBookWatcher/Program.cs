using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace ttapiOrderBookWatcher
{
    class Program
    {
        static void Main(string[] args)
        {
            TradingFrameworkUtility TFU = new TradingFrameworkUtility();

            TFU.trace("{0}", Environment.GetCommandLineArgs()[0]);
            TFU.trace("started at: {0}", DateTime.Now);
        
            using (TTAPIEvents tt = new TTAPIEvents())
            {
                tt.Start(TFU);
            }

        }
    }
}
