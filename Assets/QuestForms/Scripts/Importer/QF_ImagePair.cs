using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace QuestForms
{
    [System.Serializable]
    public class QF_ImagePair : IEquatable<string>, IEquatable<QF_ImagePair>
    {
        public string id;
        public Sprite image;
        public ImageAnchor position;
        public TextAnchor alignment;

        public QF_ImagePair(string id) 
        {
            this.id = id;
        }

        public bool Equals(string other)
        {
            if (other == null) return false;

            return other == id;
        }

        public bool Equals(QF_ImagePair other)
        {
            if (other == null) return false;
            return other.id == id;
        }
    }

    public enum ImageAnchor
    {
        Before = 0,
        After = 1,
        
    }
}
