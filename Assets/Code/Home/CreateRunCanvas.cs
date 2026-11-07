using System;
using System.Collections.Generic;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Code.Home
{
    class CreateRunCanvas : MonoBehaviour
    {
        public static CreateRunCanvas Instance;
        public GameObject CreateRunScreen;

        bool ShowRunScreen = false;

        void Start()
        {
            Instance = this;
        }

        public void RunScreen(bool show)
        {
            CreateRunScreen.SetActive(show);
        }

    }
}
