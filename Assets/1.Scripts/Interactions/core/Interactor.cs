using UnityEngine;
using UnityEngine.InputSystem;

public class Interactor : MonoBehaviour
{
    [SerializeField] private Transform _interactionPoint;
    [SerializeField] private float _interactionPointRadius = 0.5f;
    [SerializeField] private LayerMask _interactibleMask;

    private readonly Collider[] _colliders = new Collider[3];

    [SerializeField] private int _numFound;

    private FoodInteractable _focusedFood;

    private void Update()
    {
        _numFound = Physics.OverlapSphereNonAlloc(
            _interactionPoint.position,
            _interactionPointRadius,
            _colliders,
            _interactibleMask
        );

        ImInteractible interactible = null;
        FoodInteractable detectedFood = null;

        if (_numFound > 0)
        {
            for (int i = 0; i < _numFound; i++)
            {
                if (_colliders[i] == null)
                    continue;

                interactible =
                    _colliders[i].GetComponent<ImInteractible>();

                if (interactible != null)
                {
                    detectedFood =
                        interactible as FoodInteractable;

                    break;
                }
            }
        }

        SetFocusedFood(detectedFood);

        if (interactible != null &&
            Keyboard.current != null &&
            Keyboard.current.eKey.wasPressedThisFrame)
        {
            interactible.Interactor(this);
        }
    }

    private void SetFocusedFood(FoodInteractable newFood)
    {
        if (_focusedFood == newFood)
            return;

        if (_focusedFood != null)
            _focusedFood.SetFocused(false);

        _focusedFood = newFood;

        if (_focusedFood != null)
            _focusedFood.SetFocused(true);
    }

    private void OnDisable()
    {
        SetFocusedFood(null);
    }

    private void OnDrawGizmos()
    {
        if (_interactionPoint == null)
            return;

        Gizmos.color = Color.purple;

        Gizmos.DrawWireSphere(
            _interactionPoint.position,
            _interactionPointRadius
        );
    }
}