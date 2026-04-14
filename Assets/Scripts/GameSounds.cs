using UnityEngine;

public class GameSounds : MonoBehaviour
{
    public AudioSource ambientSound;
    public AudioSource attackSound;
    public AudioSource enemySound;
    public AudioSource deathSound;
    public AudioSource winSound;

    void Start()
    {
        if (ambientSound != null)
        {
            ambientSound.loop = true;
            ambientSound.Play();
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (attackSound != null)
            {
                attackSound.Play();
            }
        }
    }

    public void PlayEnemySound()
    {
        if (enemySound != null)
        {
            enemySound.Play();
        }
    }

    public void PlayDeathSound()
    {
        if (deathSound != null)
        {
            deathSound.Play();
        }
    }

    public void PlayWinSound()
    {
        if (winSound != null)
        {
            winSound.Play();
        }
    }
}