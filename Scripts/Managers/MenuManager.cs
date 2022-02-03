using System.Collections.Generic;
using UpgradeDungeon.Audio;
using UpgradeDungeon.IO;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

namespace UpgradeDungeon.Managers
{
    public class MenuManager : Manager<MenuManager>
    {
        [Header("Main")]
        public GameObject LoadButton;
        public GameObject BeatCup;
        public TMP_Text BestTimeText;

        [Header("Settings")]
        public Slider MusicSlider;
        public Slider SFXSlider;
        public TMP_Dropdown ResDrop;
        Resolution[] resolutions;
        public Toggle FSToggle;
        [System.Serializable]
        public class Settings : SaveFile
        {
            public float MusicVol;
            public float SFXVol;
            public int ResIndex;
            public bool FS;
        }
        public Settings settings;

        // Start is called before the first frame update
        void Start()
        {
            Init(this);
            AudioManager.Instance.SetMusicTrack("Menu");

            Settings LoadedSettings = Saver.Load(settings) as Settings;
            int CurResIndex = 0;
            InitRes(out CurResIndex);
            if(LoadedSettings != null)
            {
                settings = LoadedSettings;
            }
            else
            {
                settings.MusicVol = AudioManager.Instance.GetMusicVolume();
                settings.SFXVol = AudioManager.Instance.GetSFXVolume();
                settings.ResIndex = CurResIndex;
                settings.FS = Screen.fullScreen;
            }
            MusicSlider.value = settings.MusicVol;
            SFXSlider.value = settings.SFXVol;
            ResDrop.value = settings.ResIndex;
            FSToggle.isOn = settings.FS;
        }

        // Update is called once per frame
        void Update()
        {
            LoadButton.SetActive(ProgressManager.Instance.progress.CheckpointIndex >= 0);
            BeatCup.SetActive(ProgressManager.Instance.beatStats.GameBeat);
            BestTimeText.text = "Best Time:" + ProgressManager.Instance.beatStats.BestTime.ToString("0");

            settings.MusicVol = MusicSlider.value;
            settings.SFXVol = SFXSlider.value;
            settings.ResIndex = ResDrop.value;
            settings.FS = FSToggle.isOn;
            settings.Save();
        }

        public void StartGame(bool NewGame)
        {
            if(NewGame)
            {
                ProgressManager.Instance.ResetProgress();
            }
            FadeManager.Instance.FadeTo("Main");
        }
        public void QuitGame()
        {
            QuitManager.Instance.Exit();
        }

        void InitRes(out int CurRes)
        {
            ResDrop.ClearOptions();
            resolutions = Screen.resolutions;
            List<string> Res2String = new List<string>();
            Resolution curRes = Screen.currentResolution;
            int CurIndex = 0;
            for (int R = 0; R < resolutions.Length; R++)
            {
                Resolution Res = resolutions[R];
                string String = Res.width + "x" + Res.height;
                Res2String.Add(String);
                if(Res.width == curRes.width && Res.height == curRes.height)
                {
                    CurIndex = R;
                }
            }
            ResDrop.AddOptions(Res2String);
            CurRes = CurIndex;
        }
        public void UpdateSound()
        {
            AudioManager.Instance.SetMusicVolume(settings.MusicVol);
            AudioManager.Instance.SetSFXVolume(settings.SFXVol);
        }
        public void UpdateScreen()
        {
            Resolution Res = resolutions[ResDrop.value];
            Screen.SetResolution(Res.width, Res.height, FSToggle.isOn);
        }
    }
}