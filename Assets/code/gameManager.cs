using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class gameManager : MonoBehaviour
{
    public static gameManager instance;
    public int size;
    public bool inMenu;
    public List<saveLevel> saveLevel;
    public saveLevel LevelNow;
    public CubeConA CubeConA;
    public List<Sprite> ArrowImg;
    public GameObject block;
    public Dictionary<Vector3Int, CubeConA> dataGrid = new Dictionary<Vector3Int, CubeConA>();
    public Dictionary<Vector3Int, GameObject> staticBlocks = new Dictionary<Vector3Int, GameObject>(); 
    public int blockCount = 0;
    public Transform cha;
   public SettingManager settingManager;
    public int yourUnlock=0;
    public int yourHP;
    int HPNow;
    public void Awake()
    {
        instance = this;
        if (PlayerPrefs.HasKey("yourUnlock"))
        {
            yourUnlock = PlayerPrefs.GetInt("yourUnlock");
            if (yourUnlock >= saveLevel.Count) yourUnlock = 0;
        }
        else
        {
            yourUnlock = 0;
        }
    }
    
    void Start()
    {
        inMenu = true;
        HPNow = yourHP;
    }
   
    public void loadLevel(int i)
    {
        HPNow = yourHP;
        if (i >= saveLevel.Count) i = 0;
        settingManager.Hint.gameObject.SetActive(true);
      settingManager.showArrow.gameObject.SetActive(true); 
        LevelNow = saveLevel[i];
        size = LevelNow.CubeSize;
        foreach (var c in dataGrid.Values) if (c != null) Destroy(c.gameObject);
        foreach (var b in staticBlocks.Values) if (b != null) Destroy(b.gameObject);
        foreach(Transform k in settingManager.spawnHp.gameObject.transform) { GameObject.Destroy(k.gameObject); }
        if (cha != null)
        {
            foreach (Transform child in cha)
            {
                Destroy(child.gameObject);
            }
            cha.position = Vector3.zero;
            cha.rotation = Quaternion.identity;
        }

        dataGrid.Clear();
        staticBlocks.Clear();
        CubeConA.ThanAllPos.Clear();
        blockCount = 0;
        for (int x = -(Mathf.FloorToInt(size / 2)); x<= (Mathf.FloorToInt(size / 2)); x++)
        {
            for (int y = -(Mathf.FloorToInt(size / 2)); y <= (Mathf.FloorToInt(size / 2)); y++)
            {
                for (int z = -(Mathf.FloorToInt( size/2)); z <= (Mathf.FloorToInt(size / 2)); z++)
                {
                    Vector3Int p3D = new Vector3Int(x, y, z);
                    Vector3 worldPos = new Vector3(x, y, z);

                    var staticCube = Instantiate(block, worldPos, Quaternion.identity,cha);
                    staticBlocks.Add(p3D, staticCube);
                }
            }
        }
        foreach (var data in LevelNow.CubeCon)
        {
            Vector3Int p3D = data.pos;
            Quaternion faceRot = Quaternion.identity;
            if (data.Face == Mat.mat1)
            {
                faceRot = Quaternion.LookRotation(Vector3.forward);
                p3D.z -= (Mathf.RoundToInt(size / 2) + 1); // (-Z)
            }
            else if (data.Face == Mat.mat2)
            {
                faceRot = Quaternion.LookRotation(Vector3.back);
                p3D.z += (Mathf.RoundToInt(size / 2) + 1); // (+Z)
            }
            else if (data.Face == Mat.mat3)
            {
                faceRot = Quaternion.LookRotation(Vector3.right);
                p3D.x -= (Mathf.RoundToInt(size / 2) + 1); // (-X)
            }
            else if (data.Face == Mat.mat4)
            {
                faceRot = Quaternion.LookRotation(Vector3.left);
                p3D.x += (Mathf.RoundToInt(size / 2) + 1); // (+X)
            }
            else if (data.Face == Mat.mat5)
            {
                faceRot = Quaternion.LookRotation(Vector3.down, Vector3.forward);
                p3D.y += (Mathf.RoundToInt(size / 2) + 1); //(+Y)
            }
            else if (data.Face == Mat.mat6)
            {
                faceRot = Quaternion.LookRotation(Vector3.up, Vector3.forward);
                p3D.y -= (Mathf.RoundToInt(size / 2)+1); //(-Y)
            }
            Vector3Int worldPos = new Vector3Int(p3D.x, p3D.y, p3D.z);
            if (!dataGrid.ContainsKey(p3D))
            {
                var dynamicCube = Instantiate(CubeConA, worldPos, faceRot, cha);
                
               
                List<Than> bodySteps = new List<Than>();
                foreach (var stepData in data.than)
                {
                    bodySteps.Add(stepData.than);
                }
                dynamicCube.InitCube(p3D, data.huong, data.Face);
                dynamicCube.UpdateBodyPath(bodySteps, data.Face, p3D, size);
                dataGrid.Add(p3D, dynamicCube);
                blockCount++; 
            }
        }

        if (cha != null && (dataGrid.Count > 0 || staticBlocks.Count > 0))
        {
            Vector3 centerOfGravity = Vector3.zero;
            int total = 0;

            foreach (var cube in dataGrid.Values)
            {
                if (cube != null)
                {
                    centerOfGravity += cube.transform.position;
                    total++;
                }
            }
            foreach (var cube in staticBlocks.Values)
            {
                if (cube != null)
                {
                    centerOfGravity += cube.transform.position;
                    total++;
                }
            }

            if (total > 0)
            {
                centerOfGravity /= total;

                List<Transform> children = new List<Transform>();
                foreach (Transform child in cha)
                {
                    children.Add(child);
                }
                
                cha.DetachChildren();
                cha.position = centerOfGravity;

                foreach (Transform child in children)
                {
                    child.SetParent(cha);
                }
            }
        }
        settingManager.getCHangePoint();
        for (int e1 = 0; e1 < HPNow; e1++) { var e2 = Instantiate(settingManager.hpCube, settingManager.spawnHp); }
    }
    private Vector3 previousMousePosition;
    public float rotationSpeed = 0.5f;
    public float zoomSpeed = 5f;
    public float minZoomZ = -18f; 
    public float maxZoomZ = -6f; 
    private bool isDragging = false;
    private CubeConA clickedCube = null;

    void Update()
    {
        if (inMenu) return;
        if (Input.GetMouseButtonDown(0))
        {
            previousMousePosition = Input.mousePosition;
            isDragging = false;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (!hit.collider.TryGetComponent<CubeConA>(out clickedCube))
                {
                    foreach (var cube in dataGrid.Values)
                    {
                        if (cube != null && cube.bodyParts.Any(p => p.Item1 == hit.collider.gameObject))
                        {
                            clickedCube = cube;
                            break;
                        }
                    }
                }
            }
            else
            {
                clickedCube = null;
            }
        }
        if (Input.GetMouseButton(0))
        {
            Vector3 delta = Input.mousePosition - previousMousePosition;
            if (delta.sqrMagnitude > 5f && Input.touchCount < 2)
            {
                isDragging = true;
                if (cha != null)
                {
                    cha.Rotate(Vector3.up, -delta.x * rotationSpeed, Space.World);
                    cha.Rotate(Camera.main.transform.right, delta.y * rotationSpeed, Space.World);
                }
            }
            previousMousePosition = Input.mousePosition;
        }
        if (Input.GetMouseButtonUp(0))
        {
            if (!isDragging && clickedCube != null && Input.touchCount < 2)
            {
                clickedCube.MoveArrow();
            }
            clickedCube = null;
        }
        var zoomDelta = 0f;
        var scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0.0f)
        {
            zoomDelta = scroll * zoomSpeed * 0.5f; 
        }
        if (Input.touchCount == 2)
        {
            var touchZero = Input.GetTouch(0);
            var touchOne = Input.GetTouch(1);

            var touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            var touchOnePrevPos = touchOne.position - touchOne.deltaPosition;
            var prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            var touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

            var deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

            zoomDelta = -deltaMagnitudeDiff * 0.005f * zoomSpeed;
        }
        if (zoomDelta != 0f && Camera.main != null)
        {
            var camPos = Camera.main.transform.position;
            camPos.z += zoomDelta * 5f;
            camPos.z = Mathf.Clamp(camPos.z, minZoomZ, maxZoomZ);
            Camera.main.transform.position = camPos;
        }
    }
    public void checkWin ()
    {
        blockCount--;
        settingManager.getCHangePoint();
        if (blockCount <= 0)
        {
            if (yourUnlock < saveLevel.Count) yourUnlock++;
            else yourUnlock = 0;
            PlayerPrefs.SetInt("yourUnlock", yourUnlock);
            PlayerPrefs.Save();
            foreach (Transform k in settingManager.spawnHp.gameObject.transform) { GameObject.Destroy(k); }
            settingManager.ShowWinAndBack();
            Debug.Log("gay");
        }
    }
    public void LostHP (int i)
    { 
        HPNow -= i; Destroy(settingManager.spawnHp.GetChild(0).gameObject); settingManager.audioS.PlayOneShot(settingManager.clip[3]);
        if (HPNow<=0) { HPNow = 0;settingManager.ShowLostAndBack(); }
    }    
    private void OnApplicationQuit()
    {
        PlayerPrefs.SetInt("yourUnlock", yourUnlock);
        PlayerPrefs.Save();
    }
    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            PlayerPrefs.SetInt("yourUnlock", yourUnlock);
            PlayerPrefs.Save();
        }
    }
}

