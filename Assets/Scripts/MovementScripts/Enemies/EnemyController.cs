using UnityEngine;

public class EnemyController : MonoBehaviour
{
    //This is the controller that dictates enemies movement - once the player enters the Danger Zone
    //The enemies will start following them
    private Transform playerTransform;
    private bool isPlayerInZone = false;
    //to be modified in editor - play with it and tell me how it feels
    [SerializeField]
    private float dangerZone;
    [SerializeField]
    private float minDistance;
    [SerializeField]
    private float moveSpeed;

    Rigidbody rb;

    private void Start()
    {
        //I tagged the player, so that it starts by finding their position
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        rb = GetComponent<Rigidbody>();

    }
    private void Update()
    {
        if (playerTransform == null)
        {
            Debug.LogWarning("We don't have a player");
            return;
        }
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        if (distanceToPlayer <= dangerZone)
        {
            //the player enters the observation zone of an enemy, so it starts following them
            isPlayerInZone = true;
            //an enemy can't jump after you, his y is constant
            Vector3 targetPosition = new Vector3(playerTransform.position.x, transform.position.y, playerTransform.position.z);
            //the enemy tries to face you
            transform.LookAt(targetPosition);
            //once it's too close, it stops following you (we don't want the enemy to eat us :( )
            if (distanceToPlayer >= minDistance)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

            }
        }
        else
        {
            //it stops following us the moment we escape their zone
            isPlayerInZone = false;
            rb.linearDamping = 1f; // stops the enemy from moving
        }
    }
    //this function is here just to help you visualise the distances - NOT in game mode (especially when you modify them in editor)
    //i acidentally made danger zone too big and did not understand why it never stopped following me
    private void OnDrawGizmos()
    {
        // Draws a yellow Sphere on the DangerZone
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, dangerZone);

        // Draws a blue Sphere that shows the minDistance (where it can't touch the player)
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, minDistance);

        
        if (playerTransform != null)
        {
            // Draws a red line if the player is too close
            if (isPlayerInZone)
            {
                Gizmos.color = Color.red;
            }
            // A green one if it is outside his Danger Zone
            else
            {
                Gizmos.color = Color.green;
            }
            Gizmos.DrawLine(transform.position, playerTransform.position);
        }
    }


}
