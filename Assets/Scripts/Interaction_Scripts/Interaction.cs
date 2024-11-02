using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interaction : MonoBehaviour
{
    [SerializeField] private Transform interaction_point;
    [SerializeField] private float interaction_point_radius = 0.5f;
    [SerializeField] private LayerMask interaction_mask;

    private readonly Collider[] _colliders = new Collider[3];
    [SerializeField] private int num_found;

    // Update is called once per frame
    void Update()
    {
        num_found = Physics.OverlapSphereNonAlloc(interaction_point.position, interaction_point_radius, _colliders, (int)interaction_mask);   

        if(num_found > 0 && Input.GetKeyDown(KeyCode.F))
        {
            var interactable = _colliders[0].GetComponent<IInteractable>();
            if(interactable != null )
            {
                interactable.Interact(interactor: this);
            }
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(interaction_point.position, interaction_point_radius);
    }
}
