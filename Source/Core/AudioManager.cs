using Microsoft.Xna. Framework.Audio;
using Microsoft.Xna. Framework.Content;
using Microsoft.Xna. Framework.Media;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;


namespace DungeonOfAlgorithms.Source.Core
{
    public class AudioManager
    {
        // Singleton instance
        private static AudioManager _instance;

        public static AudioManager Instance
        {
            get
            {
                return _instance ??= new AudioManager();
            }
        }

    private Song _currentAmbientMusic;

        private Dictionary<string, SoundEffect> _soundEffects;

        private float _musicVolume;

        public float MusicVolume
        {
            get => _musicVolume;
            set
            {
                _musicVolume = System.Math.Clamp(value, 0f, 1f);
                MediaPlayer.Volume = _musicVolume;
            }
        }

        public float SoundEffectsVolume { get; set; } = 0.8f;

        public bool IsMusicPlaying
        {
            get => MediaPlayer.State == MediaState.Playing;
        }
        // Outra forma de escrever a propriedade IsMusicPlaying
        // public bool IsMusicPlaying => MediaPlayer.State == MediaState.Playing;
        private AudioManager()
        {
            _soundEffects = new Dictionary<string, SoundEffect>();
        }

        public void LoadSoundEffect(string key, SoundEffect soundEffect)
        {
            if (!_soundEffects.ContainsKey(key))
            {
                _soundEffects[key] = soundEffect;
            }

        }

        public void PlaySoundEffect(string key)
        {
            if (_soundEffects.ContainsKey(key))
            {
                _soundEffects[key].Play(SoundEffectsVolume, 0f, 0f);
            }
            else {
                System.Console.WriteLine($"[Audio Manager] Som {key} nao encontrado");
            }
        }

        public void PlayAmbientMusic(Song music, float volume = 0.7f)
        {
            if (_currentAmbientMusic != music)
            {
                if (MediaPlayer.State == MediaState.Playing)
                {
                    MediaPlayer.Stop();
                }

                _currentAmbientMusic = music;

                MediaPlayer.IsRepeating = true;
                MusicVolume = volume;
                MediaPlayer.Play(music);
                System.Console.WriteLine($"[Audio Manager] Tocando musica ambiente: {music.Name}");
            }
        }

        public void StopAmbientMusic()
        {
            if (MediaPlayer.State == MediaState.Playing)
            {
                MediaPlayer.Stop();
                System.Console.WriteLine($"[Audio Manager] Musica ambiente parada");
            }
        }

        public void PauseAmbientMusic()
        {
            if (MediaPlayer.State == MediaState.Playing)
            {
                MediaPlayer.Pause();
                System.Console.WriteLine($"[Audio Manager] Musica ambiente pausada");
            }
        }

        public void ResumeAmbientMusic()
        {
            if (MediaPlayer.State == MediaState.Paused)
            {
                MediaPlayer.Resume();
                System.Console.WriteLine($"[Audio Manager] Musica ambiente resumida");
            }
        }

        public void ToggleAmbientMusic()
        {
            if (MediaPlayer.State == MediaState.Playing)
            {
                PauseAmbientMusic();
            }
            else if (MediaPlayer.State == MediaState.Paused)
            {
                ResumeAmbientMusic();
            }
        }

        public bool HasSoundEffect(string key)
        {
            return _soundEffects.ContainsKey(key);
        }

        public void ClearAllSounds()
        {
            StopAmbientMusic();
            _soundEffects.Clear();
            System.Console.WriteLine($"[Audio Manager] Todos os sons foram limpos");
        }

    }
}