using UnityEngine;

namespace traVRsal.SDK
{
    [AddComponentMenu("traVRsal/Global Features")]
    public class GlobalFeatures : MonoBehaviour
    {
        private IMisc _api;
        private ISpawner _spawner;
        private IEnvironment _env;
        private ISoundAction _audio;

        private void Start()
        {
            _api = GetComponentInParent<IMisc>();
            _spawner = GetComponentInParent<ISpawner>();
            _env = GetComponentInParent<IEnvironment>();
            _audio = GetComponentInParent<ISoundAction>();
        }

        public void SetLightIntensity(float intensity)
        {
            _env?.SetLightIntensity(intensity);
        }

        public void SetLightColor(Color color)
        {
            _env?.SetLightColor(color);
        }

        public void SetAmbientLightColor(Color color)
        {
            _env?.SetAmbientLightColor(color);
        }

        public void PlayMusic(string music)
        {
            _audio?.PlayMusic(music);
        }

        public void ShowTheater(string video)
        {
            _api?.ShowTheater(video);
        }

        public void LoadWorld(string worldName)
        {
            _api?.LoadWorld(worldName);
        }

        public void ShowScreenHud(bool state)
        {
            _api?.ShowScreenHud(state);
        }

        public void Destruct(GameObject go)
        {
            _spawner?.Destruct(go);
        }
    }
}