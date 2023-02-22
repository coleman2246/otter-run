using System.Collections;
using System.Collections.Generic;
using Unity.Jobs;
using UnityEngine;
using Unity.Mathematics;
using System.Linq;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;




public enum SoundBands
{
    SubBass,
    Bass,
    LowerMidRange,
    MidRange,
    HigherMidRange,
    Presence,
    Brilliance
}


[RequireComponent (typeof (AudioSource))]
public class AudioAnalysis : MonoBehaviour
{
    private AudioSource audioSource;
    public float[] sampleBuffer;
    public static int sampleBufferSize = 1024;
    public List<float[]> dftWindow = new List<float[]>();
    public float[] powerLevelIncrease;
    AudioClip clip;
    public Dictionary<SoundBands,float> binnedPowerLevelIncreases = new Dictionary<SoundBands, float>();
    public Dictionary<SoundBands, float> highestDeltaPercent = new Dictionary<SoundBands,float>(); // what percent into the window the highest delta happens at 
    public float audioLen; // units are secs
    public int numWindows;
    public float binScaleFactor;
    public float powerScaleFactor = 10e-9f;
    public const float sampleSecs = 1.0f/10.0f; // each sample should be 1/10 of a sec

    public BurstDft dft; 
    // Start is called before the first frame update
    //
    void Awake()
    {
        audioSource = GetComponent<AudioSource>(); 


        clip = audioSource.clip;
        audioLen = audioSource.clip.length;


        dft  = new BurstDft(sampleBufferSize * 2);
        sampleBuffer = new float[sampleBufferSize];

        powerLevelIncrease = new float[sampleBufferSize];

        numWindows = Mathf.CeilToInt(1.0f/ sampleSecs * LevelUnit.unitTime);
        binScaleFactor = Mathf.FloorToInt(clip.frequency/ sampleBufferSize);

        foreach(SoundBands band in SoundBands.GetValues(typeof(SoundBands)))
        {
            binnedPowerLevelIncreases[band] = 0f;
            highestDeltaPercent[band] = 0f;
        }

    }

    public void GenerateAnalysis(int unit)
    {
        dftWindow = new List<float[]>();

        float startSample = (unit*LevelUnit.unitTime) * clip.frequency;
        float endSample = startSample + (LevelUnit.unitTime * clip.frequency);

        float currentSample = startSample;

        //Debug.Log($"Ite  {sampleBufferSize} {((endSample - startSample) / (float)sampleBufferSize) }");
        //Debug.Log($"Start: {startSample} End: {endSample}");

        float[] window = new float[sampleBufferSize];

        for (int n = 0; n < sampleBufferSize; n++)
        {
            // this is formula for Blackman window (no idea how they came up with this)
            window[n] = 0.42f - 0.5f * Mathf.Cos(2 * Mathf.PI * n / (sampleBufferSize - 1)) + 0.08f * Mathf.Cos(4 * Mathf.PI * n / (float)(sampleBufferSize - 1));
        }

        clip.GetData(sampleBuffer,Mathf.FloorToInt(startSample));
        NativeArray<float> conv = new NativeArray<float>(sampleBuffer,Allocator.TempJob);

        while(currentSample < endSample)
        {
            if(currentSample >= clip.samples)
            {
                break;
            }

            float start = Time.realtimeSinceStartup;
            clip.GetData(sampleBuffer,Mathf.FloorToInt(currentSample));

            // need to apply windowing to reduce spectral leakage; 

            for (int n = 0; n < sampleBufferSize; n++)
            {
                sampleBuffer[n]  = sampleBuffer[n] * window[n];
            }

            conv.CopyFrom(sampleBuffer);
            dft.Transform(conv);
            dftWindow.Add(dft.Spectrum.ToArray());
            currentSample += sampleBufferSize;
            //Debug.Log(start - Time.realtimeSinceStartup);
        }

        GeneratePowerLevelIncreases();

        conv.Dispose();
    }



    void GeneratePowerLevelIncreases()
    {
        SoundBands currentBand = FrequencyToBand(0);
        SoundBands prevBand = FrequencyToBand(0);

        float powerIncrease = 0;
        int n = 0;

        for(int i = 0; i < sampleBufferSize; i+=1)
        {

            n += 1;
            currentBand = FrequencyToBand(Mathf.FloorToInt(i * binScaleFactor));

            float basePowerLevel = dftWindow[0][i];
            float highestIncrease = 0;
            int currentHighestIndex = 0;
            //Debug.Log($"BasePower Level {dftWindow[0][i]}");

            for(int currWindow = 1; currWindow  < dftWindow.Count; currWindow++)
            {
                float deltaPower = powerScaleFactor*( dftWindow[currWindow][i] - basePowerLevel);


                if(deltaPower > highestIncrease )
                {
                    highestIncrease  = deltaPower;
                    currentHighestIndex = currWindow;

                }
                                
                if(deltaPower > 0)
                {
                    //Debug.Log(dftWindow[currWindow][i]);
                    powerIncrease += deltaPower/ (float)dftWindow.Count;
                }
            }

            if(currentBand != prevBand)
            {
                binnedPowerLevelIncreases[currentBand] = (float)powerIncrease / (float)n;
                highestDeltaPercent[currentBand] = (float)currentHighestIndex/(float)dftWindow.Count;
                n = 0;
            }
            prevBand = currentBand;
            //powerLevelIncrease[i] = powerIncrease;
        }

    }

    public void CleanUpDFT()
    {
        dft.Dispose();
    }

    // Update is called once per frame
    void Update()
    {
        //UpdateSpectrum();
    }

    static SoundBands FrequencyToBand(int freq)
    {
        if(freq < 60)
        {
            return SoundBands.SubBass;
        }
        else if(freq < 250)
        {
            return SoundBands.Bass;
        }
        else if(freq < 500)
        {
            return SoundBands.LowerMidRange;
        }
        else if(freq < 2000)
        {
            return SoundBands.MidRange;
        }
        else if(freq < 4000)
        {
            return SoundBands.HigherMidRange;
        }
        else if(freq < 6000)
        {
            return SoundBands.Presence;
        }
        else 
        {
            return SoundBands.Brilliance;
        }
        // frequency bands range https://www.headphonesty.com/2020/02/audio-frequency-spectrum-explained/
        

    }
}
