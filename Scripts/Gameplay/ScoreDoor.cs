using System.Collections;
using UpgradeDungeon.Audio;
using UnityEngine;
using UpgradeDungeon.Managers;

namespace UpgradeDungeon.Gameplay
{
    public class ScoreDoor : MonoBehaviour
    {
        public int RequiredScore;
        public float OpenSpeed;
        bool IsOpen;
        Vector3 StartPos;
        Vector3 OpenPos;
        float OpenPer;

        void Start()
        {
            StartPos = transform.position;
            OpenPos = StartPos + new Vector3(0f, transform.localScale.y * 2f, 0f);
            IsOpen = (GameManager.Instance.Score >= RequiredScore);
        }

        void Update()
        {
            if(IsOpen)
            {
                OpenPer += (1 / OpenSpeed) * Time.deltaTime;
            }
            OpenPer = Mathf.Clamp01(OpenPer);

            transform.position = Vector3.Lerp(StartPos, OpenPos, OpenPer);
        }

        void OnCollisionEnter2D(Collision2D other)
        {
            if(other.collider.tag == "Player")
            {
                if(GameManager.Instance.Score >= RequiredScore)
                {
                    IsOpen = true;
                    AudioManager.Instance.InteractWithSFX("Score Door Open", SoundEffectBehaviour.Play);
                }
                else
                {
                    DialogManager.Instance.ShowDialog("You Need At Least " + RequiredScore.ToString() + " Score To Open This Door!");
                }
            }
        }
    }
}