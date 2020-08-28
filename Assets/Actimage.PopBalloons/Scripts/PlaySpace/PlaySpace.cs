using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.SpatialAwareness;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using Random = UnityEngine.Random;
using UnityEngine.SceneManagement;
using PopBalloons.Utilities;

namespace PopBalloons.Boundaries
{

    /// <summary>
    /// Creation the beginning scene with the help of the landmarks 
    /// </summary>
    public class PlaySpace : MonoBehaviour
    {
        #region Variables

        /// <summary>
        /// Singleton instance
        /// </summary>
        private static PlaySpace instance;

        [Header("Visual effects :")]      
        [SerializeField]
        private Material areaMaterial;


        [Header("Mode specific: ")]
        /// <summary>
        /// Cognitive Area
        /// </summary>
        [SerializeField]
        private GameObject cognitive;

        /// <summary>
        /// Motricity Area
        /// </summary>
        [SerializeField]
        private GameObject motricity;

        /// <summary>
        /// Items that are always required
        /// </summary>
        [SerializeField]
        [Tooltip("Parent object of all required item inside playspace (CanvasScoreBoard, Landmarks, etc...)")]
        private GameObject Holograms;

        [Header("Settings :")]
        /// <summary>
        /// Floor used in editor in order
        /// </summary>
        [SerializeField]
        private GameObject FakeFloor;
        /// <summary>
        /// Warns user about space requirements
        /// </summary>
        [SerializeField]
        private GameObject infoPanel;



        /// <summary>
        ///  Allow to create a shape from the point set by the user.
        /// </summary>
        private Triangulator tr;

        /// <summary>
        /// Each vertices
        /// </summary>
        private int[] indices;

        /// <summary>
        /// Liste des triangles formant la zone de spawn dessinée par le joueur.
        /// </summary>
        private List<TriangleSpawn> spawnEmplacement;

        /// <summary>
        /// Liste des segments formant la zone de jeu
        /// </summary>
        private List<AreaSegment> segments;

        /// <summary>
        /// Center object, remains previous iteration. Now equals Cognitive sphere.
        /// </summary>
        private GameObject center;

        /// <summary>
        /// Float that store current floor height for the session.
        /// </summary>
        private float? floorHeight;

        public float? FloorHeight { get => floorHeight; }

        //Accessors
        public static PlaySpace Instance { get => instance; }
        public List<AreaSegment> Segments { get => segments;}
        #endregion


        #region Unity-Functions
        private void Start()
        {
            //SceneManager.MoveGameObjectToScene(this.transform.parent.gameObject, SceneManager.GetSceneByName("Setup"));

            //TODO : Create infoPanel
            //infoPanel.SetActive(true);

            StartCoroutine(FloorDetection());

            GameManager.OnGameStateChanged += HandleGameStateChanged;
           
        }

        public AreaSegment GetFacingSegment()
        {
            //TODO: Check if user in area
            AreaSegment closestSegment = null;
            Transform cam = Camera.main.transform;
            float currentDot = -1;
            foreach(AreaSegment segment in segments)
            {
                var dot = Vector3.Dot(cam.transform.forward,segment.Center - cam.position);
                if(dot > currentDot)
                {
                    currentDot = dot;
                    closestSegment = segment;
                }
            }

            return closestSegment;
        }

        private void HandleGameStateChanged(GameManager.GameState newState)
        {
            switch (newState)
            {
                case GameManager.GameState.INIT:
                case GameManager.GameState.SETUP:
                case GameManager.GameState.HOME:
                    motricity.SetActive(true);
                    cognitive.SetActive(false);
                    break;
                case GameManager.GameState.PLAY:
                    motricity.SetActive(GameManager.Instance.CurrentGameType == GameManager.GameType.CLASSIC || GameManager.Instance.CurrentGameType == GameManager.GameType.FREEPLAY);
                    cognitive.SetActive(GameManager.Instance.CurrentGameType == GameManager.GameType.COGNITIVE);
                    break;
            }
        }

        private void Awake()
        {
            if(Instance != null)
            {
                DestroyImmediate(this);
            }
            else
            {
                instance = this;
                segments = new List<AreaSegment>(this.GetComponentsInChildren<AreaSegment>(true));
                DontDestroyOnLoad(gameObject);
            }
        }

        private void OnDestroy()
        {
            GameManager.OnGameStateChanged -= HandleGameStateChanged;
        }
        #endregion


        #region Floor Detection

