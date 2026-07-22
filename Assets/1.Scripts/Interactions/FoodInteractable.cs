using UnityEngine;

public class FoodInteractable : MonoBehaviour, ImInteractible
{
    [SerializeField] private int saturationAmount = 10;

    public string InteractionPrompt => "Essen";

    public bool Interactor(Interactor interactor)
    {
        PlayerEating playerEating =
            interactor.GetComponentInParent<PlayerEating>();

        if (playerEating == null)
        {
            Debug.LogWarning("PlayerEating wurde am Player nicht gefunden.");
            return false;
        }

        return playerEating.StartEating(
            saturationAmount,
            gameObject
        );
    }
}
