using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;

namespace DungeonOfAlgorithms.Source.Core
{
    public class AudioManager
    {
        private static AudioManager _instance;
        public static AudioManager Instance => _instance ??= new AudioManager();

        private Song _currentAmbientMusic;
        private Dictionary<string, SoundEffect> _soundEffects;
        private float _musicVolume;

        public float MusicVolume
        {
            get => _musicVolume;
            set
            {
                _musicVolume = Math.Clamp(value, 0f, 1f);
                MediaPlayer.Volume = _musicVolume;
            }
        }

        public float SoundEffectsVolume { get; set; } = 0.8f;

        public bool IsMusicPlaying => MediaPlayer.State == MediaState.Playing;

        private AudioManager()
        {
            _soundEffects = new Dictionary<string, SoundEffect>();
        }

        public void LoadSoundEffect(string key, SoundEffect soundEffect)
        {
            if (!_soundEffects.ContainsKey(key))
                _soundEffects[key] = soundEffect;
        }

        public void PlaySoundEffect(string key)
        {
            if (_soundEffects.ContainsKey(key))
                _soundEffects[key].Play(SoundEffectsVolume, 0f, 0f);
            else
                Console.WriteLine($"[AudioManager] Som '{key}' nao encontrado");
        }

        public void PlayAmbientMusic(Song music, float volume = 0.7f)
        {
            if (_currentAmbientMusic == music) return;

            if (MediaPlayer.State == MediaState.Playing)
                MediaPlayer.Stop();

            _currentAmbientMusic = music;
            MediaPlayer.IsRepeating = true;
            MusicVolume = volume;
            MediaPlayer.Play(music);
        }

        public void StopAmbientMusic()
        {
            if (MediaPlayer.State == MediaState.Playing)
                MediaPlayer.Stop();
        }

        public void PauseAmbientMusic()
        {
            if (MediaPlayer.State == MediaState.Playing)
                MediaPlayer.Pause();
        }

        public void ResumeAmbientMusic()
        {
            if (MediaPlayer.State == MediaState.Paused)
                MediaPlayer.Resume();
        }

        public void ToggleAmbientMusic()
        {
            if (MediaPlayer.State == MediaState.Playing)
                PauseAmbientMusic();
            else if (MediaPlayer.State == MediaState.Paused)
                ResumeAmbientMusic();
        }

        public bool HasSoundEffect(string key) => _soundEffects.ContainsKey(key);

        public void ClearAllSounds()
        {
            StopAmbientMusic();
            _soundEffects.Clear();
        }
    }
}
