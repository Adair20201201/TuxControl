using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;

namespace TuxControl
{
    public partial class Service1 : ServiceBase
    {
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            CTuxControl tc = new CTuxControl();
            Thread worker = new Thread(new ThreadStart(tc.handleSimpliciTI));
            worker.Start();
        }

        protected override void OnStop()
        {
        
        }
    }
}
