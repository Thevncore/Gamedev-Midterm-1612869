using UnityEngine;

public class SputnikController : MonoBehaviour
{
    public GameObject Ball;
    public static Sprite[] Sprites = null;

    public float gravity = 200;  //20
    public float constantSpeed = 50f;
    public float smoothingFactor = 5f;

    private Rigidbody2D body;
    private CircleCollider2D ballCollider;
    private Vector2 direction;



    void Awake()
    {
        if (Sprites == null)
        {
            Sprites = new Sprite[3]
            {
                Resources.Load<Sprite>("Sprites/Balls/sputnik-blue"),
                Resources.Load<Sprite>("Sprites/Balls/sputnik-green"),
                Resources.Load<Sprite>("Sprites/Balls/sputnik-red")
            };
        }
    }

    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        ballCollider = GetComponent<CircleCollider2D>();
        
        var sps = GetComponent<ParticleSystem>();//.subEmitters.GetSubEmitterSystem(0);

        var emitparams = new ParticleSystem.EmitParams()
        {
            position = GetComponent<Transform>().position,
            applyShapeToPosition = true,
            startSize = 0.1f
        };

        sps.Emit(emitparams, 250);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float clampedVelocity = Mathf.Clamp(body.velocity.magnitude, 0, 25);

        if (Ball.gameObject != null)
        {
            direction = Ball.transform.position - transform.position;
            transform.right = direction;
            Vector2 force = direction.magnitude * direction;
            body.AddForce(force * gravity * Time.fixedDeltaTime, ForceMode2D.Force);

            Vector2 drunk = new Vector2(Random.value * 2 - 1, Random.value * 2 - 1) * Initializer.OneMagnitude;
            body.AddForce(drunk, ForceMode2D.Force);
        }

        var cvel = body.velocity;
        var tvel = cvel.normalized * constantSpeed;
        body.velocity = Vector3.Lerp(cvel, tvel, Time.deltaTime * smoothingFactor);

        if (transform.position.x > Initializer.RightBound)
        {
            transform.position = new Vector3(Initializer.RightBound, transform.position.y, 110);
            body.velocity = direction.normalized * clampedVelocity;
        }

        if (transform.position.x < Initializer.LeftBound)
        {
            transform.position = new Vector3(Initializer.LeftBound, transform.position.y, 110);
            body.velocity = direction.normalized * clampedVelocity;
        }

        if (transform.position.y > Initializer.TopBound)
        {
            transform.position = new Vector3(transform.position.x, Initializer.TopBound, 110);
            body.velocity = direction.normalized * clampedVelocity;
        }

        if (transform.position.y < Initializer.BottomBound)
        {
            transform.position = new Vector3(transform.position.x, Initializer.BottomBound, 110);
            body.velocity = direction.normalized * clampedVelocity;
        }
    }

    public void SetSputnikType(int type = -1)
    {
        var spriteRenderer = GetComponent<SpriteRenderer>();
        if (type == -1)
        {
            type = Random.Range(0, Sprites.Length);
        }

        spriteRenderer.sprite = Sprites[type];

        switch (type)
        {
            case 0: // blue
                gravity = Random.Range(125f, 225f); // was 200
                constantSpeed = Random.Range(5.8f, 12.6f); // was 1
                smoothingFactor = Random.Range(0.2f, 0.8f); // was 0.5
                break;

            case 1: // green
                gravity = Random.Range(100f, 200f); // was 200
                constantSpeed = Random.Range(7.5f, 17.5f); // was 5
                smoothingFactor = 1f; // was 1
                break;

            case 2: // red;
                gravity = Random.Range(75f, 125f); // was 100
                constantSpeed = Random.Range(15.5f, 23.5f); // was 5
                smoothingFactor = 0.5f; // was 0.5
                break;
        }
    }

    void DrawX(Vector2 point)
    {
        Vector2 aStart = new Vector2(point.x - 1, point.y - 1);
        Vector2 aEnd = new Vector2(point.x + 1, point.y + 1);
        Vector2 bStart = new Vector2(point.x - 1, point.y + 1);
        Vector2 bEnd = new Vector2(point.x + 1, point.y - 1);

        Debug.DrawLine(aStart, aEnd);
        Debug.DrawLine(bStart, bEnd);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Debug.Log(gameObject.name + ":" + collision.relativeVelocity.magnitude);
        int particles = (int)(25* collision.relativeVelocity.magnitude / 35f);
        var sps = GetComponent<ParticleSystem>();//.subEmitters.GetSubEmitterSystem(0);
        var emitparams = new ParticleSystem.EmitParams()
        {
            position = GetComponent<Transform>().position,
            applyShapeToPosition = true,

        };
        sps.Emit(emitparams, particles);
    }
}
