using UnityEngine;

public class Ball : MonoBehaviour
{
    private Transform m_Transform;
    private Collider2D m_Collider2D;

    private int m_PlayerId;

    private void Awake()
    {
        m_Transform = transform;
        m_Collider2D = GetComponent<Collider2D>();
    }

    public bool IsPickedUp { get; set; }

    public void PickUp(Transform aPlayer, int aId)
    {
        IsPickedUp = true;

        m_Transform.SetParent(aPlayer);
        m_Transform.localPosition = Vector3.zero;

        // While it is picked up, nobody can interact with the ball
        m_Collider2D.enabled = false;

        m_PlayerId = aId;
    }

    public void Drop()
    {
        IsPickedUp = false;
        m_Transform.SetParent(null);
        
        // Reenable ball interactivity
        m_Collider2D.enabled = true;
    }

    public void Reset()
    {
        Drop();

        transform.position = Vector3.zero;
    }

    public int PlayerId
    {
        get
        {
            return m_PlayerId;
        }
    }
}
