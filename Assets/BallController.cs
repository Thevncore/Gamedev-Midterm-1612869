using UnityEngine;

public class BallController : MonoBehaviour
{
    public float constantSpeed = 10f;
    public float smoothingFactor = 1f;
    public GameObject sputnikPrefab;
    public bool jiggle = true;

    private Rigidbody2D body;
    private Vector3 mousePosition;
    private Vector2 gravityShot = new Vector2(0, -1.5f);
    private float elapsedGravityTime = 0;

    public Vector2 Velocity
    {
        get { return body.velocity; }
        set { body.velocity = value; }
    }

    private void Awake()
    {
        Initializer.balls.Add(gameObject);        
    }

    private void Start()
    {
        body = GetComponent<Rigidbody2D>();
        body.AddRelativeForce(new Vector2(2, 0), ForceMode2D.Impulse);
    }

    // Update is called once per frame
    private void Update()
    {
        elapsedGravityTime += Time.deltaTime;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        var sps = GetComponent<ParticleSystem>().subEmitters.GetSubEmitterSystem(0);
        var emitparams = new ParticleSystem.EmitParams()
        {
            position = GetComponent<Transform>().position,
            applyShapeToPosition = true,

        };
        sps.Emit(emitparams, 50);
        var currentVelocity = body.velocity;

        if (Mathf.Abs(currentVelocity.y) < 0.001f)
            body.velocity = new Vector2(currentVelocity.x, currentVelocity.y - transform.position.y / 10);

        if (Mathf.Abs(currentVelocity.x) < 0.001f)
            body.velocity = new Vector2(currentVelocity.x - transform.position.x / 10, currentVelocity.y);
    }

    private void FixedUpdate()
    {
        float clampedVelocity = Mathf.Clamp(body.velocity.magnitude, 0, 25);
        //direction = Vector2.Angle(Vector2.zero, body.velocity);

        var cvel = body.velocity;
        var tvel = cvel.normalized * constantSpeed;
        body.velocity = Vector3.Lerp(cvel, tvel, Time.deltaTime * smoothingFactor);

        if (Mathf.Abs(body.velocity.y) < 0.01f)
            body.velocity = new Vector2(body.velocity.x, 10 * body.velocity.y);

        if (Mathf.Abs(body.velocity.x) < 0.01f)
            body.velocity = new Vector2(body.velocity.x * 10, body.velocity.y);

        if (transform.position.x > Initializer.RightBound)
        {
            transform.position = new Vector3(Initializer.RightBound, transform.position.y, 100);
            //body.velocity = direction.normalized * clampedVelocity;
        }

        if (transform.position.x < Initializer.LeftBound)
        {
            transform.position = new Vector3(Initializer.LeftBound, transform.position.y, 100);
            //body.velocity = direction.normalized * clampedVelocity;
        }

        if (transform.position.y > Initializer.TopBound)
        {
            transform.position = new Vector3(transform.position.x, Initializer.TopBound, 100);
            //body.velocity = direction.normalized * clampedVelocity;
        }

        if (transform.position.y < Initializer.BottomBound)
        {
            transform.position = new Vector3(transform.position.x, Initializer.BottomBound, 100);
            //body.velocity = direction.normalized * clampedVelocity;
        }

        if (Input.GetMouseButton(1) && elapsedGravityTime > 0.1f)
        {
            //Debug.Log("Gravity!");
            body.AddForce(gravityShot, ForceMode2D.Impulse);
            elapsedGravityTime = 0;
        }

        if (Velocity.magnitude > 20f)
        {
            Velocity = 20f * Velocity.normalized;
        }

        if (jiggle)
        {
            while (Mathf.Abs(Velocity.x) < 0.5f)
                Velocity = new Vector2(Random.Range(-1f, 1f), Velocity.y);

            while (Mathf.Abs(Velocity.y) < 0.5f)
                Velocity = new Vector2(Velocity.x, Random.Range(-1f, 1f));
        }

    }

    public void CreateSputnik()
    {
        var sputnikPosition = GetComponent<RectTransform>().position;
        sputnikPosition.z = 110;

        var newBlueSputnik = Instantiate(sputnikPrefab, sputnikPosition, Quaternion.identity);
        newBlueSputnik.name = "Blue sputnik";
        var blueController = newBlueSputnik.GetComponent<SputnikController>();
        blueController.Ball = gameObject;
        blueController.SetSputnikType(0);

        var newGreenSputnik = Instantiate(sputnikPrefab, sputnikPosition, Quaternion.identity);
        newGreenSputnik.name = "Green sputnik";
        var greenController = newGreenSputnik.GetComponent<SputnikController>();
        greenController.Ball = gameObject;
        greenController.SetSputnikType(1);

        var newRedSputnik = Instantiate(sputnikPrefab, sputnikPosition, Quaternion.identity);
        newRedSputnik.name = "Red sputnik";
        var redController = newRedSputnik.GetComponent<SputnikController>();
        redController.Ball = gameObject;
        redController.SetSputnikType(2);
    }
}
