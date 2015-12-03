using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading;

namespace Edge.Threads
{
    public abstract class ThreadWrapper
    {
        private readonly Thread _int;
        protected ThreadWrapper(ThreadStart action)
        {
            _int = new Thread(action);
        }
        protected ThreadWrapper(ThreadStart action, int maxstacksize)
        {
            _int = new Thread(action, maxstacksize);
        }
        protected ThreadWrapper(ParameterizedThreadStart action)
        {
            _int = new Thread(action);
        }
        protected ThreadWrapper(ParameterizedThreadStart action, int maxstacksize)
        {
            _int = new Thread(action,maxstacksize);
        }
        [Obsolete("The ApartmentState property has been deprecated.  Use GetApartmentState, SetApartmentState or TrySetApartmentState instead.", false)]
        public ApartmentState ApartmentState
        {
            get
            {
                return _int.ApartmentState;
            }
            set
            {
                _int.ApartmentState = value;
            }
        }
        public CultureInfo CurrentCulture
        {
            get
            {
                return _int.CurrentCulture;
            }
            set
            {
                _int.CurrentCulture = value;
            }
        }
        // ReSharper disable once InconsistentNaming
        public CultureInfo CurrentUICulture
        {
            get
            {
                return _int.CurrentUICulture;
            }
            set
            {
                _int.CurrentUICulture = value;
            }
        }
        public ExecutionContext ExecutionContext
        {
            get
            {
                return _int.ExecutionContext;
            }
        }
        public bool IsAlive
        {
            get
            {
                return _int.IsAlive;
            }
        }
        public bool IsBackground
        {
            get
            {
                return _int.IsBackground;
            }
            set
            {
                _int.IsBackground = value;
            }
        }
        public bool IsThreadPoolThread
        {
            get
            {
                return _int.IsThreadPoolThread;
            }
        }
        public int ManageThreadId
        {
            get
            {
                return _int.ManagedThreadId;
            }
        }
        public string Name
        {
            get
            {
                return _int.Name;
            }
            set
            {
                _int.Name = value;
            }
        }
        public ThreadPriority Priority
        {
            get
            {
                return _int.Priority;
            }
            set
            {
                _int.Priority = value;
            }
        }
        public ThreadState ThreadState
        {
            get
            {
                return _int.ThreadState;
            }
        }
        [SecuritySafeCritical]
        public void Abort()
        {
            _int.Abort();
        }
        [SecuritySafeCritical]
        public void Abort(object stateInfo)
        {
            _int.Abort(stateInfo);
        }
        [SecuritySafeCritical]
        public void DisableComObjectEagerCleanup()
        {
            _int.DisableComObjectEagerCleanup();
        }
        [SecuritySafeCritical]
        public ApartmentState GetApartmentState()
        {
            return _int.GetApartmentState();
        }
        [Obsolete("Thread.GetCompressedStack is no longer supported. Please use the System.Threading.CompressedStack class")]
        [SecurityCritical]
        public CompressedStack GetCompressedStack()
        {
            return _int.GetCompressedStack();
        }
        [ComVisible(false)]
        public override int GetHashCode()
        {
            return _int.GetHashCode();
        }
        [SecuritySafeCritical] public void Interrupt()
        {
            _int.Interrupt();
        }
        [SecuritySafeCritical] protected void Join()
        {
            _int.Join();
        }
        [SecuritySafeCritical] protected bool Join(int millisecondsTimeout)
        {
            return _int.Join(millisecondsTimeout);
        }
        protected bool Join(TimeSpan timeout)
        {
            return _int.Join(timeout);
        }
        [Obsolete("Thread.Resume has been deprecated.  Please use other classes in System.Threading, such as Monitor, Mutex, Event, and Semaphore, to synchronize Threads or protect resources.  http://go.microsoft.com/fwlink/?linkid=14202",false), SecuritySafeCritical] public void Resume()
        {
            _int.Resume();
        }
        [SecuritySafeCritical] public void SetApartmentState(ApartmentState state)
        {
            _int.SetApartmentState(state);
        }
        [Obsolete("Thread.SetCompressedStack is no longer supported. Please use the System.Threading.CompressedStack class"), SecurityCritical]  public void SetCompressedStack(CompressedStack stack)
        {
            _int.SetCompressedStack(stack);
        }
        protected void Start()
        {
            _int.Start();
        }
        protected void Start(object parameter)
        {
            _int.Start(parameter);
        }
        [Obsolete("Thread.Suspend has been deprecated.  Please use other classes in System.Threading, such as Monitor, Mutex, Event, and Semaphore, to synchronize Threads or protect resources.  http://go.microsoft.com/fwlink/?linkid=14202",false), SecuritySafeCritical]  public void Suspend()
        {
            _int.Suspend();
        }
        [SecuritySafeCritical] public bool TrySetApartmentState(ApartmentState state)
        {
            return _int.TrySetApartmentState(state);
        }
    }
    public class Thread<T1> : ThreadWrapper
    {
        public Thread(Action<T1> action) : base(a => action((T1)a))
        { }
        public Thread(Action<T1> action, int maxstacksize) : base(a => action((T1)a), maxstacksize)
        { }
        public void Start(T1 param)
        {
            base.Start(param);
        }
        public new void Join()
        {
            base.Join();
        }
        public new void Join(int timeOutmilliseconds)
        {
            base.Join(timeOutmilliseconds);
        }
        public new void Join(TimeSpan timeOut)
        {
            base.Join(timeOut);
        }
    }
    public class Thread<T1,T2> : Thread<Tuple<T1,T2>>
    {
        public Thread(Action<T1, T2> action) : base(a=> action(a.Item1,a.Item2)) { }
        public Thread(Action<T1, T2> action, int maxstacksize) : base(a => action(a.Item1, a.Item2), maxstacksize) { }
        public void Start(T1 p1, T2 p2)
        {
            Start(Tuple.Create(p1,p2));
        }
    }
    public class Thread<T1, T2, T3> : Thread<Tuple<T1, T2, T3>>
    {
        public Thread(Action<T1, T2, T3> action) : base(a => action(a.Item1, a.Item2, a.Item3)) { }
        public Thread(Action<T1, T2, T3> action, int maxstacksize) : base(a => action(a.Item1, a.Item2, a.Item3), maxstacksize) { }
        public void Start(T1 p1, T2 p2, T3 p3)
        {
            Start(Tuple.Create(p1, p2, p3));
        }
    }
    public class Thread<T1, T2, T3, T4> : Thread<Tuple<T1, T2, T3, T4>>
    {
        public Thread(Action<T1, T2, T3, T4> action) : base(a => action(a.Item1, a.Item2, a.Item3, a.Item4)) { }
        public Thread(Action<T1, T2, T3, T4> action, int maxstacksize) : base(a => action(a.Item1, a.Item2, a.Item3, a.Item4), maxstacksize) { }
        public void Start(T1 p1, T2 p2, T3 p3, T4 p4)
        {
            Start(Tuple.Create(p1, p2, p3, p4));
        }
    }
    public class Thread<T1, T2, T3, T4, T5> : Thread<Tuple<T1, T2, T3, T4, T5>>
    {
        public Thread(Action<T1, T2, T3, T4, T5> action) : base(a => action(a.Item1, a.Item2, a.Item3, a.Item4, a.Item5)) { }
        public Thread(Action<T1, T2, T3, T4, T5> action, int maxstacksize) : base(a => action(a.Item1, a.Item2, a.Item3, a.Item4, a.Item5), maxstacksize) { }
        public void Start(T1 p1, T2 p2, T3 p3, T4 p4, T5 p5)
        {
            Start(Tuple.Create(p1, p2, p3, p4, p5));
        }
    }
    public class ReturnThread<R> : ThreadWrapper
    {
        private readonly R[] _returnval = new R[1];
        public ReturnThread(Func<R> action) : base(a => ((R[])a)[0] = action())
        { }
        public ReturnThread(Func<R> action, int maxstacksize) : base(a => ((R[])a)[0] = action(), maxstacksize)
        { }
        public new void Start()
        {
            Start(_returnval);
        }
        public new R Join()
        {
            base.Join();
            return _returnval[0];
        }
        public new R Join(int timeOutmilliseconds)
        {
            base.Join(timeOutmilliseconds);
            return _returnval[0];
        }
        public new R Join(TimeSpan timeOut)
        {
            base.Join(timeOut);
            return _returnval[0];
        }
    }
    public class ReturnThread<T1, R> : ThreadWrapper
    {
        private readonly R[] _returnval = new R[1];
        public ReturnThread(Func<T1,R> action) : base(a =>
        {
            var args = (Tuple<T1,R[]>)a;
            args.Item2[0] = action(args.Item1);
        }) { }
        public ReturnThread(Func<T1, R> action, int maxstacksize) : base(a =>
        {
            var args = (Tuple<T1, R[]>)a;
            args.Item2[0] = action(args.Item1);
        }, maxstacksize) { }
        public void Start(T1 param)
        {
            Start(Tuple.Create(param,_returnval));
        }
        public new R Join()
        {
            base.Join();
            return _returnval[0];
        }
        public new R Join(int timeOutmilliseconds)
        {
            base.Join(timeOutmilliseconds);
            return _returnval[0];
        }
        public new R Join(TimeSpan timeOut)
        {
            base.Join(timeOut);
            return _returnval[0];
        }
    }
    public class ReturnThread<T1,T2, R> : ReturnThread<Tuple<T1,T2>, R>
    {
        public ReturnThread(Func<T1, T2, R> action) : base(a=> action(a.Item1,a.Item2)) { }
        public ReturnThread(Func<T1, T2, R> action, int maxstacksize) : base(a => action(a.Item1, a.Item2), maxstacksize) { }
        public void Start(T1 p1, T2 p2)
        {
            Start(Tuple.Create(p1,p2));
        }
    }
    public class ReturnThread<T1, T2, T3, R> : ReturnThread<Tuple<T1, T2, T3>, R>
    {
        public ReturnThread(Func<T1, T2, T3, R> action) : base(a => action(a.Item1, a.Item2, a.Item3)) { }
        public ReturnThread(Func<T1, T2, T3, R> action, int maxstacksize) : base(a => action(a.Item1, a.Item2, a.Item3), maxstacksize) { }
        public void Start(T1 p1, T2 p2, T3 p3)
        {
            Start(Tuple.Create(p1, p2, p3));
        }
    }
    public class ReturnThread<T1, T2, T3, T4, R> : ReturnThread<Tuple<T1, T2, T3, T4>, R>
    {
        public ReturnThread(Func<T1, T2, T3, T4, R> action) : base(a => action(a.Item1, a.Item2, a.Item3, a.Item4)) { }
        public ReturnThread(Func<T1, T2, T3, T4, R> action, int maxstacksize) : base(a => action(a.Item1, a.Item2, a.Item3, a.Item4), maxstacksize) { }
        public void Start(T1 p1, T2 p2, T3 p3, T4 p4)
        {
            Start(Tuple.Create(p1, p2, p3, p4));
        }
    }
}
