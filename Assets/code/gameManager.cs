using System.Collections.Generic;
using UnityEngine;

public class gameManager : MonoBehaviour
{
    public static gameManager instance;
    public int size;
    public bool inMenu;
    public List<saveLevel> saveLevel;
    public saveLevel LevelNow;
    public CubeConA CubeConA;
    public GameObject Than;
    public List<Sprite> ArrowImg;
    public GameObject block;
    public Dictionary<Vector3Int, CubeConA> dataGrid = new Dictionary<Vector3Int, CubeConA>();
    public Dictionary<Vector3Int, GameObject> staticBlocks = new Dictionary<Vector3Int, GameObject>(); // Quản lý khối tĩnh
    public int blockCount = 0;
    public Transform cha;
   public SettingManager settingManager;
    public int yourUnlock=1;
    public void Awake()
    {
        instance = this;
        if (PlayerPrefs.HasKey("yourUnlock"))
        {
            yourUnlock = PlayerPrefs.GetInt("yourUnlock");
        }
        else
        {
            yourUnlock = 1;
        }
    }
    
    void Start()
    {
        inMenu = true;
    }
   
    public void loadLevel(int i)
    {
        settingManager.Hint.gameObject.SetActive(true);
      settingManager.showArrow.gameObject.SetActive(true); 
        LevelNow = saveLevel[i];
        size = LevelNow.CubeSize;
        foreach (var c in dataGrid.Values) if (c != null) Destroy(c.gameObject);
        foreach (var b in staticBlocks.Values) if (b != null) Destroy(b.gameObject);

        if (cha != null)
        {
            cha.position = Vector3.zero;
            cha.rotation = Quaternion.identity;
        }

        dataGrid.Clear();
        staticBlocks.Clear();
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
                var dynamicCube = Instantiate(CubeConA, worldPos, faceRot,cha);
                Vector3Int posThanPast = worldPos;
                foreach (var cube in data.than)
                {

                  var e1 =  CubeConA.getThan(cube.than);

                    var e2 = CubeConA.GetThanPos(CubeConA.CheckMat(data.Face, posThanPast, Mathf.FloorToInt(size / 2)).Item1, e1, CubeConA.CheckMat(data.Face, posThanPast, Mathf.FloorToInt(size / 2)).Item2);
                    
                    CubeConA.ThanAllPos.Add(e2);
                    var e = Instantiate(Than,e2,faceRot,cha);
                    
                    posThanPast = e2;
                }
                dynamicCube.InitCube(p3D, data.huong);
                
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
    }
    private Vector3 previousMousePosition;
    public float rotationSpeed = 0.5f;
    public float zoomSpeed = 5f;
    public float minZoom = 10f; 
    public float maxZoom = 100f; 
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
                hit.collider.TryGetComponent<CubeConA>(out clickedCube);
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

        if (Camera.main != null)
        {
            float zoomDelta = 0f;
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll != 0.0f)
            {
                zoomDelta = -scroll * zoomSpeed * 2f; 
            }
            if (Input.touchCount == 2)
            {
                Touch touchZero = Input.GetTouch(0);
                Touch touchOne = Input.GetTouch(1);

                Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
                Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

                float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
                float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

                float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

                zoomDelta = deltaMagnitudeDiff * 0.05f * zoomSpeed;
            }

            if (zoomDelta != 0f)
            {
                if (Camera.main.orthographic)
                {
                    Camera.main.orthographicSize += zoomDelta;
                    Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, minZoom, maxZoom);
                }
                else
                {
                    Camera.main.fieldOfView += zoomDelta * 5f; 
                    Camera.main.fieldOfView = Mathf.Clamp(Camera.main.fieldOfView, minZoom, maxZoom);
                }
            }
        }
    }
    
    public void checkWin ()
    {

        blockCount--;
        if (blockCount <= 0)
        {
            if (yourUnlock<saveLevel.Count) yourUnlock++;
            PlayerPrefs.SetInt("yourUnlock", yourUnlock);
            PlayerPrefs.Save();
            settingManager.ShowWinAndBack();
            Debug.Log("gay");
        }
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

