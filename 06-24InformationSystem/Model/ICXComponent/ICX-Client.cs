using log4net;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Timers;
// ICX_Client
namespace _06_24InformationSystem.Model.ICXComponent
{
    public class ICX_Client
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly System.Timers.Timer _sendIdleTimer = new System.Timers.Timer(60000);
        private readonly List<byte> _ipDataInBuffer = new List<byte>();
        private DateTime _connectedTimestamp = DateTime.MinValue;
        private bool _connected;
        private TcpClient _icxClient;
        private Thread _receiveThread;
        private bool _wasWrongPassword;
        private bool _isConnecting;
        private byte[] _resendDataMessage;

        public ICX_Client()
        {
            IcxIp = "";
        }

        public event EventHandler<NewIdleEventArgs> NewIdleReceived;
        public event EventHandler<NewDataEventArgs> NewDataReceived;
        public event EventHandler<NewDataEventArgs> NewDataSent;
        public event EventHandler<ClientErrorEventArgs> ClientError;
        public event EventHandler<ConnectedChangedEventArgs> ConnectedChanged;

        public string IcxIp { get; set; }

        public int IcxPort { get; set; }

        public int SendIdleInterval
        {
            get
            {
                return (int)_sendIdleTimer.Interval;
            }
            set
            {
                _sendIdleTimer.Interval = Math.Max(1, value);
                _sendIdleTimer.Enabled = value > 0;
            }
        }

        public bool Connected
        {
            get
            {
                return _connected;
            }
            set
            {
                Log.Debug("Setting _connected value");
                if (_connected == value)
                    return;
                _connected = value;
                RaiseConnectedChanged(new ConnectedChangedEventArgs(_connected));
            }
        }

        public void Connect(string server, int port)
        {
            Connected = false;
            _isConnecting = false;
            Log.Debug("Connecting...");
            if (_isConnecting)
                return;
            try
            {
                _isConnecting = true;
                Disconnect();
                _connectedTimestamp = DateTime.Now;
                _wasWrongPassword = false;
                _icxClient = new TcpClient();

                try
                {
                    var asyncResult = _icxClient.BeginConnect(server, port, null, null);
                    var asyncWaitHandle = asyncResult.AsyncWaitHandle;
                    try
                    {
                        if (!asyncResult.AsyncWaitHandle.WaitOne(5000, false))
                        {
                            if (_icxClient.Connected)
                                _icxClient.GetStream().Close();
                            _icxClient.Close();
                            throw new TimeoutException();
                        }
                        else
                            _icxClient.EndConnect(asyncResult);
                    }
                    finally
                    {
                        asyncWaitHandle.Close();
                    }
                    _icxClient.Client.ReceiveTimeout = 60000;
                    _icxClient.Client.SendTimeout = 60000;
                    IcxIp = server;
                    IcxPort = port;
                    _receiveThread = new Thread(ReceiveThread)
                    {
                        Name = "receiveThread",
                        IsBackground = true,
                        Priority = ThreadPriority.AboveNormal
                    };
                    _sendIdleTimer.Elapsed -= _sendIdleTimer_Elapsed;
                    _sendIdleTimer.Elapsed += _sendIdleTimer_Elapsed;
                    Connected = true;
                    _receiveThread.Start();

                }
                catch (Exception ex)
                {
                    Log.Error("EX: " + ex.Message);
                    Disconnect();
                }
            }
            finally
            {
                _isConnecting = false;
                Log.Debug("_isConnecting false");
            }
        }

        public void forceDisconnect()
        {
            _receiveThread.Abort();
            _icxClient.Close();
        }

        public void Disconnect()
        {
            Connected = false;
            Log.Debug("Disconnecting...");
            try
            {
                DateTime now = DateTime.Now;
                if (_connectedTimestamp != DateTime.MinValue && (now - _connectedTimestamp).Ticks / 10000L < 500L)
                {
                    RaiseClientError(new ClientErrorEventArgs("Wrong Password!", false));
                    _wasWrongPassword = true;
                }
                _connectedTimestamp = DateTime.MinValue;
                if (_icxClient != null)
                {
                    try
                    {
                        if (_icxClient.Connected)
                        {
                            _icxClient.GetStream().Close();
                        }
                    }
                    catch
                    {
                        // ignored
                    }
                    _icxClient.Close();
                    Log.Warn("I Closed _icxClient");
                    if (_receiveThread != null)
                    {
                        Connected = false;
                        Log.Debug("--- disconnected ---");
                        _receiveThread.Abort();
                        Thread.Sleep(100);
                    }
                }
            }
            catch
            {
                // ignored
            }
            try
            {
                _icxClient = null;
            }
            catch
            {
                // ignored
            }
        }

