using System.Collections;
using System.Collections.Generic;
using UpgradeDungeon.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UpgradeDungeon.Managers
{
    public class ProgressManager : Manager<ProgressManager>
    {
        [System.Serializable]
        public class Progress : SaveFile
        {
            [Space]
            public int CheckpointIndex;
            public int Lives = 3;
            public int Score;
            public bool[] UnlockedKeybinds;
            public string[] Abillities;
            public List<string> InvokedEvents;
            public List<string> BrokenBlocks;
            public bool GravityFlipped;
            public bool Crowching;
            public float time;
        }
        [System.Serializable]
        public class BeatStats : SaveFile
        {
            [Space]
            public float BestTime;
            public bool GameBeat;
        }
        [HideInInspector]
        public Progress progress;
        [Space]
        public Progress StartProgress;
        public BeatStats beatStats;
        

        // Start is called before the first frame update
        void Start()
        {
            Init(this);

            Progress LoadedProgress = Saver.Load(progress) as Progress;
            if(LoadedProgress != null)
            {
                progress = LoadedProgress;
            }
            else
            {
                ResetProgress();
            }

            BeatStats LoadedBeatStats = Saver.Load(beatStats) as BeatStats;
            if(LoadedBeatStats != null)
            {
                beatStats = LoadedBeatStats;
            }
            else
            {
                beatStats.BestTime = 0f;
                beatStats.GameBeat = false;
            }

            FadeManager.Instance.FadeTo("Menu");
        }

        // Update is called once per frame
        void Update()
        {
            progress.Save();
            beatStats.Save();
        }

        public void ResetProgress()
        {
            Debug.Log("Reset");
            progress.CheckpointIndex = StartProgress.CheckpointIndex;
            progress.Lives = StartProgress.Lives;
            progress.Score = StartProgress.Score;
            progress.UnlockedKeybinds = StartProgress.UnlockedKeybinds;
            progress.Abillities = StartProgress.Abillities;
            GameManager.CloneList<string>(progress.InvokedEvents, StartProgress.InvokedEvents);
            GameManager.CloneList<string>(progress.BrokenBlocks, StartProgress.BrokenBlocks);
            progress.GravityFlipped = StartProgress.GravityFlipped;
            progress.Crowching = StartProgress.Crowching;
            progress.time = StartProgress.time;
        }
    }
}