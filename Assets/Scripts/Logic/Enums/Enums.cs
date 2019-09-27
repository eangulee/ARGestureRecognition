using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Logic.Enums
{
    public enum LayerType
    {
        None = 0,
        UI = 5,
        AR = 8,
    }

    public enum UIType
    {
        None = 0,
        Normal = 1,
        HideOther = 2,//隐藏上一个view
    }

    public enum WallPaperType
    {
        None = 0,
        GroupOne = 1,
        GroupTwo = 2,
    }
}