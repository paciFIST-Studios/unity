using System;

using UnityEngine;

using Sirenix.OdinInspector;


[Serializable]
public class PIDController
{
    // patron saint
    // http://luminaryapps.com/blog/use-a-pid-loop-to-control-unity-game-objects/

    [FoldoutGroup("PID Controller")]
    [Tooltip("Proportional constant (counters error)")]
    public float Kp = 0.2f;

    [FoldoutGroup("PID Controller")]
    [Tooltip("Integral constant (counters accumulated error)")]
    public float Ki = 0.05f;

    [FoldoutGroup("PID Controller")]
    [Tooltip("Derivative constant (fights oscillation)")]
    public float Kd = 1f;

    [FoldoutGroup("PID Controller")]
    [Tooltip("current control value")]
    public float value = 0f;

    private float lastError;
    private float integral;

    public float Update(float error)
    {
        return Update(error, Time.deltaTime);
    }

    public float Update(float error, float dt)
    {
        float derivative = (error - lastError) / dt;
        integral += error * dt;
        lastError = error;

        value = Kp * error + Ki * integral + Kd * derivative;
        return value;
    }
}