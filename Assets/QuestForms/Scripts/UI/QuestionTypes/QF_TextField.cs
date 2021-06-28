using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class QF_TextField : QF_PageElement
{
    private TMP_InputField inputField;

    public override void Clear()
    {
        inputField.text = string.Empty;   
    }

    public override bool Valid()
    {
        throw new System.NotImplementedException();
    }
}
