using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace V2DRuntime.Audio
{
    public delegate void AudioHandler(Cue cue);

    /// <summary>
    /// This class just is a central place to deal with loading and looking up resources and their attributes.
    /// </summary>
    public class AudioManager
    {
        private AudioEngine audioEngine;
        private WaveBank waveBank;
        private SoundBank soundBank;

        private Dictionary<string, Cue> cueList = new Dictionary<string, Cue>();
        private Dictionary<Cue, AudioHandler> callbackList = new Dictionary<Cue, AudioHandler>();

        public SoundBank SoundBank { get { return soundBank; } }

        public AudioManager(string audioEnginePath, string waveBankPath, string soundBankPath)
        {
            Initialize(audioEnginePath, waveBankPath, soundBankPath);
        }

        private void Initialize(string audioEnginePath, string waveBankPath, string soundBankPath)
        {
            audioEngine = new AudioEngine(audioEnginePath);//@"Content\audio\smuck.xgs");
            waveBank = new WaveBank(audioEngine, waveBankPath);//@"Content\audio\Wave Bank.xwb");
            soundBank = new SoundBank(audioEngine, soundBankPath);//@"Content\audio\Sound Bank.xsb");
        }

        private Cue GetCue(string soundName)
        {
            Cue result;
            if (cueList.ContainsKey(soundName))
            {
                result = cueList[soundName];
                if (!result.IsCreated)
                {
                    result = soundBank.GetCue(soundName);
                    cueList[soundName] = result;
                }
            }
            else
            {
                result = soundBank.GetCue(soundName);
                cueList[soundName] = result;
            }

            return result;
        }
        public bool IsPlaying(string soundName)
        {
            bool result = false;

            if (cueList.ContainsKey(soundName) && cueList[soundName].IsPlaying)
            {
                result = true;
            }

            return result;
        }
        public Cue PlaySound(string soundName, AudioListener audioListener, AudioEmitter audioEmitter)
        {
            Cue result = GetCue(soundName);
            result.Apply3D(audioListener, audioEmitter);
            result.Play();
            return result;
        }
        public Cue PlaySound(string soundName, AudioListener audioListener, AudioEmitter audioEmitter, AudioHandler callback)
        {
            Cue result = PlaySound(soundName, audioListener, audioEmitter);

            // todo: need to make this multicast
            if (callbackList[result] != null)
            {
                callbackList[result].Invoke(result); 
            }

            callbackList[result] = callback;  
            return result;
        }
        public Cue PlaySound(string soundName)
        {
            Cue result = GetCue(soundName);      
            result.Play();
            return result;
        }
        public Cue PlaySound(string soundName, AudioHandler callback)
        {
            Cue result = PlaySound(soundName);

            // todo: need to make this multicast
            if (callbackList.ContainsKey(result))
            {
                callbackList[result].Invoke(result); 
            }

            callbackList[result] = callback;  
            return result;
        }

        public void StopSound(string soundName)
        {
            if (cueList.ContainsKey(soundName) && cueList[soundName].IsPlaying)
            {
                cueList[soundName].Stop(AudioStopOptions.AsAuthored);
            }
        }
        public void PauseSound(string soundName)
        {
            if (cueList.ContainsKey(soundName))
            {
                cueList[soundName].Pause();
            }
        }
        public void ResumeSound(string soundName)
        {
            if (cueList.ContainsKey(soundName))
            {
                cueList[soundName].Resume();
            }
        }

        public void Update()
        {
            audioEngine.Update();
            if (callbackList.Count > 0)
            {
                List<Cue> removeCue = new List<Cue>();
                foreach (Cue c in callbackList.Keys)
                {
                    if (c.IsStopped)
                    {
                        callbackList[c].Invoke(c);
                        removeCue.Add(c);
                    }
                }
                foreach (Cue c in removeCue)
                {
                    callbackList.Remove(c);
                }
            }
        }
    }

}
