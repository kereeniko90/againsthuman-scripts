using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MusicManager : MonoBehaviour
{
     
    [SerializeField] private List<AudioClip> musicPlaylist; 
    [SerializeField] private float crossfadeDuration = 2f;
    private AudioSource audioSource1; 
    private AudioSource audioSource2;  

    private AudioSource currentAudioSource; 
    private AudioSource nextAudioSource;
    private int currentTrackIndex = -1; 
    private bool isCrossfading = false;
    private float musicVolume;

    private void Awake() {
        musicVolume = PlayerPrefs.GetFloat("musicVolume", 0.5f); 

        AudioSource[] audioSources = GetComponents<AudioSource>();
        if (audioSources.Length < 2)
        {
            Debug.LogError("MusicManager requires exactly 2 AudioSource components on the same GameObject.");
            return;
        }

        audioSource1 = audioSources[0];
        audioSource2 = audioSources[1];
    }

    private void Start()
    {
        if (musicPlaylist == null || musicPlaylist.Count == 0)
        {
            Debug.LogWarning("Music playlist is empty!");
            return;
        }

        
        currentAudioSource = audioSource1;
        nextAudioSource = audioSource2;

        
        ShufflePlaylist();
        PlayNextTrack();
    }

    private void ShufflePlaylist()
    {
        for (int i = 0; i < musicPlaylist.Count; i++)
        {
            int randomIndex = Random.Range(0, musicPlaylist.Count);
            AudioClip temp = musicPlaylist[i];
            musicPlaylist[i] = musicPlaylist[randomIndex];
            musicPlaylist[randomIndex] = temp;
        }
    }

    private void PlayNextTrack()
    {
        currentTrackIndex = (currentTrackIndex + 1) % musicPlaylist.Count;

        if (!isCrossfading)
        {
            StartCoroutine(CrossfadeToNextTrack(musicPlaylist[currentTrackIndex]));
        }
    }

    private IEnumerator CrossfadeToNextTrack(AudioClip nextClip)
    {
        isCrossfading = true;

        
        nextAudioSource.clip = nextClip;
        nextAudioSource.volume = 0f;
        nextAudioSource.Play();

        float timer = 0f;

        
        while (timer < crossfadeDuration)
        {
            timer += Time.deltaTime;
            float progress = timer / crossfadeDuration;

            currentAudioSource.volume = Mathf.Lerp(musicVolume, 0f, progress);
            nextAudioSource.volume = Mathf.Lerp(0f, musicVolume, progress);

            yield return null;
        }

        
        currentAudioSource.Stop();
        AudioSource temp = currentAudioSource;
        currentAudioSource = nextAudioSource;
        nextAudioSource = temp;

        isCrossfading = false;

        
        StartCoroutine(WaitForTrackToEnd());
    }

    private IEnumerator WaitForTrackToEnd()
    {
        
        yield return new WaitForSeconds(currentAudioSource.clip.length - crossfadeDuration);

        
        PlayNextTrack();
    }
}
