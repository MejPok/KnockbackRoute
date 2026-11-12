using Assets.Code.Home;
using System;
using System.Collections.Generic;
using System.Text;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Code.Round
{
    class RunManager : MonoBehaviour
    {
        public static RunManager Instance;
        public int NumberOfCompletedRuns;

        private void Start()
        {
            if(Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        public int numberOfOptions;

        public List<RunOption> runOptions = new List<RunOption>();

        public int ChosenOption = 0;

        public bool createdNewOptions;
        private void Update()
        {
            if(SceneManager.GetActiveScene().name == "HomeScene" && !createdNewOptions)
            {
                CreateNewOptions();
                createdNewOptions = true;
            }
        }

        public void RunCompleted(bool completed)
        {
            if (completed)
            {
                Debug.Log("increase completed runs");
                NumberOfCompletedRuns += 1;
                PlayerShoot.Instance.GetComponent<MoneyCount>().Money += runOptions[ChosenOption].AdditionalMoneyReward;
            }
            CreateNewOptions();
        }
        public void CreateNewOptions()
        {
            runOptions.Clear();
            
            var random = new System.Random();
            for(int i = 0; i <= numberOfOptions; i++)
            {

                var option = new RunOption
                {
                    Difficulty = Math.Min( random.Next(0, NumberOfCompletedRuns + 1), 4),
                    RoomCount = random.Next((NumberOfCompletedRuns + 1) * 5, (NumberOfCompletedRuns + 1) * 15)
                };

                option.AdditionalMoneyReward = option.Difficulty * option.RoomCount;

                runOptions.Add(option);
            }
        }
    }
}
