using _06_24InformationSystem.Model;
using _06_24InformationSystem.Model.ICXComponent;
using _06_24InformationSystem.Model.MySQL.Actions;
using _06_24InformationSystem.Model.MySQL.Export;
using _06_24InformationSystem.Model.MySQL.Logbook;
using _06_24InformationSystem.Model.MySQL.News;
using _06_24InformationSystem.Model.MySQL.QA;
using _06_24InformationSystem.Model.MySQL.Summation;
using _06_24InformationSystem.View;
using GPLED.Logic;
using log4net;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Timers;
using System.Windows.Forms;

namespace _06_24InformationSystem
{
    public partial class MainView : Form
    {
        private static System.Timers.Timer _timer;

        private Thread thread;
        private string IcxServerIp = XMLReader.getICXIP();
        private int IcxServerPort = XMLReader.getICXPort();
        public static bool stop = false;
        private ICX_Client icx;
        private string FileNameAndPath;

        private void StartIdleTimer()
        {
            log.Info("Started Ping auto-reconnecting timer");
            _timer = new System.Timers.Timer(130000); // Set up the timer for 130 seconds
            _timer.Elapsed += new ElapsedEventHandler(_timer_Elapsed);
            _timer.Enabled = true;
            _timer.AutoReset = true;
            OnlineOfflineStatus.Image = Image.FromFile(@"online.png");
        }

        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            OnlineOfflineStatus.Image = Image.FromFile(@"offline.png");
            log.Error("No ping or NewDataRecieved detected!");
            log.Info("Timer has elapsed - execute disconnect/reconnect");
            icx.Disconnect();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static string GetCurrentMethod()
        {
            StackTrace st = new StackTrace();
            StackFrame sf = st.GetFrame(1);

            return sf.GetMethod().Name;
        }

        private int intercom;
        private string loggedUser;
        private int activeLog = 0;

        // Lists
        private List<Logged> loggedList = new List<Logged>();
        private List<Unlogged> unloggedList = new List<Unlogged>();
        private List<string> eqpList = new List<string>();
        private List<string> custList = new List<string>();
        private List<string> usrList = new List<string>();
        private List<string> acList = new List<string>();
        //

        /// <summary>
        /// Log4Net setup
        /// </summary>
        /// <param name="stringToLog"></param>
        #region Logging Functions

        private static ILog log;

        private static void LogString(string stringToLog)
        {
            log.Info(stringToLog);
        }

        private static void InitializeLogger()
        {
            if (log4net.LogManager.GetCurrentLoggers().Length == 0)
            {
                string path = AppDomain.CurrentDomain.BaseDirectory.ToString();
                string configFile = path + "log4net.config";
                log4net.Config.XmlConfigurator.Configure(new FileInfo(configFile));
            }
            log = LogManager.GetLogger(typeof(MainView));
            log.Info("Started 06-24 Information System");
        }
        #endregion

