using UnityEngine;
using System.Collections;

public class WolfBoss : MonoBehaviour
{
    public Transform player;
    
    public GameObject minionPrefab;
    public GameObject swipeSlashPrefab;
    public Transform[] summonPoints;

    public float lungeSpeed = 5f;
    public float lungeDuration = 0.3f;
    public GameObject lungeHitboxPrefab; // Prefab with a trigger collider + damage script
    public Transform hitboxSpawnPoint;   // An empty GameObject in front of wolf's mouth

    private bool isLunging = false;


    private bool isBattling;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip lungeSFX;
    public AudioClip howlSFX;
    public AudioClip swipeSFX;

    //[Header("Visual FX")]
    //public GameObject lungeEffectPrefab;
    //public GameObject howlEffectPrefab;

    public void StartBattle()
    {
        isBattling = true;
        StartCoroutine(BossLoop());
    }

    private IEnumerator BossLoop()
    {
        while (isBattling)
        {
            int move = Random.Range(0, 3);

            switch (move)
            {
                case 0: yield return LungeAttack(); break;
                case 1: yield return HowlSummon(); break;
                case 2: yield return SwipeAttack(); break;
            }

            yield return new WaitForSeconds(1.5f);
        }
    }
    public void StartLungeAttack()
    {
        if (!isLunging)
            StartCoroutine(LungeAttack());
    }

    private IEnumerator LungeAttack()
    {
        isLunging = true;

        // Optional: play lunge sound or animation
        Vector3 direction = transform.localScale.x > 0 ? Vector3.right : Vector3.left;

        // Spawn hitbox
        GameObject hitbox = Instantiate(lungeHitboxPrefab, hitboxSpawnPoint.position, Quaternion.identity);
        hitbox.transform.localScale = new Vector3(transform.localScale.x, 1, 1); // Flip hitbox if needed

        float elapsed = 0f;
        while (elapsed < lungeDuration)
        {
            transform.position += direction * lungeSpeed * Time.deltaTime;
            elapsed += Time.deltaTime;
            yield return null;
        }

        Destroy(hitbox);
        isLunging = false;
    }

    private IEnumerator HowlSummon()
    {
        // Play sound
        audioSource.PlayOneShot(howlSFX);

        // Visual FX
        //if (howlEffectPrefab)
            //Instantiate(howlEffectPrefab, transform.position, Quaternion.identity);

        yield return new WaitForSeconds(0.3f);

        foreach (var point in summonPoints)
        {
            Instantiate(minionPrefab, point.position, Quaternion.identity);
            yield return new WaitForSeconds(0.3f);
        }
    }

    private IEnumerator SwipeAttack()
    {
        // Play audio, screen shake, etc. here
        Instantiate(swipeSlashPrefab, transform.position, Quaternion.identity);
        yield return new WaitForSeconds(0.5f); // wait before next action
    }
}