        public float? TryGetFloorHeight()
        {
            
            Transform player = Camera.main.transform;
            Vector3 pos = player.position;

            List<Vector3> checkers = new List<Vector3>();
            Vector3 v1 = (player.forward + 0.25f * player.right).normalized ;
            Vector3 v2 = (player.forward - 0.25f * player.right).normalized ;
            Vector3 v3 = (player.forward - 0.25f * player.up).normalized ;
            Vector3 v4 = (player.forward);

            checkers.Add(v1);
            checkers.Add(v2);
            checkers.Add(v3);
            checkers.Add(v4);

            float minY = Mathf.Infinity;
            float maxY = -Mathf.Infinity;
            int missingValue = 0;
            foreach(Vector3 v in checkers)
            {
                //Debug.Log("Layer : " + LayerMask.NameToLayer("Spatial Awareness"));
                RaycastHit hit;
                Debug.DrawRay(pos, v*10f, Color.cyan);
                if (Physics.Raycast(pos, v, out hit, 10f, 1<<31))
                {
                    if(Vector3.Dot(hit.normal,Vector3.up) < 0.9f)
                    {
                        missingValue++;
                        continue;
                    }



                    minY = Mathf.Min(hit.point.y,minY);
                    maxY = Mathf.Max(hit.point.y, maxY);
                    //Debug.Log("Floor Hit");
                }
                else
                {
                    missingValue++;
                    //Debug.Log("Ray did not hit !");
                }
            }

            if(missingValue >= 2 || Mathf.Abs(maxY-minY) >= 0.1f)
            {
                return null;
            }
            //Debug.Log("Floor found at :" + ((minY + maxY) / 2.0f));
            return ((minY + maxY) / 2.0f);

           
        } 


        #endregion


        #region Shape Creation Functions


        /// <summary>
        /// Information concernant les triangles composant le mesh de l'aire de jeu.
        /// </summary>
        private class TriangleSpawn
        {
            public Vector3 A, B, C;
            public float weight;
            public float Area
            {
                get
                {
                    if (A != null && B != null && C != null)
                        return 1 / 2 * Mathf.Abs((A.x - C.x) * (B.z - A.z) - (A.x - B.x) * (C.z - A.z));
                    return 0;
                }
            }

            public TriangleSpawn(Vector3 a, Vector3 b, Vector3 c)
            {
                A = a;
                B = b;
                C = c;
            }

            public Vector3 GetRandomPoint()
            {
                if (A != null && B != null && C != null)
                {
                    float Px, Pz;
                    float r1, r2;
                    r1 = Random.value;
                    r2 = Random.value;

                    Px = ((1 - Mathf.Sqrt(r1)) * A.x) + (Mathf.Sqrt(r1) * (1 - r2) * B.x) + (Mathf.Sqrt(r1) * r2) * C.x;
                    Pz = ((1 - Mathf.Sqrt(r1)) * A.z) + (Mathf.Sqrt(r1) * (1 - r2) * B.z) + (Mathf.Sqrt(r1) * r2) * C.z;
                    return new Vector3(Px, 0, Pz);
                }
                return Vector2.zero;
            }

            //TODO convertir en vector3
            public Vector2 Center()
            {
                if (A != null && B != null && C != null)
                {
                    float cx = (A.x + B.x + C.x) / 3;
                    float cy = (A.y + B.y + C.y) / 3;

                    return new Vector2(cx, cy);
                }
                return Vector2.zero;
            }


            public float DistanceFrom(Vector2 v)
            {
                return Vector2.Distance(v, Center());
            }
        }

        public GameObject GetCenter()
        {
            if(this.center == null)
            {
                this.center = new GameObject("PlaySpace-Center");
                this.center.transform.SetParent(this.transform);
                this.center.transform.position = this.GetAreaCenter();
            }
            return this.center;
        }

        public Vector2[] GetLandmarksPoint()
        {
            Vector2[] landmarksPointArray = new Vector2[Segments.Count];
            Vector2 landmarkPoint = new Vector2();
            int occ = 0;

            foreach (AreaSegment segment in Segments)
            {
                landmarkPoint.x = segment.V1.transform.position.x;
                landmarkPoint.y = segment.V1.transform.position.z;
                landmarksPointArray[occ] = landmarkPoint;
                occ++;
            }

            return landmarksPointArray;
        }

        public Vector3[] GetLandmarksPoint3D()
        {
            Vector3[] landmarksPointArray = new Vector3[Segments.Count];
            Vector3 landmarkPoint = new Vector3();
            int occ = 0;

            foreach (AreaSegment segment in Segments)
            {
                landmarkPoint.x = segment.V1.transform.position.x;
                landmarkPoint.y = segment.V1.transform.position.y;
                landmarkPoint.z = segment.V1.transform.position.z;
                landmarksPointArray[occ] = landmarkPoint;
                occ++;
            }

            return landmarksPointArray;
        }

