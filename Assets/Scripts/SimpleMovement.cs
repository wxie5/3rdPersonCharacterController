using UnityEngine;

public class SimpleMovement : MonoBehaviour
{
    #region Movement Vars
    [Header("Movement Setting")]
    [SerializeField] private float walkSpeed = 3.7f;                  //walk speed
    [SerializeField] private float runSpeed = 6f;                   //run speed
    [SerializeField] private float playerRotSmoothTime = 0.05f;     //the total time for smoothing the player rotation
    [SerializeField] private float playerMoveSmoothTime = 0.1f;
    private float playerRotSmoothRef;                               //a variable for storing the current velocity of player rotation
    private float playerMoveSmoothRef;
    private float curMoveSpeed;
    private float newMoveSpeed;
    private Rigidbody rigid;
    private bool isMoveable = true;
    #endregion

    #region Camera Rotation Vars
    [Header("Camera Setting")]
    public Vector2 mouseSensi = new Vector2(12f, 8f);               //the sensitivity of mouse (x for horizontal, y for vertical)
    [SerializeField] private Transform camFollowPoint;              //the center of camera rotation
    [Range(0.5f, 10f)]
    [SerializeField] private float defaultCamDis;                   //the default(farest) distance btw camera and player
    [SerializeField] private Vector2 yLimit = new Vector2(-40, 80); //the degree limitation of vertical rotation
    [SerializeField] private float camRotSmoothSpeed = 5f;         //the total time for smoothing the camera rotation
    private Quaternion camRot;                                         //the current camera rotation (euler angles)
    [SerializeField] private float camRotSmoothTime = 0.1f;
    private Vector3 camRotSmoothRef;
    [SerializeField] private float camPosSmoothSpeed = 5f;
    private Vector3 camPos;

    private Camera mainCam;                                         //the main camera
    private float mouseX;                                           //mouse horizontal movement     
    private float mouseY;                                           //mouse vertical movement
    private Vector2 axis;

    private bool isCamLocked;
    private bool isFocuingAttack;
    //serialized for testing only
    [SerializeField] private Transform camLockTarget;

    #endregion

    #region Camera Collision/Occulsion Vars
    [Header("Camera Collision Settings")]
    [Range(2f, 4f)]
    [SerializeField] private float occulFactor = 3f;
    [SerializeField] private float camMoveSmoothTime = 0.2f;
    private float curCamDis;                                        //the current distance between camera and player
    private Vector3[] camOriginalClipPoints;                        //the camera's default clip points (camera at defaultCamDis)
    private float camMoveSmoothRef;
    #endregion

    #region Animation Vars
    private Animator animator;                                      //player's animator
    #endregion

    #region LayerMask Vars
    [Header("LayerMask")]
    [SerializeField] private LayerMask groundLayer;                      //the ground layermask
    [SerializeField] private LayerMask cameraCollLayers;            //all layermasks that collide with camera
    #endregion

    #region Public Attributes
    public float CurMoveSpeed
    {
        get { return curMoveSpeed; }
    }

    public void IsMoveable(bool m)
    {
        if (m != isMoveable)
        {
            isMoveable = !isMoveable;
            curMoveSpeed = 0f;
        }
    }

    public bool IsCamLocked
    {
        get { return isCamLocked; }
    }

    #endregion

