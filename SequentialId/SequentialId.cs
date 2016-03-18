using System;
using System.Diagnostics;
using System.Threading;

namespace SequentialId
{
    /// SequentialId    
    ///     4 byte timestamp - seconds since the the Unix epoch
    ///     4 byte machine identifier
    ///     4 byte process identifier
    ///     4 byte counter
    [Serializable]
    public struct SequentialId : IComparable, IFormattable, IComparable<SequentialId>, IEquatable<SequentialId>
    {
        private static readonly ThreadLocal<Random> _randomGenerator = new ThreadLocal<Random>(() => new Random());
        private static readonly int _staticMachineName = (GetMachineHash() + GetApplicationId());
        private static readonly int _staticPid = GetProcessId();
        private static int _randomSeed = _randomGenerator.Value.Next();
        private static readonly DateTime _unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        private const int _primeInitializer = 17;
        private const int _primeMultiplier = 37;

        public static readonly SequentialId Empty = new SequentialId();

        private readonly int _timestamp;
        private readonly int _machine;
        private readonly int _pid;
        private readonly int _random;

        #region Properties

        public int Timestamp
        {
            get { return _timestamp; }
        }

        public int Machine
        {
            get { return _machine; }
        }

        public int Pid
        {
            get { return _pid; }
        }

        public int RandomSeed
        {
            get { return _random; }
        }

        #endregion

        #region Constructors

        public SequentialId(DateTime timestamp)
            : this(GetTimestampSeconds(timestamp))
        { }

        public SequentialId(int timestamp)
            : this(timestamp, Interlocked.Increment(ref _randomSeed))
        { }

        public SequentialId(DateTime timestamp, int random)
            : this(GetTimestampSeconds(timestamp), random)
        { }

        public SequentialId(int timestamp, int random)
        {
            _timestamp = timestamp;
            _machine = _staticMachineName;
            _pid = _staticPid;
            _random = random;
        }

        public SequentialId(byte[] b)
        {
            if (b == null)
                throw new ArgumentNullException("b");
            if (b.Length != 16)
                throw new ArgumentException("b");

            _timestamp = (b[0] << 24) + (b[1] << 16) + (b[2] << 8) + b[3];
            _machine = (b[4] << 24) + (b[5] << 16) + (b[6] << 8) + b[7];
            _pid = (b[8] << 24) + (b[9] << 16) + (b[10] << 8) + b[11];
            _random = (b[12] << 24) + (b[13] << 16) + (b[14] << 8) + b[15];
        }

        #endregion

        #region Methods

        public byte[] ToByteArray()
        {
            byte[] b = new byte[16];

            b[0] = (byte)(_timestamp >> 24);
            b[1] = (byte)(_timestamp >> 16);
            b[2] = (byte)(_timestamp >> 8);
            b[3] = (byte)(_timestamp);
            b[4] = (byte)(_machine >> 24);
            b[5] = (byte)(_machine >> 16);
            b[6] = (byte)(_machine >> 8);
            b[7] = (byte)(_machine);
            b[8] = (byte)(_pid >> 24);
            b[9] = (byte)(_pid >> 16);
            b[10] = (byte)(_pid >> 8);
            b[11] = (byte)(_pid);
            b[12] = (byte)(_random >> 24);
            b[13] = (byte)(_random >> 16);
            b[14] = (byte)(_random >> 8);
            b[15] = (byte)(_random);

            return b;
        }

        public DateTime ToTimestamp()
        {
            return _unixEpoch.AddSeconds(_timestamp);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = _primeInitializer;
                hashCode = (hashCode * _primeMultiplier) ^ _timestamp;
                //hashCode = (hashCode * _primeMultiplier) ^ _machine;
                //hashCode = (hashCode * _primeMultiplier) ^ _pid;
                hashCode = (hashCode * _primeMultiplier) ^ _random;

                return hashCode;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            if (!(obj is SequentialId))
            {
                throw new ArgumentException("obj must be a SequentialId");
            }

            return Equals((SequentialId)obj);
        }

        public override String ToString()
        {
            return ToString("D", null);
        }

        public int CompareTo(object obj)
        {
            if (obj == null)
            {
                return 1;
            }
            if (!(obj is SequentialId))
            {
                throw new ArgumentException("obj must be a SequentialId");
            }

            return CompareTo((SequentialId)obj);
        }

        public int CompareTo(SequentialId other)
        {

            if (other._timestamp != _timestamp)
            {
                return (_timestamp < other._timestamp) ? -1 : 1;
            }

            return 0;
        }

        public bool Equals(SequentialId other)
        {
            if (_random != other._random)
                return false;
            if (_timestamp != other._timestamp)
                return false;
            if (_machine != other._machine)
                return false;
            if (_pid != other._pid)
                return false;

            return true;
        }

        public String ToString(String format)
        {
            return ToString(format, null);
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            return new Guid(ToByteArray()).ToString(format, formatProvider);
        }

        #endregion

        #region Operators

        public static implicit operator Guid(SequentialId a)
        {
            return new Guid(a.ToByteArray());
        }

        public static implicit operator SequentialId(Guid a)
        {
            if (a == null) return Empty;
            return new SequentialId(a.ToByteArray());
        }


        public static bool operator !=(SequentialId a, SequentialId b)
        {
            return !(a == b);
        }

        public static bool operator ==(SequentialId a, SequentialId b)
        {
            return a.Equals(b);
        }


        public static bool operator !=(Guid x, SequentialId y)
        {
            return !(x == y);
        }

        public static bool operator ==(Guid x, SequentialId y)
        {
            return (x == y);
        }

        public static bool operator !=(SequentialId x, Guid y)
        {
            return !(x == y);
        }

        public static bool operator ==(SequentialId x, Guid y)
        {
            return (x == y);
        }


        public static bool operator !=(byte[] x, SequentialId y)
        {
            return !(x == y);
        }

        public static bool operator ==(byte[] x, SequentialId y)
        {
            return (x == y);
        }

        public static bool operator !=(SequentialId x, byte[] y)
        {
            return !(x == y);
        }

        public static bool operator ==(SequentialId x, byte[] y)
        {
            return (x == y);
        }       

        #endregion

        #region Static Methods

        public static SequentialId NewId()
        {
            return new SequentialId(DateTime.UtcNow);
        }

        private static int GetMachineHash()
        {
            return Environment.MachineName.GetHashCode();
        }

        private static int GetProcessId()
        {
            return Process.GetCurrentProcess().Id;
        }

        private static int GetApplicationId()
        {
            return AppDomain.CurrentDomain.Id;
        }

        // TODO Manipulate Environment.TickCount for performance.
        private static int GetTimestampSeconds(DateTime timestamp)
        {
            return Convert.ToInt32(Math.Floor((timestamp - _unixEpoch).TotalSeconds));
        }

        #endregion

    }
}