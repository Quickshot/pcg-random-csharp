using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PcgRandom
{
    // This project can output the Class library as a NuGet Package.
    // To enable this option, right-click on the project and select the Properties menu item. In the Build tab select "Produce outputs on build".
    public class Pcg32 : Random
    {
        private ulong _state;
        private ulong _inc;

        public Pcg32() : this(unchecked((ulong)DateTime.Now.Ticks))
        {
        }

        public Pcg32(int state) : this(unchecked((ulong)state))
        {
        }

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

        public uint Random()
        {
            ulong oldstate = _state;
            _state = unchecked(oldstate *6364136223846793005U + _inc);
            uint xorshifted = (uint)((oldstate >> 18) ^ oldstate) >> 27;
            int rot = (int)(oldstate >> 59);
            return (xorshifted >> rot) | (xorshifted << (-rot & 31));
        }

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

        public override int Next()
        {
            return (int)Range(int.MaxValue);
        }

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

        public override int Next(int minValue, int maxValue)
        {
            if (!(minValue < maxValue))
            {
                throw new ArgumentOutOfRangeException($"{nameof(minValue)} must be smaller than {nameof(minValue)}.");
            }
            uint bound = (uint)(maxValue - minValue);
            uint result = Range(bound);
            return (int)(result + minValue);
        }

        protected override double Sample()
        {
            return Random() * Math.Pow(2,-32);
        }

        public override double NextDouble()
        {
            return Sample();
        }

        public override string ToString()
        {
            return $"[PCG32 state: {_state}, sequence: {_inc}]";
        }

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
