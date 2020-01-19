using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PadController : MonoBehaviour
{
    [SerializeField]
    protected Camera m_Camera;

    private Rigidbody2D body;
    private float worldXMax;
    private float worldXMin;
    private float localXMax;
    private float localXMin;

    private EdgeCollider2D edgeCollider;
    private LineRenderer lineRenderer;

    private Vector2[] m_points;

    public Vector2[] Points
    {
        get { return m_points; }
        set
        {
            m_points = value;

            localXMax = localXMin = m_points[0].x;

            for (int i = 1; i < m_points.Length; i++)
            {
                var currentPoint = m_points[i];

                if (currentPoint.x > localXMax)
                    localXMax = currentPoint.x;

                if (currentPoint.x < localXMin)
                    localXMin = currentPoint.x;
            }

            edgeCollider.points = m_points;
            lineRenderer.positionCount = m_points.Length;

            for (int i = 0; i < m_points.Length; i++)
            {
                lineRenderer.SetPosition(i, m_points[i]);
            }

            RecalculateBounds();
        }
    }

    // Start is called before the first frame update
    private void Start()
    {
        edgeCollider = GetComponent<EdgeCollider2D>();
        lineRenderer = GetComponent<LineRenderer>();

        List<Vector2> pts = new List<Vector2>();

        //pts.Add(Vector2.zero);

        float centerX = 0;
        float centerY = 0;
        float radiusX = 1.5f;
        float radiusY = 1f;

        for (var degree = 15; degree < 165; degree++)
        {
            var radians = degree * Mathf.PI / 180;
            var x = centerX + radiusX * Mathf.Cos(radians);
            var y = centerY + radiusY * Mathf.Sin(radians);
            pts.Add(new Vector2(x, y));
        }

        //pts.Add(Vector2.zero);

        Points = pts.ToArray();
        

        body = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        body.velocity = new Vector2(Input.GetAxis("Mouse X") * 10f, 0);

        if (body.position.x > worldXMax || body.position.x < worldXMin)
        {
            Vector2 position = new Vector2(Mathf.Clamp(body.position.x, worldXMin, worldXMax), body.position.y);
            body.MovePosition(position);
        }
    }

    public void RecalculateBounds()
    {
        var rect = GetComponent<RectTransform>().rect;
        var worldXmin = transform.TransformPoint(new Vector2(localXMin, 0)).x;
        var worldXmax = transform.TransformPoint(new Vector2(localXMax, 0)).x;
        var width = worldXmax - worldXmin;

        worldXMax = Initializer.RightBound - width / 2;
        worldXMin = Initializer.LeftBound + width / 2;
    }
}
