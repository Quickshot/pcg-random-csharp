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

        public uint Range(uint bound)
        {
            uint threshold = (uint)((0x100000000UL - bound) % bound);

            for (;;)
            {
                uint r = Random();
                if (r >= threshold)
                {
                    return r % bound;
                }
            }
        }

        public override int Next()
        {
            return (int)Random();
        }

        public override int Next(int maxValue)
        {
            if (maxValue <= 0)
            {
                throw new ArgumentOutOfRangeException($"{nameof(maxValue)} must be larger than 0.");
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
    }
}
