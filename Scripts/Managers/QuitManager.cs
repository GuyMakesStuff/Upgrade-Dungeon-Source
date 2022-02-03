using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UpgradeDungeon.Managers
{
    public class QuitManager : Manager<QuitManager>
    {
        void Start()
        {
            Init(this);
        }

        public void Exit()
        {
            Application.Quit();
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.ExitPlaymode();
            #endif
        }
    }
}