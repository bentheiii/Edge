using System;
using Edge.SystemExtensions;

namespace Edge.Guard
{
    public enum AccessType {Get, Set, other};
    public class Guard<T> : ICloneable
    {
        public T value { get; set; }
        public Guard() : this(default(T)) { }
        public Guard(T load)
        {
            this.value = load;
        }
        public virtual object Clone()
        {
            var ret = new EventGuard<T>(value);
            return ret;
        }
        public override int GetHashCode()
        {
            return this.value.GetHashCode();
        }
        public static implicit operator T(Guard<T> @this)
        {
            return @this.value;
        }
        public override string ToString()
        {
            return value.ToString();
        }
    }
    public class EventGuard<T> : Guard<T>
    {
        public class EventGuardChangeArgs : EventArgs
        {
            public EventGuardChangeArgs(T oldVal, T newVal)
            {
                this.oldVal = oldVal;
                this.newVal = newVal;
            }
            public T oldVal { get; }
            public T newVal { get; }
        }
        public class EventGuardAccessArgs : EventArgs
        {
            public EventGuardAccessArgs(AccessType accesType)
            {
                AccesType = accesType;
            }
            public AccessType AccesType { get; }
        }
        public delegate void GuardChangedHandler(object sender, EventGuardChangeArgs e);
        public delegate void GuardAccessedHandler(object sender, EventGuardAccessArgs e);
        public delegate void GuardDrawHandler(object sender, EventArgs e);
	    public T EventValue
        {
            get
            {
	            this.accessed?.Invoke(this, new EventGuardAccessArgs(AccessType.Get));
	            this.drawn?.Invoke(this, EventArgs.Empty);
	            return this.value;
            }
            set
            {
                T temp = this.value;
                this.value = value;
	            this.accessed?.Invoke(this, new EventGuardAccessArgs(AccessType.Set));
	            this.changed?.Invoke(this,new EventGuardChangeArgs(temp, value));
            }
        }
	    /// <summary>
        /// is called when the direct value is changed, first parameter([0]) is the new value, second parameter ([1]) is the old value, third is whether the value is equal to the old value
        /// </summary>
        public event GuardChangedHandler changed;
        /// <summary>
        /// is called whenever the value is accessed, first parameter dictates whether the value was accessed from get or set ("get","set")
        /// </summary>
        public event GuardAccessedHandler accessed;
        /// <summary>
        /// is called whenever the value is looked at, has no parameters
        /// </summary>
        public event GuardDrawHandler drawn;
        public EventGuard() : base(default(T)) {}
        public EventGuard(T load) : base (load) {}
        public override object Clone()
        {
            var ret = new EventGuard<T>(value);
            ret.accessed = this.accessed.Copy();
            ret.changed = this.changed.Copy();
            ret.drawn = this.drawn.Copy();
            return ret;
        }
        public override int GetHashCode()
        {
            return this.value.GetHashCode() ^ this.changed.GetInvocationList().GetHashCode() ^
                this.accessed.GetInvocationList().GetHashCode() ^ this.drawn.GetInvocationList().GetHashCode();
        }
        public override string ToString()
        {
            return value.ToString();
        }
    }
}
