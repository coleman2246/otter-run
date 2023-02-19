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
    public int sampleBufferSize;
    public List<float[]> window = new List<float[]>();
    public float[] powerLevelIncrease;
    public Dictionary<SoundBands,float> binnedPowerLevelIncreases = new Dictionary<SoundBands, float>();
    public float audioLen; // units are secs
    public float frequency;
    public int numWindows;
    public float binScaleFactor;
    public float powerScaleFactor = 1e6f;
    public const float sampleSecs = 1.0f/10.0f; // each sample should be 1/10 of a sec

    public BurstDft t; 
    // Start is called before the first frame update
    //
    void Awake()
    {
        audioSource = GetComponent<AudioSource>(); 


        audioLen = audioSource.clip.length;
        frequency = audioSource.clip.frequency;
        sampleBufferSize = 1024;


        //sampleBufferSize = Mathf.CeilToInt(frequency * sampleSecs);
        /*
        if( (sampleBufferSize & ( sampleBufferSize-1)) != 0)
        {
            sampleBufferSize =  (int)Mathf.Pow(2, Mathf.CeilToInt(Mathf.Log(sampleBufferSize)/Mathf.Log(2)));

        }
        */

       // each sample should be 1/10th of a sec 
       //
        t  = new BurstDft(sampleBufferSize);
        sampleBuffer = new float[sampleBufferSize];
        powerLevelIncrease = new float[sampleBufferSize];

        numWindows = Mathf.CeilToInt(1.0f/ sampleSecs * LevelUnit.unitTime);
        binScaleFactor = Mathf.FloorToInt(frequency/ sampleBufferSize);

        foreach(SoundBands band in SoundBands.GetValues(typeof(SoundBands)))
        {
            binnedPowerLevelIncreases[band] = 0f;
        }

        Analayze();
    }

    void Analayze()
    {

        AudioClip clip = audioSource.clip;
        clip.GetData(sampleBuffer,(int)frequency*3);

        NativeArray<float> conv = new NativeArray<float>(sampleBuffer,Allocator.TempJob);

        t.Transform(conv);
        for(int i = 0; i < 512; i++)
        {

        Debug.Log(t.Spectrum[i]);
        }
        conv.Dispose();
        t.Dispose();
    }

    // Update is called once per frame
    void Update()
    {
        //UpdateSpectrum();
    }

    void UpdateSpectrum()
    {
        audioSource.GetSpectrumData(sampleBuffer,0,FFTWindow.Blackman);

        window.Add(sampleBuffer);


        if(window.Count > numWindows)
        {
            window.RemoveAt(0);
        }

        UpdatePowerLevelIncrease();

    }


    void UpdatePowerLevelIncrease()
    {
        SoundBands currentBand = FrequencyToBand(0);
        SoundBands prevBand = FrequencyToBand(0);

        float powerIncrease = 0;
        int n = 0;

        for(int i = 0; i < sampleBufferSize; i+=1)
        {

            n += 1;
            currentBand = FrequencyToBand(Mathf.FloorToInt(i * binScaleFactor));

            float basePowerLevel = window[0][i] * powerScaleFactor;

            for(int currWindow = 1; currWindow  < window.Count; currWindow++)
            {
                float deltaPower = powerScaleFactor*( (powerScaleFactor * window[currWindow][i]) - basePowerLevel);
                                
                if(deltaPower > 0)
                {
                    powerIncrease += deltaPower;
                }
            }

            if(currentBand != prevBand)
            {
                binnedPowerLevelIncreases[currentBand] = (float)powerIncrease / (float)n;
                n = 0;
            }

            prevBand = currentBand;
            //powerLevelIncrease[i] = powerIncrease;
        }

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
