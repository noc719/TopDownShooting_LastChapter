using UnityEngine;

public class SoundSource :MonoBehaviour
{
    private AudioSource audioSource;

    public void Play(AudioClip clip, float soundEffectVolume, float soundEffectPitchVariance)
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        CancelInvoke(); //모든 반복되는 함수를 취소
        audioSource.clip = clip;
        audioSource.volume = soundEffectVolume;
        audioSource.pitch = 1f + Random.Range(-soundEffectPitchVariance, soundEffectPitchVariance);
        audioSource.Play();

        Invoke("Disable", clip.length + 2);

    }
    public void Disable()
    {
        audioSource.Stop();
        gameObject.SetActive(false);
    }
}