using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UpgradeDungeon.Audio
{
    [System.Serializable]
    public class SoundEffect
    {
        public string Name;
        public AudioClip Clip;
        [Range(0f, 1f)]
        public float Volume = 1f;
        public bool Loop;
        AudioSource OutputSource;
        public bool IgnoresAllInteraction;
        public bool IsBackground;

        public void UpdateSettings(Transform SourceContainer)
        {
            if(OutputSource == null)
            {
                OutputSource = new GameObject(Name).AddComponent<AudioSource>();
                OutputSource.transform.SetParent(SourceContainer);
            }

            OutputSource.outputAudioMixerGroup = (IsBackground) ? AudioManager.Instance.MusicOutputGroup : AudioManager.Instance.SFXOutputGroup;
            OutputSource.clip = Clip;
            OutputSource.volume = Volume;
            OutputSource.loop = Loop;
        }

        public void Interact(SoundEffectBehaviour behaviour)
        {
            switch (behaviour)
            {
                case SoundEffectBehaviour.Play:
                    OutputSource.Play();
                    break;
                case SoundEffectBehaviour.Pause:
                    OutputSource.Pause();
                    break;
                case SoundEffectBehaviour.Resume:
                    OutputSource.UnPause();
                    break;
                case SoundEffectBehaviour.Stop:
                    OutputSource.Stop();
                    break;
            }
        }
        public void InteractOneShot(SoundEffectBehaviour behaviour)
        {
            switch (behaviour)
            {
                case SoundEffectBehaviour.Play:
                    if(!OutputSource.isPlaying)
                        OutputSource.Play();
                    break;
                case SoundEffectBehaviour.Pause:
                    if (!OutputSource.isPlaying)
                        OutputSource.Pause();
                    break;
                case SoundEffectBehaviour.Resume:
                    if (!OutputSource.isPlaying)
                        OutputSource.UnPause();
                    break;
                case SoundEffectBehaviour.Stop:
                    if (OutputSource.isPlaying)
                        OutputSource.Stop();
                    break;
            }
        }
    }
}