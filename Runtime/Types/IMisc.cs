﻿namespace traVRsal.SDK
{
    public interface IMisc
    {
        void ShowTheater(string video);
        void LoadWorld(string worldName);
        void UnlockAchievement(string key);
        void ShowScreenHud(bool state);
    }
}