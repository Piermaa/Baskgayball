using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BaskgayBall.Audio
{
    public class AudioManager : MonoBehaviour
    {
        private static AudioManager _instance;

        public static AudioManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindAnyObjectByType<AudioManager>();
                    if (_instance == null)
                    {
                        GameObject go = new GameObject("[AudioManager]");
                        _instance = go.AddComponent<AudioManager>();
                        DontDestroyOnLoad(go);
                    }
                }
                return _instance;
            }
        }

        [Header("Master & Category Volumes")]
        [Range(0f, 1f)]
        [SerializeField] private float masterVolume = 1f;

        [Range(0f, 1f)]
        [SerializeField] private float sfxVolume = 1f;

        [Range(0f, 1f)]
        [SerializeField] private float musicVolume = 0.8f;

        [Header("Background Music Tracks")]
        [Tooltip("Musica para la escena de Menu Principal.")]
        [SerializeField] private AudioClip menuMusic;

        [Tooltip("Musica para la escena de Gameplay.")]
        [SerializeField] private AudioClip gameplayMusic;

        [Tooltip("Tiempo de transicion (crossfade) en segundos al cambiar de tema musical.")]
        [SerializeField] private float musicCrossfadeDuration = 1f;

        [Tooltip("Si esta activado, detecta la escena cargada (MainMenu / Gameplay) y cambia la musica automaticamente.")]
        [SerializeField] private bool autoSwitchMusicByScene = true;

        [Header("Sound Configurations")]
        [Tooltip("Sonido de salto")]
        [SerializeField] private SoundEffect jumpSound = new SoundEffect("Jump", 1f, 0.95f, 1.05f, 0.05f);

        [Tooltip("Sonido de aterrizaje")]
        [SerializeField] private SoundEffect landSound = new SoundEffect("Land", 0.9f, 0.9f, 1.1f, 0.08f);

        [Tooltip("Sonido de gol")]
        [SerializeField] private SoundEffect goalSound = new SoundEffect("Goal", 1f, 1f, 1f, 0.2f);

        [Tooltip("Sonido de presionar boton")]
        [SerializeField] private SoundEffect buttonPressSound = new SoundEffect("ButtonPress", 1f, 0.98f, 1.02f, 0.05f);

        [Tooltip("Sonido de pelota rebotando")]
        [SerializeField] private SoundEffect ballBounceSound = new SoundEffect("BallBounce", 1f, 0.9f, 1.15f, 0.04f);

        [Tooltip("Sonido de lanzar pelota")]
        [SerializeField] private SoundEffect ballThrowSound = new SoundEffect("BallThrow", 1f, 0.95f, 1.05f, 0.05f);

        [Header("Audio Pool Settings")]
        [SerializeField] private int initialSfxPoolSize = 10;

        private readonly List<AudioSource> _sfxPool = new List<AudioSource>();

        // Dual music sources for smooth crossfades
        private AudioSource _musicSourceA;
        private AudioSource _musicSourceB;
        private AudioSource _activeMusicSource;
        private Coroutine _musicCrossfadeCoroutine;

        public float MasterVolume
        {
            get => masterVolume;
            set
            {
                masterVolume = Mathf.Clamp01(value);
                UpdateMusicVolume();
            }
        }

        public float SFXVolume
        {
            get => sfxVolume;
            set => sfxVolume = Mathf.Clamp01(value);
        }

        public float MusicVolume
        {
            get => musicVolume;
            set
            {
                musicVolume = Mathf.Clamp01(value);
                UpdateMusicVolume();
            }
        }

        public AudioClip MenuMusic
        {
            get => menuMusic;
            set => menuMusic = value;
        }

        public AudioClip GameplayMusic
        {
            get => gameplayMusic;
            set => gameplayMusic = value;
        }

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            if (transform.parent == null)
            {
                DontDestroyOnLoad(gameObject);
            }

            InitializeMusicSources();
            InitializeSfxPool();
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += HandleSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= HandleSceneLoaded;
        }

        private void Start()
        {
            if (autoSwitchMusicByScene)
            {
                PlayMusicForCurrentScene(SceneManager.GetActiveScene().name);
            }
        }

        private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (autoSwitchMusicByScene)
            {
                PlayMusicForCurrentScene(scene.name);
            }
        }

        private void PlayMusicForCurrentScene(string sceneName)
        {
            if (sceneName.Contains("Menu", System.StringComparison.OrdinalIgnoreCase))
            {
                PlayMenuMusic();
            }
            else if (sceneName.Contains("Gameplay", System.StringComparison.OrdinalIgnoreCase))
            {
                PlayGameplayMusic();
            }
        }

        #region Initialization

        private void InitializeMusicSources()
        {
            _musicSourceA = gameObject.AddComponent<AudioSource>();
            _musicSourceA.playOnAwake = false;
            _musicSourceA.spatialBlend = 0f;
            _musicSourceA.loop = true;

            _musicSourceB = gameObject.AddComponent<AudioSource>();
            _musicSourceB.playOnAwake = false;
            _musicSourceB.spatialBlend = 0f;
            _musicSourceB.loop = true;

            _activeMusicSource = _musicSourceA;
        }

        private void InitializeSfxPool()
        {
            for (int i = 0; i < initialSfxPoolSize; i++)
            {
                CreateNewSfxSource();
            }
        }

        private AudioSource CreateNewSfxSource()
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.spatialBlend = 0f;
            source.loop = false;
            _sfxPool.Add(source);
            return source;
        }

        private AudioSource GetAvailableSfxSource()
        {
            for (int i = 0; i < _sfxPool.Count; i++)
            {
                if (!_sfxPool[i].isPlaying)
                {
                    return _sfxPool[i];
                }
            }

            return CreateNewSfxSource();
        }

        #endregion

        #region Music Management

        /// <summary>
        /// Reproduce la musica asignada al Menu Principal.
        /// </summary>
        public void PlayMenuMusic(float fadeDuration = -1f)
        {
            if (menuMusic == null) return;
            PlayMusic(menuMusic, true, fadeDuration);
        }

        /// <summary>
        /// Reproduce la musica asignada al Gameplay.
        /// </summary>
        public void PlayGameplayMusic(float fadeDuration = -1f)
        {
            if (gameplayMusic == null) return;
            PlayMusic(gameplayMusic, true, fadeDuration);
        }

        /// <summary>
        /// Reproduce cualquier pista de musica con soporte de crossfade suave.
        /// </summary>
        public void PlayMusic(AudioClip clip, bool loop = true, float fadeDuration = -1f)
        {
            if (clip == null) return;

            // Si ya esta sonando el mismo clip, no lo reiniciamos
            if (_activeMusicSource != null && _activeMusicSource.clip == clip && _activeMusicSource.isPlaying)
            {
                return;
            }

            float duration = fadeDuration >= 0f ? fadeDuration : musicCrossfadeDuration;

            if (_musicCrossfadeCoroutine != null)
            {
                StopCoroutine(_musicCrossfadeCoroutine);
            }

            _musicCrossfadeCoroutine = StartCoroutine(CrossfadeMusicCoroutine(clip, loop, duration));
        }

        /// <summary>
        /// Detiene la musica actual con o sin fade-out.
        /// </summary>
        public void StopMusic(float fadeDuration = -1f)
        {
            float duration = fadeDuration >= 0f ? fadeDuration : musicCrossfadeDuration;
            if (_musicCrossfadeCoroutine != null)
            {
                StopCoroutine(_musicCrossfadeCoroutine);
            }
            _musicCrossfadeCoroutine = StartCoroutine(CrossfadeMusicCoroutine(null, false, duration));
        }

        public void PauseMusic()
        {
            _activeMusicSource?.Pause();
        }

        public void ResumeMusic()
        {
            _activeMusicSource?.UnPause();
        }

        private IEnumerator CrossfadeMusicCoroutine(AudioClip newClip, bool loop, float duration)
        {
            AudioSource outgoingSource = _activeMusicSource;
            AudioSource incomingSource = (_activeMusicSource == _musicSourceA) ? _musicSourceB : _musicSourceA;

            float targetMaxVolume = masterVolume * musicVolume;

            if (newClip != null)
            {
                incomingSource.clip = newClip;
                incomingSource.loop = loop;
                incomingSource.volume = 0f;
                incomingSource.Play();
            }

            if (duration <= 0.01f)
            {
                if (outgoingSource != null)
                {
                    outgoingSource.Stop();
                    outgoingSource.volume = 0f;
                }
                if (newClip != null)
                {
                    incomingSource.volume = targetMaxVolume;
                }
            }
            else
            {
                float startOutgoingVolume = outgoingSource != null ? outgoingSource.volume : 0f;
                float elapsed = 0f;

                while (elapsed < duration)
                {
                    elapsed += Time.unscaledDeltaTime;
                    float t = Mathf.Clamp01(elapsed / duration);

                    if (outgoingSource != null)
                    {
                        outgoingSource.volume = Mathf.Lerp(startOutgoingVolume, 0f, t);
                    }
                    if (newClip != null)
                    {
                        incomingSource.volume = Mathf.Lerp(0f, targetMaxVolume, t);
                    }

                    yield return null;
                }

                if (outgoingSource != null)
                {
                    outgoingSource.Stop();
                    outgoingSource.volume = 0f;
                }
                if (newClip != null)
                {
                    incomingSource.volume = targetMaxVolume;
                }
            }

            _activeMusicSource = incomingSource;
            _musicCrossfadeCoroutine = null;
        }

        private void UpdateMusicVolume()
        {
            if (_activeMusicSource != null && _activeMusicSource.isPlaying && _musicCrossfadeCoroutine == null)
            {
                _activeMusicSource.volume = masterVolume * musicVolume;
            }
        }

        #endregion

        #region SFX Play Methods

        public void PlayJump()
        {
            PlaySound(jumpSound);
        }

        public void PlayLand()
        {
            PlaySound(landSound);
        }

        public void PlayGoal()
        {
            PlaySound(goalSound);
        }

        public void PlayButtonPress()
        {
            PlaySound(buttonPressSound);
        }

        public void PlayBallBounce(float volumeScale = 1f)
        {
            PlaySound(ballBounceSound, volumeScale);
        }

        public void PlayBallThrow()
        {
            PlaySound(ballThrowSound);
        }

        public void PlaySound(SoundEffect sound, float extraVolumeMultiplier = 1f)
        {
            if (sound == null || !sound.CanPlay()) return;

            AudioClip clip = sound.GetRandomClip();
            if (clip == null) return;

            float finalVolume = masterVolume * sfxVolume * sound.Volume * Mathf.Clamp01(extraVolumeMultiplier);
            if (finalVolume <= 0.0001f) return;

            AudioSource source = GetAvailableSfxSource();
            source.clip = clip;
            source.volume = finalVolume;
            source.pitch = sound.GetRandomPitch();
            source.Play();

            sound.RegisterPlay();
        }

        public void PlayClip(AudioClip clip, float volume = 1f, float pitch = 1f)
        {
            if (clip == null) return;

            float finalVolume = masterVolume * sfxVolume * Mathf.Clamp01(volume);
            if (finalVolume <= 0.0001f) return;

            AudioSource source = GetAvailableSfxSource();
            source.clip = clip;
            source.volume = finalVolume;
            source.pitch = pitch;
            source.Play();
        }

        #endregion

        #region Volume Public Setters

        public void SetMasterVolume(float volume)
        {
            MasterVolume = volume;
        }

        public void SetSFXVolume(float volume)
        {
            SFXVolume = volume;
        }

        public void SetMusicVolume(float volume)
        {
            MusicVolume = volume;
        }

        #endregion
    }
}
