using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpringAttempt : MonoBehaviour
{
    public Transform StartPoint;
    public Transform EndPoint;
    public int Segments = 10;
    public float SegmentLength = 0.5f;
    public LineRenderer lineRenderer;
    private List<Vector3> points;
    public Rigidbody bobber;
    public float springConstant = 0.1f;


    void Start()
    {
        points = new List<Vector3>();
        float distance = Vector3.Distance(StartPoint.position, EndPoint.position);
        for (int i = 0; i < Segments; i++)
        {
            Vector3 point = Vector3.Lerp(StartPoint.position, EndPoint.position, i / (float)(Segments - 1));
            points.Add(point);
        }
        lineRenderer.positionCount = points.Count;
    }

    void FixedUpdate()
    {
        // Apply forces from the first segment to the last
        for (int i = 0; i < points.Count - 1; i++)
        {
            ApplySpringForce(i, i + 1, SegmentLength);
        }
        // Apply forces from the last segment to the first
        for (int i = points.Count - 2; i >= 0; i--)
        {
            ApplySpringForce(i, i + 1, SegmentLength);
        }

        points[0] = StartPoint.position;
        points[points.Count - 1] = EndPoint.position;
        lineRenderer.SetPositions(points.ToArray());
    }

void ApplySpringForce(int index1, int index2, float segLength)
{
    Vector3 diff = points[index2] - points[index1];
    float dist = diff.magnitude;
    float stretch = dist - segLength;
    Vector3 force = diff.normalized * (stretch * springConstant);
    force -= bobber.mass * Physics.gravity;
    points[index1] += force * Time.deltaTime;
    points[index2] -= force * Time.deltaTime;
}


}