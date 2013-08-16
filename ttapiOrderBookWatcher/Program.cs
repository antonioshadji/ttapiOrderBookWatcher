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

            TFU.prod_write("{0}", Environment.GetCommandLineArgs()[0]);
            TFU.prod_write("started at: {0}", DateTime.Now);
        
            using (TTAPIEvents tt = new TTAPIEvents())
            {
                tt.Start(TFU);
            }

        }
    }
}
