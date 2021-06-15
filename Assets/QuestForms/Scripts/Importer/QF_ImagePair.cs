using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QuestForms
{
    [System.Serializable]
    public class QF_ImagePair
    {
        public string id;
        public Sprite image;
        public ImageAnchor position;
    }

    public enum ImageAnchor
    {
        Before,
        After,
        Left,
        Right
    }
}
