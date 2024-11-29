using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SoundType
{
    BGM,
    EFFECT,
    MAXCOUNT,
}

public class SoundManager
{
    public static SoundManager Instance { get; private set; } // 외부에서 접근 가능, 변경 불가능

    public AudioSource[] audioSources = new AudioSource[(int)SoundType.MAXCOUNT];
    private Dictionary<string, AudioClip> audioClips = new Dictionary<string, AudioClip>(); // 사운드 파일을 저장할 딕셔너리 <경로, 해당 오디오 클립> -> Object Pooling

    //private 생성자 : 외부에서 생성 불가능
    private SoundManager() { }

    // 정적 생성자 : 클래스 로드 시 호출되며, 단 한 번 실행
    static SoundManager()
    {
        Instance = new SoundManager();
    }

    private AudioClip GetOrAddAudioClip(string path, SoundType type = SoundType.EFFECT)
    {
        if (path.Contains("Sounds/") == false)
            path = $"Sounds/{path}"; // Sounds 폴더 안에 저장될 수 있도록

        AudioClip audioClip = null;

        if (type == SoundType.BGM) // BGM 배경음악 클립 붙이기
        {
            audioClip = Resources.Load<AudioClip>(path);
        }
        else // Effect 효과음 클립 붙이기
        {
            if (audioClips.TryGetValue(path, out audioClip) == false)
            {
                audioClip = Resources.Load<AudioClip>(path);
                audioClips.Add(path, audioClip);
            }
        }

        if (audioClip == null)
            Debug.LogFormat("[SoundManager] 오디오 클립이 없습니다: {0}", path);

        return audioClip;
    }

    public void Init()
    {
        GameObject root = GameObject.Find("@Sound");
        if (root == null)
        {
            root = new GameObject { name = "@Sound" };
            Object.DontDestroyOnLoad(root);

            string[] soundNames = System.Enum.GetNames(typeof(SoundType)); // "BGM", "EFFECT"
            for (int i = 0; i < soundNames.Length - 1; i++)
            {
                GameObject go = new GameObject { name = soundNames[i] };
                audioSources[i] = go.AddComponent<AudioSource>();
                go.transform.parent = root.transform;
            }

            audioSources[(int)SoundType.BGM].loop = true;       // bgm 재생기는 무한 반복 재생
            audioSources[(int)SoundType.BGM].volume = 1f;
            audioSources[(int)SoundType.EFFECT].volume = 1f;
        }
    }

    public void Clear()
    {
        // 재생기 전부 재생 스탑, 음반 빼기
        foreach (AudioSource audioSource in audioSources)
        {
            audioSource.clip = null;
            audioSource.Stop();
        }
        // 효과음 Dictionary 비우기
        audioClips.Clear();
    }

    public void Play(AudioClip audioClip, SoundType type = SoundType.EFFECT, float pitch = 1.0f, float volume = 1.0f)
    {
        if (audioClip == null)
            return;

        if (type == SoundType.BGM) // BGM 배경음악 재생
        {
            AudioSource audioSource = audioSources[(int)type];
            if (audioSource.isPlaying)
                audioSource.Stop();

            audioSource.pitch = pitch;
            audioSource.clip = audioClip;
            audioSource.volume = volume;
            audioSource.Play();
        }
        else // Effect 효과음 재생
        {
            AudioSource audioSource = audioSources[(int)type];
            audioSource.pitch = pitch;
            audioSource.PlayOneShot(audioClip);
        }
    }

    public void Play(string path, SoundType type = SoundType.EFFECT, float pitch = 1.0f, float volume = 1.0f)
    {
        AudioClip audioClip = GetOrAddAudioClip(path, type);
        Play(audioClip, type, pitch, volume);
    }

    public void Stop(AudioClip audioClip, SoundType type = SoundType.EFFECT)
    {
        if (audioClip == null)
            return;

        if (type == SoundType.BGM) // BGM 배경음악 정지
        {
            AudioSource audioSource = audioSources[(int)SoundType.BGM];
            if (audioSource.clip == audioClip)
            {
                audioSource.Stop();
                audioSource.clip = null;
            }
        }
        else // Effect 효과음 정지
        {
            AudioSource audioSource = audioSources[(int)SoundType.EFFECT];
            if (audioSource.clip == audioClip)
            {
                audioSource.Stop();
                audioSource.clip = null;
            }
        }
    }

    public void Stop(string path, SoundType type = SoundType.EFFECT)
    {
        AudioClip audioClip = GetOrAddAudioClip(path, type);
        Stop(audioClip, type);
    }

    public void StopAll()
    {
        foreach (AudioSource audioSource in audioSources)
        {
            audioSource.Stop();
            audioSource.clip = null;
        }
    }

    public void FadeOut(float duration, string path)
    {
        AudioClip audioClip = GetOrAddAudioClip(path, SoundType.BGM);

        AudioSource audioSource = audioSources[(int)SoundType.BGM];
        {
            if (audioSource != null)
            {
                audioSource.clip = audioClip;

                if (!audioSource.isPlaying)
                {
                    audioSource.Play();
                }
                //볼륨을 서서히 줄이기 위한 코루틴 호출
                CoroutineHandler.StartStaticCoroutine(FadeOutCoroutine(audioSource, duration));
            }
        }
    }

    private IEnumerator FadeOutCoroutine(AudioSource audioSource, float duration)
    {
        float startVolume = audioSource.volume;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, 0f, elapsedTime / duration);
            yield return null;
        }

        audioSource.Stop();
        audioSource.volume = startVolume;
    }

    public void FadeIn(float duration, string path)
    {
        AudioClip audioClip = GetOrAddAudioClip(path, SoundType.BGM);

        AudioSource audioSource = audioSources[(int)SoundType.BGM];

        if (audioSource != null)
        {
            audioSource.clip = audioClip;
            audioSource.volume = 0f;

            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
            //볼륨을 서서히 키우기 위한 코루틴 호출
            CoroutineHandler.StartStaticCoroutine(FadeInCoroutine(audioSource, duration));
        }
    }

    private IEnumerator FadeInCoroutine(AudioSource audioSource, float duration)
    {
        float startVolume = audioSource.volume;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, 1f, elapsedTime / duration);
            yield return null;
        }
        audioSource.volume = 1;
    }

    public void OnVolumeChanged(float value, SoundType type)
    {
        if (type == SoundType.BGM)
        {
            AudioSource audioSource = audioSources[(int)type];
            audioSource.volume = value;
        }
        else
        {
            AudioSource audioSource = audioSources[(int)type];
            audioSource.volume = value;
        }
    }

}