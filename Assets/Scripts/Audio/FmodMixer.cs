using System.Collections.Generic;
using System.Linq;
using FMOD;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

namespace Audio
{
    public class FMODMixer : MonoBehaviour
    {
        private Bus _sfx;
        private Bus _music;

        [SerializeField] private List<EventReference> gameSongEvents;
        private List<GUID> _gameSongGuids;

        private void Start()
        {
            _sfx = RuntimeManager.GetBus("bus:/Sfx");
            _music = RuntimeManager.GetBus("bus:/music");

            if (!PlayerPrefs.HasKey("music"))
            {
                PlayerPrefs.SetFloat("music", -12f);
                PlayerPrefs.SetFloat("sfx", -5f);
            }
            
            var dbMusic = PlayerPrefs.GetFloat("music");
            var dbSfx = PlayerPrefs.GetFloat("sfx");

            gameSongEvents.ForEach(e => _gameSongGuids.Add(e.Guid));

            _music.setVolume(DecibelToLinear(dbMusic));
            _sfx.setVolume(DecibelToLinear(dbSfx));
        }

        public void ChangeMusicVolume(float dB)
        {
            dB = BottomDecibelsIfLowEnough(dB);
            _music.setVolume(DecibelToLinear(dB));
            SaveVolumePreferences("music", dB);
        }

        public void ChangeSfxVolume(float dB)
        {
            dB = BottomDecibelsIfLowEnough(dB);
            _sfx.setVolume(DecibelToLinear(dB));
            SaveVolumePreferences("sfx", dB);
        }

        public void KillEverySound()
        {
            if (FindObjectsOfType(typeof(StudioEventEmitter)) is StudioEventEmitter[] eventEmitters)
            {
                foreach (var eventEmitter in eventEmitters)
                {
                    eventEmitter.AllowFadeout = false;
                    eventEmitter.Stop();
                }
            }
        }

        public void KillEverySoundExcept(params GUID[] guids)
        {
            if (guids.Length == 0)
            {
                UnityEngine.Debug.LogWarning("Called FmodMixer#KillEverySoundExcept without any GUIDs! Please call FmodMixer#KillEverySound instead");
            }

            if (FindObjectsOfType(typeof(StudioEventEmitter)) is StudioEventEmitter[] eventEmitters)
            {
                foreach (var eventEmitter in eventEmitters)
                {
                    if (!guids.Contains(eventEmitter.EventReference.Guid))
                    {
                        eventEmitter.AllowFadeout = false;
                        eventEmitter.Stop();
                    }
                }
            }
        }

        public void FindAllSfxAndPlayPause(bool isGamePaused)
        {
            if (FindObjectsOfType(typeof(StudioEventEmitter)) is StudioEventEmitter[] eventEmitters)
            {
                // filter out events within this.GameSongEvents (we don't want to pause the music in this case)
                var sfxEvents = eventEmitters.Where(e => !_gameSongGuids.Contains(e.EventReference.Guid));
                foreach (var eventEmitter in sfxEvents)
                {
                    
                    switch (isGamePaused)
                    {
                        case true when eventEmitter.IsPlaying():
                            eventEmitter.EventInstance.setPaused(true);
                            break;
                        case false when !eventEmitter.IsPlaying():
                            eventEmitter.EventInstance.setPaused(false);
                            break;
                    }
                }
            }
        }

        private float DecibelToLinear(float dB)
        {
            var linear = Mathf.Pow(10f, dB / 20f);
            return linear;
        }

        private float BottomDecibelsIfLowEnough(float dB)
        {
            return dB == -19.5f ? -200f : dB;
        }

        private void SaveVolumePreferences(string bus, float dB)
        {
            PlayerPrefs.SetFloat(bus.ToLower(), dB);
            PlayerPrefs.Save();
        }
    }
}
