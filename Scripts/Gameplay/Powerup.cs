using UpgradeDungeon.Audio;
using UnityEngine.Events;
using UnityEngine;
using UpgradeDungeon.Managers;

namespace UpgradeDungeon.Gameplay
{
    public class Powerup : MonoBehaviour
    {
        [System.Serializable]
        public class PowerupProfile
        {
            public string Name;
            public float Duration;
            public UnityEvent OnStart;
            public UnityEvent OnEnd;
        }
        public PowerupProfile profile;
        [HideInInspector]
        public bool PreviouslyEnabled;

        void Start()
        {
            PreviouslyEnabled = gameObject.activeSelf;
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if(other.tag == "Player")
            {
                Player player = other.GetComponent<Player>();
                if(player.HasAbillity("Collect Powerups"))
                {
                    gameObject.SetActive(false);
                    GameManager.Instance.SpawnPopUp(transform.position, "+" + profile.Name + "(" + profile.Duration.ToString("0.0") + "Secs)", Color.white);
                    GameManager.Instance.AddPowerup(profile);
                    AudioManager.Instance.InteractWithSFX("Powerup", SoundEffectBehaviour.Play);
                }
            }
        }

        public void Enable()
        {
            PreviouslyEnabled = true;
        }
    }
}