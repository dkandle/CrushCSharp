using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crush
{
    public class MainLoop
    {
        string inputFn;
        string outputFn;
        ProcessBlock myProcessBlock;
        short[] data;

        public MainLoop(string fn)
        {
            inputFn = fn;
            
            int num_elements = 1000; // just a block size
            data = new short[num_elements];

            // make up an output file name -- same as in but add Crushed at the end
            var justFn = Path.GetFileNameWithoutExtension(fn);
            justFn = justFn + "Crushed.wav";
            outputFn = Path.Combine(Path.GetDirectoryName(fn), justFn);

        }

        public int DoProcessing()
        {
            // open the named FIFO
            var stream = File.Open(inputFn, FileMode.Open);
            BinaryReader fin = new BinaryReader(stream, Encoding.UTF8, false);
            if( File.Exists(outputFn) )
            {
                File.Delete(outputFn);
            }
            var oStream = File.Open(outputFn, FileMode.OpenOrCreate);
            BinaryWriter fout = new BinaryWriter(oStream);


            
            int waveHeaderSize = 44;
            

            // do the first read with the header
            byte[] wavHeaderBuf = fin.ReadBytes( waveHeaderSize);
            int vRead = wavHeaderBuf.Length;
            if (vRead < 44)
            {
                // this should never happen, exit
                
                return 1;
            }
            WaveHeader myWaveHeader = new WaveHeader(wavHeaderBuf);
            ProcessBlock myProcessBlock = new ProcessBlock(myWaveHeader, fout); // this will need the header

            
            // now read and toss about 1 second of data while the input stabalizes
            int samplesRead = 0;
            while (samplesRead < myWaveHeader.SampleRate)
            {
                fin.ReadInt16();
                
                samplesRead++;
            }

            int zeroReturnCounter = 0;

            while (true)
            {
                try
                {
                    for (int k = 0; k < data.Length; k++)
                    {
                        data[k] = fin.ReadInt16();
                    }
                } catch (Exception ex)
                {
                    // we are done here
                    return 0;
                }
                
                
                samplesRead = data.Length;
                if (samplesRead > 0)
                {
                    var status = myProcessBlock.ProcessTheBlock(data, samplesRead);
                    if (status.Length>0)
                    {
                        // print the message
                        Console.WriteLine(status);
                        Form1.TheForm.UpdateStatus(status);
                    }

                }

            }

            return 0;
        }
    }
}
