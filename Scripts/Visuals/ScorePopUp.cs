using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace UpgradeDungeon.Visuals
{
    [RequireComponent(typeof(TMP_Text))]
    public class ScorePopUp : MonoBehaviour
    {
        public float Speed;
        public float Dist;
        float T;
        Vector3 StartPos;
        Vector3 TargetPos;
        [HideInInspector]
        public string Text;
        [HideInInspector]
        public Color color;
        TMP_Text text;

        // Start is called before the first frame update
        void Start()
        {
            text = GetComponent<TMP_Text>();
            T = 0f;
            StartPos = transform.position;
            TargetPos = StartPos + (Vector3.up * Dist);
        }

        // Update is called once per frame
        void Update()
        {
            T += Speed * Time.deltaTime;

            text.text = Text;
            text.color = new Color(color.r, color.g, color.b, 1f - T);
            transform.position = Vector3.Lerp(StartPos, TargetPos, T);

            if(T >= 1f)
            {
                Destroy(gameObject);
            }
        }
    }
}