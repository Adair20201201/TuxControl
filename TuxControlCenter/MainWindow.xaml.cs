using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Reflection;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using TuxControl;
using System.Threading;

namespace TuxControlCenter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        System.Windows.Forms.NotifyIcon ico;
        CTuxControl tc;
        
        public MainWindow()
        {
            
            //Stream s = Assembly.GetExecutingAssembly().GetManifestResourceStream("TuxControlCenter.icon.png");
             
            ico = new System.Windows.Forms.NotifyIcon();
            ico.Icon = new System.Drawing.Icon(typeof(MainWindow), "icon.ico");
            ico.BalloonTipTitle = "TuxControl Center";
            ico.BalloonTipText = "TuxControl is Active.";
            //ico.MouseMove += new System.Windows.Forms.MouseEventHandler(ico_MouseMove);
            ico.Text = "TuxControl is Active";
            
            //.FromHandle(((System.Drawing.Bitmap)System.Drawing.Image.FromStream(s)).GetHicon());

            System.Windows.Forms.ContextMenu m = new System.Windows.Forms.ContextMenu();
            m.Name = "test";
             
            ico.ContextMenu = m;

            System.Windows.Forms.MenuItem m1 = new System.Windows.Forms.MenuItem("Open log",new EventHandler(onOpen));
            m.MenuItems.Add(m1);
            System.Windows.Forms.MenuItem m2 = new System.Windows.Forms.MenuItem("Exit", new EventHandler(onExit));
            m.MenuItems.Add(m2);

            SplashScreen sp = new SplashScreen("FullBanner.jpg");
            sp.Show(false);
            
            
            this.Hide();
            InitializeComponent();
            
             
            ico.Visible = true;

            tbLog.AppendText(DateTime.Now.ToString()+" : GUI initialized. Starting worker thread.\r\n");

            Thread worker = new Thread(new ThreadStart(handleSimpliciTI));
            worker.IsBackground = true;
            worker.Start();

            Thread.Sleep(2000);
            
            sp.Close(new TimeSpan(0,0,2));

            
        }

        void ico_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            
            ico.ShowBalloonTip(1000);
            
        }

        delegate void writeLog(string msg);
        delegate void toEnd();
        public void log(string msg)
        {
          Dispatcher.Invoke(new writeLog(tbLog.AppendText),msg+"\r\n");
          Dispatcher.Invoke(new toEnd(tbLog.ScrollToEnd), null);
        }
        public void onOpen(object o, System.EventArgs e)
        {
            this.Show();
        }
        public void onExit(object o, System.EventArgs e)
        {
            ico.Visible = false;
            ico.Dispose();
            Environment.Exit(0);
        }

        private void handleSimpliciTI()
        {
            tc = new CTuxControl(new dLog(log));
            tc.handleSimpliciTI();   
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.Hide();
            e.Cancel = true;
        }


    }
}
