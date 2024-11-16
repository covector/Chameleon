using System;
using UnityEngine;

public class Updater
{
    private float period;
    private Action callback;
    private float counter = float.PositiveInfinity;

    public Updater(float period, Action callback)
    {
        this.period = period;
        this.callback = callback;
    }

    public void Update()
    {
        if (counter > period)
        {
            counter = 0;
            callback();
        }
        counter += Time.deltaTime;
    }
}