        private void reconnect()
        {
            if (_icxClient == null || _icxClient.Client == null)
            {
                _isConnecting = false;
                Connected = false;
                _connected = false;

                if (_receiveThread != null)
                {
                    _receiveThread.Start();
                }

                Connect("10.45.241.205", 17000);
            }
        }

        private void _sendIdleTimer_Elapsed(object sender, ElapsedEventArgs e)
        {

            if (_icxClient == null || _icxClient.Client == null)
            {
                Log.Debug(DateTime.Now.ToShortTimeString() + " : _sendIdleTimer_Elapsed -  || this._tcpClient.Client == null)");
                Disconnect();
                //   Connect("10.45.241.205", 17000);
                return;
            }
            Send("", IcXdataType.Idle);
        }


        public void Send(string data)
        {
            //  Log.Debug("In send 2");
            Send(data, IcXdataType.Data);
        }

        public void Send(string data, IcXdataType ipDataType)
        {
            //   Log.Debug("In send 3");
            Send(new ASCIIEncoding().GetBytes(data), ipDataType);
        }

        public void Send(byte[] dataBytes, IcXdataType ipDataType)
        {
            Log.Debug("In send");
            try
            {
                const byte subType = (byte)0;
                byte[] numArray;
                if (ipDataType == IcXdataType.Data) //F2
                {
                    try
                    {
                        numArray = new byte[dataBytes.Length + 3];
                        Array.Copy(dataBytes, 0, numArray, 2, dataBytes.Length);
                        numArray[0] = (byte)ipDataType;
                        numArray[1] = subType;
                        numArray[numArray.Length - 1] = byte.MaxValue;
                        //Log.Debug((object)("IPPort Snd: " + ((object)ipDataType).ToString() + "; " + Encoding.ASCII.GetString(dataBytes)));
                        _resendDataMessage = numArray;
                    }
                    catch (Exception ex)
                    {
                        Log.Error("Send(byte[] dataBytes, ICXclient.ICXdataType ipDataType, bool hideInLog) EXCEPTION: " + ex.Message);
                        numArray = null;
                    }
                }
                else
                {
                    try
                    {
                        numArray = new byte[dataBytes.Length + 3];
                        numArray[0] = (byte)ipDataType;
                        numArray[1] = 0;
                        for (var index = 0; index < dataBytes.Length; ++index)
                        {
                            numArray[index + 2] = dataBytes[index];
                        }
                        numArray[numArray.Length - 1] = byte.MaxValue;
                        Log.Debug("numArray clear for send");
                        //Log.Debug((object)("IPPort Snd: " + ((object)ipDataType).ToString()));
                    }
                    catch (Exception ex)
                    {
                        numArray = null;
                        Log.Error("EX: " + ex.Message);
                    }
                }
                try
                {
                    if (numArray == null || _icxClient == null)
                        return;
                    if (_icxClient.Client != null)
                    {
                        _icxClient.Client.Send(numArray);
                        Log.Debug("Sending numArray");

                        RaiseNewDataSent(new NewDataEventArgs(subType, numArray));

                        if (_resendDataMessage == null)
                            return;
                        _resendDataMessage = null;
                    }
                    else
                    {
                        Disconnect();
                        RaiseClientError(new ClientErrorEventArgs("Tcp-Client disconnected", false));
                    }
                }
                catch (Exception ex)
                {
                    Log.Error("EX: " + ex.Message);
                }
            }
            catch (Exception ex)
            {
                var icxClient = _icxClient;
                if (icxClient != null && icxClient.Connected)
                {
                    if (_icxClient != null)
                    {
                        _icxClient.GetStream().Close();
                    }
                }
                if (_icxClient != null) _icxClient.Close();
                RaiseClientError(new ClientErrorEventArgs(ex.Message, false));
                Log.Error("EX: " + ex.Message);
            }
        }
        private void ReceiveThread()
        {
            var buffer = new byte[4096];
            while (true)
            {
                try
                {
                    try
                    {

                        SocketError errorCode;
                        var length = _icxClient.Client.Receive(buffer, 0, buffer.Length, SocketFlags.None, out errorCode);

                        if (errorCode == SocketError.Success && length > 0)
                        {
                            byte[] data = new byte[length];

                            Array.Copy(buffer, data, length);
                            //this.ShowBuffer(data, "IPPort Rvd: ");
                            _ipDataInBuffer.AddRange(data);

                            var temp = Encoding.ASCII.GetString(_ipDataInBuffer.ToArray());
                            Log.Info("Incomming RAW string: " + temp);
                            if (temp.Length == 3)
                            {
                                Log.Info("Probable IDLE Ping");
                            }
                            Console.WriteLine(temp);
                            temp = "";

                            ProcessReceiveBuffer();
                            Connected = true;

                            //
                            var stringBuilder1 = new StringBuilder();
                            var stringBuilder2 = new StringBuilder();
                            foreach (var t in data)
                            {
                                //stringBuilder1.Append(data[index].ToString("X2") + " ");
                                stringBuilder1.Append(t.ToString("X2"));
                                if (!char.IsControl((char)t))
                                    stringBuilder2.Append((char)t);
                                else
                                    stringBuilder2.Append('.');
                            }
                            Log.Debug(DateTime.Now.ToShortTimeString() + " : " + "  -  [" + stringBuilder1.ToString() + "]");
                            //Console.WriteLine(stringBuilder1.ToString());
                        }
                        else if (errorCode == SocketError.TimedOut)
                            Thread.Sleep(1);
                        else if (errorCode == SocketError.WouldBlock)
                            Thread.Sleep(10);
                        else if (errorCode == SocketError.Shutdown)
                        {
                            Disconnect();
                            RaiseClientError(new ClientErrorEventArgs("Tcp-Shutdown from IntercomServer", false));
                            break;
                        }
                        else if (errorCode == SocketError.ConnectionReset)
                        {
                            Disconnect();
                            RaiseClientError(new ClientErrorEventArgs("The connection was reset by the remote peer", false));
                            break;
                        }
                        else
                        {
                            Disconnect();
                            Connected = false;
                            if (_wasWrongPassword)
                                break;
                            RaiseClientError(new ClientErrorEventArgs("Disconnected!", false));
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error("ReceiveThread 1 EXCEPTION: " + ex.Message);
                        Disconnect();
                        if (_wasWrongPassword)
                            break;
                        RaiseClientError(new ClientErrorEventArgs("disconnected!", false));
                        break;
                    }
                }
                catch (Exception ex)
                {
                    Log.Error("ReceiveThread 2 EXCEPTION: " + ex.Message);
                    RaiseClientError(new ClientErrorEventArgs("EX in ReceiveThread: " + ex.Message, false));
                }
                Thread.Sleep(1);
            }
        }

        private static bool FindPredicateStartTag(byte b)
        {
            switch (b)
            {
                case 241:
                case 242:
                case 243:
                case 244:
                    return true;
                default:
                    return false;
            }
        }

        private static bool FindPredicateIdleTag(byte b)
        {
            return b == 241;
        }

        private static bool FindPredicateEndTag(byte b)
        {
            return b == byte.MaxValue;
        }

        private void ProcessReceiveBuffer()
        {
            try
            {
                while (_ipDataInBuffer.FindIndex(0, FindPredicateEndTag) >= 0)
                {
                    try
                    {
                        var temp = Encoding.ASCII.GetString(_ipDataInBuffer.ToArray());
                        /* if (temp.Contains("00420010")) // 00420010 == bortkoppling, not needed
                         {
                             Log.Debug("Found 00420010!");
                           //  _ipDataInBuffer.Clear();
                           //  break;
                         }*/

                        var index1 = _ipDataInBuffer.FindIndex(0, FindPredicateIdleTag);
                        var index2 = _ipDataInBuffer.FindIndex(index1 + 1, FindPredicateEndTag);
                        if (index1 >= 0 && index2 >= 0)
                        {
                            var subType = _ipDataInBuffer[index1 + 1];
                            _ipDataInBuffer.RemoveRange(index1, index2 - index1 + 1);
                            RaiseNewIdleReceived(new NewIdleEventArgs(subType));
                            Log.Debug((object)"IPPort Rvd: cuting idle...");
                        }
                        var index3 = _ipDataInBuffer.FindIndex(0, FindPredicateStartTag);
                        if (index3 < 0) continue;

                        var ipDataType = (IcXdataType)_ipDataInBuffer[index3];
                        var index4 = _ipDataInBuffer.FindIndex(index3 + 1, FindPredicateEndTag);
                        if (index4 < 0) continue;

                        var index5 = _ipDataInBuffer.FindIndex(index3 + 1, FindPredicateStartTag);
                        if (index5 >= 0 && _ipDataInBuffer[index5 - 1] != byte.MaxValue)
                        {
                            Log.Debug((object)"IPPort Inf: Invalid StartTag!!!");
                            _ipDataInBuffer.RemoveRange(0, index5);
                        }
                        else
                        {
                            var subType = _ipDataInBuffer[index3 + 1];
                            var data = new byte[index4 - index3 - 2];
                            Array.Copy(_ipDataInBuffer.ToArray(), index3 + 2, data, 0, data.Length);
                            switch (ipDataType)
                            {
                                case IcXdataType.Data:
                                    Log.Debug("DataInBuffer: " + Encoding.ASCII.GetString(data));
                                    RaiseNewDataReceived(new NewDataEventArgs(subType, data));
                                    break;
                                case IcXdataType.Challenge:
                                    //this.HandleIoIPChallenge(data, this.Password);
                                    break;
                                    //    default:
                                    // throw new ApplicationException("unexpected DataType " + ipDataType + "...");
                            }
                            _ipDataInBuffer.RemoveRange(index3, index4 + 1);
                            /*   if (_ipDataInBuffer.Count >= 1000)
                               {
                                   _ipDataInBuffer.Clear();
                               } */
                        }
                    }
                    catch (Exception ex)
                    {
                        _ipDataInBuffer.Clear();
                        Log.Error("EX: " + ex.Message);
                    }
                }
            }
            catch (Exception err)
            {
                Log.Error("EX ProcessReceiveBuffer " + err);
            }
        }

        protected virtual void RaiseNewIdleReceived(NewIdleEventArgs e)
        {
            EventHandler<NewIdleEventArgs> eventHandler = NewIdleReceived;
            if (eventHandler == null)
                return;
            Log.Error("In RaiseNewIdleReceived");
            eventHandler(this, e);
        }

        protected virtual void RaiseNewDataReceived(NewDataEventArgs e)
        {
            try
            {
                EventHandler<NewDataEventArgs> eventHandler = NewDataReceived;
                if (eventHandler == null)
                    return;
                //  Log.Error("In RaiseNewDataReceived");
                eventHandler(this, e);
            }
            catch (Exception error)
            {
                Log.Error("Exception in RaiseNewDataReceived" + error);
            }
        }

        protected virtual void RaiseNewDataSent(NewDataEventArgs e)
        {
            EventHandler<NewDataEventArgs> eventHandler = NewDataSent;
            if (eventHandler == null)
                return;
            //    Log.Error("In RaiseNewDataSent");
            eventHandler(this, e);
        }

        protected virtual void RaiseClientError(ClientErrorEventArgs e)
        {
            EventHandler<ClientErrorEventArgs> eventHandler = ClientError;
            if (eventHandler == null)
                return;
            Log.Error("In RaiseClientError");
            eventHandler(this, e);

        }

        protected virtual void RaiseConnectedChanged(ConnectedChangedEventArgs e)
        {
            EventHandler<ConnectedChangedEventArgs> eventHandler = ConnectedChanged;
            if (eventHandler == null)
                return;
            Log.Error("In RaiseConnectedChanged");
            eventHandler(this, e);
        }

        public class NewIdleEventArgs : EventArgs
        {
            public byte SubType;
            public NewIdleEventArgs(byte subType)
            {
                SubType = subType;
            }
        }

        public class NewDataEventArgs : EventArgs
        {
            public byte SubType;
            public byte[] Data;
            public NewDataEventArgs(byte subType, byte[] data)
            {
                SubType = subType;
                Data = data;
            }
        }

        public class ClientErrorEventArgs : EventArgs
        {
            public string Error;
            public bool IsConnected;
            public ClientErrorEventArgs(string error, bool isConnected)
            {
                Error = error;
                IsConnected = isConnected;
            }
        }

        public class ConnectedChangedEventArgs : EventArgs
        {
            public bool Connected;
            public ConnectedChangedEventArgs(bool connected)
            {
                Connected = connected;
            }
        }

        public enum IcXdataType : byte
        {
            None = 0,
            Idle = (byte)241,
            Data = (byte)242,
            Password = (byte)243,
            Challenge = (byte)244,
            ChallengeResponse = (byte)245,
            End = (byte)255,
        }

    }
}
