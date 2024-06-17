using UnityEngine;

public class WallDeathTag : MonoBehaviour
{
    [SerializeField] private BoxCollider2D parentBoxCollider;
    public BoxCollider2D ParentBoxCollider => parentBoxCollider;
}
