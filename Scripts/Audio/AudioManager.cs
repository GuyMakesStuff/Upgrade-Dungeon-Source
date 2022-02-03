using System;
using UpgradeDungeon.Managers;
using UnityEngine.Audio;
using UnityEngine;

namespace UpgradeDungeon.Audio
{
    public class AudioManager : Manager<AudioManager>
    {
        [Header("Sound Effects")]
        public SoundEffect[] SoundEffects;
        public AudioMixerGroup SFXOutputGroup;

        [Header("Music")]
        public AudioClip[] MusicClips;
        public AudioMixerGroup MusicOutputGroup;
        AudioSource MusicOutput;

        // Start is called before the first frame update
        void Awake()
        {
            Init(this);

            MusicOutput = new GameObject("Background And Music").AddComponent<AudioSource>();
            MusicOutput.transform.SetParent(transform);
            MusicOutput.outputAudioMixerGroup = MusicOutputGroup;
            MusicOutput.volume = 1f;
            MusicOutput.loop = true;

            Update();
        }

        // Update is called once per frame
        void Update()
        {
            foreach (SoundEffect SFX in SoundEffects)
            {
                SFX.UpdateSettings(transform);
            }
        }

        public void InteractWithSFX(string SFXName, SoundEffectBehaviour behaviour)
        {
            SoundEffect soundEffect = Array.Find(SoundEffects, SoundEffect => SoundEffect.Name == SFXName);
            if(soundEffect == null)
            {
                Debug.LogError("Sound Effect " + SFXName + " Does Not Exist In The Audio Manager!");
                return;
            }

            soundEffect.Interact(behaviour);
        }
        public void InteractWithSFXOneShot(string SFXName, SoundEffectBehaviour behaviour)
        {
            SoundEffect soundEffect = Array.Find(SoundEffects, SoundEffect => SoundEffect.Name == SFXName);
            if (soundEffect == null)
            {
                Debug.LogError("Sound Effect " + SFXName + " Does Not Exist In The Audio Manager!");
                return;
            }

            soundEffect.InteractOneShot(behaviour);
        }
        public void InteractWithAllSFX(SoundEffectBehaviour behaviour)
        {
            foreach (SoundEffect SFX in SoundEffects)
            {
                if(!SFX.IgnoresAllInteraction)
                {
                    SFX.Interact(behaviour);
                }
            }
        }
        public void InteractWithAllSFXOneShot(SoundEffectBehaviour behaviour)
        {
            foreach (SoundEffect SFX in SoundEffects)
            {
                if (!SFX.IgnoresAllInteraction)
                {
                    SFX.InteractOneShot(behaviour);
                }
            }
        }

        public void SetMusicTrack(string TrackName)
        {
            AudioClip Clip = Array.Find(MusicClips, AudioClip => AudioClip.name == TrackName);
            if(Clip == null)
            {
                Debug.LogError("Music Clip " + TrackName + " Does Not Exist In The Audio Manager!");
                MuteMusic();
                return;
            }
            else if(Clip == MusicOutput.clip)
            {
                Debug.LogError("Music Clip " + TrackName + " Is Already Playing!");
                return;
            }

            MusicOutput.Stop();
            MusicOutput.clip = Clip;
            MusicOutput.Play();
        }
        public void MuteMusic()
        {
            MusicOutput.Stop();
            MusicOutput.clip = null;
        }
        public void InteractWithMusic(SoundEffectBehaviour behaviour)
        {
            switch (behaviour)
            {
                case SoundEffectBehaviour.Play:
                    MusicOutput.Play();
                    break;
                case SoundEffectBehaviour.Pause:
                    MusicOutput.Pause();
                    break;
                case SoundEffectBehaviour.Resume:
                    MusicOutput.UnPause();
                    break;
                case SoundEffectBehaviour.Stop:
                    MusicOutput.Stop();
                    break;
            }
        }

        public void SetMusicVolume(float Value)
        {
            MusicOutputGroup.audioMixer.SetFloat("MusicVol", Value);
        }
        public float GetMusicVolume()
        {
            float Output = 0f;
            MusicOutputGroup.audioMixer.GetFloat("MusicVol", out Output);
            return Output;
        }
        public void SetSFXVolume(float Value)
        {
            SFXOutputGroup.audioMixer.SetFloat("SFXVol", Value);
        }
        public float GetSFXVolume()
        {
            float Output = 0f;
            SFXOutputGroup.audioMixer.GetFloat("SFXVol", out Output);
            return Output;
        }
    }
}