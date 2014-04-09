using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eZ430ChronosNet;
using System.IO;

namespace TuxControl
{

    public delegate void dLog(string msg);

    class CTuxControl
    {
        
        Chronos chron;
        CKeyControl key;
        StreamWriter log;
        string version;
        dLog externalLog=null;

        public CTuxControl()
        {
            version = "1.0";
            key = new CKeyControl();
            log = new StreamWriter(File.Open("c:\\log.txt", FileMode.Create, FileAccess.Write, FileShare.Read));

            writeLog("TuxController startet. Version: " + version);
        }
        public CTuxControl(dLog exLog)
        {
            version = "1.0";
            key = new CKeyControl();
            log = new StreamWriter(File.Open("c:\\log.txt",FileMode.Create,FileAccess.Write,FileShare.Read));
            externalLog = exLog;
            writeLog("TuxController startet. Version: "+version);
        }

        private void writeLog(string msg)
        {
            Console.WriteLine(DateTime.Now.ToString() + " : " + msg);
            log.WriteLine(DateTime.Now.ToString()+" : "+msg);
            log.Flush();
            if(externalLog!=null)
            externalLog(DateTime.Now.ToString() + " : " + msg);
        }

        public void handleSimpliciTI()
        {
           chron = new Chronos();

           while (true)
           {
               try
               {
                   chron.OpenComPort(chron.GetComPortName());
                   writeLog("Found access point on port: " + chron.GetComPortName());
                   break;
               }
               catch (Exception)
               {
                   System.Threading.Thread.Sleep(1000);
               }
           }

           chron.StartSimpliciTI();
           
           writeLog("SimpliciTI started");
           
           while (true)
           {
               uint data;
               if (!chron.GetData(out data))
                   break;

               if(data!=0xFF)
                  writeLog("Got data: "+data);

               //Star
               if (data == 2)
               {
                   key.pressKey(0x44);
               }
               if (data == 3)
               {
                   key.releaseKey(0x44);
               }
               
               //NUM
               if (data == 4)
               {
                   key.pressKey(0x41);
               }
               if (data == 5)
               {
                   key.releaseKey(0x41);
               }

               //UP
               if (data == 6)
               {
                   key.pressKey(0x57);
               }
               if (data == 7)
               {
                   key.releaseKey(0x57);
               }

           }

           writeLog("Error!");
        }
    
    }
}
