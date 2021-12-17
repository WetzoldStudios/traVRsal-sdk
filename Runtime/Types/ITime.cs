using System;

namespace traVRsal.SDK
{
    public interface ITime
    {
        void Delay(float duration, Action callback);
    }
}