using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Crush
{
    public class ProcessBlock
    {
        //public static int SAMPLE_RATE =12000;

        float[] outX;

        DeadTime myDeadTime;
        byte[] waveHead;


        ProcessBlock me;
        WaveHeader myWaveHeader;

        int levelCheckSamples = 0;
        double powerSum = 0;
        int samplesWritten;

        BinaryWriter myBinaryWriter;

        public ProcessBlock(WaveHeader wh, BinaryWriter fout)
        {
            myBinaryWriter = fout;
            myWaveHeader = wh;
            waveHead = myWaveHeader.CopyDefaultHeader();
            myDeadTime = new DeadTime(myWaveHeader);

            // write the header to the first file opened
            myBinaryWriter.Write(waveHead);


           
            outX  = new float[50000];
            me = this;
            samplesWritten = 0;
        }

        

        int blockCount = 0;
        int clipCount = 0;
        public string ProcessTheBlock(short[] inX, int len)
        {
            string returnMsg = "";

            blockCount++;

            // convert our input to floats, then pass them all through the DeadTime widget
            // buffer to hold the max outputs per input buf
            



            for (int sIdx = 0; sIdx < len; sIdx++)
            {

                float inFloat = inX[sIdx] / 32768.0f;
                int outValues = myDeadTime.Work(inFloat,ref outX);
                if (outValues > 2)
                {


                    // this means we are starting a new clip

                    
                    clipCount++; // keep count so we can log it

                    if (levelCheckSamples > 1)
                    {

                        
                        powerSum = powerSum / levelCheckSamples * myWaveHeader.SampleRate;
                        double v = 10.0 * Math.Log10(powerSum);
                        returnMsg += " power " + v.ToString("F2") + " Clips count="+clipCount;
                        
                        levelCheckSamples = 0;
                        powerSum = 0; // reset

                    }
                }

                if (outValues < 0)
                {
                    return returnMsg;
                }


                for (int k = 0; k < outValues; k++)
                {
                    float convertedF = outX[k];
                    short x =(short) ( outX[k] * 32767.0f);

                    WriteSample(x);

                    // Keep track of level
                    powerSum += convertedF * convertedF;

                    levelCheckSamples++;

                }


                // check the time and see if it is time to move to the next file
                

            }
            return returnMsg;
        }


        void WriteSample(short x)
        {
            samplesWritten++; // keep track for header rewrite
            short v = x;
            myBinaryWriter.Write(v);
            
        }



        public void CloseFile()
        {
            myWaveHeader.SetLength(ref waveHead, samplesWritten);

            myBinaryWriter.Seek(0,System.IO.SeekOrigin.Begin);
            myBinaryWriter.Write(waveHead);
            myBinaryWriter.Close();
        }









    }
}