        /// <summary>
        /// Initialise un triangulator à partir du polygone dessiné par l'utilisateur.
        /// </summary>
        public void Init2DMesh()
        {
            
            float sumArea = 0;
                

            gameObject.AddComponent<MeshFilter>();
            Renderer r = gameObject.AddComponent<MeshRenderer>();
            Mesh mesh = GetComponent<MeshFilter>().mesh;

            mesh.Clear();
            r.material = this.areaMaterial;
            //We hide the floor
            r.enabled = false;
            spawnEmplacement = new List<TriangleSpawn>();
            tr = new Triangulator(this.GetLandmarksPoint());
            indices = tr.Triangulate();
            // Create the Vector3 vertices
            Vector3[] vertices = GetLandmarksPoint3D();// new Vector3[this.GetLandmarksPoint().Length];
            //for (int i = 0; i < vertices.Length; i++)
            //{
            //    vertices[i] = new Vector3(this.GetLandmarksPoint()[i].x, 0, this.GetLandmarksPoint()[i].y);
            //}

            mesh.vertices = vertices;
            //mesh.uv = new Vector2[] { new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 1) };
                


            Vector3 pos;
            GameObject obj;
            TriangleSpawn ts;
            // Initialisation de la liste de triangle
            List<int> triangles = new List<int>();
            for (int i = 0; i < indices.Length; i += 3)
            {

                ts = new TriangleSpawn(vertices[indices[i]], vertices[indices[i + 1]], vertices[indices[i + 2]]);

                triangles.Add(indices[i]);
                triangles.Add(indices[i + 1]);
                triangles.Add(indices[i + 2]);

                sumArea += ts.Area;
                spawnEmplacement.Add(ts);
            }

            mesh.triangles = triangles.ToArray();
                
            //Uncomment in order to test spawning
            //foreach (TriangleSpawn t in spawnEmplacement)
            //{
            //    if (sumArea != 0)
            //        t.weight = t.Area / sumArea;

            //    for (int i = 0; i < 100; i++)
            //    {
            //        pos = t.GetRandomPoint();
            //        obj = Instantiate(Instance.landmarkLimitPrefab, pos, Instance.landmarkLimitPrefab.transform.rotation,GetCenter().transform);
            //        obj.transform.localPosition = Vector3.Scale(obj.transform.localPosition, new Vector3(1f, 0, 1f));
            //        //.obj.transform.parent = Instance.gameObject.transform;
            //    }

            //}
            

        }


        /// <summary>
        /// Will place the playSpace matching FloorHeight, if any 
        /// </summary>
        private void DetectFloor()
        {
            Vector3 position = Camera.main.transform.position;
            
            if (floorHeight.HasValue)//(MixedRealityToolkit.BoundarySystem.FloorHeight.HasValue)
            {
                position.y = floorHeight.Value;// MixedRealityToolkit.BoundarySystem.FloorHeight.Value;
                //Debug.Log("Floor height is :"+ position.y);
            }
            else
            {
#if UNITY_EDITOR
                position.y = FakeFloor.transform.position.y;
#else
                position.y = -1.7f;
#endif
                //Debug.Log("Floor not found");
            }
            this.InitPlaySpace();
            this.transform.position = position;
            areaMaterial.SetFloat("_FloorHeight", position.y);

            this.transform.LookAt(position + Vector3.Scale(new Vector3(1f, 0f, 1f), Camera.main.transform.forward));

        }

        /// <summary>
        /// Return area center, by default return user position
        /// </summary>
        /// <returns></returns>
        private Vector3 GetAreaCenter()
        {
            if(Segments == default || Segments.Count == 0)
            {
                return Camera.main.transform.position;
            }

            Vector3 pos = Vector3.zero;
            foreach(AreaSegment segment in Segments)
            {
                pos += segment.V1.transform.position;
            }
            return pos / Segments.Count;
        }

        #endregion

        #region Functions
        public static Vector3 GetRandomPointInArea()
        {
            float value = Random.value;
            float sumWeight = 0;
            Vector3 pos;
            foreach (TriangleSpawn t in Instance.spawnEmplacement)
            {
                sumWeight += t.weight;
                if (value <= Mathf.Min(sumWeight, 1f))
                {
                    pos = t.GetRandomPoint();
                    return pos;//SharingManager.getSharedCollection().transform.InverseTransformPoint(new Vector3(pos.x, 0, pos.y));
                }
            }
            pos = Instance.spawnEmplacement[Random.Range(0, Instance.spawnEmplacement.Count)].GetRandomPoint();
            return Instance.transform.TransformPoint(pos);

        }

        private void InitPlaySpace()
        {
            Init2DMesh();
            //We enable all time objects & items
            Holograms?.SetActive(true);
            //Motricity is the default area.
            motricity?.SetActive(true);
            cognitive.transform.position = this.GetAreaCenter();
            this.Validate();
        }


        public void Validate()
        {
            GameManager.Instance.SetupCompleted();
        }
        #endregion

        #region Coroutines
        private IEnumerator FloorDetection()
        {
            //Debug.Log("Begin floor detection");
            infoPanel.SetActive(true);
            float waitTime = 5f;
            float time = Time.time;
            //Not handled yet by Microsoft
            MixedRealityToolkit.BoundarySystem.Initialize();
            MixedRealityToolkit.BoundarySystem.Enable();
            floorHeight = MixedRealityToolkit.BoundarySystem.FloorHeight;
            
            while (!floorHeight.HasValue) //!MixedRealityToolkit.BoundarySystem.FloorHeight.HasValue)
            {
                //Display floor detection info & loader
                floorHeight = this.TryGetFloorHeight();

#if UNITY_EDITOR
                //Boundaries does not work in editor, no need to wait infinitly
                //break;
#endif
                yield return null;
            }
            time = Time.time - time;
            //Debug.Log("Floor detected in " + time + "seconds");
            //We wait a few seconds in order to prevent
            yield return new WaitForSeconds(Mathf.Max(0,waitTime - time));
            infoPanel.SetActive(false);
            DetectFloor();
        }

        #endregion
    }
}