        private void hideAdminTab()
        {
            try
            {
                MainInformationTab.TabPages.Remove(tabAdmin);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        private void hideExportTab()
        {
            try
            {
                MainInformationTab.TabPages.Remove(mysqlBackupTab);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public MainView(int level, int inter, string user)
        {
            intercom = inter;
            loggedUser = user;
            InitializeComponent();
            if (level == 1)
            {
                hideAdminTab();
                hideExportTab();
            }
            InitializeLogger();
            StartICXParser();
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
        }

        private void StartICXParser()
        {
            thread = new Thread(new ThreadStart(WorkerThread))
            {
                Name = "WorkerThread",
                IsBackground = true,
                Priority = ThreadPriority.AboveNormal
            };
            thread.Start();
        }

        public void WorkerThread()
        {
            log.Debug("Starting WorkerThread!");
            stop = false;
            //    restart = false;
            string IcxServerIp = XMLReader.getICXIP();
            int IcxServerPort = XMLReader.getICXPort();

            try
            {
                StartIdleTimer();

                icx = new ICX_Client();
                icx.NewIdleReceived += _ICXclient_NewIdleReceived;
                icx.NewDataReceived += _ICXclient_NewDataReceived;
                icx.NewDataSent += _ICXclient_NewDataSent;
                icx.ClientError += _ICXclient_ClientError;
                icx.ConnectedChanged += _ICXclient_ConnectedChanged;

                icx.SendIdleInterval = 30000;

                // Connect and start listening on the Commend interface for ICX messages
                icx.Connect(IcxServerIp, IcxServerPort);
                if (icx.Connected)
                {
                    log.Debug("CONNECTED to " + icx.IcxIp);
                    Console.WriteLine("Connected");
                }
                do
                {
                    Thread.Sleep(1000);
                    if (!icx.Connected)
                    {
                        icx.Connect(IcxServerIp, IcxServerPort);
                        log.Debug(DateTime.Now.ToShortTimeString() + " : Reconnect to " + icx.IcxIp);
                        Console.WriteLine("Reconnecting");
                    }

                } while (true);

            }
            catch (Exception ex)
            {
                log.Debug(DateTime.Now.ToShortTimeString() + " : exception " + ex);
                Console.WriteLine(ex);
            }
        }

        private void _ICXclient_NewIdleReceived(object sender, ICX_Client.NewIdleEventArgs e)
        {
            log.Info("Got PING!");
            byte[] idle = { 0xF1, 0x00, 0xFF };
            Reset();
        }

        public void Reset()
        {
            log.Info("Resetting Timer: ");
            _timer.Stop();
            StartIdleTimer();
        }

        private void ICXParser(string message)
        {
            log.Debug("In ICX-Parser...");
            try
            {
                // hämta rätt Intercom.
                if (intercom == findIntercom(message))
                {
                    log.Debug("Answered on intercom [" + intercom + "]");
                    DateTime dt = DateTime.Now;
                    var eqpCalled = string.Empty;

                    eqpCalled = message.Substring(10, 4);
                    log.Warn("ICX-Parser ELSE called. Eqp called: [" + eqpCalled + "]");

                    var temp = MySQLHandler.getICXInfo(eqpCalled);
                    InformationCustomerComboBox.SelectedIndex = InformationCustomerComboBox.FindStringExact(temp[0].CustomerName);
                    InformationEquipmentComboBox.SelectedIndex = InformationEquipmentComboBox.FindStringExact(temp[0].EquipmentName);
                    var tempDispName = DateTime.Now.ToString("HH:mm tt") + temp[0].CustomerName + " " + temp[0].EquipmentName;
                    Unlogged ul = new Unlogged();
                    ul.Customer = temp[0].CustomerName;
                    ul.Equipment = temp[0].EquipmentName;
                    ul.Time = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
                    ul.Day = dt.DayOfWeek.ToString();
                    ul.Date = DateTime.Now.ToString("dd-MM-yyy");
                    ul.DispName = tempDispName;
                    ul.GUID = Guid.NewGuid().ToString();
                    unloggedList.Add(ul);

                    reloadLists();
                }
                else
                {

                }
            }
            catch (Exception e)
            {
                log.Debug(DateTime.Now.ToShortTimeString() + " : exception ICX_Parser" + e);
            }
        }

        private int findIntercom(string message)
        {
            try
            {
                if (message.Length == 12)
                {
                    log.Debug("Message is 12 in length. ignoring");
                }
                var currIntercom = message.Substring(9, 1);
                return Convert.ToInt32(currIntercom);
            }
            catch (Exception e)
            {
                log.Debug(DateTime.Now.ToShortTimeString() + " : exception " + e);
                return 404; // dont know how to handle error correctly yet
            }
        }

        private static string GetString(byte[] bytes)
        {
            char[] chars = new char[bytes.Length / sizeof(char)];
            System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }

        private void _ICXclient_NewDataReceived(object sender, ICX_Client.NewDataEventArgs e)
        {
            try
            {
                if (e.Data == null)
                    return;

                Reset();
                log.Debug("Got NewDataReceived: " + Encoding.ASCII.GetString(e.Data));

                if (Encoding.ASCII.GetString(e.Data).StartsWith("004200") && Encoding.ASCII.GetString(e.Data).EndsWith("12"))
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        Console.WriteLine("Answered Call from: " + Encoding.ASCII.GetString(e.Data));
                        log.Debug("Answered Call from: " + Encoding.ASCII.GetString(e.Data));
                        ICXParser(Encoding.ASCII.GetString(e.Data));
                    });

                }
                if (Encoding.ASCII.GetString(e.Data).StartsWith("004200") && Encoding.ASCII.GetString(e.Data).EndsWith("10"))
                {
                    log.Debug("Hung up call from: " + Encoding.ASCII.GetString(e.Data));
                    Console.WriteLine("Hung up call from: " + Encoding.ASCII.GetString(e.Data));
                }
            }
            catch (Exception)
            {
                log.Debug(DateTime.Now.ToShortTimeString() + " : exception _ICXclient_NewDataReceived" + e);
            }
        }

        private void _ICXclient_NewDataSent(object sender, ICX_Client.NewDataEventArgs e)
        {
        }

        private void _ICXclient_ClientError(object sender, ICX_Client.ClientErrorEventArgs e)
        {
        }

        private void _ICXclient_ConnectedChanged(object sender, ICX_Client.ConnectedChangedEventArgs e)
        {
        }

        private void MainView_Load(object sender, EventArgs e)
        {
            eventComboBox.SelectedIndex = 0;

            ListManuals();

            updateQuestionListBox();
            this.ControlBox = false;

            var dataSourceCustomerMain = MySQLHandler.getCustomers();

            var bugListMain = MySQLHandler.getBugReports();

            this.bugListComboBox.DataSource = bugListMain;
            this.bugListComboBox.ValueMember = "Value";
            this.bugListComboBox.DisplayMember = "Value";

            var actionList = MySQLHandler.getActions();

            this.ActionComboBox.DataSource = actionList;
            this.ActionComboBox.ValueMember = "Value";
            this.ActionComboBox.DisplayMember = "Name";
            this.ActionComboBox.SelectedIndex = -1;

            this.singleCustCombobox.DataSource = dataSourceCustomerMain;
            this.singleCustCombobox.DisplayMember = "Name";
            this.singleCustCombobox.ValueMember = "Value";

            var userList = MySQLHandler.getUsers();

            this.userComboBox.DataSource = userList;
            this.userComboBox.ValueMember = "Value";
            this.userComboBox.DisplayMember = "Name";

            this.userExportComboBox.DataSource = userList;
            this.userExportComboBox.ValueMember = "Value";
            this.userExportComboBox.DisplayMember = "Name";

            IntercomIDLabel.Text = "Inloggad intercom: " + intercom;
            CurrentUserLabel.Text = "Inloggad Användare: " + loggedUser.ToUpper();

            this.InformationCustomerComboBox.DataSource = dataSourceCustomerMain;
            this.InformationCustomerComboBox.DisplayMember = "Name";
            this.InformationCustomerComboBox.ValueMember = "Value";

            this.InformationCustomerComboBox.DropDownStyle = ComboBoxStyle.DropDownList;

            this.AddEQPCustomerTextbox.DataSource = dataSourceCustomerMain;
            this.AddEQPCustomerTextbox.DisplayMember = "Name";
            this.AddEQPCustomerTextbox.ValueMember = "Value";

            this.AddEQPCustomerTextbox.DropDownStyle = ComboBoxStyle.DropDownList;

            this.AddMapCustomerComboBox.DataSource = dataSourceCustomerMain;
            this.AddMapCustomerComboBox.DisplayMember = "Name";
            this.AddMapCustomerComboBox.ValueMember = "Value";

            this.AddMapCustomerComboBox.DropDownStyle = ComboBoxStyle.DropDownList;

            this.changeEqpInfoCustomerComboBox.DataSource = dataSourceCustomerMain;
            this.changeEqpInfoCustomerComboBox.DisplayMember = "Name";
            this.changeEqpInfoCustomerComboBox.ValueMember = "Value";

            this.changeEqpInfoCustomerComboBox.DropDownStyle = ComboBoxStyle.DropDownList;

            this.DelCustomerComboBox.DataSource = dataSourceCustomerMain;
            this.DelCustomerComboBox.DisplayMember = "Name";
            this.DelCustomerComboBox.ValueMember = "Value";

            this.DelCustomerComboBox.DropDownStyle = ComboBoxStyle.DropDownList;

            this.DelEqpCustomerComboBox.DataSource = dataSourceCustomerMain;
            this.DelEqpCustomerComboBox.DisplayMember = "Name";
            this.DelEqpCustomerComboBox.ValueMember = "Value";

            this.DelEqpCustomerComboBox.DropDownStyle = ComboBoxStyle.DropDownList;

            this.DelEqpComboBox.DataSource = dataSourceCustomerMain;
            this.DelEqpComboBox.DisplayMember = "Name";
            this.DelEqpComboBox.ValueMember = "Value";

            this.DelEqpComboBox.DropDownStyle = ComboBoxStyle.DropDownList;

            this.removeCustMapComboBox.DataSource = dataSourceCustomerMain;
            this.removeCustMapComboBox.DisplayMember = "Name";
            this.removeCustMapComboBox.ValueMember = "Value";

            this.removeCustMapComboBox.DropDownStyle = ComboBoxStyle.DropDownList;


            this.ICXCustomer.DataSource = dataSourceCustomerMain;
            this.ICXCustomer.DisplayMember = "Name";
            this.ICXCustomer.ValueMember = "Value";

            this.ICXCustomer.DropDownStyle = ComboBoxStyle.DropDownList;


            this.AddCustPriceComboBox.DataSource = dataSourceCustomerMain;
            this.AddCustPriceComboBox.DisplayMember = "Name";
            this.AddCustPriceComboBox.ValueMember = "Value";

            this.AddCustPriceComboBox.DropDownStyle = ComboBoxStyle.DropDownList;


            this.AddCustomerContactInformationComboBox.DataSource = dataSourceCustomerMain;
            this.AddCustomerContactInformationComboBox.DisplayMember = "Name";
            this.AddCustomerContactInformationComboBox.ValueMember = "Value";

            this.AddCustomerContactInformationComboBox.DropDownStyle = ComboBoxStyle.DropDownList;


            this.changeGeneralCustInfoComboBox.DataSource = dataSourceCustomerMain;
            this.changeGeneralCustInfoComboBox.DisplayMember = "Name";
            this.changeGeneralCustInfoComboBox.ValueMember = "Value";

            this.changeGeneralCustInfoComboBox.DropDownStyle = ComboBoxStyle.DropDownList;


            this.changeCustNameComboBox.DataSource = dataSourceCustomerMain;
            this.changeCustNameComboBox.DisplayMember = "Name";
            this.changeCustNameComboBox.ValueMember = "Value";

            this.changeCustNameComboBox.DropDownStyle = ComboBoxStyle.DropDownList;


            this.swapaccessCustomerComboBox.DataSource = dataSourceCustomerMain;
            this.swapaccessCustomerComboBox.DisplayMember = "Name";
            this.swapaccessCustomerComboBox.ValueMember = "Value";

            this.swapaccessCustomerComboBox.DropDownStyle = ComboBoxStyle.DropDownList;


            var dataSourceEquipmentMain = MySQLHandler.getEquipment(InformationCustomerComboBox.Text);
            this.ICXEquipment.DataSource = dataSourceEquipmentMain;
            this.ICXEquipment.DisplayMember = "EquipmentName";
            this.ICXEquipment.ValueMember = "EquipmentID";

            this.ICXEquipment.DropDownStyle = ComboBoxStyle.DropDownList;


            DelEqpComboBox.SelectedIndex = -1;
            byte[] data = Convert.FromBase64String("PGh0bWw+DQoJPGhlYWQ+DQoJCTxtZXRhIGNoYXJzZXQ9IlVURi04Ij4NCgk8L2hlYWQ+DQoJDQoJPGJvZHk+DQoJCQ0KCQlIVE1MIEluc3RydWt0aW9uZXI6IDxicj48YnI+DQoJCUbDtnJzdCBrb21tZXIgSFRNTCBzeW50YXggc2VuIHZpc2FzIGV0dCBleGVtcGVsDQoJCQ0KCQk8YnI+DQoJCTxicj4NCgkJPGZpZWxkc2V0IHN0eWxlPSJkaXNwbGF5OmlubGluZTsiPg0KCQkJPGxlZ2VuZD5TdG9yIHJ1YnJpazo8L2xlZ2VuZD4NCgkJCSZsdDtoMSZndDsgRXhlbXBlbCAmbHQ7L2gxJmd0OyA8aDE+IEV4ZW1wZWwgPC9oMT4NCgkJPC9maWVsZHNldD4NCgkJPGJyPg0KCQk8YnI+DQoJCQ0KCQk8ZmllbGRzZXQgc3R5bGU9ImRpc3BsYXk6aW5saW5lOyI+DQoJCQk8bGVnZW5kPk1lbGxhbiBzdG9yIHJ1YnJpazo8L2xlZ2VuZD4NCgkJCSZsdDtoMiZndDsgRXhlbXBlbDIgJmx0Oy9oMiZndDsgPGgyPiBFeGVtcGVsMiA8L2gyPg0KCQk8L2ZpZWxkc2V0Pg0KCQkNCgkJPGJyPg0KCQk8YnI+DQoJCTxmaWVsZHNldCBzdHlsZT0iZGlzcGxheTppbmxpbmU7Ij4NCgkJCTxsZWdlbmQ+TGl0ZW4gcnVicmlrOjwvbGVnZW5kPg0KCQkJJmx0O2gzJmd0OyBFeGVtcGVsMyAmbHQ7L2gzJmd0OyA8aDM+IEV4ZW1wZWwzIDwvaDM+DQoJCTwvZmllbGRzZXQ+DQoJCQ0KCQkNCgkJPGJyPg0KCQk8YnI+DQoJCTxmaWVsZHNldCBzdHlsZT0iZGlzcGxheTppbmxpbmU7Ij4NCgkJCTxsZWdlbmQ+VGpvY2sgdGV4dCAoQm9sZCk6PC9sZWdlbmQ+DQoJCQkmbHQ7YiZndDsgRXhlbXBlbDMgJmx0Oy9iJmd0OyA8Yj4gRXhlbXBlbDMgPC9iPg0KCQk8L2ZpZWxkc2V0Pg0KCQkNCgkJPGJyPg0KCQk8YnI+DQoJCTxmaWVsZHNldCBzdHlsZT0iZGlzcGxheTppbmxpbmU7Ij4NCgkJCTxsZWdlbmQ+VW5kZXJzdHJ1a2VuIHRleHQ6PC9sZWdlbmQ+DQoJCQkmbHQ7aW5zJmd0OyBFeGVtcGVsMyAmbHQ7L2lucyZndDsgPGlucz4gRXhlbXBlbDMgPC9pbnM+DQoJCTwvZmllbGRzZXQ+DQoJCQ0KCQk8YnI+DQoJCTxicj4NCgkJPGZpZWxkc2V0IHN0eWxlPSJkaXNwbGF5OmlubGluZTsiPg0KCQkJPGxlZ2VuZD5SYWQgYnJ5dG5pbmc6PC9sZWdlbmQ+DQoJCQkmbHQ7YnImZ3Q7IEV4ZW1wZWwzICZsdDsvYnImZ3Q7IDxicj4gRXhlbXBlbDMgPC9icj4NCgkJPC9maWVsZHNldD4NCgkJDQoJCTxicj4NCgkJPGJyPg0KCQlIVE1MIE1hbGw6DQoJCTxicj4NCgkJPGJyPg0KCQkmbHQ7aHRtbCZndDsNCgkJPGJyPg0KCQkmbmJzcDsmbmJzcDsmbmJzcDsmbmJzcDsmbHQ7aGVhZCZndDsNCgkJPGJyPg0KCQkmbmJzcDsmbmJzcDsmbmJzcDsmbmJzcDsmbmJzcDsmbmJzcDsmbmJzcDsmbmJzcDsmbHQ7bWV0YSBjaGFyc2V0PSJVVEYtOCImZ3Q7DQoJCQ0KCQk8YnI+DQoJCSZuYnNwOyZuYnNwOyZuYnNwOyZuYnNwOyZsdDsvaGVhZCZndDsNCgkJPGJyPg0KCQk8YnI+DQoJCSZuYnNwOyZuYnNwOyZuYnNwOyZuYnNwOyZsdDtib2R5Jmd0Ow0KCQk8YnI+DQoJCTxicj4NCgkJU2tyaXYgYWxsIHRleHQgaSBCb2R5LCBrbGlzdHJhIGluIGRlbm5hIG1hbGwgaSBldHQgdG9tdCBkb2t1bWVudCwgc2tyaXYgZGluIHRleHQgb2NoIHNwYXJhIHNvbSBYWFhYLmh0bWwuIDxicj5IYXIgZHUgbsOlZ3JhIGZyw6Vnb3Igc8OlIGJhcmEgZnLDpWdhIHPDpSBoasOkbHBlciBqYQ0KCQk8YnI+DQoJCTxicj4NCgkJJm5ic3A7Jm5ic3A7Jm5ic3A7Jm5ic3A7Jmx0Oy9ib2R5Jmd0Ow0KCQk8YnI+DQoJCTxicj4NCgkJJmx0Oy9odG1sJmd0Ow0KCQk8YnI+DQoJCTxicj4NCgkJDQoJCQ0KCTwvYm9keT4NCgkNCjwvaHRtbD4=");

            AdminManualBrowser.Navigate("about:blank");
            AdminManualBrowser.Document.Encoding = "utf-8";
            Font fon = new Font("Palatino Linotype", 12.0f);
            AdminManualBrowser.Font = fon;
            HtmlDocument doc = AdminManualBrowser.Document;
            doc.Write(String.Empty);

            AdminManualBrowser.DocumentText = Encoding.UTF8.GetString(data);

            var InfoBlob = MySQLHandler.getManual();

            byte[] bytes3 = Encoding.Default.GetBytes(InfoBlob);
            InfoBlob = Encoding.UTF8.GetString(bytes3);

            ManualBrowser.Font = fon;
            ManualBrowser.Navigate("about:blank");
            ManualBrowser.Document.Encoding = "utf-8";
            HtmlDocument doc2 = ManualBrowser.Document;
            doc2.Write(String.Empty);
            try
            {
                InformationCustomerBrowser.Document.Encoding = "utf-8";
            }
            catch (Exception errz)
            {

            }
            ManualBrowser.DocumentText = InfoBlob;

            manualTextBox.Text = InfoBlob;

            var newsBlob = NewsMySQLHandler.getNews();
            newsTextBox.Text = newsBlob;


            newsMainTabBrowser.Font = fon;
            newsMainTabBrowser.Navigate("about:blank");
            newsMainTabBrowser.Document.Encoding = "utf-8";
            HtmlDocument doc3 = newsMainTabBrowser.Document;
            doc3.Write(String.Empty);

            newsMainTabBrowser.DocumentText = newsBlob;

            foreach (MySQLHandler.Actions ac in actionList)
            {
                actionCheckListbox.Items.Add(ac.Name);
            }

            foreach (MySQLHandler.Users ul in userList)
            {
                usersCheckboxList.Items.Add(ul.Name);
            }

            foreach (MySQLHandler.Customer c in dataSourceCustomerMain)
            {
                customerCheckboxList.Items.Add(c.Name);
            }

        }

        private void reloadLists()
        {

            NotYetLoggedComboBox.DataSource = null;
            //  unloggedList.Sort();

            NotYetLoggedComboBox.DataSource = unloggedList;
            NotYetLoggedComboBox.DisplayMember = "DispName";
            NotYetLoggedComboBox.ValueMember = "GUID";

        }

        private void reloadLoggedLists()
        {

            LoggedComboBox.DataSource = null;
            //   loggedList.Sort();
            LoggedComboBox.DataSource = loggedList;
            LoggedComboBox.DisplayMember = "DispName";
            LoggedComboBox.ValueMember = "GUID";

        }

        private void reload()
        {

            ListManuals();

            var newsBlob = NewsMySQLHandler.getNews();

            Font fon = new Font("Palatino Linotype", 12.0f);

            newsMainTabBrowser.Font = fon;
            newsMainTabBrowser.Navigate("about:blank");
            newsMainTabBrowser.Document.Encoding = "utf-8";
            HtmlDocument doc3 = newsMainTabBrowser.Document;
            doc3.Write(String.Empty);

            newsMainTabBrowser.DocumentText = newsBlob;


            // var newsBlob = NewsMySQLHandler.getNews();

            newsTextBox.Text = newsBlob;

            var userList = MySQLHandler.getUsers();

            this.userComboBox.DataSource = userList;
            this.userComboBox.ValueMember = "Value";
            this.userComboBox.DisplayMember = "Name";

            this.userExportComboBox.DataSource = userList;
            this.userExportComboBox.ValueMember = "Value";
            this.userExportComboBox.DisplayMember = "Name";

            var bugListMain = MySQLHandler.getBugReports();

            this.bugListComboBox.DataSource = bugListMain;
            this.bugListComboBox.ValueMember = "Value";
            this.bugListComboBox.DisplayMember = "Value";


            var dataSourceCustomerMain = MySQLHandler.getCustomers();

            this.InformationCustomerComboBox.DataSource = dataSourceCustomerMain;
            this.InformationCustomerComboBox.DisplayMember = "Name";
            this.InformationCustomerComboBox.ValueMember = "Value";


            this.singleCustCombobox.DataSource = dataSourceCustomerMain;
            this.singleCustCombobox.DisplayMember = "Name";
            this.singleCustCombobox.ValueMember = "Value";

            this.InformationCustomerComboBox.DropDownStyle = ComboBoxStyle.DropDownList;

            this.AddEQPCustomerTextbox.DataSource = dataSourceCustomerMain;
            this.AddEQPCustomerTextbox.DisplayMember = "Name";
            this.AddEQPCustomerTextbox.ValueMember = "Value";

            this.AddEQPCustomerTextbox.DropDownStyle = ComboBoxStyle.DropDownList;

            this.AddMapCustomerComboBox.DataSource = dataSourceCustomerMain;
            this.AddMapCustomerComboBox.DisplayMember = "Name";
            this.AddMapCustomerComboBox.ValueMember = "Value";

            this.AddMapCustomerComboBox.DropDownStyle = ComboBoxStyle.DropDownList;


            this.changeCustNameComboBox.DataSource = dataSourceCustomerMain;
            this.changeCustNameComboBox.DisplayMember = "Name";
            this.changeCustNameComboBox.ValueMember = "Value";

            this.changeCustNameComboBox.DropDownStyle = ComboBoxStyle.DropDownList;

            this.removeCustMapComboBox.DataSource = dataSourceCustomerMain;
            this.removeCustMapComboBox.DisplayMember = "Name";
            this.removeCustMapComboBox.ValueMember = "Value";

            this.removeCustMapComboBox.DropDownStyle = ComboBoxStyle.DropDownList;

            this.swapaccessCustomerComboBox.DataSource = dataSourceCustomerMain;
            this.swapaccessCustomerComboBox.DisplayMember = "Name";
            this.swapaccessCustomerComboBox.ValueMember = "Value";

            this.swapaccessCustomerComboBox.DropDownStyle = ComboBoxStyle.DropDownList;

            this.DelCustomerComboBox.DataSource = dataSourceCustomerMain;
            this.DelCustomerComboBox.DisplayMember = "Name";
            this.DelCustomerComboBox.ValueMember = "Value";

            this.DelCustomerComboBox.DropDownStyle = ComboBoxStyle.DropDownList;

            this.DelEqpCustomerComboBox.DataSource = dataSourceCustomerMain;
            this.DelEqpCustomerComboBox.DisplayMember = "Name";
            this.DelEqpCustomerComboBox.ValueMember = "Value";

            this.DelEqpCustomerComboBox.DropDownStyle = ComboBoxStyle.DropDownList;

            this.DelEqpComboBox.DataSource = dataSourceCustomerMain;
            this.DelEqpComboBox.DisplayMember = "Name";
            this.DelEqpComboBox.ValueMember = "Value";

            this.DelEqpComboBox.DropDownStyle = ComboBoxStyle.DropDownList;

            this.changeEqpInfoCustomerComboBox.DataSource = dataSourceCustomerMain;
            this.changeEqpInfoCustomerComboBox.DisplayMember = "Name";
            this.changeEqpInfoCustomerComboBox.ValueMember = "Value";

            this.changeEqpInfoCustomerComboBox.DropDownStyle = ComboBoxStyle.DropDownList;

            this.ICXCustomer.DataSource = dataSourceCustomerMain;
            this.ICXCustomer.DisplayMember = "Name";
            this.ICXCustomer.ValueMember = "Value";

            this.ICXCustomer.DropDownStyle = ComboBoxStyle.DropDownList;

            var dataSourceEquipmentMain = MySQLHandler.getEquipment(InformationCustomerComboBox.Text);
            this.ICXEquipment.DataSource = dataSourceEquipmentMain;
            this.ICXEquipment.DisplayMember = "EquipmentName";
            this.ICXEquipment.ValueMember = "EquipmentID";

            this.ICXEquipment.DropDownStyle = ComboBoxStyle.DropDownList;

            foreach (MySQLHandler.Users ul in userList)
            {
                usersCheckboxList.Items.Add(ul.Name);
            }

            foreach (MySQLHandler.Customer c in dataSourceCustomerMain)
            {
                customerCheckboxList.Items.Add(c.Name);
            }
        }

        private void reloadEqp()
        {
            var dataSourceEquipmentMain = MySQLHandler.getEquipment(InformationCustomerComboBox.Text);
            this.InformationEquipmentComboBox.DataSource = dataSourceEquipmentMain;
            this.InformationEquipmentComboBox.DisplayMember = "EquipmentName";
            this.InformationEquipmentComboBox.ValueMember = "EquipmentID";
            this.InformationEquipmentComboBox.SelectedIndex = -1;

            InformationEquipmentBrowser.Navigate("about:blank");
            HtmlDocument doc2 = InformationEquipmentBrowser.Document;
            doc2.Write(String.Empty);

            var InfoBlob = MySQLHandler.getCustomerInfo(InformationCustomerComboBox.Text);

            byte[] bytes3 = Encoding.Default.GetBytes(InfoBlob);
            InfoBlob = Encoding.UTF8.GetString(bytes3);

            InformationCustomerBrowser.Navigate("about:blank");
            HtmlDocument doc = InformationCustomerBrowser.Document;
            doc.Write(String.Empty);

            InformationCustomerBrowser.DocumentText = InfoBlob;

            var PriceInfoBlob = MySQLHandler.getCustPriceInfo(InformationCustomerComboBox.Text);

            byte[] bytes2 = Encoding.Default.GetBytes(PriceInfoBlob);
            PriceInfoBlob = Encoding.UTF8.GetString(bytes2);

            CustomerPricesBrowser.Navigate("about:blank");
            Font fon = new Font("Palatino Linotype", 12.0f);
            CustomerPricesBrowser.Font = fon;
            HtmlDocument doc3 = CustomerPricesBrowser.Document;
            doc3.Write(String.Empty);

            CustomerPricesBrowser.DocumentText = PriceInfoBlob;

            var CustContactBlob = MySQLHandler.getCustContactInfo(InformationCustomerComboBox.Text);

            byte[] bytes = Encoding.Default.GetBytes(CustContactBlob);
            CustContactBlob = Encoding.UTF8.GetString(bytes);

            ContactPersonsBrowser.Navigate("about:blank");
            HtmlDocument doc4 = ContactPersonsBrowser.Document;
            doc4.Write(String.Empty);

            ContactPersonsBrowser.DocumentText = CustContactBlob;
        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {

            Console.WriteLine(InformationCustomerComboBox.SelectedIndex);
            Console.WriteLine(InformationCustomerComboBox.SelectedText);
            Console.WriteLine(InformationCustomerComboBox.SelectedValue);

            var dataSourceEquipmentMain = MySQLHandler.getEquipment(InformationCustomerComboBox.Text);
            this.InformationEquipmentComboBox.DataSource = dataSourceEquipmentMain;
            this.InformationEquipmentComboBox.DisplayMember = "EquipmentName";
            this.InformationEquipmentComboBox.ValueMember = "EquipmentID";

            this.InformationEquipmentComboBox.DropDownStyle = ComboBoxStyle.DropDownList;

            var dataSourceMaps = MySQLHandler.getEquipment(InformationCustomerComboBox.Text);
            this.InformationEquipmentComboBox.DataSource = dataSourceEquipmentMain;
            this.InformationEquipmentComboBox.DisplayMember = "EquipmentName";
            this.InformationEquipmentComboBox.ValueMember = "EquipmentID";

            this.InformationEquipmentComboBox.DropDownStyle = ComboBoxStyle.DropDownList;

            var InfoBlob = MySQLHandler.getCustomerInfo(InformationCustomerComboBox.Text);

            byte[] bytes = Encoding.Default.GetBytes(InfoBlob);
            InfoBlob = Encoding.UTF8.GetString(bytes);

            InformationCustomerBrowser.Navigate("about:blank");
            HtmlDocument doc = InformationCustomerBrowser.Document;
            doc.Write(String.Empty);

            InformationCustomerBrowser.DocumentText = InfoBlob;

            var PriceInfoBlob = MySQLHandler.getCustPriceInfo(InformationCustomerComboBox.Text);

            byte[] bytes2 = Encoding.Default.GetBytes(PriceInfoBlob);
            PriceInfoBlob = Encoding.UTF8.GetString(bytes2);

            CustomerPricesBrowser.Navigate("about:blank");
            HtmlDocument doc2 = CustomerPricesBrowser.Document;
            doc2.Write(string.Empty);

            CustomerPricesBrowser.DocumentText = PriceInfoBlob;

            var CustContactBlob = MySQLHandler.getCustContactInfo(InformationCustomerComboBox.Text);

            byte[] bytes3 = Encoding.Default.GetBytes(CustContactBlob);
            CustContactBlob = Encoding.UTF8.GetString(bytes3);

            ContactPersonsBrowser.Navigate("about:blank");
            HtmlDocument doc3 = ContactPersonsBrowser.Document;
            doc3.Write(String.Empty);

            ContactPersonsBrowser.DocumentText = CustContactBlob;

            var CustSwapInfo = MySQLHandler.getCustSwapInfo(InformationCustomerComboBox.Text);

            byte[] bytes9 = Encoding.Default.GetBytes(CustSwapInfo);
            CustSwapInfo = Encoding.UTF8.GetString(bytes9);

            SwapAccessBrowser.Navigate("about:blank");
            HtmlDocument doc4 = SwapAccessBrowser.Document;
            doc3.Write(String.Empty);

            SwapAccessBrowser.DocumentText = CustSwapInfo;


            try
            {
                var MapSource = MySQLHandler.getMaps(InformationCustomerComboBox.Text);
                this.SelectMapComboBox.DataSource = MapSource;
                this.SelectMapComboBox.DisplayMember = "Name";
                this.SelectMapComboBox.ValueMember = "Value";

                this.SelectMapComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            }
            catch (Exception)
            {
                // ignored
            }

            try
            {
                var MapSource = MySQLHandler.getMaps(InformationCustomerComboBox.Text);
                this.removeMapSelectMapComboBox.DataSource = MapSource;
                this.removeMapSelectMapComboBox.DisplayMember = "Name";
                this.removeMapSelectMapComboBox.ValueMember = "Value";

                this.removeMapSelectMapComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private void comboBox5_SelectedIndexChanged(object sender, EventArgs e)
        {
            var InfoBlob = MySQLHandler.getEquipmentBlob(InformationEquipmentComboBox.Text, InformationCustomerComboBox.Text);

            InformationEquipmentBrowser.Navigate("about:blank");
            HtmlDocument doc = InformationEquipmentBrowser.Document;
            doc.Write(String.Empty);

            byte[] bytes = Encoding.Default.GetBytes(InfoBlob);
            InfoBlob = Encoding.UTF8.GetString(bytes);

            InformationEquipmentBrowser.DocumentText = InfoBlob;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (CustomerName.Text != "" && AddCustomerTextBox.Text != "")
            {
                MySQLHandler.createCustomer(CustomerName.Text, AddCustomerTextBox.Text);
                reload();
                MessageBox.Show("Kund Skapad.");
                CustomerName.Text = "";
                AddCustomerTextBox.Text = "";
            }
            else { showWarning("Du måste fylla i alla fält"); }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                var tempHolder = AddEQPCustomerTextbox.SelectedIndex;
                if (AddEqpInformationTextBox.Text != "" && AddEQPCustomerTextbox.Text != "" && AddEquipmentTextBox.Text != "")
                {
                    MySQLHandler.createEquipment(AddEquipmentTextBox.Text, AddEQPCustomerTextbox.Text, AddEqpInformationTextBox.Text);
                    reload();
                    AddEQPCustomerTextbox.SelectedIndex = tempHolder;
                    MessageBox.Show("Utrustning Skapad.");
                    AddEquipmentTextBox.Text = "";
                }
                else
                {
                    MessageBox.Show("Fyll i alla fält.");
                }
            }
            catch (Exception err)
            {
                log.Error("Exception thrown from method " + GetCurrentMethod(), err);
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void showWarning(string text)
        {
            MessageBox.Show(text, "Varning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
        }

        /// <summary>
        /// Opens a new window that previews the HTML written
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void previewBtn_Click(object sender, EventArgs e)
        {
            PreviewHTML prev = new PreviewHTML(AddEqpInformationTextBox.Text);
            prev.Show();
        }

        private void PreviewHtml2_Click(object sender, EventArgs e)
        {
            PreviewHTML prev = new PreviewHTML(AddCustomerTextBox.Text);
            prev.Show();
        }

        /// <summary>
        /// Remove Customer selected by admin
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RemoveCustBtn_Click(object sender, EventArgs e)
        {
            try
            {
                var cust = DelCustomerComboBox.Text;
                DialogResult result1 = MessageBox.Show("Säker på att du vill ta bort Kunden? [" + cust + "] ", "Important Question", MessageBoxButtons.YesNo);
                if (result1 == DialogResult.Yes)
                {
                    MySQLHandler.deleteCustomer(cust);
                    reload();
                    MessageBox.Show("Kund borttagen.");
                }
            }
            catch (Exception err)
            {
                log.Error("Exception thrown from method " + GetCurrentMethod(), err);
            }
        }

        /// <summary>
        /// Removes equipment selected by admin
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RemoveEqpBtn_Click(object sender, EventArgs e)
        {
            try
            {
                var eqp = DelEqpComboBox.Text;
                var cust = DelEqpCustomerComboBox.Text;
                DialogResult result1 = MessageBox.Show("Säker på att du vill ta bort Utrustningen? [" + eqp + "] från kund [" + cust + "] ?", "Important Question", MessageBoxButtons.YesNo);
                if (result1 == DialogResult.Yes)
                {
                    MySQLHandler.deleteEquipment(eqp, cust);
                    reload();
                    reloadEqp();
                    MessageBox.Show("Utrustning borttagen.");
                    DelEqpComboBox.SelectedIndex = -1;
                }
            }
            catch (Exception err)
            {
                log.Error("Exception thrown from method " + GetCurrentMethod(), err);
            }
        }

        /// <summary>
        /// Sends bug report to DB
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BugReportBtn_Click(object sender, EventArgs e)
        {
            if (BugReportRichText.Text != "")
            {
                MySQLHandler.AddBugReport(loggedUser, BugReportRichText.Text);
                BugReportRichText.Text = "";
                MessageBox.Show("Rapporten är skickad :)");
            }
            else
            {
                showWarning("Du måste fylla i alla fält");
            }
        }


        private void comboBox7_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                var dataSourceEquipmentMain = MySQLHandler.getEquipment(DelEqpCustomerComboBox.Text);
                this.DelEqpComboBox.DataSource = dataSourceEquipmentMain;
                this.DelEqpComboBox.DisplayMember = "EquipmentName";
                this.DelEqpComboBox.ValueMember = "EquipmentID";

                this.DelEqpComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            }
            catch (Exception err)
            {
                log.Error("Exception thrown from method " + GetCurrentMethod(), err);
            }
        }

        /// <summary>
        /// Create user Button, performs some checks on the input and then creates the User
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CreateUserBtn_Click(object sender, EventArgs e)
        {
            try
            {
                if (UserNameTextBox.Text == "" || CreateLevelBox.Text == "")
                {
                    showWarning("Alla fält måste bli korrekt ifyllda");
                }
                else if (CreateLevelBox.Text != "1" && CreateLevelBox.Text != "2")
                {
                    showWarning("Användarnivån måste vara 1 eller 2");
                }

                else
                {
                    if (MySQLHandler.CreateUser(UserNameTextBox.Text, CreateLevelBox.Text) == true)
                    {
                        MessageBox.Show("Användaren [" + UserNameTextBox.Text + "] är skapad");
                        UserNameTextBox.Text = "";
                        CreateLevelBox.Text = "";
                        reload();
                    }
                    else
                    {
                        showWarning("Något fel har inträffat, användaren kanske redan finns?");
                    }


                }
            }
            catch (Exception err)
            {
                log.Error("Exception thrown from method " + GetCurrentMethod(), err);
            }
        }

        /// <summary>
        /// Logout button, resets the AuthLevel and closes the MainView and opens the LoginView
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LogoutBtn_Click(object sender, EventArgs e)
        {
            LogoutPrompt lp = new LogoutPrompt(unloggedList.Count, loggedUser); // change the parameter
            lp.Show();
        }

        /// <summary>
        /// Select customer map Combobox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectMapBtn_Click_1(object sender, EventArgs e)
        {
            Stream myStream = null;
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = "C:\\";
            openFileDialog1.Filter = "Image files (*.png) | *.png";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if ((myStream = openFileDialog1.OpenFile()) != null)
                    {
                        using (myStream)
                        {
                            SelectedMapTextBox.Text = openFileDialog1.FileName;
                            FileNameAndPath = openFileDialog1.FileName;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Fel: kunde inte läsa fil från disk " + ex.Message);
                    log.Error("Exception thrown from method " + GetCurrentMethod(), ex);
                }
            }
        }

        /// <summary>
        /// Add the map button, gets path and places the contents in DB in Base64 Format
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddMapBtn_Click_1(object sender, EventArgs e)
        {
            try
            {
                var finalPath = Application.StartupPath + @"\Maps";
                Guid id = Guid.NewGuid();
                var newPath = finalPath + @"\" + id.ToString() + ".png";
                File.Copy(FileNameAndPath, newPath);
                var DBPath = finalPath + @"\" + id.ToString() + ".png";

                Image img;
                img = Image.FromFile(DBPath);

                MySQLHandler.AddCustomerMapBase64(newPath, AddMapCustomerComboBox.Text, ImageBase64Logic.ImageToBase64(img, ImageFormat.Png), MapNameTextBox.Text);

                AddMapCustomerComboBox.SelectedIndex = -1;
                SelectedMapTextBox.Text = "";
                MessageBox.Show("Karta tillagd.");
            }
            catch (Exception err)
            {
                log.Error("Exception thrown from method " + GetCurrentMethod(), err);
            }
        }

        private void ICXCreateBtn_Click(object sender, EventArgs e)
        {
            try
            {
                if (ICXInput.Text.Length == 4)
                {
                    MySQLHandler.AddICXString(ICXEquipment.Text, ICXCustomer.Text, ICXInput.Text);
                    MessageBox.Show("ICX-Sträng tillagd.");
                    ICXInput.Text = "";

                }
                else
                {
                    showWarning("ICX Strängen måste vara 4 siffror");
                }
            }
            catch (Exception err)
            {
                log.Error("Exception thrown from method " + GetCurrentMethod(), err);
            }
        }

        private void PreviewContacts_Click(object sender, EventArgs e)
        {
            PreviewHTML prev = new PreviewHTML(CustContactsTextbox.Text);
            prev.Show();
        }

        private void PreviewPrices_Click(object sender, EventArgs e)
        {
            PreviewHTML prev = new PreviewHTML(CustPriceTextBox.Text);
            prev.Show();
        }

        private void AddPricesBtn_Click(object sender, EventArgs e)
        {
            if (AddCustPriceComboBox.Text != "" && CustPriceTextBox.Text != "")
            {
                MySQLHandler.AddPriceInformation(AddCustPriceComboBox.Text, CustPriceTextBox.Text);
                AddCustPriceComboBox.Text = "";
                CustPriceTextBox.Text = "";
                MessageBox.Show("Kund pris information ändrad");
                reload();
            }
            else
            {
                showWarning("Du måste fylla i alla fält");
            }
        }

        private void AddContactsBtn_Click(object sender, EventArgs e)
        {
            if (AddCustomerContactInformationComboBox.Text != "" && CustContactsTextbox.Text != "")
            {
                MySQLHandler.AddCustContactInfo(AddCustomerContactInformationComboBox.Text, CustContactsTextbox.Text);
                AddCustomerContactInformationComboBox.Text = "";
                CustContactsTextbox.Text = "";
                MessageBox.Show("Kund kontakt information ändrad");
                reload();
            }
            else
            {
                showWarning("Du måste fylla i alla fält");
            }
        }

        private void MainView_FormClosing(object sender, FormClosingEventArgs e)
        {
            stop = true;
        }

        private void ShowMapBtn_Click(object sender, EventArgs e)
        {
            var temp = SelectMapComboBox.Text;

            MapForm mapView = new MapForm(MySQLHandler.getSpecificMap(temp));
            mapView.Dock = DockStyle.Fill;

            mapView.Show();

        }

        private void AddCustomerContactInformationComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                var info = MySQLHandler.ContactPersonsOldInfo(AddCustomerContactInformationComboBox.Text);

                byte[] bytes = Encoding.Default.GetBytes(info);
                info = Encoding.UTF8.GetString(bytes);

                CustContactsTextbox.Text = info;
            }
            catch (Exception err)
            {
                log.Error("Error in AddCustomerContactInformationComboBox_SelectedIndexChanged() " + err);
            }
        }

        private void AddCustPriceComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                var info = MySQLHandler.PriceOldInformation(AddCustPriceComboBox.Text);

                byte[] bytes = Encoding.Default.GetBytes(info);
                info = Encoding.UTF8.GetString(bytes);

                CustPriceTextBox.Text = info;
            }
            catch (Exception err)
            {
                log.Error("Error in AddCustPriceComboBox_SelectedIndexChanged() " + err);
            }
        }

