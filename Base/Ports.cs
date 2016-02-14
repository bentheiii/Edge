using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using CCDefault.Annotations;
using Edge.Arrays;
using Edge.Credentials;
using Edge.Factory;
using Edge.Funnels;
using Edge.Looping;
using Edge.Ports.AutoCommands;
using Edge.Processes;
using Edge.Serializations;

namespace Edge.Ports
{
    namespace AutoCommands
    {
        public interface ConnectionAutoCommand
        {
            void onRecieve(ReadOnlyConnection c);
        }
        [Serializable]
        public class ConnectionReplyCommand : ConnectionAutoCommand
        {
            private string expectedreply { get; }
            public ConnectionReplyCommand(string expectedreply)
            {
                this.expectedreply = expectedreply;
            }
            public void onRecieve(ReadOnlyConnection c)
            {
                c.Send(expectedreply);
            }
        }
        [Serializable]
        public class ConnectionIgnoreCommand : ConnectionAutoCommand
        {
            public void onRecieve(ReadOnlyConnection c)
            {}
        }
        [Serializable]
        public abstract class ConnectionChangeCommand : ConnectionAutoCommand
        {
            protected EndPoint target { get; set; }
            public abstract void onRecieve(ReadOnlyConnection c);
        }
    }
    public interface IPortBound: IDisposable
    {
        EndPoint Source { get; set; }
    }
    public interface IConnection:IPortBound
    {
        EndPoint Target { get; set; }
        bool AutoCommandsEnabled { get; set; }
        int Send(object o);
        //receive without autocommands
        object silentrecieve(out EndPoint from, int buffersize);
    }
    public static class Connextention
    {
        private const int DEFAULTBUFFERSIZE = 1024;
        [CanBeNull]
        public static object recieve(this IConnection c, out EndPoint from, int buffersize)
        {
            object r = c.silentrecieve(out from, buffersize);
            ConnectionAutoCommand a = r as ConnectionAutoCommand;
            if (a != null && c.AutoCommandsEnabled)
            {
                a.onRecieve(new ReadOnlyConnection(c));
                return r;
            }
            return r;
        }
        [CanBeNull]
        public static object recieve(this IConnection c, out EndPoint p)
        {
            return c.recieve(out p, DEFAULTBUFFERSIZE);
        }
        [CanBeNull]
        public static object recieve(this IConnection c)
        {
            EndPoint p;
            return c.recieve(out p);
        }
        public static bool ping(this IConnection c, [CanBeNull] out Exception ex)
        {
            try
            {
                const string pingstring = "ping0112358";
                ConnectionReplyCommand p = new ConnectionReplyCommand(pingstring);
                c.Send(p);
                object reply = c.recieve();
                ex = null;
                return pingstring.Equals(reply as string);
            }
            catch(Exception e)
            {
                ex = e;
                return false;
            }
        }
        public static bool ping(this IConnection c)
        {
            Exception e;
            return c.ping(out e);
        }
        public static EndPoint LocalEndPoint(int port)
        {
            return new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);
        }
        public static void setLocalTarget(this IConnection c, int port)
        {
            c.Target = LocalEndPoint(port);
        }
        public static void setLocalSource(this IPortBound c, int port)
        {
            c.Source = LocalEndPoint(port);
        }
        public static EndPoint EnsureSource(this IPortBound c)
        {
            if (c.Source == null)
            {
                c.setLocalSource(0);
            }
            return c.Source;
        }
        public static T recieve<T>(this IConnection c)
        {
            EndPoint @from;
            return recieve<T>(c, out @from);
        }
        public static T recieve<T>(this IConnection c, out EndPoint from)
        {
            return recieve<T>(c, out @from, TimeSpan.FromMilliseconds(int.MaxValue));
        }
        public static T recieve<T>(this IConnection c, out EndPoint from, TimeSpan timeout)
        {
            TimeSpan time;
            return recieve<T>(c, out @from, timeout, out time);
        }
        public static T recieve<T>(this IConnection c, TimeSpan timeout)
        {
            TimeSpan time;
            return recieve<T>(c, timeout, out time);
        }
        public static T recieve<T>(this IConnection c, TimeSpan timeout, out TimeSpan time)
        {
            EndPoint @from;
            return recieve<T>(c, out @from, timeout, out time);
        }
        public static T recieve<T>(this IConnection c, out EndPoint from, TimeSpan timeout, out TimeSpan time)
        {
            object recievedval;
            EndPoint source = LocalEndPoint(0);
            bool finished = Routine.TimeOut(() => c.recieve(out source), timeout, out time, out recievedval);
            if (!finished)
                throw new TimeoutException();
            from = source;
            if (!(recievedval is T))
                throw new InvalidCastException();
            return (T)recievedval;
        } 
    }
    public class ReadOnlyConnection
    {
        private readonly IConnection _c;
        public ReadOnlyConnection(IConnection c)
        {
            this._c = c;
        }
        public bool AutoCommandsEnabled
        {
            get
            {
                return _c.AutoCommandsEnabled;
            }
        }
        public int Send(object o)
        {
            return _c.Send(o);
        }
    }
    public class UdpConnection : IConnection
    {
        private readonly Socket _sock;
        public EndPoint Target { get; set; }
        public bool AutoCommandsEnabled{ get; set; }
        public UdpConnection()
        {
            this.AutoCommandsEnabled = false;
            this.Target = null;
            _sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        }
        public EndPoint Source
        {
            get
            {
                return _sock.LocalEndPoint;
            }
            set
            {
                _sock.Bind(value);
            }
        }
        public int Send(object o)
        {
            return _sock.SendTo(Serialization.Serialize(o), Target);
        }
        public object silentrecieve(out EndPoint from, int bufferSize)
        {
            from = new IPEndPoint(0,0);
            byte[] buffer = new byte[bufferSize];
            int l = _sock.ReceiveFrom(buffer, ref from);
            byte[] ret = new byte[l];
            ret.Fill(buffer);
            return Serialization.Deserialize(ret);
        }
        ~UdpConnection()
        {
            this.Dispose(false);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
                _sock.Dispose();
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
    public class TcpServer : IPortBound, ICreator<IConnection>
    {
        private readonly Socket _sock;
        public int Backlog { get; }
        public TcpServer()
        {
            _sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Backlog = 10;
        }
        public EndPoint Source
        {
            get
            {
                return _sock.LocalEndPoint;
            }
            set
            {
                _sock.Bind(value);
            }
        }
        public IConnection Create()
        {
            _sock.Listen(Backlog);
            return new PrivateTcpServerConnection(_sock.Accept());
        }
        ~TcpServer()
        {
            this.Dispose(false);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
                _sock.Dispose();
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        private class PrivateTcpServerConnection : IConnection
        {
            public bool AutoCommandsEnabled { get; set; }
            private readonly Socket _sock;
            public PrivateTcpServerConnection(Socket sock)
            {
                this.AutoCommandsEnabled = false;
                this._sock = sock;
            }
            public EndPoint Source
            {
                get
                {
                    return _sock.LocalEndPoint;
                }
                set
                {
                    throw new Exception("cannot change source of dedicated TCP connection");
                }
            }
            public EndPoint Target
            {
                get
                {
                    return _sock.RemoteEndPoint;
                }
                set
                {
                    throw new Exception("cannot change target of dedicated TCP connection");
                }
            }
            public int Send(object o)
            {
                return _sock.Send(Serialization.Serialize(o));
            }
            public object silentrecieve(out EndPoint from, int buffersize)
            {
                from = new IPEndPoint(0, 0);
                byte[] buffer = new byte[buffersize];
                int l = _sock.Receive(buffer);
                byte[] ret = new byte[l];
                ret.Fill(buffer);
                return Serialization.Deserialize(ret);
            }
            ~PrivateTcpServerConnection()
            {
                this.Dispose();
            }
            public void Dispose()
            {
                _sock.Dispose();
            }
        }
    }
    public class TcpClient : IConnection
    {
        public bool AutoCommandsEnabled { get; set; }
        private readonly Socket _sock;
        private EndPoint _target;
        public TcpClient()
        {
            this.AutoCommandsEnabled = false;
            _sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _target = null;
        }
        public EndPoint Target
        {
            get
            {
                return _target;
            }
            set
            {
                if (Target == null)
                {
                    _sock.Connect(value);
                    _target = value;
                }
                else
                    throw new Exception("the socket is connected and cannot be re-targeted");
            }
        }
        public EndPoint Source
        {
            get
            {
                return _sock.LocalEndPoint;
            }
            set
            {
                if (Target == null)
                    _sock.Bind(value);
                else
                    throw new Exception("the socket is connected and cannot be rebound");
            }
        }
        public int Send(object o)
        {
            return _sock.Send(Serialization.Serialize(o));
        }
        public object silentrecieve(out EndPoint @from, int buffersize)
        {
            from = new IPEndPoint(0, 0);
            byte[] buffer = new byte[buffersize];
            int l = _sock.Receive(buffer);
            byte[] ret = new byte[l];
            ret.Fill(buffer);
            return Serialization.Deserialize(ret);
        }
        ~TcpClient()
        {
            this.Dispose(false);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
                _sock.Dispose();
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
    public class MessageRecieveEventArgs : EventArgs
    {
        public MessageRecieveEventArgs(object message, EndPoint source)
        {
            this.message = message;
            this.source = source;
        }
        public object message { get; }
        public EndPoint source { get; }
    }
    public delegate void MessageRecieveHandler(object sender, MessageRecieveEventArgs e);
    public class RecieverThread : IDisposable
    {
        public RecieverThread(IConnection conn, ThreadPriority p = ThreadPriority.Normal)
        {
            this._thread = new Thread(this.threadmain);
            _thread.Priority = p;
	        _thread.IsBackground = true;
            this.conn = conn;
        }
        public IConnection conn { get; }
        public event MessageRecieveHandler onRecieve;
        private readonly Thread _thread;
        public void start()
        {
            _thread.Start();
        }
        private void stop()
        {
            if(_thread.IsAlive)
                _thread.Abort();
        }
        public void threadmain()
        {
            while (true)
            {
                EndPoint p;
                object val = conn.recieve(out p);
	            this.onRecieve?.Invoke(this, new MessageRecieveEventArgs(val,p));
            }
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
                stop();
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
    public class NewConnectionEventArgs : EventArgs
    {
        public NewConnectionEventArgs(IConnection connection, int instanceIndex)
        {
            this.connection = connection;
            this.instanceIndex = instanceIndex;
        }
        public IConnection connection { get; }
        public int instanceIndex { get; }
    }
    public delegate void NewConnectionHandler(object sender, NewConnectionEventArgs e);
    public class SocketListenerThread : IDisposable
    {
        private int _instancecount = 1;
        public SocketListenerThread(ICreator<IConnection> creator, ThreadPriority mainpriority = ThreadPriority.Normal)
        {
            this._mainthread = new Thread(this.threadmain);
            _mainthread.Priority = mainpriority;
            _mainthread.IsBackground = true;
            this.creator = creator;
        }
        public ICreator<IConnection> creator { get; }
        public event NewConnectionHandler onCreate;
        private readonly Thread _mainthread;
        private readonly List<IDisposable> _dependants = new List<IDisposable>();
        public void AddDependant(IDisposable d)
        {
            _dependants.Add(d);
        }
        public void start()
        {
            _mainthread.Start();
        }
        public void stop()
        {
            if (_mainthread.IsAlive)
                _mainthread.Abort();
        }
        private void threadmain()
        {
            while (true)
            {
                IConnection conn = creator.Create();
                onCreate.Invoke(this,new NewConnectionEventArgs(conn, _instancecount++));
            }
        }
        protected virtual void Dispose(bool disposing)
        {
            stop();
            if (disposing)
            {
                foreach (IDisposable iDisposable in _dependants)
                {
                    iDisposable.Dispose();
                }
            }
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
    namespace MultiSocket
    {
        public static class MultiSocket
        {
            [Serializable]
            public class ForwardedDataGram
            {
                public ForwardedDataGram(object data, EndPoint origSource, DateTime origRecivDate)
                {
                    this.data = data;
                    this.origSource = origSource;
                    this.origRecivDate = origRecivDate;
                }
                public object data { get; }
                public EndPoint origSource { get; }
                public DateTime origRecivDate { get; }
            }
            [Serializable]
            public class SubscriptionRequest
            {
                public Credential access { get; }
                public SubscriptionRequest(Credential access)
                {
                    this.access = access;
                }
            }
            [Serializable]
            public class SubsciptionConfirmation
            {
                public SubsciptionConfirmation(bool allowed)
                {
                    this.allowed = allowed;
                }
                public bool allowed { get; }
            }
            [Serializable]
            public class UnsubscribeRequest
            {

            }
            [Serializable]
            public class SendRequest
            {
                public SendRequest(object message, EndPoint target)
                {
                    this.message = message;
                    this.Target = target;
                }
                public object message { get; }
                public EndPoint Target { get; }
            }
            [Serializable]
            public class SentConfirmation
            {
                public SentConfirmation(int sentBytes)
                {
                    this.sentBytes = sentBytes;
                }
                public int sentBytes { get; }
            }
            [Serializable]
            public class MultiSocketNotifierConfirmationRequest
            { }
            [Serializable]
            public class MultiSocketNotifierConfirmationReply
            { }
            [Serializable]
            public class ConsiderClosingRequest { }
            private static bool IsNotifier(EndPoint p, TimeSpan timeout)
            {
                IConnection testConnection = new UdpConnection();
                testConnection.Target = p;
                testConnection.Send(new MultiSocketNotifierConfirmationRequest());
                ForwardedDataGram g;
                if (Routine.TimeOut(() =>
                {
                    EndPoint source;
                    var o = testConnection.recieve(out source);
                    return new ForwardedDataGram(o, source, DateTime.Now);
                }, timeout, out timeout, out g))
                {
                    return g.data is MultiSocketNotifierConfirmationReply;
                }
                return false;

            }
            public enum NotifierState
            {
                Exists = 1,
                Empty = 0,
                Occupied = -1
            }
            private static NotifierState NotifierExists(EndPoint p)
            {
                TimeSpan defaulttimeout = TimeSpan.FromSeconds(3);
                IConnection testConnection = new UdpConnection();
                try
                {
                    testConnection.Source = p;
                    return NotifierState.Empty;
                }
                catch (SocketException e)
                {
                    if (e.SocketErrorCode == SocketError.AddressAlreadyInUse)
                    {
                        return IsNotifier(p, defaulttimeout) ? NotifierState.Exists : NotifierState.Occupied;
                    }
                    throw new Exception("unexpected error while checking for MultiSocketNotifier",e);
                }
                finally
                {
                    testConnection.Dispose();
                }
            }
            private static void EnsureNotifierExists(EndPoint p, ICredentialValidator c)
            {
                var r = NotifierExists(p);
                switch (r)
                {
                    case NotifierState.Empty:
                        MultiSocketNotifier n = new MultiSocketNotifier(c,p);
                        RecieverThread t = new RecieverThread(n);
                        t.onRecieve += n.onRecieve;
                        t.start();
                        break;
                    case NotifierState.Occupied:
                        throw new Exception("the port is occupied by a non-MultiSocket socket");
                }
            }
            public static IConnection ConnectToMultiSocket(out EndPoint p, ICredentialValidator notifierCredentials, Credential accessCredential)
            {
                var master = new MultiSocketNotifier(notifierCredentials,out p);
                RecieverThread t = new RecieverThread(master);
                t.onRecieve += master.onRecieve;
                t.start();
                IConnection retConnection = new MultiSocketListener(p, accessCredential);
                return retConnection;
            }
            public static IConnection ConnectToMultiSocket(EndPoint p, ICredentialValidator notifierCredentials, Credential accessCredential)
            {
                EnsureNotifierExists(p,notifierCredentials);
                IConnection retConnection = new MultiSocketListener(p,accessCredential);
                return retConnection;
            }
            public static IConnection ConnectToStrippedMultiSocket(EndPoint p, ICredentialValidator notifierCredentials, Credential accessCredential)
            {
                return new MultiSocketStrippedListener(ConnectToMultiSocket(p,notifierCredentials,accessCredential) as MultiSocketListener);
            }
            public static IConnection ConnectToStrippedMultiSocket(out EndPoint p, ICredentialValidator notifierCredentials, Credential accessCredential)
            {
                return new MultiSocketStrippedListener(ConnectToMultiSocket(out p, notifierCredentials, accessCredential) as MultiSocketListener);
            }
        }
        class MultiSocketListener : IConnection
        {
            private readonly IConnection _int;
            public MultiSocketListener(EndPoint multiSocketPort, Credential c)
            {
                this._int = new UdpConnection();
                _int.Target = multiSocketPort;
                _int.Send(new MultiSocket.SubscriptionRequest(c));
                var reply = _int.recieve(out multiSocketPort);
                if (!(reply as MultiSocket.SubsciptionConfirmation).allowed)
                    throw new AccessViolationException("MultiSocketNotifier Denied access");
            }
            public void Dispose()
            {
                _int.Send(new MultiSocket.UnsubscribeRequest());
                _int.Send(new MultiSocket.ConsiderClosingRequest());
                this._int.Dispose();
            }
            public EndPoint Source
            {
                get
                {
                    return _int.Target;
                }
                set
                {
                    throw new Exception("MultiPortListener's source cannot be changed");
                }
            }
            public EndPoint Target { get; set; }
            public bool AutoCommandsEnabled
            {
                get
                {
                    return false;
                }
                set
                {
                    throw new Exception("MultiPortListener's cannot handle autocommands");
                }
            }
            public int Send(object o)
            {
                if (this.Target == null)
                    throw new Exception("connection target is not set!");
                var m = Target.Equals(_int.Target) ? o : new MultiSocket.SendRequest(o, this.Target);
                var ret =_int.Send(m);
                var r = Target.Equals(_int.Target) ? new MultiSocket.SentConfirmation(ret) : _int.recieve();
                var confirmation = r as MultiSocket.SentConfirmation;
                if (confirmation == null)
                    throw new Exception($"expected a {nameof(MultiSocket.SentConfirmation)}, got {r}");
                return confirmation.sentBytes;
            }
            public object silentrecieve(out EndPoint @from, int buffersize)
            {
                var ret = _int.silentrecieve(out from, buffersize);
                return ret;
            }
        }
        class MultiSocketStrippedListener : IConnection
        {
            private readonly MultiSocketListener _int;
            public MultiSocketStrippedListener(MultiSocketListener c)
            {
                this._int = c;
            }
            public void Dispose()
            {
                this._int.Dispose();
            }
            public EndPoint Source
            {
                get
                {
                    return _int.Source;
                }
                set
                {
                    _int.Source = value;
                }
            }
            public EndPoint Target
            {
                get
                {
                    return _int.Target;
                }
                set
                {
                    this._int.Target = value;
                }
            }
            public bool AutoCommandsEnabled
            {
                get
                {
                    return this._int.AutoCommandsEnabled;
                }
                set
                {
                    this._int.AutoCommandsEnabled = value;
                }
            }
            public int Send(object o)
            {
                return this._int.Send(o);
            }
            public object silentrecieve(out EndPoint @from, int buffersize)
            {
                return (this._int.silentrecieve(out @from, buffersize) as MultiSocket.ForwardedDataGram).data;
            }
        }
        class MultiSocketNotifier : IConnection
        {
            private readonly List<EndPoint> _subscibers = new List<EndPoint>();
            private readonly IConnection _int;
            private readonly ICredentialValidator _access;
            public MultiSocketNotifier(int localport) : this(Connextention.LocalEndPoint(localport)) { }
            public MultiSocketNotifier(EndPoint p) : this(new OpenCredentialValidator(), p) { }
            public MultiSocketNotifier(ICredentialValidator access, int localport) : this(access, Connextention.LocalEndPoint(localport)) { }
            public MultiSocketNotifier(ICredentialValidator access, EndPoint p)
            {
                this._access = access;
                _int = new UdpConnection();
                _int.Source = p;
            }
            public MultiSocketNotifier(ICredentialValidator access, out EndPoint p)
            {
                this._access = access;
                _int = new UdpConnection();
                p = this._int.EnsureSource();
            }
            public void Dispose()
            {
                _int.Dispose();
            }
            public EndPoint Source
            {
                get
                {
                    return _int.Source;
                }
                set
                {
                    throw new Exception("cannot change MultiPort source");
                }
            }
            public EndPoint Target
            {
                get
                {
                    return _int.Target;
                }
                set
                {
                    _int.Target = value;
                }
            }
            public bool AutoCommandsEnabled
            {
                get
                {
                    return false;
                }
                set
                {
                    throw new Exception("MultiPort doesn't support autocommands");
                }
            }
            public int Send(object o)
            {
                return _int.Send(o);
            }
            public object silentrecieve(out EndPoint @from, int buffersize)
            {
                return _int.silentrecieve(out from, buffersize);
            }
            public void onRecieve(object o, MessageRecieveEventArgs messageRecieveEventArgs)
            {
                Funnel<object> handler = new Funnel<object>();
                // ReSharper disable ImplicitlyCapturedClosure
                handler.Add(a =>
                {
                    MultiSocket.SubscriptionRequest request = a as MultiSocket.SubscriptionRequest;
                    if (request != null)
                    {
                        var reply = this._access.isValid(request.access);
                        if (reply)
                        {
                            this._subscibers.Add(messageRecieveEventArgs.source);
                        }
                        this._int.Target = messageRecieveEventArgs.source;
                        this._int.Send(new MultiSocket.SubsciptionConfirmation(reply));
                        return true;
                    }
                    return false;
                });
                handler.Add(a =>
                {
                    if (a is MultiSocket.UnsubscribeRequest)
                    {
                        _subscibers.Remove(messageRecieveEventArgs.source);
                        return true;
                    }
                    return false;
                });
                handler.Add(a =>
                {
                    if (a is MultiSocket.ConsiderClosingRequest)
                    {
                        if (_subscibers.Count == 0)
                        {
                            this.Dispose();
                            ((RecieverThread)o).Dispose();
                        }
                    }
                    return false;
                });
                handler.Add(a =>
                {
                    MultiSocket.SendRequest request = a as MultiSocket.SendRequest;
                    if (request != null)
                    {
                        Target = request.Target;
                        var ret = Send(request.message);
                        Target = messageRecieveEventArgs.source;
                        Send(new MultiSocket.SentConfirmation(ret));
                        return true;
                    }
                    return false;
                });
                handler.Add(a =>
                {
                    var request = a as MultiSocket.MultiSocketNotifierConfirmationRequest;
                    if (request != null)
                    {
                        Target = messageRecieveEventArgs.source;
                        var ret = Send(new MultiSocket.MultiSocketNotifierConfirmationReply());
                        return true;
                    }
                    return false;
                });
                handler.Add(a =>
                {
                    DateTime t = DateTime.Now;
                    foreach (EndPoint subsciber in _subscibers.Except(messageRecieveEventArgs.source))
                    {
                        Target = subsciber;
                        Send(new MultiSocket.ForwardedDataGram(messageRecieveEventArgs.message, messageRecieveEventArgs.source, t));
                    }
                    return true;
                });
                // ReSharper restore ImplicitlyCapturedClosure
                handler.Process(messageRecieveEventArgs.message);
            }
        }
    }
}
