using UnityEngine;
using UnityEngine.UI;

public class ClosingWalls : MonoBehaviour
{
    public float sidesmax = 0.25f; 
    public float topbottommax = 0.15f; 
    public float closetime = 120f; 
    public bool waitforfirstlap = true; 
    public float testdelay = 5f; 
    public Color wallcolor = Color.black;
    

    public static ClosingWalls Instance;

    float closed; 
    float testtimer;
    RectTransform left, right, top, bottom;

    public float SideCover { get { return closed * sidesmax; } }
    public float TopBottomCover { get { return closed * topbottommax; } }
    public bool HasStarted { get { return closed > 0f; } }

    void Awake()
    {
        Instance = this;
        BuildWalls();
        DrawWalls();
    }

    void Update()
    {
        if (ShouldClose())
        {
            // walls close faster as the race speeds up, and never open again
            float race = CheckpointManager.Current(1f);
            if (closetime > 0f) closed = Mathf.MoveTowards(closed, 1f, race * Time.deltaTime / closetime);
        }

        DrawWalls();
    }

    // true once the walls are allowed to start closing in
    bool ShouldClose()
    {
        if (GameManager.Instance != null && !GameManager.Instance.GameStarted) return false;

        if (CheckpointManager.Instance != null)
        {
            if (!waitforfirstlap) return true;
            return CheckpointManager.Instance.MostLaps >= 1;
        }

        testtimer += Time.deltaTime;
        return testtimer >= testdelay;
    }

    void BuildWalls()
    {
        GameObject holder = new GameObject("Closing Walls Canvas");
        holder.transform.SetParent(transform, false);

        Canvas canvas = holder.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = -10; 

        left = MakeWall(holder.transform, "Left Wall");
        right = MakeWall(holder.transform, "Right Wall");
        top = MakeWall(holder.transform, "Top Wall");
        bottom = MakeWall(holder.transform, "Bottom Wall");
    }

    RectTransform MakeWall(Transform parent, string wallname)
    {
        GameObject wall = new GameObject(wallname, typeof(RectTransform), typeof(Image));
        wall.transform.SetParent(parent, false);

        Image image = wall.GetComponent<Image>();
        image.color = wallcolor;
        image.raycastTarget = false; 

        return wall.GetComponent<RectTransform>();
    }

    
    void DrawWalls()
    {
        float side = SideCover;
        float topbottom = TopBottomCover;

        Place(left, new Vector2(0f, 0f), new Vector2(side, 1f));
        Place(right, new Vector2(1f - side, 0f), new Vector2(1f, 1f));
        Place(bottom, new Vector2(0f, 0f), new Vector2(1f, topbottom));
        Place(top, new Vector2(0f, 1f - topbottom), new Vector2(1f, 1f));
    }

    void Place(RectTransform wall, Vector2 bottomleft, Vector2 topright)
    {
        wall.anchorMin = bottomleft;
        wall.anchorMax = topright;
        wall.offsetMin = Vector2.zero;
        wall.offsetMax = Vector2.zero;
    }
}
