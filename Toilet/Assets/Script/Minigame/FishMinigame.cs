using UnityEngine;

public class FishMinigame : MonoBehaviour
{
    [SerializeField] Transform Top;
    [SerializeField] Transform Bottom;
    [SerializeField] Transform pooshit;

    float fishpo;
    float fishdes;
    float fishtime;
    [SerializeField] float FTime = 3f;
    float fishspeed;
    [SerializeField] float Fspeed = 1f;

    [SerializeField] Transform hook;
    float hookPo;
    float hookVeclocity;
    [SerializeField] float hookpullPower = 0.01f;
    [SerializeField] float hookGravity = 0.05f;

    [Header("Progress Bar")]
    [SerializeField] Transform progressBar; // ใช้ Transform แทน Image
    float progress = 0f;
    [SerializeField] float progressGainSpeed = 0.3f;
    [SerializeField] float progressDecaySpeed = 0.1f;
    [SerializeField] float detectionRange = 0.1f; // ระยะที่ Hook กับ ปลาอยู่ด้วยกัน
    [SerializeField] Vector3 minScale = new Vector3(0, 1, 1); // ขนาดต่ำสุด
    [SerializeField] Vector3 maxScale = new Vector3(1, 1, 1); // ขนาดสูงสุด
    [SerializeField] GameObject fishing;
    public CanvasText canvasTextScript;
    [SerializeField] GameObject canvasUI;
    [SerializeField] float TimePlust;


    private void Update()
    {
        FishMovement();
        HookMovement();
        ProgressBar();
    }

    void HookMovement()
    {
        if (Input.GetMouseButton(0))
        {
            hookVeclocity += hookpullPower;
        }
        hookVeclocity -= hookGravity * Time.deltaTime;
        hookPo += hookVeclocity;
        hookPo = Mathf.Clamp(hookPo, 0, 1);
        hook.position = Vector3.Lerp(Bottom.position, Top.position, hookPo);
    }

    void FishMovement()
    {
        fishtime -= Time.deltaTime;
        if (fishtime < 0)
        {
            fishtime = UnityEngine.Random.value * FTime;
            fishdes = UnityEngine.Random.value;
        }
        fishpo = Mathf.SmoothDamp(fishpo, fishdes, ref fishspeed, Fspeed);
        pooshit.position = Vector3.Lerp(Bottom.position, Top.position, fishpo);
    }

    void ProgressBar()
    {
        // เช็คระยะระหว่าง Hook กับ ปลา
        float distance = Mathf.Abs(hookPo - fishpo);

        if (distance < detectionRange)
        {
            progress += progressGainSpeed * Time.deltaTime;
        }
        else
        {
            progress -= progressDecaySpeed * Time.deltaTime;
        }

        progress = Mathf.Clamp01(progress);

        // ปรับขนาดของ Progress Bar
        if (progressBar != null)
        {
            progressBar.localScale = Vector3.Lerp(minScale, maxScale, progress);
        }

        // เมื่อเกจเต็ม (progress >= 1)
        if (progress >= 1f)
        {
            if (canvasUI != null)
            {
                canvasUI.SetActive(false);  // ปิด Canvas หรืออะไรก็แล้วแต่
            }

            // เพิ่มเวลา 10 วินาทีให้กับ RemainingTime ใน CanvasText
            CanvasText canvasTextScript = FindObjectOfType<CanvasText>();  // หาค่าจาก CanvasText
            if (canvasTextScript != null)
            {
                canvasTextScript.RemainingTime += TimePlust;  // เพิ่มเวลา 10 วินาที
                Debug.Log("Time increased by 10 seconds!");  // สำหรับดีบัก
            }

            ResetProgressBar();  // รีเซ็ต progress bar
        }
        void ResetProgressBar()
        {
            if (progressBar != null)
            {
                progress = 0f;  // รีเซ็ต progress เป็น 0
                progressBar.localScale = minScale;  // ตั้งขนาด progressBar ใหม่
            }
        }
    }




    public void ResetProgress()
    {
        progress = 0f; // รีเซ็ต Progress
    }
}