        private void ChangeGeneralInfoBtn_Click(object sender, EventArgs e)
        {
            if (changeGeneralCustInfoComboBox.Text != "" && ChangeGeneralCustInfoTextBox.Text != "")
            {
                MySQLHandler.AddGeneralInfo(changeGeneralCustInfoComboBox.Text, ChangeGeneralCustInfoTextBox.Text);
                ChangeGeneralCustInfoTextBox.Text = "";
                CustContactsTextbox.Text = "";
                MessageBox.Show("Generell information updaterad");
                reload();
            }
            else
            {
                showWarning("Du måste fylla i alla fält");
            }
        }

        private void changeGeneralCustInfoComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            var info = MySQLHandler.GeneralInfoOld(changeGeneralCustInfoComboBox.Text);

            byte[] bytes = Encoding.Default.GetBytes(info);
            info = Encoding.UTF8.GetString(bytes);

            ChangeGeneralCustInfoTextBox.Text = info;
        }

        private void ChangeEqpInfoBtn_Click(object sender, EventArgs e)
        {
            MySQLHandler.ChangeEqpInfo(changeEqpInfoCustomerComboBox.Text, changeEqpInfoEquipmentComboBox.Text, changeEqpInfoTextBox.Text);

            MessageBox.Show("Utrustningsinformation updaterad");
            //reload();
        }

        private void changeEqpInfoCustomerComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                var dataSourceEquipmentMain = MySQLHandler.getEquipment(changeEqpInfoCustomerComboBox.Text);
                this.changeEqpInfoEquipmentComboBox.DataSource = dataSourceEquipmentMain;
                this.changeEqpInfoEquipmentComboBox.DisplayMember = "EquipmentName";
                this.changeEqpInfoEquipmentComboBox.ValueMember = "EquipmentID";

                this.changeEqpInfoEquipmentComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            }
            catch (Exception err)
            {
                log.Error("Exception thrown from method " + GetCurrentMethod(), err);
            }
        }

        private void changeEqpInfoEquipmentComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            var info = MySQLHandler.EqpInfoOld(changeEqpInfoCustomerComboBox.Text, changeEqpInfoEquipmentComboBox.Text);

            byte[] bytes = Encoding.Default.GetBytes(info);
            info = Encoding.UTF8.GetString(bytes);

            changeEqpInfoTextBox.Text = info;
        }

        private void ICXCustomer_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                var dataSourceEquipmentMain = MySQLHandler.getEquipment(ICXCustomer.Text);
                this.ICXEquipment.DataSource = dataSourceEquipmentMain;
                this.ICXEquipment.DisplayMember = "EquipmentName";
                this.ICXEquipment.ValueMember = "EquipmentID";

                this.ICXEquipment.DropDownStyle = ComboBoxStyle.DropDownList;
            }
            catch (Exception err)
            {
                log.Error("Exception thrown from method " + GetCurrentMethod(), err);
            }
        }

        private void PreviewHTMLChangeCustInfo_Click(object sender, EventArgs e)
        {
            PreviewHTML prev = new PreviewHTML(ChangeGeneralCustInfoTextBox.Text);
            prev.Show();
        }

        private void PreviewHTMLChangeEqp_Click(object sender, EventArgs e)
        {
            PreviewHTML prev = new PreviewHTML(changeEqpInfoTextBox.Text);
            prev.Show();
        }

        private void changeCustNameBtn_Click(object sender, EventArgs e)
        {
            var temp = changeCustNameNewTextBox.Text;
            MySQLHandler.ChangeCustName(changeCustNameComboBox.Text, temp);

            MessageBox.Show("Kundnamn ändrat!");
            reload();

            changeCustNameNewTextBox.Text = "";

        }

        private void removeMapBtn_Click(object sender, EventArgs e)
        {
            MySQLHandler.deleteMap(removeCustMapComboBox.Text, removeMapSelectMapComboBox.Text);
            reload();
            MessageBox.Show("Karta borttagen!");
        }

        private void AddManualBtn_Click(object sender, EventArgs e)
        {
            MySQLHandler.AddManual(manualTextBox.Text);
            reloadManual();
            MessageBox.Show("Manual ändrad!");
        }

        private void previewManualBtn_Click(object sender, EventArgs e)
        {
            PreviewHTML prev = new PreviewHTML(manualTextBox.Text);
            prev.Show();
        }

        private void reloadManual()
        {
            var InfoBlob = MySQLHandler.getManual();

            byte[] bytes3 = Encoding.Default.GetBytes(InfoBlob);
            InfoBlob = Encoding.UTF8.GetString(bytes3);

            ManualBrowser.Navigate("about:blank");
            ManualBrowser.Document.Encoding = "utf-8";
            HtmlDocument doc2 = ManualBrowser.Document;
            doc2.Write(String.Empty);
            InformationCustomerBrowser.Document.Encoding = "utf-8";
            ManualBrowser.DocumentText = InfoBlob;
        }

        private void swapaccessCustomerComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            //  SwapAccessBrowser
            var info = MySQLHandler.getCustSwapInfo(swapaccessCustomerComboBox.Text);

            byte[] bytes = Encoding.Default.GetBytes(info);
            info = Encoding.UTF8.GetString(bytes);

            SwapInfoTextBox.Text = info;
        }

        private void previewHTMLSwap_Click(object sender, EventArgs e)
        {
            PreviewHTML prev = new PreviewHTML(SwapInfoTextBox.Text);
            prev.Show();
        }

        private void changeSwapInfoBtn_Click(object sender, EventArgs e)
        {
            if (swapaccessCustomerComboBox.Text != "" && SwapInfoTextBox.Text != "")
            {
                MySQLHandler.UpdateSwapInfo(swapaccessCustomerComboBox.Text, SwapInfoTextBox.Text);
                MessageBox.Show("SwapAccess information ändrad");
            }
            else
            {
                showWarning("Du måste fylla i alla fält");
            }
        }

        private void bugListComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            var temp = MySQLHandler.getBugInfo(bugListComboBox.Text);

            byte[] bytes = Encoding.Default.GetBytes(temp[0].Desc);
            var tempDesc = Encoding.UTF8.GetString(bytes);

            bugReport.Text = tempDesc;
            bugUser.Text = temp[0].UserName;
            bugTime.Text = temp[0].DateTime;
        }

        private void removeRepportBtn_Click(object sender, EventArgs e)
        {
            try
            {
                var cust = DelCustomerComboBox.Text;
                DialogResult result1 = MessageBox.Show("Säker på att du vill ta bort rapporten?", "Important Question", MessageBoxButtons.YesNo);
                if (result1 == DialogResult.Yes)
                {
                    MySQLHandler.deleteBugReport(bugListComboBox.Text);
                    reload();
                    bugListComboBox.SelectedIndex = -1;
                    MessageBox.Show("Rapport borttagen.");
                }
            }
            catch (Exception err)
            {
                log.Error("Exception thrown from method " + GetCurrentMethod(), err);
            }
        }

        private void saveAnswerBtn_Click(object sender, EventArgs e)
        {
            QAMySQLHandler.updateAnswer(QListBox.SelectedItem.ToString(), ATextBox.Text, loggedUser);
            MessageBox.Show("Svaret är uppdaterat!");
            updateQuestionListBox();
        }

        private void updateQuestionListBox()
        {
            try
            {
                List<string> QAitems = new List<string>();
                QAitems = QAMySQLHandler.getQuestionTitle();

                QListBox.DataSource = QAitems;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in updateQuestionListBox() " + e);
            }
        }

        private void AskQBtn_Click(object sender, EventArgs e)
        {
            if (QlabelTextBox.Text != "" && QTextBox.Text != "")
            {
                QAMySQLHandler.AskQuestion(QTextBox.Text, loggedUser, QlabelTextBox.Text, null);
                MessageBox.Show("Frågan är ställd.");
                QlabelTextBox.Text = "";
                QTextBox.Text = "";
                updateQuestionListBox();
            }
        }

        private void QListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedItem = QListBox.SelectedItem.ToString();

            if (selectedItem.Contains("-- NY --"))
            {
                selectedItem = selectedItem.Remove(0, 9);
            }
            Console.WriteLine(selectedItem);
            var QATemp = QAMySQLHandler.getQuestion(selectedItem);

            byte[] bytes = Encoding.Default.GetBytes(QATemp);
            QATemp = Encoding.UTF8.GetString(bytes);

            QuestionTextBox.Text = QATemp;
            QuestionUser.Text = QAMySQLHandler.getQuestionUser(selectedItem);
            dateTimeQuestion.Text = QAMySQLHandler.getQuestionTime(selectedItem);
            var ATTemp = QAMySQLHandler.getQuestionAnswer(selectedItem);

            byte[] utf8Bytes = Encoding.UTF8.GetBytes(ATTemp);
            string correctedString = Encoding.UTF8.GetString(utf8Bytes);


            var AnsweredBy = QAMySQLHandler.getAnswerUser(selectedItem);

            byte[] utf8Bytes2 = Encoding.UTF8.GetBytes(AnsweredBy);
            string FixedAnsweredBy = Encoding.UTF8.GetString(utf8Bytes2);

            AnsweredByTextBox.Text = FixedAnsweredBy;

            ATextBox.Text = correctedString;
        }

        private void label61_Click(object sender, EventArgs e)
        {

        }

        private void logThisBtn_Click(object sender, EventArgs e)
        {
            bool complete = true;

            if (ErrorTextBox.Text == "")
            {
                MessageBox.Show("Textrutan 'Fel' är inte ifylld.");
                complete = false;
            }
            else if (CommentTextBox.Text == "")
            {
                MessageBox.Show("Textrutan 'Kommentar' är inte ifylld.");
                complete = false;
            }
            else if (ActionComboBox.Text == "")
            {
                MessageBox.Show("Listan 'Åtgärd' är inte ifylld.");
                complete = false;
            }
            else if (RegNrText.Text != "")
            {
                if (SwapCheckBox.Checked == false)
                {
                    MessageBox.Show("Registrerings nummer är ifyllt men Swap är inte ikryssad? Kryssa i eller ta bort reg nr.");
                    complete = false;
                }
            }
            else if (DateTextBox.Text == "" || LogDateTextBox.Text == "" || CustomerTextBox.Text == "" || EqpTextBox.Text == "")
            {
                MessageBox.Show("Vänligen välj ett samtal att logga.");
                complete = false;
            }
            if (complete == true)
            {

                string currGUID = string.Empty;

                try
                {
                    if (activeLog == 2)
                    {
                        updateLogged();
                        currGUID = LoggedComboBox.SelectedValue.ToString();
                    }
                    else
                    {
                        currGUID = NotYetLoggedComboBox.SelectedValue.ToString();
                    }
                }
                catch (Exception err)
                {
                    MessageBox.Show("Du kan inte logga utan att ha ett samtal valt.");
                    log.Debug(DateTime.Now.ToShortTimeString() + " : exception currGUID & logThisBtn_Click " + err);
                    currGUID = string.Empty;
                }

                try
                {
                    if (currGUID != "")
                    {
                        string dateNow = DateTime.Now.ToString();
                        LOGMySQLHandler.logThis(ActionComboBox.SelectedValue.ToString(), loggedUser, ErrorTextBox.Text, CommentTextBox.Text, SwapCheckBox.Checked, RegNrText.Text, CustomerTextBox.Text, EqpTextBox.Text, LogDateTextBox.Text, currGUID);
                        removeLog();
                        ActionComboBox.SelectedIndex = -1;
                        MessageBox.Show("Ärendet är loggat");
                        if (activeLog == 1)
                        {
                            NotYetLoggedComboBox.SelectedIndex = -1;
                        }

                        if (activeLog == 2)
                        {
                            ErrorTextBox.Text = "";
                            CommentTextBox.Text = "";
                        }
                    }
                }
                catch (Exception err)
                {
                    log.Debug(DateTime.Now.ToShortTimeString() + " : exception logThis " + err);
                }
            }
        }

        private void updateLogged()
        {
            try
            {
                var t = LoggedComboBox.SelectedValue;
                for (int i = 0; i <= loggedList.Count; i++)
                {
                    if ((string)t == loggedList[i].GUID)
                    {
                        loggedList[i].Error = ErrorTextBox.Text;
                        loggedList[i].Comment = CommentTextBox.Text;
                        loggedList[i].Swap = SwapCheckBox.Checked;
                        loggedList[i].RegNr = RegNrText.Text;
                        loggedList[i].Action = ActionComboBox.SelectedIndex;
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                log.Error("Error in updateLogged() " + e);
            }
        }

        private void removeLog()
        {
            try
            {
                if (activeLog == 1)
                {
                    var j = NotYetLoggedComboBox.SelectedValue;
                    for (int i = 0; i <= unloggedList.Count; i++)
                    {
                        if (unloggedList[i].GUID == (string)j)
                        {
                            copyToLoggedList(i);
                            unloggedList.Remove(unloggedList[i]);
                            reloadLists();
                            nullifyLogInfo();
                            break;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                log.Error("Error in removeLog() " + e);
            }
        }

        private void copyToLoggedList(int index)
        {
            try
            {
                Logged ll = new Logged();

                // OLD values
                ll.Customer = unloggedList[index].Customer;
                ll.Date = unloggedList[index].Date;
                ll.Day = unloggedList[index].Day;
                ll.Equipment = unloggedList[index].Equipment;
                ll.Time = unloggedList[index].Time;
                ll.GUID = unloggedList[index].GUID;
                ll.DispName = unloggedList[index].DispName;

                // New copied values
                ll.Action = ActionComboBox.SelectedIndex;
                ll.Swap = SwapCheckBox.Checked;
                ll.RegNr = RegNrText.Text;
                ll.Error = ErrorTextBox.Text;
                ll.Comment = CommentTextBox.Text;

                loggedList.Add(ll);

                reloadLoggedLists();
            }
            catch (Exception e)
            {
                log.Error("Error in copyToLoggedList() " + e);
            }
        }

        private void nullifyLogInfo()
        {
            try
            {
                DateTextBox.Text = "";
                LogDateTextBox.Text = "";
                CustomerTextBox.Text = "";
                EqpTextBox.Text = "";
                ErrorTextBox.Text = "";
                CommentTextBox.Text = "";
                RegNrText.Text = "";
                SwapCheckBox.Checked = false;
            }
            catch (Exception e)
            {
                log.Error("Error in nullifyLogInfo() " + e);
            }
        }

        public class Logged
        {
            public string Day { get; set; }
            public string Date { get; set; }
            public string Time { get; set; }
            public string Customer { get; set; }
            public string Equipment { get; set; }
            public bool Swap { get; set; }
            public string RegNr { get; set; }
            public int Action { get; set; }
            public string Error { get; set; }
            public string Comment { get; set; }
            public string GUID { get; set; }
            public string DispName { get; set; }
        }

        public class Unlogged
        {
            public string Day { get; set; }
            public string Date { get; set; }
            public string Time { get; set; }
            public string Customer { get; set; }
            public string Equipment { get; set; }
            public string DispName { get; set; }
            public string GUID { get; set; }
        }

        private void addActionBtn_Click(object sender, EventArgs e)
        {
            DialogResult result1 = MessageBox.Show("Vill du lägga till [" + addActionTextBox.Text + "] som ny åtgärd?", "Important Question", MessageBoxButtons.YesNo);
            if (result1 == DialogResult.Yes)
            {
                ActionMySQLHandler.addAction(addActionTextBox.Text);
                MessageBox.Show("Åtgärd tillagd.");
            }
        }

        private string DayTranslator(string day)
        {
            if (day == "Monday")
            {
                return "Måndag";
            }
            else if (day == "Tuesday")
            {
                return "Tisdag";
            }
            else if (day == "Wednesday")
            {
                return "Onsdag";
            }
            else if (day == "Thursday")
            {
                return "Torsdag";
            }
            else if (day == "Friday")
            {
                return "Fredag";
            }
            else if (day == "Saturday")
            {
                return "Lördag";
            }
            else if (day == "Sunday")
            {
                return "Söndag";
            }
            return "error";
        }

        private void summationCalendar_DateSelected(object sender, DateRangeEventArgs e)
        {
            try
            {
                var val = eventComboBox.Text;
                if (val == "Summering av pass")
                {
                    loggedCallsEventInformation.Text = "";

                    dataGridViewLogBook.Visible = false;
                    dataGridViewPass.Visible = true;

                    dataGridViewPass.DataSource = EventsMySQLHandler.getSummation(eventCalendar.SelectionStart.Date);
                    dataGridViewPass.AllowUserToResizeColumns = true;
                    dataGridViewPass.AllowUserToOrderColumns = true;

                    dataGridViewPass.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                    dataGridViewPass.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                    dataGridViewPass.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                }
                else
                {
                    dataGridViewPass.Visible = false;
                    dataGridViewLogBook.Visible = true;

                    DataTable dt = EventsMySQLHandler.getLogBookDataAdapter(eventCalendar.SelectionStart.Date);

                    dataGridViewLogBook.DataSource = dt;
                    dataGridViewLogBook.AllowUserToResizeColumns = true;
                    dataGridViewLogBook.AllowUserToOrderColumns = true;

                    dateEventInformation.Text = eventCalendar.SelectionStart.Date.ToShortDateString();
                    loggedCallsEventInformation.Text = dt.Rows.Count.ToString();

                    dataGridViewLogBook.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                    dataGridViewLogBook.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                    dataGridViewLogBook.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                    dataGridViewLogBook.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                    dataGridViewLogBook.Columns[4].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                    dataGridViewLogBook.Columns[5].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    dataGridViewLogBook.Columns[6].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                    dataGridViewLogBook.Columns[7].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    dataGridViewLogBook.Columns[8].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                }
            }
            catch (Exception err)
            {

            }
        }

        private void eventComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (eventComboBox.Text == "Summering av pass")
                {
                    loggedCallsEventInformation.Text = "";

                    dataGridViewLogBook.Visible = false;
                    dataGridViewPass.Visible = true;

                    dataGridViewPass.DataSource = EventsMySQLHandler.getSummation(eventCalendar.SelectionStart.Date);
                    dataGridViewPass.AllowUserToResizeColumns = true;
                    dataGridViewPass.AllowUserToOrderColumns = true;

                    dateEventInformation.Text = eventCalendar.SelectionStart.Date.ToShortDateString();

                    dataGridViewPass.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                    dataGridViewPass.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                    dataGridViewPass.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                }
                else
                {
                    dataGridViewPass.Visible = false;
                    dataGridViewLogBook.Visible = true;

                    DataTable dt = EventsMySQLHandler.getLogBookDataAdapter(eventCalendar.SelectionStart.Date);

                    dataGridViewLogBook.DataSource = dt;
                    dataGridViewLogBook.AllowUserToResizeColumns = true;
                    dataGridViewLogBook.AllowUserToOrderColumns = true;

                    dateEventInformation.Text = eventCalendar.SelectionStart.Date.ToShortDateString();
                    loggedCallsEventInformation.Text = dt.Rows.Count.ToString();

                    dataGridViewLogBook.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                    dataGridViewLogBook.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                    dataGridViewLogBook.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                    dataGridViewLogBook.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                    dataGridViewLogBook.Columns[4].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                    dataGridViewLogBook.Columns[5].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    dataGridViewLogBook.Columns[6].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                    dataGridViewLogBook.Columns[7].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    dataGridViewLogBook.Columns[8].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                }
            }
            catch (Exception err)
            {

            }
        }


        private void NotYetLoggedComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (activeLog == 2)
                {
                    activeLog = 1;
                    ActionComboBox.SelectedIndex = -1;
                    Console.WriteLine(NotYetLoggedComboBox.SelectedValue);

                    CommentTextBox.Text = "";
                    ErrorTextBox.Text = "";

                    var i = NotYetLoggedComboBox.SelectedIndex;
                    if (i >= 0)
                    {
                        changeValue(i);
                    }
                }
                else
                {
                    activeLog = 1;
                    ActionComboBox.SelectedIndex = -1;
                    Console.WriteLine(NotYetLoggedComboBox.SelectedValue);

                    var i = NotYetLoggedComboBox.SelectedIndex;
                    if (i >= 0)
                    {
                        changeValue(i);
                    }
                }
            }
            catch (Exception err)
            {
                log.Error("Error in NotYetLoggedComboBox_SelectedIndexChanged() " + err);
            }
        }

        private void emptyInfo()
        {
            try
            {
                SwapCheckBox.Checked = false;
                RegNrText.Text = "";
                ErrorTextBox.Text = "";
                CommentTextBox.Text = "";
                ActionComboBox.SelectedIndex = -1;
            }
            catch (Exception e)
            {
                log.Error("Error in emptyInfo() " + e);
            }
        }

        private void changeValue(int i)
        {
            try
            {
                ActionComboBox.SelectedIndex = -1;
                DateTextBox.Text = DayTranslator(unloggedList[i].Day);
                LogDateTextBox.Text = unloggedList[i].Time;
                CustomerTextBox.Text = unloggedList[i].Customer;
                EqpTextBox.Text = unloggedList[i].Equipment;

                NotYetLoggedComboBox.SelectedIndex = i;
                //     emptyInfo();
            }
            catch (Exception e)
            {
                log.Error("Error in changeValue() " + e);
            }
        }

        private void LoggedComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                var t = LoggedComboBox.SelectedValue;
                var ty = LoggedComboBox.SelectedText.ToString();
                var tyz = LoggedComboBox.SelectedItem;

                //     var test = Convert.ToInt32(t);

                activeLog = 2;
                if (t != null)
                {
                    for (int i = 0; i <= loggedList.Count; i++)
                    {
                        if (loggedList[i].GUID == (string)t)
                        {
                            DateTextBox.Text = loggedList[i].Date;
                            DateTextBox.Text = DayTranslator(loggedList[i].Day);
                            LogDateTextBox.Text = loggedList[i].Time;
                            CustomerTextBox.Text = loggedList[i].Customer;
                            EqpTextBox.Text = loggedList[i].Equipment;
                            SwapCheckBox.Checked = loggedList[i].Swap;
                            RegNrText.Text = loggedList[i].RegNr;
                            ActionComboBox.SelectedIndex = loggedList[i].Action;
                            ErrorTextBox.Text = loggedList[i].Error;
                            CommentTextBox.Text = loggedList[i].Comment;

                            LoggedComboBox.SelectedText = loggedList[i].DispName;
                            LoggedComboBox.SelectedValue = loggedList[i].GUID;
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("Error in LoggedComboBox_SelectedIndexChanged() " + ex);
            }
        }

        private void dataGridViewLogBook_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int index = e.RowIndex;
            if (!(index < 0))
            {
                DataGridViewRow row = dataGridViewLogBook.Rows[index];
                ViewLogbookEntry view = new ViewLogbookEntry(row.Cells["Fel"].Value.ToString(), row.Cells["Kommentar"].Value.ToString());
                view.Show();
            }
        }

        private void missedCallsBtn_Click(object sender, EventArgs e)
        {
            missedCallsDataGridView.DataSource = LOGMySQLHandler.getMissedCallList();
            missedCallsDataGridView.AutoResizeColumns();
            missedCallsDataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            missedCallsDataGridView.AllowUserToResizeColumns = true;
            missedCallsDataGridView.AllowUserToOrderColumns = true;
        }

        private void dataGridViewPass_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int index = e.RowIndex;
            if (!(index < 0))
            {
                DataGridViewRow row = dataGridViewPass.Rows[index];
                viewCommentEntry view = new viewCommentEntry(row.Cells["Kommentar"].Value.ToString());
                view.Show();
            }
        }

        private void ResetPassBtn_Click(object sender, EventArgs e)
        {
            if (userComboBox.Text == "Dev")
            {
                for (int i = 0; i < 25; i++)
                {
                    Easter east = new Easter();
                    east.Show();
                }
            }
            else
            {
                MySQLHandler.resetPassword(userComboBox.Text);
                MessageBox.Show("Lösenordet är återställt.");
            }
        }

        private void updateNewsBtn_Click(object sender, EventArgs e)
        {
            NewsMySQLHandler.updateNews(newsTextBox.Text);
            NewsMySQLHandler.resetNewsRead();
            reload();
            MessageBox.Show("Nyheterna uppdaterade");
        }

        private void PreviewNewsHTML_Click(object sender, EventArgs e)
        {
            PreviewHTML prev = new PreviewHTML(newsTextBox.Text);
            prev.Show();
        }

        private void ListManuals()
        {
            try
            {
                var manPath = XMLReader.getManualPath();
                string[] filePaths = Directory.GetFiles(manPath);
                List<string> testList = new List<string>();

                for (int i = 0; i < filePaths.Length; i++)
                {
                    string temp = filePaths[i].Remove(0, 19);
                    if (temp.EndsWith(".doc"))
                    {
                        temp = temp.Remove(temp.Length - 4);
                    }
                    else
                    {
                        temp = temp.Remove(temp.Length - 5);
                    }
                    testList.Add(temp);
                }
                ManualsListBox.DataSource = testList;
            }
            catch (Exception e)
            {
                log.Error("Error in ListManuals() " + e);
            }
        }

        private static void OpenMicrosoftWord(string file)
        {
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = "WINWORD.EXE";
                startInfo.Arguments = file;
                Process.Start(startInfo);
            }
            catch (Exception e)
            {
                log.Error("Error in OpenMicrosoftWord() " + e);
            }
        }

        private void OpenManualBtn_Click(object sender, EventArgs e)
        {
            try
            {
                var path = XMLReader.getManualPath();
                Console.WriteLine(ManualsListBox.SelectedItem);
                string[] searchFiles = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
                foreach (string file in searchFiles)
                {
                    if (file.Contains(ManualsListBox.SelectedItem.ToString()))
                    {
                        OpenMicrosoftWord(file);
                    }
                }
            }
            catch (Exception err)
            {
                log.Error("Error in OpenMicrosoftWord() " + err);
            }
        }

        private void ExportDB_Click(object sender, EventArgs e)
        {
            string path = @"C:\Swarco\0624\Export\";
            try
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
            }
            catch (Exception)
            {

            }

            MySQLHandler.ExportDB();
            MessageBox.Show("Databasen är exporterad");
        }

        private void userExportButton_Click(object sender, EventArgs e)
        {
            var startDate = fromCalendar.SelectionStart.Date.ToString();
            var endDate = toCalendar.SelectionStart.Date.ToString();

            string path = @"C:\Swarco\0624\LogBookExport\User";
            try
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
            }
            catch (Exception)
            {

            }

            if (startDate == endDate)
            {
                MessageBox.Show("Från datum och Till datum kan ej vara samma");
            }
            else if (toCalendar.SelectionStart < fromCalendar.SelectionStart.Date)
            {
                MessageBox.Show("Från datum kan ej vara större än Till datum");
            }
            else
            {
                ExportMySQL.getCSVFromUser(userExportComboBox.Text, startDate, endDate, filenameUserFilter.Text);
            }
        }


        private void exportBetweenDatesBtn_Click(object sender, EventArgs e)
        {
            var startDate = startDateDate.SelectionStart.Date.ToString();
            var endDate = endDateDate.SelectionStart.Date.ToString();

            string path = @"C:\Swarco\0624\LogBookExport\Date";
            try
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
            }
            catch (Exception)
            {

            }

            if (startDate == endDate)
            {
                MessageBox.Show("Från datum och Till datum kan ej vara samma");
            }
            else if (endDateDate.SelectionStart.Date < startDateDate.SelectionStart.Date)
            {
                MessageBox.Show("Från datum kan ej vara större än Till datum");
            }
            else
            {
                ExportMySQL.getCSVBetweenDates(startDate, endDate, filenameDatefilter.Text);
            }
        }

        private void customerCheckboxList_SelectedIndexChanged(object sender, EventArgs e)
        {
            singleCustCombobox.Text = "";
            equipmentCheckboxList.Items.Clear();
            custList.Clear();
        }

        private void singleCustCombobox_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedCust = singleCustCombobox.Text;
            Console.WriteLine(selectedCust);

            foreach (int i in customerCheckboxList.CheckedIndices) // clear multiple customers when single customer is selected
            {
                customerCheckboxList.SetItemCheckState(i, CheckState.Unchecked);
            }

            var dataSourceEquipmentMain = ExportMySQL.getEquipmentExport(singleCustCombobox.Text);

            equipmentCheckboxList.Items.Clear(); // clear to prevent stacking

            foreach (MySQLHandler.Equipment c in dataSourceEquipmentMain)
            {
                equipmentCheckboxList.Items.Add(c.EquipmentName); // adds all equipments to the checkedlistbox
            }
        }

        private void equipmentCheckboxList_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void filterExportBtn_Click(object sender, EventArgs e)
        {
            eqpList.Clear();
            custList.Clear();
            usrList.Clear();
            acList.Clear();

            var startDate = fromFilterCalendar.SelectionStart.Date.ToString();
            var endDate = toFilterCalendar.SelectionStart.Date.ToString();

            for (int i = 0; i < customerCheckboxList.Items.Count; i++)
            {

                if (customerCheckboxList.GetItemChecked(i))
                {
                    string str = (string)customerCheckboxList.Items[i];
                    custList.Add(str);
                }
            }

            for (int i = 0; i < equipmentCheckboxList.Items.Count; i++)
            {
                if (equipmentCheckboxList.GetItemChecked(i))
                {
                    string str = (string)equipmentCheckboxList.Items[i];
                    eqpList.Add(str);
                }
            }

            for (int i = 0; i < actionCheckListbox.Items.Count; i++)
            {

                if (actionCheckListbox.GetItemChecked(i))
                {
                    string str = (string)actionCheckListbox.Items[i];
                    acList.Add(str);
                }
            }

            for (int i = 0; i < usersCheckboxList.Items.Count; i++)
            {

                if (usersCheckboxList.GetItemChecked(i))
                {
                    string str = (string)usersCheckboxList.Items[i];
                    usrList.Add(str);
                }
            }


            if (filterFileNameTextbox.Text != "" && acList.Count != 0)
            {
                if (startDate == endDate)
                {
                    MessageBox.Show("Från datum och Till datum kan ej vara samma");
                }
                else if (toFilterCalendar.SelectionStart < fromFilterCalendar.SelectionStart.Date)
                {
                    MessageBox.Show("Från datum kan ej vara större än Till datum");
                }
                else
                {
                    ExportMySQL.exportFilterToCSV(singleCustCombobox.Text, custList, eqpList, acList, usrList, startDate, endDate, filterFileNameTextbox.Text, SwapCheckBoxFilter.Checked);
                    //MessageBox.Show("Work");
                }
            }
            else
            {
                exportErrorLabel.Show();
            }
        }
    }
}