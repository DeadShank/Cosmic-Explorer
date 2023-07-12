using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CollisionHandler : MonoBehaviour
{
    [SerializeField] private float levelLoadDelay = 2f;
    [SerializeField] private AudioClip crash;
    [SerializeField] private AudioClip success;

    [SerializeField] private ParticleSystem crashParticles;
    [SerializeField] private ParticleSystem successParticles;

    AudioSource audioSource;

    bool isTransitioning = false;
    bool collisionDisabled = false;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        RespondToDebugKeys();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isTransitioning || collisionDisabled) { return; }

        switch (collision.gameObject.tag)
        {
            case "Friendly":
                Debug.Log("Friendly");
                break;
            case "Finish":
                UniqSequence("LoadNextLevel", success, successParticles);
                break;
            default:
                UniqSequence("ReloadLevel", crash, crashParticles);
                break;
        }
    }

    private void UniqSequence(string loadLevel, AudioClip audio, ParticleSystem particle)
    {
        GetComponent<Movement>().enabled = false;
        particle.Play();
        audioSource.Stop();
        audioSource.PlayOneShot(audio);
        isTransitioning = true;
        Invoke(loadLevel, levelLoadDelay);
    }
    private void ReloadLevel()
    {
        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentIndex);
    }

    void LoadNextLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;
        if (nextSceneIndex == SceneManager.sceneCountInBuildSettings)
        {
            nextSceneIndex = 0;
        }
        SceneManager.LoadScene(nextSceneIndex);
    }

    void RespondToDebugKeys()
    {
        if (Input.GetKeyDown(KeyCode.L)) {
            LoadNextLevel();
        }
        else if(Input.GetKeyDown(KeyCode.C)) 
        {
            collisionDisabled = !collisionDisabled;
        }
    }
}
