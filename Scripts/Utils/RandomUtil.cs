using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RandomUtil
{

    public static float Gaussian(float mean, float variance, float min, float max)
    {
        float x;
        do
        {
            x = Gaussian(mean, variance);
        } while (x < min || x > max);
        return x;
    }

    public static float Gaussian(float mean, float standard_deviation)
    {
        return mean + Gaussian() * standard_deviation;
    }

    public static float Gaussian()
    {
        float v1, v2, s;
        do
        {
            v1 = 2.0f * Random.Range(0f, 1f) - 1.0f;
            v2 = 2.0f * Random.Range(0f, 1f) - 1.0f;
            s = v1 * v1 + v2 * v2;
        } while (s >= 1.0f || s == 0f);
        s = Mathf.Sqrt((-2.0f * Mathf.Log(s)) / s);
        return v1 * s;
    }

}