    void Start()
    {
        //initialize private variables
        rigid = GetComponent<Rigidbody>();
        newMoveSpeed = walkSpeed;

        mainCam = Camera.main;

        curCamDis = defaultCamDis;
        camOriginalClipPoints = new Vector3[5];

        animator = GetComponent<Animator>();

        //Lock Cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }


    void Update()
    {
        #region Movement
        //get input axis (WASD) and normalized
        float verti = Input.GetAxisRaw("Vertical");
        float hori = Input.GetAxisRaw("Horizontal");
        axis = new Vector2(hori, verti).normalized;

        if (isMoveable)
        {

            //if axis is not zero, rotate the character based on the current axis
            //(keyboard only can turn forward, back, left, right, and 45 degree diagnoal direction)
            if (axis != Vector2.zero)
            {
                if (isFocuingAttack)
                {
                    Vector3 focusDir = camLockTarget.position - transform.position;
                    focusDir = new Vector3(focusDir.x, 0f, focusDir.z).normalized;
                    transform.rotation = Quaternion.LookRotation(focusDir);
                }
                else
                {
                    float targetRot = Mathf.Atan2(axis.x, axis.y) * Mathf.Rad2Deg + mainCam.transform.eulerAngles.y;
                    transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRot, ref playerRotSmoothRef, playerRotSmoothTime);
                }
            }

            //calculate move speed
            InputCheck();
            //curMoveSpeed = Mathf.SmoothDamp(curMoveSpeed, newMoveSpeed * axis.magnitude, ref playerMoveSmoothRef, playerMoveSmoothTime);
            curMoveSpeed = newMoveSpeed * axis.magnitude;

            //calculate the forward direction and move the character in world space
            Vector3 dir = SlopeAdjustedDirection(transform.forward);

            transform.Translate(dir * Time.deltaTime * curMoveSpeed, Space.World);
            //rigid.velocity = new Vector3(dir.x, 0f, dir.z).normalized * curMoveSpeed + new Vector3(0f, rigid.velocity.y, 0f);
        }
        #endregion
    }

    private void FixedUpdate()
    {
        #region Camera Occulsion/Collision Check
        //get the direction from camera to target point
        Vector3 camFollowDir = (mainCam.transform.position - camFollowPoint.position).normalized;
        //calculate the default camera position
        //(since currently camera may collide with some objects, we must have the original position of camera to correctly check collision)
        Vector3 originalCamPos = camFollowDir * defaultCamDis + camFollowPoint.position;
        //calculate clip points on near plane
        CalculateClipPoints(originalCamPos, mainCam.transform.rotation, ref camOriginalClipPoints);
        #endregion
    }

    private void LateUpdate()
    {
        #region Camera Rotation
        if (!isCamLocked)
        {
            //increse mouseX and mouseY, apply the Y limitation
            mouseX += Input.GetAxis("Mouse X") * mouseSensi.x;
            mouseY -= Input.GetAxis("Mouse Y") * mouseSensi.y;
            mouseY = Mathf.Clamp(mouseY, yLimit.x, yLimit.y);

            //smoothly rotate the camera based on the mouse movement
            camRot = Quaternion.Lerp(camRot, Quaternion.Euler(mouseY, mouseX, 0f), camRotSmoothSpeed * Time.deltaTime);
            camPos = Vector3.Lerp(camPos, camFollowPoint.position - mainCam.transform.forward * curCamDis, camPosSmoothSpeed * Time.deltaTime);
        }
        else
        {
            Vector3 lockCamCenter = camFollowPoint.position + Vector3.up * 0.2f;
            Vector3 targetToCenterDir = (lockCamCenter - camLockTarget.position).normalized;
            camRot = Quaternion.Lerp(camRot, Quaternion.LookRotation(-targetToCenterDir), camRotSmoothSpeed * Time.deltaTime);
            camPos = Vector3.Lerp(camPos, lockCamCenter + targetToCenterDir * curCamDis, camPosSmoothSpeed * Time.deltaTime);
        }

        mainCam.transform.rotation = camRot;
        mainCam.transform.position = camPos;
        #endregion

        #region Camera Collision/Occulsion
        //check camera collision
        float minDis = -1f;
        Vector3 maxDisClipPoint = Vector3.zero;
        foreach (Vector3 clipPoint in camOriginalClipPoints)
        {
            Ray ray = new Ray(camFollowPoint.position, (clipPoint - camFollowPoint.position).normalized);
            RaycastHit hit;
            //if hit, find the distance from hit point to target(camFollowPoint)
            if (Physics.Raycast(ray, out hit, defaultCamDis, cameraCollLayers))
            {
                float clipPointDis = Vector3.Distance(camFollowPoint.position, hit.point);
                //find the min distance 
                if (minDis < 0f || clipPointDis < minDis)
                {
                    minDis = clipPointDis;
                    maxDisClipPoint = clipPoint;
                }
            }
        }

        //based on min distance, update camera's distance to player
        float newCamDis;
        if (minDis >= 0)
        {
            float disRatio = minDis / Vector3.Distance(camFollowPoint.position, maxDisClipPoint);
            newCamDis = disRatio * defaultCamDis;
        }
        else
        {
            //if minDis is still -1, no camera collision, then set back to default camera distance
            newCamDis = defaultCamDis;
        }

        curCamDis = Mathf.SmoothDamp(curCamDis, newCamDis, ref camMoveSmoothRef, camMoveSmoothTime);

        #endregion
    }

