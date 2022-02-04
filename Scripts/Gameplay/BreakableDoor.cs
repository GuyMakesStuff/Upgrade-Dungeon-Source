using System.Collections;
using UpgradeDungeon.Audio;
using UnityEngine;
using UpgradeDungeon.Managers;

namespace UpgradeDungeon.Gameplay
{
    public class BreakableDoor : Destructable
    {
        public GameObject BreakFX;

        void OnCollisionEnter2D(Collision2D other)
        {
            if(other.collider.tag == "Player")
            {
                Player player = other.collider.GetComponent<Player>();
                if(player.IsStorng)
                {
                    GameManager.Instance.SpawnFX(BreakFX, transform.position);
                    AudioManager.Instance.InteractWithSFX("Weak Break", SoundEffectBehaviour.Play);
                    Destruct();
                }
            }
        }
    }
}