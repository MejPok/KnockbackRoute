using Assets.Code.Round;
using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Code.Home
{
    class CreateRunCanvas : MonoBehaviour
    {
        public GameObject CreateRunScreen;

        public TextMeshProUGUI[] runTexts;

        public void RunScreen(bool show)
        {
            CreateRunScreen.SetActive(show);
            
            for(int i = 0; i < runTexts.Length; i++)
            {
                var option = RunManager.Instance.runOptions[i];
                runTexts[i].text = $"Difficulty: {option.Difficulty} \n Rooms: {option.RoomCount} \n Completion money reward: {option.AdditionalMoneyReward} ";
            }

        }

        public void ChooseRun(int runNumber)
        {
            var runCreator = GetComponent<CreateRun>();

            runCreator.Difficulty = RunManager.Instance.runOptions[runNumber].Difficulty;
            runCreator.NumberOfRooms = RunManager.Instance.runOptions[runNumber].Difficulty;
            
            RunManager.Instance.ChosenOption = runNumber;
            runCreator.CreateRunSetup();

        }

    }
}
