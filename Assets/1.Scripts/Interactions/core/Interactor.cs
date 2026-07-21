using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;

public class Interactor : MonoBehaviour
{
  [SerializeField] private Transform _interactionPoint;
  [SerializeField] private float _interactionPointRadius = 0.5f;
  [SerializeField] private LayerMask _interactibleMask;


  private readonly Collider[] _colliders = new Collider[3];
    [SerializeField] private int _numFound;


    private void Update()
    {
       _numFound = Physics.OverlapSphereNonAlloc(_interactionPoint.position, _interactionPointRadius, _colliders,
           _interactibleMask);

        if (_numFound > 0)
        {
            ImInteractible interactible = null;

            for (int i = 0; i < _numFound; i++)
            {
                if (_colliders[i] == null) continue;

                interactible = _colliders[i].GetComponent<ImInteractible>();
                if (interactible != null)
                    break;
            }


            if (interactible != null && Keyboard.current.eKey.wasPressedThisFrame)
              {
                  interactible.Interactor(this);
            }
        }
    }
    private void OnDrawGizmos()
    {
       Gizmos.color = Color.purple;
       Gizmos.DrawWireSphere(_interactionPoint.position, _interactionPointRadius);
    }
}
