using UnityEngine;
using System.Collections.Generic;

public class WallTransparency : MonoBehaviour
{
    [Header("Setup")]
    public Transform player;
    public LayerMask wallLayer; // ใช้ Layer แทน Tag

    [Header("Transparency Settings")]
    [SerializeField] private float transparentAlpha = 0.3f;
    [SerializeField] private float normalAlpha = 1.0f;

    private Dictionary<Renderer, Material> originalMaterials = new Dictionary<Renderer, Material>();
    private HashSet<Renderer> transparentWalls = new HashSet<Renderer>();

    void Update()
    {
        CheckWallBetweenCameraAndPlayer();
    }

    void CheckWallBetweenCameraAndPlayer()
    {
        Vector3 direction = player.position - transform.position;
        float distance = direction.magnitude;

        RaycastHit[] hits = Physics.RaycastAll(transform.position, direction.normalized, distance, wallLayer);

        HashSet<Renderer> currentWalls = new HashSet<Renderer>();

        foreach (RaycastHit hit in hits)
        {
            Renderer wallRenderer = hit.collider.GetComponent<Renderer>();

            if (wallRenderer != null)
            {
                currentWalls.Add(wallRenderer);

                if (!transparentWalls.Contains(wallRenderer))
                {
                    SetTransparency(wallRenderer, transparentAlpha);
                    transparentWalls.Add(wallRenderer);
                }
            }
        }

        // กำแพงที่ไม่บังแล้วให้กลับมาเป็นปกติ
        List<Renderer> toReset = new List<Renderer>();
        foreach (var wall in transparentWalls)
        {
            if (!currentWalls.Contains(wall))
            {
                ResetTransparency(wall);
                toReset.Add(wall);
            }
        }

        foreach (Renderer r in toReset)
        {
            transparentWalls.Remove(r);
        }
    }

    void SetTransparency(Renderer renderer, float alpha)
    {
        if (renderer == null) return;

        if (!originalMaterials.ContainsKey(renderer))
        {
            originalMaterials[renderer] = renderer.material; // บันทึก Material เดิมไว้
            Material newMat = new Material(renderer.material); // Clone Material ใหม่
            renderer.material = newMat; // ใช้ Material ใหม่
        }

        Material mat = renderer.material;

        // ✨ บังคับให้ Shader อยู่ในโหมด Transparent ✨
        mat.SetOverrideTag("RenderType", "Transparent");
        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        mat.SetInt("_ZWrite", 0);
        mat.DisableKeyword("_ALPHATEST_ON");
        mat.EnableKeyword("_ALPHABLEND_ON");
        mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent; // ทำให้ Render เป็นโปร่งใส!

        // เปลี่ยนสี + Alpha
        Color color = mat.color;
        mat.color = new Color(color.r, color.g, color.b, alpha);

        Debug.Log($"Set {renderer.name} Alpha: {alpha}"); // Debug ดูว่าเปลี่ยนจริงไหม
    }


    void ResetTransparency(Renderer renderer)
    {
        if (renderer == null) return;

        if (originalMaterials.ContainsKey(renderer))
        {
            renderer.material = originalMaterials[renderer]; // คืนค่า Material เดิม
            originalMaterials.Remove(renderer);
        }
    }
}
