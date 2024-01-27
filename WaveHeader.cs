using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crush
{
    public class WaveHeader
    {
        byte[] WaveHeadDefault;
        public int SampleRate;
        public WaveHeader(byte[] headDefault) {
            WaveHeadDefault = new byte[headDefault.Length];
            // copy over to Default header
            for (int k = 0; k < 44; k++)
            {
                WaveHeadDefault[k] = headDefault[k];
            }
            SampleRate = BitConverter.ToInt32(new byte[4]
            {
                WaveHeadDefault[24], WaveHeadDefault[25], WaveHeadDefault[26],WaveHeadDefault[27]
            });
            ;
        }
        public void SetLength(ref byte[] headOut, int N)
        {
            if (N == 0)
            {
                // just return the full default header
                Buffer.BlockCopy(WaveHeadDefault, 40, headOut, 40, 4);
                Buffer.BlockCopy(WaveHeadDefault, 4, headOut, 4, 4);
               
            }
            else
            {
                Buffer.BlockCopy(BitConverter.GetBytes(N*2), 0, headOut, 40, 4);
                Buffer.BlockCopy(BitConverter.GetBytes(36 + N * 2), 0, headOut, 4, 4);

            }
        }


        public byte[] CopyDefaultHeader()
        {
            byte[] dest = new byte[WaveHeadDefault.Length];
            for (int k = 0; k < 44; k++)
            {
                dest[k] = WaveHeadDefault[k];
            }
            return dest;
        }

    }
}
