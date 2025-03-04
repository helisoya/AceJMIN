using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents the Ace JMIN Inputs
/// </summary>
public class AJInput
{
    [System.Serializable]
    public class Controls
    {
        public KeyCode moveUp = KeyCode.UpArrow;
        public KeyCode moveDown = KeyCode.DownArrow;
        public KeyCode moveRight = KeyCode.RightArrow;
        public KeyCode moveLeft = KeyCode.LeftArrow;
        public KeyCode confirm = KeyCode.Return;
        public KeyCode cancel = KeyCode.Backspace;
        public KeyCode present = KeyCode.E;
        public KeyCode press = KeyCode.Q;
        public KeyCode courtRecord = KeyCode.Tab;
        public KeyCode options = KeyCode.Escape;
        public KeyCode profilesEvidence = KeyCode.R;
        public KeyCode returnToTitle = KeyCode.J;
    }

    private Controls controls;
    private static AJInput instance;


    public static AJInput Instance
    {
        get
        {
            if (instance == null) instance = new AJInput();
            return instance;
        }
    }

    private AJInput()
    {
        controls = new Controls();
    }

    public bool GetMoveUpDown()
    {
        return Input.GetKeyDown(controls.moveUp);
    }

    public bool GetMoveDownDown()
    {
        return Input.GetKeyDown(controls.moveDown);
    }

    public bool GetMoveLeftDown()
    {
        return Input.GetKeyDown(controls.moveLeft);
    }

    public bool GetMoveRightDown()
    {
        return Input.GetKeyDown(controls.moveRight);
    }


    public bool GetConfirmDown()
    {
        return Input.GetKeyDown(controls.confirm);
    }

    public bool GetCancelDown()
    {
        return Input.GetKeyDown(controls.cancel);
    }

    public bool GetPresentDown()
    {
        return Input.GetKeyDown(controls.present);
    }

    public bool GetPressDown()
    {
        return Input.GetKeyDown(controls.press);
    }

    public bool GetCourtRecordDown()
    {
        return Input.GetKeyDown(controls.courtRecord);
    }

    public bool GetOptionsDown()
    {
        return Input.GetKeyDown(controls.options);
    }

    public bool GetProfileEvidenceDown()
    {
        return Input.GetKeyDown(controls.profilesEvidence);
    }

    public bool GetReturnToTitleDown()
    {
        return Input.GetKeyDown(controls.returnToTitle);
    }

}
