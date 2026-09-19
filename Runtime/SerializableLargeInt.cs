
namespace TeaSpoons.LargeNumbers
{
    using UnityEngine;

    /// <summary>
    /// A serializable struct for setting up a <see cref="LargeInt"/>.
    /// </summary>
    [System.Serializable]
    public struct SerializableLargeInt
    {
        [SerializeField]
        private long digits;
        [SerializeField]
        private ushort exponent;

        public readonly LargeInt Get() => new LargeInt(digits, exponent);

        public static implicit operator SerializableLargeInt(LargeInt number)
        {
            return new SerializableLargeInt(number);
        }

        public SerializableLargeInt(LargeInt number)
        {
            digits = number.RawDigits;
            exponent = number.Exponent;
        }

        public SerializableLargeInt(long number) : this((LargeInt)number)
        {
            
        }
    }
}
