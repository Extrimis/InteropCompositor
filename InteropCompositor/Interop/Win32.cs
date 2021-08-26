using System;
using System.Linq;
using System.Reflection;
using System.ComponentModel;
using System.Collections.Concurrent;
using InteropCompositor.Utilities;

namespace InteropCompositor.Interop
{
    public class Win32
    {
        public enum HRESULTS
        {
            S_OK = 0,
            S_FALSE = 1,
            E_FAIL = -2147467259,
            E_NOINTERFACE = -2147467262,
            E_INVALIDARG = -2147024809,
            E_ACCESSDENIED = -2147024891,
            E_NOTIMPL = -2147467263,
            E_NOT_VALID_STATE = -2147019873,
            E_UNEXPECTED = -2147418113,
            E_NOT_SUFFICIENT_BUFFER = -2147024774,
            E_BOUNDS = -2147483637,
            ERROR_CANCELLED = -2147023673,
            ERROR_INTERNAL_ERROR = -2147023537,
            ERROR_INVALID_DATA = -2147024883,
            ERROR_INVALID_OPERATION = -2147020579,
            ERROR_INVALID_NAME = -2147024773,
            ERROR_INVALID_HANDLE = -2147024890,
        }

        public struct HRESULT : IEquatable<HRESULT>, IFormattable
        {
            private static readonly ConcurrentDictionary<int, string> _names = new ConcurrentDictionary<int, string>();
            public int Value
            {
                get;
            }

            public uint UValue => (uint)Value;

            public string Name => ToString("n", null);

            public bool IsError => Value < 0;

            public bool IsSuccess => Value >= 0;

            public bool IsOk => Value == 0;

            public bool IsFalse => Value == 1;

            public HRESULT(int value)
            {
                Value = value;
            }

            public HRESULT(uint value) : this((int)value) { }

            public HRESULT(HRESULTS value) : this((int)value) { }

            public Exception GetException()
            {
                if (Value == -2147352567)
                {
                    Exception error = ComError.GetError();
                    if (error != null)
                    {
                        return error;
                    }
                }

                if (Value < 0)
                {
                    return new Win32Exception(Value);
                }

                return null;
            }

            public int ThrowOnError(bool throwOnError = true)
            {
                if (!throwOnError)
                {
                    return Value;
                }

                Exception exception = GetException();
                if (exception != null)
                {
                    throw exception;
                }

                return Value;
            }

            public override bool Equals(object obj)
            {
                return Value.Equals(obj);
            }

            public override int GetHashCode()
            {
                return Value.GetHashCode();
            }

            public bool Equals(HRESULT other)
            {
                return Value.Equals(other.Value);
            }

            public override string ToString()
            {
                return ToString(null, null);
            }

            public string ToString(string format, IFormatProvider formatProvider)
            {
                switch (format?.ToLowerInvariant())
                {
                    case "i":
                        return Value.ToString();
                    case "u":
                        return UValue.ToString();
                    case "x":
                        return "0x" + Value.ToString("X8");
                    case "n":
                        if (!_names.TryGetValue(Value, out string value2))
                        {
                            int value = Value;
                            value2 = typeof(HRESULTS).GetFields(BindingFlags.Static | BindingFlags.Public).FirstOrDefault((FieldInfo f) => (HRESULTS)f.GetValue(null) == (HRESULTS)value)?.Name;
                            _names[Value] = value2;
                        }
                        return value2;
                    default:
                        string text = ToString("n", formatProvider);
                        if (text != null)
                        {
                            return text + " (0x" + Value.ToString("X8") + ")";
                        }

                        return "0x" + Value.ToString("X8");
                }
            }

            public static bool operator ==(HRESULT left, HRESULT right)
            {
                return left.Value == right.Value;
            }

            public static bool operator !=(HRESULT left, HRESULT right)
            {
                return left.Value != right.Value;
            }

            public static implicit operator HRESULT(int value)
            {
                return new HRESULT(value);
            }

            public static implicit operator HRESULT(uint result)
            {
                return new HRESULT(result);
            }

            public static implicit operator HRESULT(HRESULTS result)
            {
                return new HRESULT(result);
            }

            public static explicit operator uint(HRESULT hr)
            {
                return hr.UValue;
            }

            public static explicit operator int(HRESULT hr)
            {
                return hr.Value;
            }

            public static explicit operator HRESULTS(HRESULT hr)
            {
                return (HRESULTS)hr.UValue;
            }
        }
    }
}
