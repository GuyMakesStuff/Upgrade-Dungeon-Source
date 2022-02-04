using System.Collections;
using UpgradeDungeon.Gameplay;
using UpgradeDungeon.Audio;
using UnityEngine;
using TMPro;

namespace UpgradeDungeon.Managers
{
    public class DialogManager : Manager<DialogManager>
    {
        [Space]
        public Animator DialogBox;
        public TMP_Text DialogText;
        public float CharDelay;
        string CurSentence;
        public static bool IsOpen;
        bool IsTyping;

        // Start is called before the first frame update
        void Start()
        {
            Init(this);
        }

        // Update is called once per frame
        void Update()
        {
            DialogBox.SetBool("IsOpen", IsOpen);

            if(Player.RequestingDialogSkip && IsOpen)
            {
                if(IsTyping)
                {
                    IsTyping = false;
                    StopAllCoroutines();
                    DialogText.text = CurSentence;
                    return;
                }

                IsOpen = false;
                PlaySelectSound();
                FindObjectOfType<Player>().SetFrezze(false);
            }
        }

        public void ShowDialog(string Sentence)
        {
            IsOpen = true;
            CurSentence = Sentence;
            DialogText.text = "";
            FindObjectOfType<Player>().SetFrezze(true);
            StartCoroutine(TypeSentence());
        }
        IEnumerator TypeSentence()
        {
            IsTyping = true;
            char[] Chars = CurSentence.ToCharArray();
            foreach (char C in Chars)
            {
                DialogText.text += C;
                AudioManager.Instance.InteractWithSFX("Dialog", SoundEffectBehaviour.Play);
                yield return new WaitForSeconds(CharDelay);
            }
            IsTyping = false;
        }
    }
}