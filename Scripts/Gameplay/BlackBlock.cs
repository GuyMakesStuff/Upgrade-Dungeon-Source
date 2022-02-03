using System.Collections;
using UpgradeDungeon.Audio;
using UnityEngine;
using UpgradeDungeon.Managers;

public class BlackBlock : MonoBehaviour
{
    public GameObject BreakFX;

    void OnMouseOver()
    {
        if(Player.RequestingClick)
        {
            GameManager.Instance.BrokenBlocks.Add(gameObject.name);
            GameManager.Instance.SpawnFX(BreakFX, transform.position);
            AudioManager.Instance.InteractWithSFX("Black Break", SoundEffectBehaviour.Play);
            Destroy(gameObject);
        }
    }
}
