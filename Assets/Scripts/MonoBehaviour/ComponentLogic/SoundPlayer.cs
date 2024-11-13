using System.Collections;
using UnityEngine;

public class SoundPlayer : MonoBehaviour
{
  [SerializeField] private AudioClip[] audioClips;
  [SerializeField] private float nextPlayWaitMin = 4.0f;
  [SerializeField] private float nextPlayWaitMax = 8.0f;
  private AudioSource audioSource;

  void Start()
  {
    audioSource = GetComponent<AudioSource>();
    StartCoroutine(PlaySoundCoroutine());
  }

  private IEnumerator PlaySoundCoroutine()
  {
      while (true)
      {
          // Play a random sound from the audioClips array
          if (audioClips.Length > 0)  // Ensure there's at least one clip in the array
          {
              AudioClip randomClip = audioClips[Random.Range(0, audioClips.Length)];
              audioSource.PlayOneShot(randomClip);
          }

          // Generate a random cooldown between minCooldownSeconds and maxCooldownSeconds
          float randomCooldown = Random.Range(nextPlayWaitMin, nextPlayWaitMax);
          yield return new WaitForSeconds(randomCooldown);
      }
  }
}