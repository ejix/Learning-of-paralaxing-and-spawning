using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxScript : MonoBehaviour {

	class poolObject
    {
        public Transform transform;
        public bool inUse;
        public poolObject(Transform t) { transform = t; }
        public void Use() { inUse = true; }
        public void Dispose() { inUse = false; }

    }

    [System.Serializable]
    public  struct YspawnRange
    {
        public float min;
        public float max;
    }
    public GameObject Prefab;
    public int poolSize;
    public float shiftSpeed;
    public float spawnRate;

    public YspawnRange ySpawnRange;
    public Vector3 defaultSpawnPos;
    public bool spawnImmediate;
    public Vector3 ImmediateSpawnPos;
    public Vector2 targetAspectRatio;

    float spawnTimer;
    float targetAspect;
    poolObject[] poolobjects;
    GameManager game;

    private void Awake()
    {
        Configure();
    }

    private void Start()
    {
        game = GameManager.Instance;
    }
    private void OnEnable()
    {
        GameManager.OnGameOverConfirmed += OnGameOverConfirmed;
    }
    private void OnDisable()
    {
        GameManager.OnGameOverConfirmed -= OnGameOverConfirmed;
    }
    void OnGameOverConfirmed()
    {
        for (int i = 0; i < poolobjects.Length; i++)
        {
            poolobjects[i].Dispose();
            poolobjects[i].transform.position = Vector3.one * 1000;
        }
        if (spawnImmediate) { SpawnImediate(); }
    }
    private void Update()
    {
        if (game.GameOver) return;

        Shift();
        spawnTimer += Time.deltaTime;
        if (spawnTimer > spawnRate)
        {
            Spawn();
            spawnTimer = 0;
        }
    }
    void Configure()
    {
        targetAspect = targetAspectRatio.x/targetAspectRatio.y;
        poolobjects = new poolObject[poolSize];
        for (int i = 0; i < poolobjects.Length; i++)
        {
            GameObject go = Instantiate(Prefab) as GameObject;
            Transform t = go.transform;
            t.SetParent(transform);
            t.position = Vector3.one * 1000;
            poolobjects[i] = new poolObject(t);
        }
        if (spawnImmediate) { SpawnImediate(); }
    }
    void Spawn()
    {
        Transform t = GetPoolObject();
        if (t == null) return; // if true, that indicates that poolSize is too small
        Vector3 pos = Vector3.zero;
        pos.x = defaultSpawnPos.x;
        pos.y = Random.Range(ySpawnRange.min, ySpawnRange.max);
        t.position = pos;
}
    void SpawnImediate()            //funkcja do przesuwania chmurek bez przerw
    {
        Transform t = GetPoolObject();
        if (t == null) return; // if true, that indicates that poolSize is too small
        Vector3 pos = Vector3.zero;
        pos.x = ImmediateSpawnPos.x;
        pos.y = Random.Range(ySpawnRange.min, ySpawnRange.max);
        t.position = pos;
        Spawn();
    }
    void Shift() {                                                                                                  //funkcja do przesuwania
        for (int i = 0; i < poolobjects.Length; i++) {
            poolobjects[i].transform.position+= -Vector3.right * shiftSpeed * Time.deltaTime;
            CheckDisposeObject(poolobjects[i]);
        }
    }
    void CheckDisposeObject(poolObject poolObject)
    {
        if (poolObject.transform.position.x < -defaultSpawnPos.x)
        {
            poolObject.Dispose();
            poolObject.transform.position = Vector3.one * 1000;
        }
    }
    Transform GetPoolObject() 
    {
        for (int i = 0; i < poolobjects.Length; i++)
        {
            if (!poolobjects[i].inUse)
            {
                poolobjects[i].Use();
                return poolobjects[i].transform; }
        }
        return null;
    }
  
}
