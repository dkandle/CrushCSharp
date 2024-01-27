using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crush
{
    public class DeadTime
    {
        static int B_SIZE = 28000;
        static int TONE_SIZE = 6000;
        double ZERO_THRESHOLD = 0.12;
        //static int SAMPLE_RATE = 12000;

        float[] circBuf = new float[B_SIZE];

        float[] returnedValues;
        int returnedValueIdx; // this will point to the oldest entry in the returned value buffer

        float[] tone = new float[TONE_SIZE];
        int oldestIdx;
        int currentXindex; // current index into circ buffer for good sample
        int deadZoneEndIdx; // Index into circ buffer where the last tested sample is located
        float[] digitTable = new float[10]; // translates a digit into a value for output
        float[] timeTag = new float[100];
        int timeTagIdx;
        int sampleNumber;
        int outputBufIdx;
        WaveHeader myWaveHeader;
        int NewStillActiveV;

        enum StripStates { On, Off };
        StripStates StripState;

        public DeadTime(WaveHeader wh)
        {
            myWaveHeader = wh;
            NewStillActiveV = myWaveHeader.SampleRate / 2; // they start off all over
            Init();
        }

        void Init()
        {
            returnedValues = new float[myWaveHeader.SampleRate / 2 + 1]; // keep the last 1/2 second of saved values
            // fill in the digit table
            for (int k = 0; k < 10; k++)
            {
                digitTable[k] = k / 10.0f;
            }


            oldestIdx = 0;
            int i = 0;

            while (i < B_SIZE)
            {
                circBuf[i++] = 0;
            }
            i = 0;
            while (i < returnedValues.Length)
            {
                returnedValues[i++] = 1.0f;
            }
            currentXindex = 0;
            float samplePeriod = 1.0f / myWaveHeader.SampleRate;
            float toneFreq = 400.0f;

            for (int k = 0; k < TONE_SIZE; k++)
            {
                tone[k] = (float)(0.5 * Math.Sin(2 * Math.PI * toneFreq * samplePeriod * k));
            }

            StripState = StripStates.On;
            sampleNumber = 0; 
        }


        /* */
        float[] myOutputBuf;
        public int Work(float xIn, ref float[] outX)
        {
            sampleNumber++;
            myOutputBuf = outX;
            outputBufIdx = 0; // nothing created yet
            int success = ProcessSample(xIn);
            if (success != 0) return success;
            return outputBufIdx;
        }
        int callcount = 0;
        bool StillActive()
        {
            
            if (NewStillActiveV > 50) return true;
            return false;
        }
        
        void NewStillActive() // 1/2 second count
        {
            // returnedValueIdx points to the oldest one
            if (returnedValues[returnedValueIdx] > ZERO_THRESHOLD)
            {
                NewStillActiveV--; // we just pushed this one out
            }
            int k = returnedValueIdx - 1; // then newest one
            if (k < 0) k += returnedValues.Length;
            if (returnedValues[k] > ZERO_THRESHOLD)
            {
                NewStillActiveV++;
            }
        }


        int ProcessSample(float x)
        {
            PushSample(x); // Add to Circ Buffer

            if (StripState == StripStates.Off)
            {
                // we are in an active area, verify still there
                bool isStillActive = StillActive();
                if (isStillActive)
                {
                    PushOut(circBuf[currentXindex]);
                }
                else
                {
                    // CoutWrite( "strip state going on");
                    StripState = StripStates.On;
                    // return -1;
                }

            }
            else
            {
                // We are in a dead zone, verify still there
                bool inDeadZone = VerifyBasic();
                // if(inDeadZone){
                //     std::cout << "basic result in dead zone\n";
                // } else {
                //     std::cout << "basic result NOT IN Dead Zone\n";
                // }
                if (!inDeadZone)
                {
                    // try this alternative version
                    inDeadZone = VerifyPass2();
                    //std::cout << "Running Pass 2\n";
                }
                if (!inDeadZone)
                {
                    // Time to turn on
                    StripState = StripStates.Off;
                    //std::cout << "no longer in strip state on\n";
                    // output the tone and time code & small quite time
                    AddTone();
                    // point the output to the end of the dead zone
                    deadZoneEndIdx = (oldestIdx - myWaveHeader.SampleRate / 2); // go back one second
                    if (deadZoneEndIdx < 0)
                    { // could have wrapped
                        deadZoneEndIdx += B_SIZE;
                    }
                    currentXindex = deadZoneEndIdx;
                    //sprintf(msgBuf,"Setting xIdx=%d\n",currentXindex);
                    //std::cout<<msgBuf;
                    // output the point at the output pointer
                }
            }

            return 0;
        }
        bool IsInGap()
        {
            if (StripState == StripStates.On) return true;
            return false;
        }
        int basicNumberOverThreshold = -1; // starting point

        int BrutForce()
        {
            int nSamplesOver = 0;
            int s = myWaveHeader.SampleRate;
            for (int k = 0; k < s; k++)
            {
                if (Math.Abs(GetSampleAt(-k)) > ZERO_THRESHOLD)
                {
                    nSamplesOver++;
                }
            }
            return nSamplesOver;
        }
        bool VerifyBasic()
        {
            if (basicNumberOverThreshold < 0)
            {
                // we have to initialize the number
                // at most 50 points over threshold in the previous second
                int nSamplesOver = 0;
                int s = myWaveHeader.SampleRate;
                for (int k = 0; k < s; k++)
                {
                    if (Math.Abs(GetSampleAt(-k)) > ZERO_THRESHOLD)
                    {
                        nSamplesOver++;
                    }
                }
                basicNumberOverThreshold = nSamplesOver;
            }
            else
            {
                // just look at the newest and one second back
                if (Math.Abs(GetSampleAt(0)) > ZERO_THRESHOLD)
                {
                    basicNumberOverThreshold++;
                }
                if (Math.Abs(GetSampleAt(-myWaveHeader.SampleRate)) > ZERO_THRESHOLD)
                {
                    basicNumberOverThreshold--;
                }
            }


            if (basicNumberOverThreshold < 50) return true; // we are in a dead zone
            return false;
        }

        int quarterSecondAboveThreshold = 0;
        void QuarterSecondAbove()
        {
            if (quarterSecondAboveThreshold < 0)
            {
                // bad things
                ;
            }
            if( Math.Abs(GetSampleAt(-1))> ZERO_THRESHOLD){
                quarterSecondAboveThreshold++;
            }
            if (Math.Abs(GetSampleAt(-myWaveHeader.SampleRate / 4-1)) > ZERO_THRESHOLD)
            {
                quarterSecondAboveThreshold--;
            }
            
        }
        bool VerifyPass2()
        {
            // return false if the most receint 1/4 second contains 1000 over threshold
            

            if (quarterSecondAboveThreshold > 1000) return false;
            return true;
        }

        float GetSampleAt(int k)
        {

            int n = oldestIdx + k;
            if (n < 0)
            {
                n += B_SIZE;
            }
            else
            {
                n = n % B_SIZE;
            }

            return circBuf[n];
        }


        void PushSample(float x)
        {
            circBuf[oldestIdx] = x;
            oldestIdx = (oldestIdx + 1) % B_SIZE;
            // of we are cirremtly in the on state, then incriment the output pointer
            {
                currentXindex = (currentXindex + 1) % B_SIZE;
            }
            QuarterSecondAbove();
        }


        void AddTone()
        {
            // Move tone 
            //CoutWrite( "adding tone");

            for (int k = 0; k < TONE_SIZE; k++)
            {
                PushOut(tone[k]);

            }

            // Now put a bit of silence so the tone isn't right up against the voice
            for (int k = 0; k < 500; k++)
            {
                PushOut(0);

            }

            EncodeTime();
            // push all of the time code stuff 

            for (int k = 0; k < timeTagIdx; k++)
            {
                PushOut(timeTag[k]);

            }

        }

        void PushOut(float x)
        {
            myOutputBuf[outputBufIdx++] = x;
            returnedValues[returnedValueIdx] =Math.Abs( x);
            returnedValueIdx = (returnedValueIdx + 1) % returnedValues.Length;
            NewStillActive(); // update
        }

        void EncodeTime()
        {
            // fill in the Time Tag stuff
            timeTagIdx = 0;
            // write out the sync code (+-1 * 5)
            for (int k = 0; k < 5; k++)
            {
                timeTag[timeTagIdx++] = 1.0f;
                timeTag[timeTagIdx++] = -1.0f;
            }


            // write out the 9 digits of the sample number
            int s = sampleNumber;
            for (int k = 0; k < 9; k++)
            {
                int d = s % 10;
                PushTag(d);
                s = s / 10;
            }


            // write out the time
            // Get the current time
            DateTime now = DateTime.Now;
            var DateTimeLcl = now.ToLocalTime();


            /*
                char tBuf[100];
                sprintf(tBuf,"mon=%d day=%d hour=%d min=%d",locTime->tm_mon,locTime->tm_mday,
                    locTime->tm_hour,locTime->tm_min);
                WriteLog(tBuf);

            */
            Push2Digits(20);
            int year = DateTimeLcl.Year % 1000;
            Push2Digits(year);
            Push2Digits(DateTimeLcl.Month);
            Push2Digits(DateTimeLcl.Day);
            Push2Digits(DateTimeLcl.Hour);
            Push2Digits(DateTimeLcl.Minute);
            Push2Digits(DateTimeLcl.Second);


        }

        void Push2Digits(int v)
        {
            // used for month, day hour minute, second
            int d1 = v % 10;
            int d2 = (v / 10) % 10;
            PushTag(d1);
            PushTag(d2);
        }

        void PushTag(int v)
        {
            float d = digitTable[v];
            timeTag[timeTagIdx++] = d;
            timeTag[timeTagIdx++] = d;
            timeTag[timeTagIdx++] = -0.5f;
        }
    }
}
