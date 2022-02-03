using UpgradeDungeon.Visuals;
using UpgradeDungeon.Audio;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Enemy : MonoBehaviour
{
    [Header("Movement")]
    public float Speed;
    public float Direction;
    Rigidbody2D Body;

    [Header("Obstacle Detection")]
    public Transform GroundChecker;
    public float GroundCheckRadius;
    public LayerMask GroundLayer;
    public float WallDetectionDist;
    bool HittingWall;
    bool Grounded;

    // Start is called before the first frame update
    void Start()
    {
        Body = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Physics2D.queriesHitTriggers = false;
        HittingWall = Physics2D.Raycast(transform.position + (transform.right * 1.1f), transform.right, WallDetectionDist);
        Grounded = Physics2D.CircleCast(GroundChecker.position, GroundCheckRadius, Vector2.down, 0.1f, GroundLayer);

        if(HittingWall || !Grounded)
        {
            ChangeDir();
        }
    }

    void ChangeDir()
    {
        Direction *= -1f;
        if(Direction == 1f)
        {
            transform.rotation = Quaternion.identity;
        }
        else if(Direction == -1f)
        {
            transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        }

        if(FindObjectOfType<CameraFollow>().InRangeOfCamera(transform))
        {
            AudioManager.Instance.InteractWithSFX("Wall Hit", SoundEffectBehaviour.Play);
        }
    }

    void FixedUpdate()
    {
        Body.velocity = new Vector2(Direction * Speed, Body.velocity.y);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(WallDetectionDist + 1f, 0f, 0f));
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(GroundChecker.position, GroundCheckRadius);
    }
}
