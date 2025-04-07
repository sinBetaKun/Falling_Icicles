namespace Falling_Icicles
{
    internal class Xoshiro256StarStar
    {
        readonly private ulong[] state = new ulong[4];

        public Xoshiro256StarStar(ulong seed)
        {
            Seed(seed);
        }

        public void Seed(ulong seed)
        {
            // SplitMix64を使用してシード値を4つの状態に分割
            ulong x = seed;
            for (int i = 0; i < 4; i++)
            {
                x ^= x >> 30;
                x *= 0xbf58476d1ce4e5b9UL;
                x ^= x >> 27;
                x *= 0x94d049bb133111ebUL;
                x ^= x >> 31;
                state[i] = x;
            }
        }

        public ulong Next()
        {
            ulong result = RotateLeft(state[1] * 5, 7) * 9;
            ulong t = state[1] << 17;

            state[2] ^= state[0];
            state[3] ^= state[1];
            state[1] ^= state[2];
            state[0] ^= state[3];

            state[2] ^= t;
            state[3] = RotateLeft(state[3], 45);

            return result;
        }

        public double NextDouble()
        {
            return (Next() >> 11) * (1.0 / (1UL << 53));
        }

        private static ulong RotateLeft(ulong x, int k)
        {
            return (x << k) | (x >> (64 - k));
        }
    }
}