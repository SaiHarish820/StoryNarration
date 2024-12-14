using UnityEngine;

public class UniformLevelCardScaler : MonoBehaviour
{
    public RectTransform levelCardBoundary; // Parent boundary RectTransform
    public RectTransform levelCard; // Level card RectTransform
    public float scaleFactor = 0.9f; // Scale factor (90% of the smaller dimension)

    void Start()
    {
        ScaleLevelCardUniformly();
    }

    void ScaleLevelCardUniformly()
    {
        if (levelCardBoundary == null || levelCard == null)
        {
            Debug.LogError("LevelCardBoundary or LevelCard is not assigned.");
            return;
        }

        // Get the size of the boundary
        Vector2 boundarySize = levelCardBoundary.rect.size;

        // Determine the smaller dimension of the boundary
        float smallerDimension = Mathf.Min(boundarySize.x, boundarySize.y);

        // Calculate the uniform scale size
        float uniformScale = smallerDimension * scaleFactor;

        // Apply the calculated size to the level card
        levelCard.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, uniformScale);
        levelCard.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, uniformScale);
    }

    void Update()
    {
        // Optional: Update scale in real-time (e.g., for dynamic resizing)
        ScaleLevelCardUniformly();
    }
}
