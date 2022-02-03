using UpgradeDungeon.Audio;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine;
using UpgradeDungeon.Visuals;
using TMPro;

namespace UpgradeDungeon.Managers
{
    public class GameManager : Manager<GameManager>
    {
        [Space]
        public Player player;
        public Transform FXContainer;

        [Header("Score")]
        public int Score;
        public TMP_Text ScoreText;
        public GameObject CoinsConatiner;

        [Header("Powerups")]
        public TMP_Text PowerupText;
        Powerup.PowerupProfile CurPowerup;
        float PowerupTimer;

        [Header("Checkpoints")]
        public TMP_Text LivesCounter;
        public int Lives;
        public Checkpoint[] Checkpoints;
        public int CheckpointIndex;
        [HideInInspector]
        public List<string> InvokedEvents;
        [HideInInspector]
        public List<string> BrokenBlocks;

        [Header("Mini Menus")]
        public GameObject PauseMenu;
        [HideInInspector]
        public bool IsPaused;
        public GameObject GameOverMenu;
        public GameObject WinMenu;

        [Header("Other")]
        public Animator CheckpointText;
        public Animator HitOverlay;
        public GameObject ScorePopUpPrefab;
        float PlayTime;
        public TMP_Text TimeText;
        public GameObject NewBestTimeText;
        public GameObject ExitBlocker;
        bool CanCountTime;

        // Start is called before the first frame update
        void Awake()
        {
            Init(this);

            AudioManager.Instance.SetMusicTrack("Main");

            for (int C = 0; C < Checkpoints.Length; C++)
            {
                Checkpoints[C].Index = C;
            }

            CheckpointIndex = ProgressManager.Instance.progress.CheckpointIndex;
            Lives = ProgressManager.Instance.progress.Lives;
            Score = ProgressManager.Instance.progress.Score;
            CloneList<string>(InvokedEvents, ProgressManager.Instance.progress.InvokedEvents);
            CloneList<string>(BrokenBlocks, ProgressManager.Instance.progress.BrokenBlocks);
            CanCountTime = true;
            PlayTime = ProgressManager.Instance.progress.time;
            if(CheckpointIndex >= 0)
            {
                player.transform.position = Checkpoints[CheckpointIndex].transform.position;
            }

            foreach (BlackBlock BB in FindObjectsOfType<BlackBlock>())
            {
                if(BrokenBlocks.Contains(BB.gameObject.name))
                {
                    Destroy(BB.gameObject);
                }
            }
        }

        // Update is called once per frame
        void Update()
        {
            bool CanScore = player.HasAbillity("Score");
            ScoreText.gameObject.SetActive(CanScore);
            CoinsConatiner.SetActive(CanScore);

            ScoreText.text = "Score:" + Score.ToString("000");
            LivesCounter.text = "x" + Lives.ToString();
            TimeText.text = "Time:" + PlayTime.ToString("0");

            PowerupText.gameObject.SetActive(CurPowerup != null);
            if(CurPowerup != null)
            {
                PowerupText.text = CurPowerup.Name + ":" + PowerupTimer.ToString("0.0");
                PowerupTimer -= Time.deltaTime;
                if(PowerupTimer <= 0)
                {
                    ClearPowerups();
                }
            }

            if(player.GetKeybind("Pause").IsDown() && !IsPaused)
            {
                if(!FadeManager.Instance.IsFaded)
                {
                    IsPaused = true;
                    PlaySelectSound();
                }
            }
            Time.timeScale = (IsPaused) ? 0f : 1f;
            PauseMenu.SetActive(IsPaused);
            SoundEffectBehaviour SFXBehaviour = (IsPaused) ? SoundEffectBehaviour.Pause : SoundEffectBehaviour.Resume;
            AudioManager.Instance.InteractWithAllSFXOneShot(SFXBehaviour);
            AudioManager.Instance.InteractWithMusic(SFXBehaviour);
            if((!IsPaused && !DialogManager.IsOpen) && !FadeManager.Instance.IsFaded)
            {
                if(CanCountTime)
                {
                    PlayTime += Time.deltaTime;
                }
            }

            ExitBlocker.SetActive(player.IsFlipped);
        }

