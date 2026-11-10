using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<T> where T : Component
{
    /// <summary>
    /// Returns the number of effective (meaning non null) elements in the pool, regardless of whether they're currently active.
    /// </summary>
    public int EffectivePoolSize
    {
        get
        {
            var num = 0;
            if (pool == null || pool.Count == 0) return 0;
            foreach (var item in pool)
            {
                if (item != null) num++;
            }
            return num;
        }
    }

    [SerializeField] private T prefab;
    [SerializeField] private int initialSize;

    private readonly Queue<T> pool = new();
    public Queue<T> Pool => pool;

    private readonly List<T> allObjects = new();
    public List<T> AllObjects => allObjects;

    public delegate void InitializePoolObject(T obj);
    /// <summary>
    /// Non-required callback that will be invoked whenever a new pool object has been spawned.
    /// This can be useful if the objects need to be initialized before being used, regardless of whether they're spawned on Start or throughout runtime.
    /// </summary>
    public InitializePoolObject initializePoolObject;

    /// <summary>
    /// This constructor takes the given prefab and spawns a pool of them, rather than creating a 'prefab' game object at runtime with only the component of type T.
    /// </summary>
    public ObjectPool(T _prefab, int _initialSize, InitializePoolObject _initializePoolObject = null)
    {
        Initialize(_prefab, _initialSize, _initializePoolObject);
    }

    /// <summary>
    /// While not recommended in most cases, this constructor will create new game objects with the component of type T instead of using an actual prefab asset.
    /// </summary>
    public ObjectPool(int _initialSize, InitializePoolObject _initializePoolObject = null)
    {
        // Create new disabled game object that has the component T added to it, then use it as the prefab.
        var _prefab = new GameObject("Object Pool Prefab").AddComponent<T>();
        _prefab.gameObject.SetActive(false);

        Initialize(_prefab, _initialSize, _initializePoolObject);
    }

    /// <summary>
    /// Initialization method called from constructors.
    /// </summary>
    private void Initialize(T _prefab, int _initialSize, InitializePoolObject _initializePoolObject = null)
    {
        prefab = _prefab;
        initialSize = _initialSize;
        initializePoolObject = _initializePoolObject;

        for (int i = 0; i < initialSize; i++)
        {
            SpawnObject(true);
        }
    }

    public T GetObject()
    {
        T obj = null;

        // Get next object in the queue.
        // If the returned object is null, keep returning objects until either one isn't null or the queue is empty.
        while (obj == null && pool.Count != 0)
        {
            obj = pool.Dequeue();
        }

        // Spawn object if needed.
        if (obj == null) obj = SpawnObject(false); // Don't enqueue the newly spawned object, since it'll be added back to the queue once it's returned.

        obj.gameObject.SetActive(true);

        return obj;
    }

    public void ReturnObject(T obj)
    {
        if (obj == null)
        {
            Debug.LogWarning("The given object was null when trying to return an object to this Object Pool. While this doesn't cause any immediate problems with this ObjectPool, it may indicate that something has gone wrong elsewhere, or maybe you just forgot to add a null check before returning an object back to this pool. Returning.");
            return;
        }

        obj.gameObject.SetActive(false);
        pool.Enqueue(obj);
    }

    public void ReturnAllObjects()
    {
        foreach (var obj in allObjects)
        {
            if (obj != null && obj.gameObject.activeSelf)
            {
                ReturnObject(obj);
            }
        }
    }

    private T SpawnObject(bool autoEnqueue)
    {
        // If the required prefab is null (meaning the non-prefab constructor was called AND the scene has been switched), simply create a new one.
        if (prefab == null)
        {
            prefab = new GameObject("Object Pool Prefab").AddComponent<T>();
            prefab.gameObject.SetActive(false);
        }

        var obj = Object.Instantiate(prefab);
        allObjects.Add(obj);
        if (initializePoolObject != null) initializePoolObject.Invoke(obj);
        if (autoEnqueue) pool.Enqueue(obj);
        obj.gameObject.SetActive(!autoEnqueue);
        return obj;
    }

    /// <summary>
    /// Destroys all objects in the pool.
    /// Should another pool be needed, a new instance of ObjectPool<T> should be created.
    /// </summary>
    public void Dispose()
    {
        while (pool.Count > 0)
        {
            var obj = pool.Dequeue();
            if (obj != null) Object.Destroy(obj.gameObject);
        }

        allObjects.Clear();
    }
}
