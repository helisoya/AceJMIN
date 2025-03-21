using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public class Choice
{
    public List<ChoiceAnswer> answers;

    public Choice()
    {
        this.answers = new List<ChoiceAnswer>();
    }

    [Serializable]
    public class ChoiceAnswer
    {
        public string choice;
        public string action;

        public ChoiceAnswer(string choice, string action)
        {
            this.choice = choice;
            this.action = action;
        }
    }
}
