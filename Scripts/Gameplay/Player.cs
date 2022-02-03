using System;
using System.Collections;
using System.Collections.Generic;
using UpgradeDungeon.Managers;
using UpgradeDungeon.Audio;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    [Header("Keybinds")]
    public Keybind[] Keybinds;
    [Serializable]
    public class Keybind
    {
        public string KeybindName;
        public bool Enabled;
        public enum KeybindType { Key, MouseButton }
        public KeybindType Type;

        [Space]
        public KeyCode MainKeyCode;
        public KeyCode AltKeyCode;
        public int MouseButtonIndex;

        public bool IsHold()
        {
            if(Enabled)
            {
                switch (Type)
                {
                    case KeybindType.Key:
                    {
                        return (Input.GetKey(MainKeyCode) || Input.GetKey(AltKeyCode));
                    }
                    case KeybindType.MouseButton:
                    {
                        return Input.GetMouseButton(MouseButtonIndex);
                    }
                }
            }

            return false;
        }
        public bool IsDown()
        {
            if(Enabled)
            {
                switch (Type)
                {
                    case KeybindType.Key:
                    {
                        return (Input.GetKeyDown(MainKeyCode) || Input.GetKeyDown(AltKeyCode));
                    }
                    case KeybindType.MouseButton:
                    {
                        return Input.GetMouseButtonDown(MouseButtonIndex);
                    }
                }
            }
            
            return false;
        }
    }

    [Header("Movement")]
    public float MovementSpeed;
    float X;
    public float JumpHeight;
    public float GroundCheckRadius;
    public LayerMask GroundLayer;
    bool CanGroundCheck;
    bool IsGrounded;
    float HeightGap;
    float RegularHeight;
    float CrowchHeight;
    [HideInInspector]
    public bool IsCrowching;
    BoxCollider2D boxCollider;
    Rigidbody2D Body;
    Vector2 PrevVel;
    [HideInInspector]
    public bool IsFlipped;

    [Header("Other")]
    public Transform StartingPoint;
    public List<string> Abillities;
    public static bool RequestingDialogSkip;
    public static bool RequestingClick;
    public Animator animator;
    public SpriteRenderer spriteRenderer;
    [HideInInspector]
    public bool IsStorng;
    public GameObject DieFX;

    // Start is called before the first frame update
    void Awake()
    {
        Body = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        Abillities = new List<string>();
        CanGroundCheck = true;
        IsCrowching = ProgressManager.Instance.progress.Crowching;

        RegularHeight = boxCollider.size.y;
        CrowchHeight = RegularHeight / 2f;

        for (int KB = 0; KB < Keybinds.Length; KB++)
        {
            Keybinds[KB].Enabled = ProgressManager.Instance.progress.UnlockedKeybinds[KB];
        }
        for (int A = 0; A < ProgressManager.Instance.progress.Abillities.Length; A++)
        {
            Abillities.Add(ProgressManager.Instance.progress.Abillities[A]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(!(GameManager.Instance.IsPaused || DialogManager.IsOpen))
        {
            HeightGap = (boxCollider.size.y / 2f) + boxCollider.edgeRadius;
            X = GetHoriAxis() * MovementSpeed;
            if(IsCrowching) { X /= 2; }
            IsGrounded = Physics2D.CircleCast(transform.position, GroundCheckRadius, Vector2.down * Body.gravityScale, HeightGap, GroundLayer) && CanGroundCheck;

            if(GetKeybind("Jump").IsDown() && IsGrounded)
            {
                IsGrounded = false;
                AudioManager.Instance.InteractWithSFX("Jump", SoundEffectBehaviour.Play);
                Body.AddForce(new Vector2(0, (JumpHeight * 100f) * Body.gravityScale));
                StartCoroutine(ToggleGroundCheck());
            }

            if(GetKeybind("Crowch").IsDown() && !IsCrowching)
            {
                IsCrowching = true;
            }
            else if(GetKeybind("Uncrowch").IsDown() && IsCrowching)
            {
                IsCrowching = false;
            }
            float NewHeight = (IsCrowching) ? CrowchHeight : RegularHeight;
            boxCollider.size = new Vector2(boxCollider.size.x, NewHeight);

            if(GetKeybind("Restart").IsDown())
            {
                transform.position = StartingPoint.position;
                AudioManager.Instance.InteractWithSFX("Reset", SoundEffectBehaviour.Play);
            }

            animator.SetBool("IsGrounded", IsGrounded);
            animator.SetFloat("YVelocity", Body.velocity.y * Body.gravityScale);
            spriteRenderer.size = new Vector2(1.85f, boxCollider.size.y + 0.4f);

            RequestingClick = GetKeybind("Click").IsDown();
        }

        if(!GameManager.Instance.IsPaused)
        {
            RequestingDialogSkip = GetKeybind("Space").IsDown();
        }
    }
    IEnumerator ToggleGroundCheck()
    {
        CanGroundCheck = false;
        yield return new WaitForSeconds(0.15f);
        CanGroundCheck = true;
    }
    float GetHoriAxis()
    {
        float Result = 0f;
        if(GetKeybind("Right").IsHold() && !GetKeybind("Left").IsHold())
        {
            Result = 1f;
            transform.rotation = Quaternion.identity;
        }
        else if(!GetKeybind("Right").IsHold() && GetKeybind("Left").IsHold())
        {
            Result = -1f;
            transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        }
        return Result;
    }

    void FixedUpdate()
    {
        Body.velocity = new Vector2(X, Body.velocity.y);
    }

    public Keybind GetKeybind(string KeybindName)
    {
        return Array.Find(Keybinds, Keybind => Keybind.KeybindName == KeybindName);
    }

    public void AddAbillity(string Abillity)
    {
        if(HasAbillity(Abillity))
        {
            Debug.LogError("Player Already Has Abillity-" + Abillity);
            return;
        }
        Abillities.Add(Abillity);
    }
    public bool HasAbillity(string Abillity)
    {
        return Abillities.Contains(Abillity);
    }

    public void SetFrezze(bool Value)
    {
        if(Value)
        {
            PrevVel = Body.velocity;
            Body.constraints = RigidbodyConstraints2D.FreezeAll;
        }
        else
        {
            Body.constraints = RigidbodyConstraints2D.FreezeRotation;
            Body.velocity = PrevVel;
        }
    }

    public void AddSpeed()
    {
        MovementSpeed *= 2f;
    }
    public void RemoveSpeed()
    {
        MovementSpeed /= 2f;
    }

    public void SetStrength(bool Value)
    {
        IsStorng = Value;
    }

    public void ReverseGravity()
    {
        IsFlipped = true;
        Body.gravityScale *= -1f;
        spriteRenderer.flipY = true;
    }

    public bool[] GetUnlockedKeybinds()
    {
        bool[] Results = new bool[Keybinds.Length];
        for (int KB = 0; KB < Results.Length; KB++)
        {
            Results[KB] = Keybinds[KB].Enabled;
        }

        return Results;
    }

    void OnDrawGizmos()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position - new Vector3(0f, (boxCollider.size.y / 2f) + boxCollider.edgeRadius), GroundCheckRadius);
    }
}
