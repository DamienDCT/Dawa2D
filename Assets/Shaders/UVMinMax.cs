using System;
using UnityEngine;

/// <summary>
/// Writes sprite min and max uv points to materials automatically
/// </summary>
/// https://github.com/rob5300
[ExecuteAlways, DisallowMultipleComponent]
public class UVMinMax : MonoBehaviour
{
    [SerializeField]
    SpriteRenderer spriteRenderer;

    [SerializeField]
    string minMaxVec4Name = "_SpriteMinMax";

    public bool useSharedMaterial = false;

    private Material instancedMaterial;

#if UNITY_EDITOR
    //Tracks current renderer in use in editor only
    private SpriteRenderer activeSpriteRenderer;
#endif

    void Awake()
    {
        RegisterSpriteChangeCallback();
    }

    public void SetSpriteRenderer(SpriteRenderer newSpriteRenderer)
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.UnregisterSpriteChangeCallback(OnSpriteChanged);
        }

#if UNITY_EDITOR
        if (activeSpriteRenderer != null)
        {
            activeSpriteRenderer.UnregisterSpriteChangeCallback(OnSpriteChanged);
            activeSpriteRenderer = null;
        }
#endif

        if (spriteRenderer != null)
        {
            RegisterSpriteChangeCallback();
        }

        spriteRenderer = newSpriteRenderer;
    }

    private void RegisterSpriteChangeCallback()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.RegisterSpriteChangeCallback(OnSpriteChanged);

#if UNITY_EDITOR
            activeSpriteRenderer = spriteRenderer;
#endif

            if (spriteRenderer.sharedMaterial != null)
                UpdateMinMax();
        }
    }

    private void OnSpriteChanged(SpriteRenderer _renderer)
    {
        if (_renderer.sprite != null)
        {
            UpdateMinMax();
        }
    }

    [ContextMenu("Update Min Max Now")]
    public void UpdateMinMax()
    {
        if (spriteRenderer == null)
            throw new InvalidOperationException("Sprite renderer is required");

        if (spriteRenderer.sharedMaterial == null)
            throw new InvalidOperationException("Sprite renderer must have a material assigned");

        if (spriteRenderer.sprite == null)
        {
            return;
        }

        var material = GetMaterial();
        int id = Shader.PropertyToID(minMaxVec4Name);

        if (material.HasProperty(id))
        {
            Vector2[] uvs = spriteRenderer.sprite.uv;

            if (uvs.Length == 0) return;

            Vector2 min = uvs[0];
            Vector2 max = uvs[0];

            for (int i = 1; i < uvs.Length; i++)
            {
                if (uvs[i].x < min.x) min.x = uvs[i].x;
                if (uvs[i].y < min.y) min.y = uvs[i].y;
                if (uvs[i].x > max.x) max.x = uvs[i].x;
                if (uvs[i].y > max.y) max.y = uvs[i].y;
            }

            material.SetVector(id, new Vector4(min.x, min.y, max.x, max.y));
        }
    }

    void OnDestroy()
    {
        spriteRenderer.UnregisterSpriteChangeCallback(OnSpriteChanged);

        if (instancedMaterial != null)
        {
            Destroy(instancedMaterial);
            instancedMaterial = null;
        }
    }

    private Material GetMaterial()
    {
        if (Application.isPlaying && !useSharedMaterial)
        {
            if (instancedMaterial == null)
            {
                instancedMaterial = spriteRenderer.material;
            }

            return instancedMaterial;
        }
        else
        {
            return spriteRenderer.sharedMaterial;
        }
    }

    private void Reset()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (spriteRenderer != activeSpriteRenderer)
        {
            SetSpriteRenderer(spriteRenderer);
        }
    }
#endif
}