using UnityEngine;
using UnityEngine.UI;

public class FishingMinigame : MonoBehaviour
{
    [Header("Fish Movement")]
    public Transform topBound;      // จุดสูงสุดที่ปลาจะไปถึง
    public Transform bottomBound;   // จุดต่ำสุดที่ปลาจะไปถึง
    public Transform fish;          // Empty Object ที่ใช้ควบคุมปลา
    private float fishPosition;
    private float fishTarget;
    private float fishSpeed;
    public float fishMoveSpeed = 1f;  // ความเร็วของปลาที่จะเคลื่อนที่

    [Header("Hook Movement")]
    public Transform hook;          // Empty Object ที่ใช้ควบคุมเบ็ด
    private float hookPosition;
    private float hookVelocity;
    public float hookPullPower = 2f;
    public float hookGravity = 1.5f;

    [Header("Progress Bar")]
    public Transform progressBar;   // UI Progress Bar
    private float progress = 0f;
    public float progressGainSpeed = 0.5f;
    public float progressDecaySpeed = 0.2f;
    public float detectionRange = 0.15f;
    private Vector3 minScale = new Vector3(0, 1, 1);
    private Vector3 maxScale = new Vector3(1, 1, 1);

    [Header("Game State")]
    public GameObject fishingUI; // Canvas ของมินิเกม
    public float timeBonus = 10f;
    private bool isFishing = true;

    void Start()
    {
        // ตั้งค่าตำแหน่งเริ่มต้นของปลา
        fishPosition = Random.Range(0f, 1f);
        fishTarget = fishPosition;
    }

    void Update()
    {
        if (!isFishing) return; // ถ้าจบมินิเกมแล้ว ไม่ต้องอัปเดต

        FishMovement();
        HookMovement();
        ProgressBarUpdate();
    }

    void HookMovement()
    {
        if (Input.GetMouseButton(0))
        {
            hookVelocity += hookPullPower * Time.deltaTime;
        }

        hookVelocity -= hookGravity * Time.deltaTime;
        hookPosition += hookVelocity * Time.deltaTime;
        hookPosition = Mathf.Clamp(hookPosition, 0f, 1f);

        hook.position = Vector3.Lerp(bottomBound.position, topBound.position, hookPosition);
    }

    void FishMovement()
    {
        if (Mathf.Abs(fishPosition - fishTarget) < 0.01f)
        {
            fishTarget = Random.Range(0f, 1f);
        }

        fishPosition = Mathf.MoveTowards(fishPosition, fishTarget, fishMoveSpeed * Time.deltaTime);
        fish.position = Vector3.Lerp(bottomBound.position, topBound.position, fishPosition);
    }

    void ProgressBarUpdate()
    {
        // เช็คระยะห่างระหว่าง hook และ fish
        float distance = Mathf.Abs(hookPosition - fishPosition);

        if (distance < detectionRange)
        {
            progress += progressGainSpeed * Time.deltaTime;
        }
        else
        {
            progress -= progressDecaySpeed * Time.deltaTime;
        }

        progress = Mathf.Clamp01(progress);

        if (progressBar != null)
        {
            progressBar.localScale = Vector3.Lerp(minScale, maxScale, progress);
        }

        // เมื่อ Progress Bar เต็ม (progress >= 1)
        if (progress >= 1f && isFishing)
        {
            FinishFishing();  // เรียกฟังก์ชันเมื่อ Progress Bar เต็ม
        }
    }

    void FinishFishing()
    {
        // ตรวจสอบว่า progress bar เต็ม
        Debug.Log("Fishing Finished! Gaining Time.");

        // ส่งค่าตัวแปร RemainingTime เพิ่ม 10 วินาที
        CanvasText canvasTextScript = FindObjectOfType<CanvasText>();
        if (canvasTextScript != null)
        {
            canvasTextScript.RemainingTime += 10f;  // เพิ่มเวลา 10 วินาที
            Debug.Log("Fishing Success! Gained 10 seconds.");
        }

        // ปิด UI ของมินิเกม
        if (fishingUI != null)
        {
            fishingUI.SetActive(false);  // ปิด Canvas หรืออะไรก็แล้วแต่
        }

        // รีเซ็ตมินิเกม
        ResetMinigame();
    }

    void ResetMinigame()
    {
        // รีเซ็ตค่าทุกอย่างที่เกี่ยวกับมินิเกม
        fishPosition = Random.Range(0f, 1f);  // ตั้งค่าตำแหน่งปลาใหม่
        fishTarget = fishPosition;
        progress = 0f;  // รีเซ็ต Progress
        hookPosition = 0f;  // รีเซ็ตตำแหน่ง hook
        hookVelocity = 0f;  // รีเซ็ตความเร็ว hook
        progressBar.localScale = minScale;  // รีเซ็ตขนาดของ progress bar
        hook.position = Vector3.Lerp(bottomBound.position, topBound.position, hookPosition);  // รีเซ็ตตำแหน่ง hook
        isFishing = true;  // เริ่มต้นการมินิเกมใหม่
    }


    public void ResetProgress()
    {
        progress = 0f; // รีเซ็ตค่าความคืบหน้า
        if (progressBar != null)
        {
            progressBar.localScale = minScale; // ตั้งค่า Progress Bar ให้เริ่มต้นใหม่
        }
        isFishing = true; // เปิดให้เล่นมินิเกมใหม่
    }

}
