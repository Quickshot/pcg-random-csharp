using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PcgRandom
{
    /// <summary>
    /// 
    /// </summary>
    public class Pcg32 : Random
    {
        /// <summary>
        /// Internal state of the generator.
        /// </summary>
        private ulong _state;

        /// <summary>
        /// Controls which RNG sequence (stream) is selected. Must <strong>always</strong> be odd.
        /// </summary>
        private readonly ulong _inc;

        /// <summary>
        /// Initializes a new instance of the Pcg32 class, using a time-dependent state value and a constant sequence value.
        /// </summary>
        public Pcg32() : this(unchecked((ulong)DateTime.Now.Ticks))
        {
        }

        /// <summary>
        /// Initializes a new instance of the Pcg32 class, using the specified state value and a constant sequence value.
        /// </summary>
        /// <param name="state">Initial state of the instance.</param>
        public Pcg32(int state) : this(unchecked((ulong)state))
        {
        }

        /// <summary>
        /// Initializes a new instance of the Pcg32 class, using the specified state sequence values.
        /// </summary>
        /// <param name="state">Initial state of the instance.</param>
        /// <param name="sequence">Sequence selection for the instance.</param>
        public Pcg32(ulong state, ulong sequence = 0xda3e39cb94b95bdb)
        {
            if (sequence%2 == 1)
            {
                _state = state;
                _inc = sequence;
            }
            else
            {
                throw new ArgumentException($"{nameof(sequence)} is not an odd number.");
            }
        }

        /// <summary>
        /// Returns a random unsigned integer.
        /// </summary>
        /// <returns>A 32-bit random unsigned integer</returns>
        public uint Random()
        {
            ulong oldstate = _state;
            _state = unchecked(oldstate *6364136223846793005U + _inc);
            uint xorshifted = (uint)((oldstate >> 18) ^ oldstate) >> 27;
            int rot = (int)(oldstate >> 59);
            return (xorshifted >> rot) | (xorshifted << (-rot & 31));
        }

        /// <summary>
        /// Returns a non-negative random unsigned integer that is less than the specified maximum.
        /// </summary>
        /// <param name="maxValue">The exclusive upper bound of the random number to be generated. maxValue must be greater than or equal to 0.</param>
        /// <returns>A 32-bit unsigned integer that is greater than or equal to 0, and less than maxValue; that is, the range of return values ordinarily
        /// includes 0 but not maxValue. However, if maxValue equals 0, maxValue is returned.</returns>
        public uint Range(uint maxValue)
        {
            if (maxValue == 0)
            {
                return 0;
            }

            uint threshold = (uint)((0x100000000UL - maxValue) % maxValue);

            for (;;)
            {
                uint r = Random();
                if (r >= threshold)
                {
                    return r % maxValue;
                }
            }
        }

        /// <summary>
        /// Returns a non-negative random integer.
        /// </summary>
        /// <returns>A 32-bit signed integer that is greater than or equal to 0 and less than Int32.MaxValue.</returns>
        public override int Next()
        {
            return (int)Range(int.MaxValue);
        }

        /// <summary>
        /// Returns a non-negative random integer that is less than the specified maximum.
        /// </summary>
        /// <param name="maxValue">The exclusive upper bound of the random number to be generated. maxValue must be greater than or equal to 0. </param>
        /// <returns>A 32-bit signed integer that is greater than or equal to 0, and less than maxValue; that is, the range of return values ordinarily
        /// includes 0 but not maxValue. However, if maxValue equals 0, maxValue is returned.</returns>
        public override int Next(int maxValue)
        {
            if (maxValue < 0)
            {
                throw new ArgumentOutOfRangeException($"{nameof(maxValue)} must be larger or equal to 0.");
            }
            if(maxValue == 0)
            {
                return 0;
            }

            return (int)Range((uint)maxValue);
        }

        /// <summary>
        /// Returns a random integer that is within a specified range.
        /// </summary>
        /// <param name="minValue">The inclusive lower bound of the random number returned.</param>
        /// <param name="maxValue">The exclusive upper bound of the random number returned. maxValue must be greater than or equal to minValue.</param>
        /// <returns></returns>
        public override int Next(int minValue, int maxValue)
        {
            if (!(minValue < maxValue))
            {
                throw new ArgumentOutOfRangeException($"{nameof(minValue)} must be smaller than {nameof(minValue)}.");
            }
            if (minValue == maxValue)
            {
                return minValue;
            }

            var bound = (uint)(maxValue - minValue);
            var result = Range(bound);
            return (int)(result + minValue);
        }

        /// <summary>
        /// Returns a random floating-point number between 0.0 and 1.0.
        /// </summary>
        /// <returns>A double-precision floating point number that is greater than or equal to 0.0, and less than 1.0.</returns>
        protected override double Sample()
        {
            return Random() * Math.Pow(2,-32);
        }

        /// <summary>
        /// Returns a random floating-point number that is greater than or equal to 0.0, and less than 1.0.
        /// </summary>
        /// <returns>A double-precision floating point number that is greater than or equal to 0.0, and less than 1.0.</returns>
        public override double NextDouble()
        {
            return Sample();
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return $"[PCG32 state: {_state}, sequence: {_inc}]";
        }

        /// <summary>
        /// Fills the elements of a specified array of bytes with random numbers.
        /// </summary>
        /// <param name="buffer">An array of bytes to contain random numbers.</param>
        public override void NextBytes(byte[] buffer)
        {
            for (var i = 0; i < buffer.Length; i=i+4)
            {
                var r = BitConverter.GetBytes(Random());
                if (buffer.Length - i >= 4)
                {
                    buffer[i] = r[0];
                    buffer[i + 1] = r[1];
                    buffer[i + 2] = r[2];
                    buffer[i + 3] = r[3];
                }
                else if(buffer.Length - i == 3)
                {
                    buffer[i] = r[0];
                    buffer[i + 1] = r[1];
                    buffer[i + 2] = r[2];
                }
                else if (buffer.Length - i == 2)
                {
                    buffer[i] = r[0];
                    buffer[i + 1] = r[1];
                }
                else if(buffer.Length - i == 1)
                {
                    buffer[i] = r[0];
                }
            }
        }
    }
}
