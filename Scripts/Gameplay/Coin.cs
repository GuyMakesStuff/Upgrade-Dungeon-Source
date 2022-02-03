using System.Collections;
using UpgradeDungeon.Audio;
using UnityEngine;
using UpgradeDungeon.Managers;

public class Coin : MonoBehaviour
{
    [Header("Visuals")]
    public float SinRange;
    public float SinSpeed;
    Vector3 BasePos;
    float Offset;

    void Start()
    {
        BasePos = transform.position;
        Offset = Random.Range(0f, 1f);
    }

    void Update()
    {
        float Y = Mathf.Sin((Time.time * SinSpeed) + Offset) * SinRange;
        transform.position = BasePos + (Vector3.up * Y);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player")
        {
            GameManager.Instance.Score += 100;
            GameManager.Instance.SpawnPopUp(transform.position, "+100", Color.yellow);
            AudioManager.Instance.InteractWithSFX("Coin", SoundEffectBehaviour.Play);
            Destroy(gameObject);
        }
    }
}
