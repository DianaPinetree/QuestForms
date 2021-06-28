using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class QF_PageElement : MonoBehaviour
{
    /// <summary>
    /// If this element is valid
    /// </summary>
    /// <returns> Valid</returns>
    public abstract bool Valid();
    public abstract void Clear();
}
