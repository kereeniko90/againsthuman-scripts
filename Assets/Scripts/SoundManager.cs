using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{   
    public static SoundManager Instance { get; private set;}
    public enum Sound {
        CactusHit,
        IceHit,
        MushroomHit,
        TurtleHit,
        Rotate,
        Boop,
        BoopSoft,
        Tik,
        EnemyMaleDie,
        EnemyFemaleDie,
        GlassHit,
        Win,
        Defeat,
        EnterPortal,
        ExitPortal,
        InteractableButton,
        BossWave,
        Block,
        Evade,
        UpgradeSound,
        WaveStart,
        
    }

    private AudioSource audioSource;
    private Dictionary<Sound, AudioClip> soundDictionary;

    private void Awake() {
        Instance = this;
        audioSource = GetComponent<AudioSource>();
        soundDictionary = new Dictionary<Sound, AudioClip>();

        foreach (Sound sound in System.Enum.GetValues(typeof(Sound))) {
            soundDictionary[sound] = Resources.Load<AudioClip>(sound.ToString());
        }
    }

    public void PlaySound(Sound sound) {
        audioSource.PlayOneShot(soundDictionary[sound]);
    }   
}
