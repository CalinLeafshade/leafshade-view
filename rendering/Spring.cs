
namespace Leafshade;

public class Spring
{

    private readonly float mass = 1f;
    private readonly float k = 100f;
    private readonly float gravity = 9f;
    private readonly float damping = 10f;

    private float value;

    public float Value => value - RestPoint;

    public float Velocity { get; set; }

    private float RestPoint => mass * gravity / k;

    public Spring()
    {
        Reset();
    }

    public void Reset()
    {
        value = RestPoint;
        Velocity = 0f;
    }

    public void Update(double delta)
    {

        float springForce = -k * value;
        float dampingForce = damping * Velocity;
        float force = springForce + (mass * gravity) - dampingForce;
        float acceleration = force / mass;
        Velocity += acceleration * (float)delta;
        value += Velocity * (float)delta;

    }
}
