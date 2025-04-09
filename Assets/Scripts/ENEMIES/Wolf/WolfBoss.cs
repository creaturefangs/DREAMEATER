using System.Collections;
using UnityEngine;

public class WolfBoss : MonoBehaviour
{
    public Transform player;

    public GameObject minionPrefab;
    public GameObject swipeSlashPrefab;
    public GameObject missilePrefab; // New missile prefab
    public Transform[] summonPoints;

    // Howl control variables
    private int howlCount = 0; // Count of how many times howl has been used
    public int maxHowls = 3; // Maximum number of howls in a row
    public float howlDelay = 5f; // Delay after howls before the next set
    private bool isBattling;

    [Header("Audio")]
    public AudioSource audioSource; // For the boss SFX
    public AudioSource musicSource; // For the background music
    public AudioClip howlSFX;
    public AudioClip swipeSFX;
    public AudioClip bossBattleMusic; // Music that plays when the battle starts
    public AudioClip battleStartSFX; // Sound effect that plays when the battle starts

    // New variable to control how many minions spawn
    public int minionsToSpawn = 3; // Default to 3 minions

    // Delay before the boss starts
    public float battleStartDelay = 3f; // Delay before starting the battle

   
    [Header("Boss Dialogue")]
    public SO_Dialogue phaseTwoDialogue;
    public SO_Dialogue phaseThreeDialogue;

    private EnemyHealth enemyHealth;

    private bool isFighting = false;
    private bool phaseTwoTriggered = false;
    private bool phaseThreeTriggered = false;

    public int phaseTwoThreshold = 100;


    public void StartBattle()
    {
        StartCoroutine(DelayedStartBattle()); // Wait before starting the battle
        // Play the sound effect at the start of the battle
        if (audioSource != null && battleStartSFX != null)
        {
            audioSource.PlayOneShot(battleStartSFX);
        }
    }

    private IEnumerator DelayedStartBattle()
    {
        yield return new WaitForSeconds(battleStartDelay); // Wait for the delay before starting
        isBattling = true;

        // Play the boss battle start music and sound effect
        PlayBossBattleMusic();
        StartCoroutine(BossLoop());
    }

    private void PlayBossBattleMusic()
    {
        // Stop any current background music
        if (musicSource.isPlaying)
        {
            musicSource.Stop();
        }

        // Start playing the boss battle music
        musicSource.clip = bossBattleMusic;
        musicSource.Play();
    }

    private IEnumerator BossLoop()
    {
        while (isBattling)
        {
            int move = Random.Range(0, 3);

            switch (move)
            {
                case 0: yield return MissileAttack(); break; // Updated to missile attack
                case 1: yield return HowlSummon(); break;
                case 2: yield return SwipeAttack(); break;
            }

            yield return new WaitForSeconds(1.5f);
        }
    }

    // Missile Attack: Shoots a missile at the player's last known location
    private IEnumerator MissileAttack()
    {
        // Slight delay before the missile launches
        yield return new WaitForSeconds(0.5f);

        Vector3 playerLastKnownPos = player.position; // Get the last known player position
        GameObject missile = Instantiate(missilePrefab, transform.position, Quaternion.identity);
        missile.GetComponent<Rigidbody2D>().linearVelocity = (playerLastKnownPos - transform.position).normalized * 10f; // Adjust speed as needed

        yield return new WaitForSeconds(1f); // Missile flight time (adjust as needed)
    }

    private IEnumerator HowlSummon()
    {
        // Play sound
        audioSource.PlayOneShot(howlSFX);

        yield return new WaitForSeconds(0.3f);

        // Spawn minions based on the minionsToSpawn count
        for (int i = 0; i < minionsToSpawn; i++)
        {
            // Spawn a minion at each summon point
            foreach (var point in summonPoints)
            {
                Instantiate(minionPrefab, point.position, Quaternion.identity);
                yield return new WaitForSeconds(0.3f); // Delay between each spawn
            }
        }

        // Howl count logic: After 3 howls, wait before the next set
        howlCount++;
        if (howlCount >= maxHowls)
        {
            howlCount = 0; // Reset howl count
            yield return new WaitForSeconds(howlDelay); // Wait for some time before next howl series
        }
    }

    private IEnumerator SwipeAttack()
    {
        // Play swipe sound
        audioSource.PlayOneShot(swipeSFX);

        // Spawn the swipe effect
        GameObject swipe = Instantiate(swipeSlashPrefab, transform.position, Quaternion.identity);

        // Disable the swipe object immediately to prevent it from interacting with the game world
        swipe.SetActive(false);

        // Move swipe from left to right
        float swipeDuration = 1f; // Duration to move swipe (adjust as needed)
        Vector3 startPosition = swipe.transform.position;
        Vector3 endPosition = new Vector3(startPosition.x + 3f, startPosition.y, startPosition.z); // Adjust distance

        float elapsedTime = 0f;

        // Move the swipe
        while (elapsedTime < swipeDuration)
        {
            swipe.transform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / swipeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Once swipe movement is finished, disable the swipe object
        swipe.SetActive(false);

        // Reset the swipe object for next use
        swipe.transform.position = startPosition;
        swipe.SetActive(true);

        // Ensure the swipe prefab is properly reset for the next swipe
    }

    // Update the boss's health and trigger dialogue if needed
    private void Awake()
    {
        enemyHealth = GetComponent<EnemyHealth>();
        if (enemyHealth == null)
        {
            Debug.LogError("EnemyHealth component not found on WolfBoss!");
        }
    }

    private void Update()
    {
        if (!isFighting || enemyHealth == null) return;

        if (!phaseTwoTriggered && enemyHealth.health <= phaseTwoThreshold && enemyHealth.health > 1)
        {
            TriggerPhaseTwoDialogue();
        }

        if (!phaseThreeTriggered && enemyHealth.health == 1)
        {
            TriggerPhaseThreeDialogue();
        }
    }

    private void TriggerPhaseTwoDialogue()
    {
        phaseTwoTriggered = true;
        DialogueManager.Instance.StartInteraction(phaseTwoDialogue);
    }

    private void TriggerPhaseThreeDialogue()
    {
        phaseThreeTriggered = true;
        DialogueManager.Instance.StartInteraction(phaseThreeDialogue);
    }
}
