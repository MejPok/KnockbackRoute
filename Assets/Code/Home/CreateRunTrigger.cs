using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Assets.Code.Home
{
    class CreateRunTrigger : MonoBehaviour
    {
        public CreateRunCanvas runCanvas;
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                if (runCanvas != null)
                    runCanvas.RunScreen(true);
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                if (runCanvas != null)
                    runCanvas.RunScreen(false);
            }
        }

    }
}
