using System;
using System.Linq;
using UnityEngine;

public class GhostSheepBehavior : AgentBehaviour
{
    private const float D_MAX = 12f;
    private const float D_MIN = 5f;
    private const float D_MIN_ROTATE = 10f;
    private const float D_MAX_ROTATE = 11.5f;
    private const float IGNORING_TRESHOLD = 0.5f;
	private const string FLEEING_TAG = Config.TAG_SHEEP;
	private const string ATTACKING_TAG = Config.TAG_GHOST;
    
    private GameObject[] m_dogs;
    private int m_rotationDirection = 1;
    private AudioSource m_audioSource;
    
    public void Start() {
        m_dogs = GameObject.FindGameObjectsWithTag(Config.TAG_DOG);
        m_audioSource = GetComponent<AudioSource>();

        float time = UnityEngine.Random.Range(5f, 20f);
        Invoke("SwitchRole", time);
    }
    
    public override Steering GetSteering()
    {
        Steering steering = new Steering();

        if (TryGetAxis(out float horizontal, out float vertical))
        {
            steering.linear = new Vector3(horizontal, 0, vertical) * agent.maxAccel;
            steering.linear = this
                .transform
                .parent
                .TransformDirection(Vector3.ClampMagnitude(steering.linear, agent.maxAccel));
        }

        return steering;
    }
    
    void OnCollisionEnter(Collision collisionInfo)
    {
        if (collisionInfo.collider.gameObject.tag == Config.TAG_BORDER)
        {
            SwitchRotationDirection();
        }
    }

    private void SwitchRole()
    {
        if (IsFleeing())
        {
            this.gameObject.tag = ATTACKING_TAG;
            this.agent.SetVisualEffect(VisualEffect.VisualEffectConstAll, Color.red, 0);
            m_audioSource.PlayOneShot(m_audioSource.clip);
        }
        else
        {
            this.gameObject.tag = FLEEING_TAG;
            this.agent.SetVisualEffect(VisualEffect.VisualEffectConstAll, Color.green, 0);
            m_audioSource.PlayOneShot(m_audioSource.clip);
        }
    }

    private void SwitchRotationDirection()
    {
        m_rotationDirection *= -1;
    }

    private bool IsFleeing()
    {
        return this.tag == FLEEING_TAG;
    }

    private bool TryGetAxis(out float horizontal, out float vertical)
    {
        horizontal = 0f;
        vertical = 0f;

        float multiplier = 0f;
        float shortestDistance = Config.UNITY_MAP_DIMENSION_X;

        if (m_dogs.Length == 0)
            return false;
        
        Vector3 direction = Vector3.zero;
        Vector3 nearestDogDistanceVector = Vector3.zero;

        foreach (GameObject dog in m_dogs)
        {
            float mult = 0f;

            Vector3 distanceVector = this.transform.position - dog.transform.position;
            float d = distanceVector.magnitude;
            
            if (d < shortestDistance)
            {
                shortestDistance = d;
                nearestDogDistanceVector = this.transform.position - dog.transform.position;
            }

            // the closer the dogs are, the more dangerous it is for the sheep
            if (d < D_MIN)
                mult = 1f;
            else if (d > D_MAX)
                mult = 0f;
            else
                mult = (D_MAX - d) / (D_MAX - D_MIN);

            direction += (mult * distanceVector);
        }

        if (IsFleeing())
            if (shortestDistance < D_MIN_ROTATE)
                multiplier = 0f;
            else if (shortestDistance > D_MAX_ROTATE)
                multiplier = 1f;
            else
                multiplier = (shortestDistance - D_MIN_ROTATE) / (D_MAX_ROTATE - D_MIN_ROTATE);
        else
            direction = nearestDogDistanceVector;

        Vector3 axis =
            (Vector3
                .RotateTowards(
                    direction,
                    new Vector3(m_rotationDirection * direction.z, 0f, -1 * m_rotationDirection * direction.x),
                    multiplier,
                    0f)
                .normalized
            ) * (IsFleeing() ? 1 : -1);

        horizontal = axis.x;
        vertical = axis.z;

        return true;
    }
}