        public void ClearPowerups()
        {
            if(CurPowerup != null)
            {
                CurPowerup.OnEnd.Invoke();
                CurPowerup = null;
            }
        }

        public void Kill()
        {
            if(!FadeManager.Instance.IsFaded)
            {
                Lives--;
                HitOverlay.SetTrigger("Fade");
                if(Lives <= 0)
                {
                    ProgressManager.Instance.ResetProgress();
                    SpawnFX(player.DieFX, player.transform.position);
                    player.GetKeybind("Pause").Enabled = false;
                    CanCountTime = false;
                    Destroy(player.gameObject);
                    GameOverMenu.SetActive(true);
                    AudioManager.Instance.MuteMusic();
                    AudioManager.Instance.InteractWithSFX("Die", SoundEffectBehaviour.Play);
                }
                else
                {
                    Save();
                    ClearPowerups();
                    player.transform.position = Checkpoints[CheckpointIndex].transform.position;
                    foreach (Powerup PU in FindObjectsOfType<Powerup>(true))
                    {
                        PU.gameObject.SetActive(PU.PreviouslyEnabled);
                    }
                    AudioManager.Instance.InteractWithSFX("Hit", SoundEffectBehaviour.Play);
                }
            }
        }

        public void Save()
        {
            Debug.Log("Saved");
            ProgressManager.Instance.progress.CheckpointIndex = CheckpointIndex;
            ProgressManager.Instance.progress.Lives = Lives;
            ProgressManager.Instance.progress.Score = Score;
            ProgressManager.Instance.progress.UnlockedKeybinds = player.GetUnlockedKeybinds();
            ProgressManager.Instance.progress.Abillities = player.Abillities.ToArray();
            CloneList<string>(ProgressManager.Instance.progress.InvokedEvents, InvokedEvents);
            CloneList<string>(ProgressManager.Instance.progress.BrokenBlocks, BrokenBlocks);
            ProgressManager.Instance.progress.GravityFlipped = player.IsFlipped;
            ProgressManager.Instance.progress.Crowching = player.IsCrowching;
            ProgressManager.Instance.progress.time = PlayTime;
        }

        public static void CloneList<T>(List<T> Target, List<T> WhatToClone)
        {
            if(Target != null)
            {
                Target.Clear();
            }
            else
            {
                Target = new List<T>();
            }

            for (int L = 0; L < WhatToClone.Count; L++)
            {
                Target.Add(WhatToClone[L]);
            }
        }

        public void Win()
        {
            Resume();
            CanCountTime = false;
            FindObjectOfType<CameraFollow>().Target = null;
            player.GetKeybind("Pause").Enabled = false;
            ProgressManager.Instance.ResetProgress();
            WinMenu.SetActive(true);
            AudioManager.Instance.MuteMusic();
            ProgressManager.Instance.beatStats.GameBeat = true;
            if(PlayTime < ProgressManager.Instance.beatStats.BestTime || ProgressManager.Instance.beatStats.BestTime <= 0f)
            {
                ProgressManager.Instance.beatStats.BestTime = PlayTime;
                NewBestTimeText.SetActive(true);
            }
        }

        public void Resume()
        {
            IsPaused = false;
        }
        public void Reload()
        {
            Resume();
            FadeManager.Instance.FadeTo(SceneManager.GetActiveScene().name);
        }
        public void Menu()
        {
            Resume();
            FadeManager.Instance.FadeTo("Menu");
        }
        public void QuitGame()
        {
            QuitManager.Instance.Exit();
        }

        public void SpawnFX(GameObject FX, Vector3 Position)
        {
            GameObject NewFX = Instantiate(FX, Position, Quaternion.identity, FXContainer);
            Destroy(NewFX, NewFX.GetComponent<ParticleSystem>().main.duration);
        }
        public void SpawnPopUp(Vector3 Position, string Text, Color color)
        {
            ScorePopUp popUp = Instantiate(ScorePopUpPrefab, Position + new Vector3(0f, 0f, 1f), Quaternion.identity, FXContainer).GetComponent<ScorePopUp>();
            popUp.Text = Text;
            popUp.color = color;
        }

        public void AddPowerup(Powerup.PowerupProfile power)
        {
            CurPowerup = power;
            CurPowerup.OnStart.Invoke();
            PowerupTimer = CurPowerup.Duration;
        }
    }
}