using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickController : MonoBehaviour
{
    private float health;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D body;
    private Rect rect;
    private float lastCollisionTime;

    public GameObject prefabAnchorTop = null;
    public GameObject prefabAnchorBottom = null;
    public GameObject prefabAnchorLeft = null;
    public GameObject prefabAnchorRight = null;
    static Vector2 gravity = new Vector2(0, -9.8f);

    [SerializeField]
    private GameObject next;

    [SerializeField]
    private GameObject previous;

    public bool NextBound = false;
    public bool PreviousBound = false;
    public bool DyingFreefall = false;

    public GameObject Next
    {
        get { return next; }
        set
        {
            next = value;
        }
    }

    public GameObject Previous
    {
        get { return previous; }
        set
        {
            previous = value;
        }
    }

    public float Health
    {
        get
        {
            return health;
        }
        set
        {
            health = value;
            UpdateColor();
        }
    }

    private void UpdateColor()
    {
        spriteRenderer.color = Color.HSVToRGB(health / 3f, 1, 1);
    }

    private GameObject CreateAnchor(Vector3 position, Vector2 anchorPosition)
    {
        var anchor = Instantiate(prefabAnchorTop, position, Quaternion.identity);
        var spring = anchor.GetComponent<SpringJoint2D>();
        spring.connectedAnchor = anchorPosition;
        spring.connectedBody = body;
        return anchor;
    }

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        lastCollisionTime = Time.time;

        Health = 1f;

        body = GetComponent<Rigidbody2D>();
        rect = GetComponent<RectTransform>().rect;

        Debug.Log("Brick onstart");

        if (prefabAnchorTop != null)
        {
            prefabAnchorTop = CreateAnchor(new Vector3(transform.position.x, transform.position.y + 2, 0), new Vector2(0, rect.yMax));
            prefabAnchorTop.name = name + " (top anchor)";
        }

        if (prefabAnchorBottom != null)
        {
            prefabAnchorBottom = CreateAnchor(new Vector3(transform.position.x, transform.position.y - 2, 0), new Vector2(0, rect.yMin));
            prefabAnchorBottom.name = name + " (bottom anchor)";
        }

        if (prefabAnchorLeft != null)
        {
            prefabAnchorLeft = CreateAnchor(new Vector3(Initializer.LeftBound, transform.position.y, 0), new Vector2(rect.xMin, 0));
            previous = prefabAnchorLeft;
            prefabAnchorLeft.name = name + " (left anchor)";
        }

        if (prefabAnchorRight != null)
        {
            prefabAnchorRight = CreateAnchor(new Vector3(Initializer.RightBound, transform.position.y, 0), new Vector2(rect.xMax, 0));
            next = prefabAnchorRight;
            prefabAnchorRight.name = name + " (right anchor)";
        }

        var constraint = GetComponent<SpringJoint2D>();
        constraint.connectedBody = next.GetComponent<Rigidbody2D>();
        constraint.connectedAnchor = Vector2.zero;
    }

    private void Update()
    {

    }

    private void FixedUpdate()
    {
        if (DyingFreefall)
        {
            body.AddForce(gravity);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //if (!Initializer.balls.Contains(collision.gameObject))
        //    return;

        if (Time.time - lastCollisionTime < 1f)
            return;

        if (DyingFreefall)
            return;

        lastCollisionTime = Time.time;

        if (collision.gameObject.layer == 9)
        {
            Health -= .3f;
            Initializer.AddScore(300);
        }
        else if (collision.gameObject.layer == 10)
        {
            Health -= .15f;
            Initializer.AddScore(150);
        }

        if (health < 0f)
        {
            Debug.Log($"-- {name} ------------------------------------------------------");

            if (!NextBound)
            {
                next.GetComponent<BrickController>().Previous = Previous;
                Debug.Log($"-- {next.name}->Previous = {Next.GetComponent<BrickController>().previous.name}");
            }
            else
            {
                var controller = previous.GetComponent<BrickController>();
                if (controller != null)
                    controller.NextBound = true;
                else
                {
                    DyingFreefall = true;
                    Destroy(gameObject, 10);
                    foreach (var ball in Initializer.balls)
                    {
                        var ballController = ball.GetComponent<BallController>();
                        ballController.constantSpeed = 0;
                        ballController.jiggle = false;
                    }
                }
            }

            if (!PreviousBound)
            {
                previous.GetComponent<BrickController>().Next = Next;
                Debug.Log($"-- {previous.name}->Next = { previous.GetComponent<BrickController>().Next.name}");
            }
            else
            {
                if (!DyingFreefall) // scene finished
                    next.GetComponent<BrickController>().PreviousBound = true;
            }

            Debug.Log($"-- {name} ------------------------------------------------------");

            previous.GetComponent<SpringJoint2D>().connectedBody = next.GetComponent<Rigidbody2D>();
            next.GetComponent<SpringJoint2D>().connectedBody = previous.GetComponent<Rigidbody2D>();

            if (prefabAnchorBottom != null)
                Destroy(prefabAnchorBottom);

            if (prefabAnchorTop != null)
                Destroy(prefabAnchorTop);

            var spring = GetComponent<SpringJoint2D>();
            Destroy(spring);
            DyingFreefall = true;

            Destroy(gameObject, 10);
        }

    }
}
