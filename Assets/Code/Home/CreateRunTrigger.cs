using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Assets.Code.Home
{
    class CreateRunTrigger : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                CreateRunCanvas.Instance.RunScreen(true);
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                CreateRunCanvas.Instance.RunScreen(false);
            }
        }

    }
}