    /// <summary>
    /// Adjust the forward direction of player when on slopes
    /// </summary>
    /// <param name="dir">character's forward direction</param>
    /// <returns></returns>
    private Vector3 SlopeAdjustedDirection(Vector3 dir)
    {
        Ray ray = new Ray(transform.position, Vector3.down);
        RaycastHit hit;
        //using normal information to adjust the forward direction
        if (Physics.Raycast(ray, out hit, 0.3f, groundLayer))
        {
            Quaternion slope = Quaternion.FromToRotation(Vector3.up, hit.normal);
            Vector3 adjustedDir = slope * dir;

            if (adjustedDir.y < 0)
            {
                return adjustedDir;
            }
        }

        return dir;
    }

    /// <summary>
    /// Calculate the near plane clip points of camera
    /// </summary>
    /// <param name="cameraPos">camera position</param>
    /// <param name="cameraRot">camera rotation</param>
    /// <param name="clipPoints">a reference of clip points</param>
    private void CalculateClipPoints(Vector3 cameraPos, Quaternion cameraRot, ref Vector3[] clipPoints)
    {
        //calculate near plane x y z of perspective camera
        //perspZ is the depth from camera to the near plane
        //perspX is half of the near plane's length
        //perspY is half of the near plane's width
        float perspZ = mainCam.nearClipPlane;
        //the correct formula should have field of view / 2f
        //however that will detect less occulsion and cause some bad effects
        //value around 3f to 4f is better for this case
        float perspX = Mathf.Tan(mainCam.fieldOfView / occulFactor) * perspZ;
        float perspY = perspX / mainCam.aspect;

        //calculate 4 clip points and the middle point
        clipPoints[0] = cameraRot * new Vector3(perspX, perspY, perspZ) + cameraPos;
        clipPoints[1] = cameraRot * new Vector3(-perspX, perspY, perspZ) + cameraPos;
        clipPoints[2] = cameraRot * new Vector3(perspX, -perspY, perspZ) + cameraPos;
        clipPoints[3] = cameraRot * new Vector3(-perspX, -perspY, perspZ) + cameraPos;
        clipPoints[4] = cameraRot * new Vector3(0f, 0f, perspZ) + cameraPos;
    }

    private void InputCheck()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            newMoveSpeed = runSpeed;
        }

        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            newMoveSpeed = walkSpeed;
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (isCamLocked)
            {
                mouseY = camRot.eulerAngles.x;
                mouseX = camRot.eulerAngles.y;
            }
            isCamLocked = !isCamLocked;
        }
    }

    public void FocusOnTarget()
    {
        if (camLockTarget == null)
        {
            return;
        }

        isFocuingAttack = true;
    }

    public void UnFocusTarget()
    {
        isFocuingAttack = false;
    }

    public void TurnDirection()
    {
        float axisX = axis.x;
        float axisY = axis.y;
        if (Mathf.Abs(axisX) > Mathf.Abs(axisY))
        {
            axisY = 0f;
        }
        else
        {
            axisX = 0f;
        }

        float targetRot = Mathf.Atan2(axisX, axisY) * Mathf.Rad2Deg + mainCam.transform.eulerAngles.y;
        transform.eulerAngles = Vector3.up * targetRot;
    }
}
