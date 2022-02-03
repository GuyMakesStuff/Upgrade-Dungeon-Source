using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UpgradeDungeon.Audio;
using UpgradeDungeon.Managers;

namespace UpgradeDungeon.Gameplay
{
    public class Goal : MonoBehaviour
    {
        void OnTriggerEnter2D(Collider2D other)
        {
            if(other.tag == "Player")
            {
                AudioManager.Instance.InteractWithSFX("Win", SoundEffectBehaviour.Play);
                GameManager.Instance.Win();
            }
        }
    }
}