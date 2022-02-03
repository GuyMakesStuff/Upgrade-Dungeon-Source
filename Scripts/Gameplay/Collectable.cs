using UpgradeDungeon.Audio;
using UnityEngine.Events;
using UnityEngine;
using UpgradeDungeon.Managers;

public class Collectable : MonoBehaviour
{
    public enum CollectableType { Input, Abillity, Event }
    public CollectableType Type;

    [Header("General")]
    public string KeybindToUnlock;
    public string Abillity;
    public UnityEvent Event;

    [Header("Apperance")]
    public Sprite IconImage;
    public SpriteRenderer Icon;
    SpriteRenderer spriteRenderer;
    [TextArea(3, 10)]
    public string Message;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if((Type == CollectableType.Input && FindObjectOfType<Player>().GetKeybind(KeybindToUnlock).Enabled) || (Type == CollectableType.Abillity && FindObjectOfType<Player>().Abillities.Contains(Abillity)) || (Type == CollectableType.Event && GameManager.Instance.InvokedEvents.Contains(gameObject.name)))
        {
            if(Type == CollectableType.Event)
            {
                Event.Invoke();
            }
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        Icon.sprite = IconImage;

        switch (Type)
        {
            case CollectableType.Input:
            {
                spriteRenderer.color = Color.cyan;
                break;
            }
            case CollectableType.Abillity:
            {
                spriteRenderer.color = Color.red;
                break;
            }
            case CollectableType.Event:
            {
                spriteRenderer.color = Color.green;
                break;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player")
        {
            Collect(other);
            FindObjectOfType<DialogManager>().ShowDialog(Message);
            AudioManager.Instance.InteractWithSFX("Collectable", SoundEffectBehaviour.Play);
            Destroy(gameObject);
        }
    }

    void Collect(Collider2D Player)
    {
        Player player = Player.GetComponent<Player>();

        switch (Type)
        {
            case CollectableType.Input:
            {
                player.GetKeybind(KeybindToUnlock).Enabled = true;
                break;
            }
            case CollectableType.Abillity:
            {
                player.AddAbillity(Abillity);
                break;
            }
            case CollectableType.Event:
            {
                Event.Invoke();
                GameManager.Instance.InvokedEvents.Add(gameObject.name);
                break;
            }
        }
    }
}
