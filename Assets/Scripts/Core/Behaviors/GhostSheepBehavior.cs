using System;
using System.Linq;
using UnityEngine;

public class GhostSheepBehavior : AgentBehaviour
{
    private const float D_MAX = 6f;
    private const float D_MIN = 1f;
    private const float D_MIN_ROTATE = 3.5f;
    private const float D_MAX_ROTATE = 6f;
    private const string FLEEING_TAG = Config.TAG_SHEEP;
    private const string ATTACKING_TAG = Config.TAG_GHOST;

    private GameObject[] m_dogs;
    private int m_rotationDirection = 1;
    private float m_speedCoefficient = Config.SLIME_SPEED_DEFAULT;

    public void SetLowVelocity()
    {
        m_speedCoefficient = Config.SLIME_SPEED_SLOW;
    }
    
    public void SetNormalVelocity()
    {
        m_speedCoefficient = Config.SLIME_SPEED_DEFAULT;
    }
    
    public void SetHighVelocity()
    {
        m_speedCoefficient = Config.SLIME_SPEED_FAST;
    }

	public void SuddenDeathMode()
	{
		CancelInvoke();
		SwitchToAngry();
	}

    public String soundOnSwitchingToWolf = "wolf";
    public String soundOnSwitchingToSheep = "sheep";

    public void Start()
    {
        m_dogs = GameObject.FindGameObjectsWithTag(Config.TAG_DOG);

        float time = UnityEngine.Random.Range(10f, 25f);
        Invoke(nameof(SwitchRole), time);

        agent.SetVisualEffect(VisualEffect.VisualEffectConstAll, Color.green, 0);

        GameManager.Instance.TryRegisterSlime(this);
    }

    public override Steering GetSteering()
    {
        Steering steering = new Steering();

        if (GameManager.Instance.IsGamePaused)
            return steering;

        if (TryGetAxis(out float horizontal, out float vertical))
        {
            steering.linear = new Vector3(horizontal, 0, vertical) * agent.maxAccel * m_speedCoefficient;
            steering.linear = this
                .transform
                .parent
                .TransformDirection(Vector3.ClampMagnitude(steering.linear, agent.maxAccel));
        }

        return steering;
    }
    
    public void GoToStartPosition()
    {
        Debug.Log("Slime moving to start position");
        agent.SetGoalPose(
            Config.SLIME_STARTPOS_X, 
            Config.SLIME_STARTPOS_Y, 
            Config.SLIME_STARTPOS_THETA, 
            3, 
            3);
    }

    protected override void OnObstacleInFront(float distance, int layer)
    {
        Debug.Log($"Obstacle! Distance from obstacle : {distance} on layer {layer}");
        agent.LockMovement();
    }
    
    protected override void NoObstacleInFront()
    {
        agent.UnlockMovement();
    }

    public override void OnGoalPoseReached()
    {
        Debug.Log("Slime reached starting position");
    }
    
    public override void OnCelluloConnect()
    {
        agent.SetVisualEffect(VisualEffect.VisualEffectConstAll, Color.green, 0);
        agent.ClearTracking();
        GoToStartPosition();
    }

    public override void OnCelluloKidnapped()
    {
        GameManager.Instance.SlimeKidnapped();
    }

    void OnCollisionEnter(Collision collisionInfo)
    {
        GameObject collider = collisionInfo.collider.transform.parent.gameObject;
        switch (collider.tag)
        {
            case Config.TAG_BORDER:
                SwitchRotationDirection();
                break;
            case Config.TAG_DOG:
                if (!IsFleeing())
                {
                    GameManager.Instance.TryUpdateScoreOf(collider, Config.POINTS_FOR_PLAYER_CAUGHT_BY_GHOST);
                    AudioManager.Instance.PlayGlobalEffect("losePoint");
                    SwitchRole();
                }
                break;
        }
    }

	private void SwitchToNice()
	{
        gameObject.tag = FLEEING_TAG;
        agent.SetVisualEffect(VisualEffect.VisualEffectConstAll, Color.green, 0);
        AudioManager.Instance.PlaySoundEffect(soundOnSwitchingToSheep);
	}

	private void SwitchToAngry()
	{
        gameObject.tag = ATTACKING_TAG;
        agent.SetVisualEffect(VisualEffect.VisualEffectConstAll, Color.red, 0);
        AudioManager.Instance.PlaySoundEffect(soundOnSwitchingToWolf);
	}

    private void SwitchRole()
    {
        CancelInvoke();
        if (IsFleeing())
        {
            SwitchToAngry();
        }
        else
        {
			SwitchToNice();
        }

        float time = UnityEngine.Random.Range(Config.MIN_ROLE_TIME, Config.MAX_ROLE_TIME);
        Invoke(nameof(SwitchRole), time);
    }

    private void SwitchRotationDirection()
    {
        m_rotationDirection *= -1;
    }

    private bool IsFleeing()
    {
        return CompareTag(FLEEING_TAG);
    }

    private bool TryGetAxis(out float horizontal, out float vertical)
    {
        horizontal = 0f;
        vertical = 0f;

        float multiplier = 0f;
        float shortestDistance = float.PositiveInfinity;

        if (m_dogs.Length == 0)
            return false;

        Vector3 direction = Vector3.zero;
        Vector3 nearestDogDistanceVector = Vector3.zero;

        foreach (GameObject dog in m_dogs)
        {
            float mult = 0f;

            Vector3 distanceVector = transform.position - dog.transform.position;
            float d = distanceVector.magnitude;

            if (d < shortestDistance)
            {
                shortestDistance = d;
                nearestDogDistanceVector = distanceVector;
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