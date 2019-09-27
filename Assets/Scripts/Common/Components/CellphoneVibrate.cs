
using UnityEngine;
using System.Collections;
/// <summary>
/// 摇一摇手机振动
/// </summary>
public class CellphoneVibrate : SingletonMono<CellphoneVibrate>
{
    public delegate void ShakePhoneHandler();
    public ShakePhoneHandler shakePhoneHandler;
    //记录上一次的重力感应的Y值
    private float old_y = 0;
    //记录当前的重力感应的Y值
    private float new_y;
    //当前手机晃动的距离
    private float currentDistance = 0;

    //手机晃动的有效距离
    public float distance;

    private void Awake()
    {
        instance = this;
    }

    void Update()
    {
        new_y = Input.acceleration.y;
        currentDistance = new_y - old_y;
        old_y = new_y;

        if (currentDistance > distance)
        {
            //实现手机晃动震动效果 
            Handheld.Vibrate();
            if (shakePhoneHandler != null)
                shakePhoneHandler();
        }
    }
}