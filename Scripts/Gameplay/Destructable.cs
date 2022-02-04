using System.Collections;
using UpgradeDungeon.Managers;
using UnityEngine;

namespace UpgradeDungeon.Gameplay
{
    public class Destructable : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            if(GameManager.Instance.BrokenBlocks.Contains(gameObject.name))
            {
                Destroy(gameObject);
            }
            SubStart();
        }
        protected virtual void SubStart()
        {

        }

        protected void Destruct()
        {
            GameManager.Instance.BrokenBlocks.Add(gameObject.name);
            Destroy(gameObject);
        }
    }
}