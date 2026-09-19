
namespace TeaSpoons.LargeNumbers
{
    using System;
    using UnityEngine;

    /// <summary>
    /// An structure that represents a signed integer
    /// that has 9 significant decimal digits
    /// and can store numbers with an upper limit of 10^65_536.
    /// </summary>
    /// <remarks>
    /// The formula for the represented number is <see cref="FloatingDigits"/> * 10 ^ <see cref="Exponent"/>.
    /// However, <see cref="FloatingDigits"/> should be used cautiously, as only <see cref="RawDigits"/> is exact.
    /// </remarks>
    public readonly struct LargeInt : IComparable<LargeInt>, IEquatable<LargeInt>
    {
        private const byte scaleDigits = 8;
        /// <summary>
        /// The factor by which the <see cref="RawDigits"/> are scaled.
        /// </summary>
        public const long ScaleFactor = 100_000_000; // 10^8
        private const double inverseScaleFactor = 1f / ScaleFactor;

        public static readonly LargeInt Zero = new LargeInt(0, 0);

        /// <summary>
        /// Creates a <see cref="LargeInt"/> without any checks.
        /// Just stores the given parameters in it and calls it a day.
        /// </summary>
        public static LargeInt GetUnchecked(long value, ushort exponent)
        {
            return new LargeInt(value, exponent, true);
        }

        /// <summary>
        /// The fixed-point digits of the number.
        /// Equal to the "actual" digits value, multiplied by <see cref="ScaleFactor"/>.
        /// </summary>
        public readonly long RawDigits;
        public double FloatingDigits => RawDigits * inverseScaleFactor;
        public readonly ushort Exponent;

        /// <summary>
        /// Creates a new <see cref="LargeInt"/>.
        /// </summary>
        /// <param name="digits">
        /// The digits of the number.
        /// The first digit will be before the decimal point, all others are fractional digits.
        /// </param>
        /// <param name="exponent">The exponent of the number. Offsets the <paramref name="digits"/> value by 10 to the power of its value.</param>
        public LargeInt(long digits, ushort exponent)
        {
            if (digits == 0)
            {
                RawDigits = 0;
                Exponent = 0;
                return;
            }

            var sign = digits >= 0 ? 1 : -1;
            digits *= sign;

            var digitsLimit = (long)Math.Pow(10, Math.Min(exponent, scaleDigits) + 1);
            while (digits >= digitsLimit)
            {
                digits /= 10;
            }

            while (digits < ScaleFactor)
            {
                digits *= 10;
            }

            while (digits >= ScaleFactor * 10)
            {
                digits /= 10;
            }

            RawDigits = sign * digits;
            Exponent = exponent;
        }

        private LargeInt(long rawDigits, ushort exponent, bool @unchecked)
        {
            RawDigits = rawDigits;
            Exponent = exponent;
        }

        #region Operators
        public static LargeInt operator +(LargeInt a, LargeInt b)
        {
            (a, b) = Align(a, b);

            return GetNormalized(a.RawDigits + b.RawDigits, a.Exponent);
        }

        public static LargeInt operator -(LargeInt a, LargeInt b)
        {
            (a, b) = Align(a, b);

            return GetNormalized(a.RawDigits - b.RawDigits, a.Exponent);
        }

        public static LargeInt operator *(LargeInt a, LargeInt b)
        {
            return GetNormalized(a.RawDigits * b.RawDigits / ScaleFactor, (ushort)(a.Exponent + b.Exponent));
        }

        public static LargeInt operator /(LargeInt a, LargeInt b)
        {
            if (b.RawDigits == 0) throw new DivideByZeroException();

            var exponentDifference = a.Exponent - b.Exponent;
            if (exponentDifference < 0)
            {
                return Zero;
            }

            return GetNormalized(a.RawDigits * ScaleFactor / b.RawDigits, (ushort)exponentDifference);
        }
        
        public static LargeInt operator *(LargeInt a, double b)
        {
            var scaledProduct = (long)(a.RawDigits * b);
            return GetNormalized(scaledProduct, a.Exponent);
        }

        public static LargeInt operator *(double a, LargeInt b)
        {
            return b * a;
        }
        
        public static LargeInt operator *(LargeInt a, float b)
        {
            return a * (double)b;
        }

        public static LargeInt operator *(float a, LargeInt b)
        {
            return b * a;
        }

        public static double Divide(LargeInt a, LargeInt b)
        {
            if (b.RawDigits == 0) throw new DivideByZeroException();

            var exponentDifference = a.Exponent - b.Exponent;
            var aDigits = a.RawDigits * inverseScaleFactor;
            var bDigits = b.RawDigits * inverseScaleFactor;

            var aHasBiggerExponent = exponentDifference > 0;
            var bHasBiggerExponent = exponentDifference < 0;

            if (aHasBiggerExponent)
            {
                bDigits *= Math.Pow(10, -exponentDifference);
            }
            else if (bHasBiggerExponent)
            {
                aDigits *= Math.Pow(10, exponentDifference);
            }

            return aDigits / bDigits;
        }

        public static bool operator >(LargeInt a, LargeInt b)
        {
            return a.CompareTo(b) > 0;
        }

        public static bool operator <(LargeInt a, LargeInt b)
        {
            return a.CompareTo(b) < 0;
        }

        public static bool operator >=(LargeInt a, LargeInt b)
        {
            return a.CompareTo(b) >= 0;
        }

        public static bool operator <=(LargeInt a, LargeInt b)
        {
            return a.CompareTo(b) <= 0;
        }

        public static LargeInt operator -(LargeInt a)
        {
            return GetUnchecked(-a.RawDigits, a.Exponent);
        }

        public static bool operator ==(LargeInt a, LargeInt b)
        {
            return a.Exponent == b.Exponent &&
                    a.RawDigits == b.RawDigits;
        }

        public static bool operator !=(LargeInt a, LargeInt b)
        {
            return a.Exponent != b.Exponent ||
                    a.RawDigits != b.RawDigits;
        }
        #endregion

        #region Type Conversions
        
        private static readonly long[] Pow10 =
        {
            1L, 10L, 100L, 1_000L, 10_000L, 100_000L,
            1_000_000L, 10_000_000L, 100_000_000L,
            1_000_000_000L, 10_000_000_000L, 100_000_000_000L,
            1_000_000_000_000L, 10_000_000_000_000L, 100_000_000_000_000L,
            1_000_000_000_000_000L, 10_000_000_000_000_000L,
            100_000_000_000_000_000L, 1_000_000_000_000_000_000L
        };
        
        public int GetInt()
        {
            if (TryGetInt(out var result))
            {
                return result;
            }
            throw new Exception("Could not cast LargeInt to int - LargeInt value is out of range!");
        }

        public bool TryGetInt(out int result)
        {
            result = 0;
            
            if (RawDigits == 0)
            {
                return true;
            }
            
            var scaler = Exponent - scaleDigits;
            if (scaler >= 0)
            {
                if (!TryPow10(scaler, out var pow))
                {
                    return false;
                }

                var limit = int.MaxValue / pow;
                if (RawDigits > limit)
                {
                    return false;
                }

                result = (int)(RawDigits * pow);
            }
            else
            {
                if (!TryPow10(-scaler, out var pow))
                {
                    return false;
                }
                
                result = (int)(RawDigits / pow);
            }

            return true;
        }
        
        public long GetLong()
        {
            if (TryGetLong(out var result))
            {
                return result;
            }
            throw new Exception("Could not cast LargeInt to long - LargeInt value is out of range!");
        }

        public bool TryGetLong(out long result)
        {
            result = 0;
            
            if (RawDigits == 0)
            {
                return true;
            }

            var scaler = Exponent - scaleDigits;
            if (scaler >= 0)
            {
                if (!TryPow10(scaler, out var pow))
                {
                    return false;
                }
                
                var absRawDigits = RawDigits >= 0 ? RawDigits : -RawDigits;
                var limit = long.MaxValue / pow;

                if (absRawDigits > limit)
                {
                    return false;
                }
                
                result = RawDigits * pow;
            }
            else
            {
                if (!TryPow10(-scaler, out var pow))
                {
                    return false;
                }

                result = RawDigits / pow;
            }

            return true;
        }

        /// <summary>
        /// Computes 10^k in long without floating point.
        /// Returns false if 10^k does not fit in a long (overflow).
        /// </summary>
        private static bool TryPow10(int k, out long pow)
        {
            if (k < Pow10.Length && k >= 0)
            {
                pow = Pow10[k]; 
                return true;
            }
            
            pow = 0; 
            return false;
        }

        public static implicit operator LargeInt(long value)
        {
            if (value == 0)
            {
                return Zero;
            }
            return new LargeInt(value, (ushort)(GetNumberOfDigits(value) - 1));
        }

        public static explicit operator long(LargeInt value)
        {
            if (!value.TryGetLong(out var result))
            {
                return -1;
            }
            
            return result;
        }
        #endregion

        #region Object Overrides
        public override bool Equals(object obj)
        {
            if (obj is LargeInt other)
            {
                return Exponent == other.Exponent &&
                    (RawDigits % (ScaleFactor * 10)) == (other.RawDigits % (ScaleFactor * 10));
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(RawDigits, Exponent);
        }

        public override string ToString()
        {
            return FloatingDigits.ToString("0.000") + "E" + Exponent;
        }
        #endregion

        #region Interface Operation Implementations
        public int CompareTo(LargeInt other)
        {
            var positive = RawDigits >= 0;
            var otherPositive = other.RawDigits >= 0;

            if (positive != otherPositive)
            {
                return positive ? 1 : -1;
            }

            if (Exponent > other.Exponent)
            {
                return positive ? 1 : -1;
            }

            if (Exponent < other.Exponent)
            {
                return positive ? -1 : 1;
            }

            if (RawDigits == other.RawDigits)
            {
                return 0;
            }

            if (RawDigits > other.RawDigits)
            {
                return 1;
            }

            return -1;
        }

        bool IEquatable<LargeInt>.Equals(LargeInt other)
        {
            return Equals(other);
        }
        #endregion

        #region Static Helpers
        /// <summary>
        ///   <para>Returns the largest of two or more values. When comparing negative values, values closer to zero are considered larger.</para>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public static LargeInt Min(LargeInt a, LargeInt b) => a < b ? a : b;

        /// <summary>
        ///   <para>Returns the largest of two or more values. When comparing negative values, values closer to zero are considered larger.</para>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public static LargeInt Max(LargeInt a, LargeInt b) => a > b ? a : b;

        /// <summary>
        /// Returns a valid <see cref="LargeInt"/>.
        /// If the scaled <paramref name="digits"/> are outside of the valid range [1, 10) or (-10, -1], the parameters get adjusted.
        /// </summary>
        private static LargeInt GetNormalized(long digits, ushort exponent)
        {
            var absDigits = digits >= 0 ? digits : -digits;

            while (absDigits >= ScaleFactor * 10)
            {
                digits /= 10;
                absDigits /= 10;
                exponent++;
            }

            while (absDigits < ScaleFactor && exponent > 0)
            {
                digits *= 10;
                absDigits *= 10;
                exponent--;
            }

            if (absDigits < ScaleFactor)
            {
                digits = 0;
            }

            return GetUnchecked(digits, exponent);
        }

        /// <summary>
        /// Increases the smaller value's exponent to match the bigger one's, and adjusts its digits accordingly.
        /// </summary>
        /// <remarks>
        /// The returned <see cref="LargeInt"/>s might be invalid, as the digits go outside the valid range during scaling.
        /// </remarks>
        private static (LargeInt a, LargeInt b) Align(LargeInt a, LargeInt b)
        {
            if (a.Exponent > b.Exponent)
            {
                var exponentDifference = a.Exponent - b.Exponent;
                var digits = b.RawDigits / (long)Mathf.Pow(10, exponentDifference);
                var exponent = a.Exponent;

                b = GetUnchecked(digits, exponent);
            }
            else if (a.Exponent < b.Exponent)
            {
                var exponentDifference = b.Exponent - a.Exponent;
                var digits = a.RawDigits / (long)Mathf.Pow(10, exponentDifference);
                var exponent = b.Exponent;

                a = GetUnchecked(digits, exponent);
            }

            return (a, b);
        }

        private static byte GetNumberOfDigits(long number)
        {
            number = Math.Abs(number);
            byte result = 0;
            while (number > 0)
            {
                result++;
                number /= 10;
            }
            return result;
        }

        /// <summary>
        /// Parses a <see cref="LargeInt"/> from <paramref name="s"/>.
        /// </summary>
        /// <param name="s">The string representation of the number to parse.</param>
        /// <returns>The parsed <see cref="LargeInt"/>.</returns>
        /// <remarks>
        /// The input must use one of the following formats (an optional leading '+' or '-' is allowed,  
        /// and the exponent separator 'E' is case-insensitive):
        /// <list type="bullet">
        /// <item>
        ///     <term><c>"123"</c></term>
        ///     <description>a <c>long</c> integer.</description>
        /// </item>
        /// <item>
        ///     <term><c>"1.23e2"</c></term>
        ///     <description>a number in scientific notation. The dot MUST BE after the first digit.</description>
        /// </item>
        /// <item>
        ///     <term><c>"123e456"</c></term>
        ///     <description>the dot can be omitted</description></item>
        /// </list>
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="s"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="FormatException">
        /// <paramref name="s"/> is empty or not in a valid format.
        /// </exception>
        public static LargeInt Parse(string s)
        {
            if (s is null)
            {
                throw new ArgumentNullException(nameof(s));
            }

            if (TryParse(s, out var number))
            {
                return number;
            }

            throw new FormatException($"Could not parse a LargeInt from \"{s}\".");
        }

        /// <summary>
        /// Attempts to parse a <see cref="LargeInt"/> from <paramref name="s"/>.
        /// </summary>
        /// <remarks>See <see cref="Parse"/> for the exact list of accepted formats.</remarks>
        /// <param name="s">The string representation of the number to parse.</param>
        /// <param name="value">The parsed <see cref="LargeInt"/> if parsing succeeded; otherwise, <see cref="LargeInt.Zero"/>.</param>
        /// <returns><c>true</c> if <paramref name="s"/> is parsed successfully, <c>false</c> otherwise.</returns>
        public static bool TryParse(string s, out LargeInt value)
        {
            if (s is null)
            {
                value = Zero;
                return false;
            }

            s = s.Trim();
            if (s.Length == 0)
            {
                value = Zero;
                return false;
            }

            s = s.Replace("_", "");

            if (long.TryParse(s, out var number))
            {
                value = number;
                return true;
            }

            try
            {
                var parts = s.ToLower().Split('e', System.StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length != 2)
                {
                    value = Zero;
                    return false;
                }

                var digitsString = parts[0];
                var expectedDecimalPointIndex = 1;
                if (digitsString[0] is '+' or '-')
                {
                    expectedDecimalPointIndex = 2;
                }
                
                if (digitsString[expectedDecimalPointIndex] is '.' or ',')
                {
                    digitsString = digitsString.Remove(expectedDecimalPointIndex, 1);
                }
                
                var digits = long.Parse(digitsString);
                var exponent = ushort.Parse(parts[1]);

                value = new LargeInt(digits, exponent);
                return true;
            }
            catch
            {
                value = Zero;
                return false;
            }
        }

        #endregion
    }
}
