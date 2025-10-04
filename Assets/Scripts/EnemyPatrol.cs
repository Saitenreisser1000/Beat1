using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    [Header("Patrol Settings")]
    public Transform pointA;
    public Transform pointB;
    public float patrolSpeed = 2f;

    [Header("Chase Settings")]
    public float chaseSpeed = 4f;
    public float detectionRange = 5f;

    [Header("References")]
    public Transform player;

    [Header("Obstacle Detection")]
    public float obstacleDetectionDistance = 0.5f;
    public LayerMask obstacleLayer;

    private Vector3 targetPoint;
    private float direction = 0.25f; // 1 = rechts, -1 = links

    [Header("Bewegungsausmaße relativ zur Startposition")]
    [SerializeField] private float xPlus = 0f;
    [SerializeField] private float xMinus = 0f;
    [SerializeField] private float yPlus = 0f;
    [SerializeField] private float yMinus = 0f;

    [Header("Geschwindigkeit")]
    [SerializeField] private float speed = 2f;

    private Vector3 startPos;
    private Vector3 targetPos;

    void Start()
    {
        if (pointA != null && pointB != null)
        {
            targetPoint = pointB.position;
        }
        if (yPlus > 0 || yMinus < 0 || xPlus > 0 || xMinus < 0)
        {
            // Bewegung im Rechteck
            startPos = transform.position;
            targetPos = startPos + new Vector3(xPlus, 0, 0); // Erstes Ziel
        }
            direction = 0.25f; // Start nach rechts
            transform.localScale = new Vector3(direction, 0.25f, 1);
    }

    void Update()
    {
        // Abstand zum Spieler
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // Geschwindigkeit wählen
        float speed = distanceToPlayer <= detectionRange ? chaseSpeed : patrolSpeed;

        if (pointA != null && pointB != null)
        {
            PatrolBetweenPoints(speed, distanceToPlayer);
        }
        else
        {
            PatrolWithObstacleDetection(speed);
        }
    // Bewegung im Rechteck
    if (yPlus > 0 || yMinus < 0 || xPlus > 0 || xMinus < 0)
        // Bewege das Objekt Richtung Zielposition
        transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);

        // Wenn das Ziel erreicht ist -> nächstes Ziel wählen
        if (Vector3.Distance(transform.position, targetPos) < 0.01f)
        {
            ChooseNextTarget();
        }
    }

    void ChooseNextTarget()
    {
        // Reihenfolge: X+ -> Y+ -> X- -> Y- -> zurück zu X+
        if (targetPos == startPos + new Vector3(xPlus, 0, 0))
            targetPos = startPos + new Vector3(0, yPlus, 0);
        else if (targetPos == startPos + new Vector3(0, yPlus, 0))
            targetPos = startPos + new Vector3(-xMinus, 0, 0);
        else if (targetPos == startPos + new Vector3(-xMinus, 0, 0))
            targetPos = startPos + new Vector3(0, -yMinus, 0);
        else
            targetPos = startPos + new Vector3(xPlus, 0, 0); // wieder von vorne
    }

    void PatrolBetweenPoints(float speed, float distanceToPlayer)
    {
        // Bewegung zwischen Punkten
        transform.position = Vector2.MoveTowards(transform.position, targetPoint, speed * Time.deltaTime);

        // Ziel wechseln, wenn erreicht (nur wenn Spieler nicht in Reichweite)
        if (Vector2.Distance(transform.position, targetPoint) < 0.1f && distanceToPlayer > detectionRange)
        {
            targetPoint = targetPoint == pointA.position ? pointB.position : pointA.position;
        }

        // Sprite-Richtung
        if (targetPoint.x > transform.position.x)
            transform.localScale = new Vector3(0.25f, 0.25f, 1);
        else
            transform.localScale = new Vector3(-0.25f, 0.25f, 1);
    }

    void PatrolWithObstacleDetection(float speed)
    {
        // Raycast nach vorne
        Vector2 origin = new Vector2(transform.position.x, transform.position.y+0.1f);
        Vector2 dir = transform.right * direction;
        RaycastHit2D hit = Physics2D.Raycast(origin, dir, obstacleDetectionDistance, obstacleLayer);
        Debug.DrawRay(origin, dir * obstacleDetectionDistance, Color.red);

        if (hit.collider != null && hit.collider.CompareTag("Ground"))
        {
            Debug.Log("Obstacle detected: " + hit.collider.name);
            // Hindernis erkannt → umdrehen
            direction *= -1;
            transform.localScale = new Vector3(direction, 0.25f, 1);
        }

        // Bewegung
        transform.Translate(Vector2.right * direction * speed * Time.deltaTime);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {   
        if (collision.gameObject.CompareTag("Player"))
        {
            // Gegner stoppt oder kann Schaden verursachen
            patrolSpeed = 0f;
            chaseSpeed = 0f;
            Debug.Log("Player touched!");
        }
    }
}
