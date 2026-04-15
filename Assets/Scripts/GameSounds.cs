using UnityEngine;

public class GameSounds : MonoBehaviour
{
    public AudioSource ambientSound;


    void Start()
    {
        if (ambientSound != null)
        {
            ambientSound.loop = true;
            ambientSound.Play();
        }
    }

}