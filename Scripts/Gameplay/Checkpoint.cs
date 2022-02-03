using System.Collections;
using UpgradeDungeon.Audio;
using UnityEngine;
using UpgradeDungeon.Managers;

public class Checkpoint : MonoBehaviour
{
    public int Index;

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player")
        {
            if(GameManager.Instance.CheckpointIndex != Index)
            {
                AudioManager.Instance.InteractWithSFX("Checkpoint", SoundEffectBehaviour.Play);
                GameManager.Instance.CheckpointText.SetTrigger("Hit");
            }
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if(other.tag == "Player")
        {
            GameManager.Instance.CheckpointIndex = Index;
            GameManager.Instance.Save();
        }
    }
}
