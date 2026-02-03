using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace DungeonOfAlgorithms.Source.Core
{
    public class AudioManager
    {
        // Singleton que existe apenas uma instancia dessa classe
        private static AudioManager _instance;

        public static AudioManager Instance
        {
            get
            {
                return _instance ??= new AudioManager();
                // Utilizamos o ??= para verificar se a instancia ja existe, se nao existir criamos uma nova
            }
        }

        private Song _currentAmbientMusic;
        private Dictionary<string, SoundEffect> _soundEffects;

        private float _musicVolume = 0.7f;
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

        public bool IsMusicPlaying => MediaPlayer.State == MediaState.Playing;

        // Construtor privado para evitar instanciacao externa
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
            else
            {
                System.Console.WriteLine($"[AudioManager] Som {key} nao encontrado");
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
                System.Console.WriteLine($"[AudioManager] Musica iniciada");
            }
        }

        public void StopAmbientMusic()
        {
            if (MediaPlayer.State == MediaState.Playing)
            {
                MediaPlayer.Stop();
                _currentAmbientMusic = null;
            }
        }

        public void PauseAmbientMusic()
        {
            if (MediaPlayer.State == MediaState.Playing)
            {
                MediaPlayer.Pause();
                System.Console.WriteLine($"[AudioManager] Musica pausada");
            }
        }

        public void ResumeAmbientMusic()
        {
            if (MediaPlayer.State == MediaState.Paused)
            {
                MediaPlayer.Resume();
                System.Console.WriteLine($"[AudioManager] Musica retomada");
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

        // Ou pode colocar dessa forma
        // public bool HasSoundEffect(string key) => _soundEffects.ContainsKey(key);

        public void ClearAllSound()
        {
            StopAmbientMusic();
            _soundEffects.Clear();
            System.Console.WriteLine($"[AudioManager] Todos os sons foram limpos");
        }
    }

}
