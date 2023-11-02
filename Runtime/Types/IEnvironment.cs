using UnityEngine;

namespace traVRsal.SDK
{
    public interface IEnvironment
    {
        EnvironmentInfo GetEnvironmentInfo();
        void SetLightIntensity(float intensity);
        void SetLightColor(Color color);
        void SetAmbientLightColor(Color color);
        void SetPassthrough(bool state);
    }
}