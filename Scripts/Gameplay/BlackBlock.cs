using System.Collections;
using UpgradeDungeon.Audio;
using UnityEngine;
using UpgradeDungeon.Managers;

namespace UpgradeDungeon.Gameplay
{
    public class BlackBlock : Destructable
    {
        public GameObject BreakFX;

        void OnMouseOver()
        {
            if(Player.RequestingClick)
            {
                GameManager.Instance.SpawnFX(BreakFX, transform.position);
                AudioManager.Instance.InteractWithSFX("Black Break", SoundEffectBehaviour.Play);
                Destruct();
            }
        }
    }
}