using System.Collections;
using UpgradeDungeon.Audio;
using UnityEngine;
using UpgradeDungeon.Managers;

public class BreakableDoor : MonoBehaviour
{
    public GameObject BreakFX;

    void Start()
    {
        if(GameManager.Instance.BrokenBlocks.Contains(gameObject.name))
        {
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if(other.collider.tag == "Player")
        {
            Player player = other.collider.GetComponent<Player>();
            if(player.IsStorng)
            {
                GameManager.Instance.BrokenBlocks.Add(gameObject.name);
                GameManager.Instance.SpawnFX(BreakFX, transform.position);
                AudioManager.Instance.InteractWithSFX("Weak Break", SoundEffectBehaviour.Play);
                Destroy(gameObject);
            }
        }
    }
}
