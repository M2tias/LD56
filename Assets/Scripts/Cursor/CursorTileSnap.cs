using System;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CursorTileSnap : MonoBehaviour
{
    [SerializeField]
    private Tilemap logicalTilemap;
    [SerializeField]
    private GameObject cursorGraphic;
    [SerializeField]
    private MultiSelectSprites multiSelectCorners;
    [SerializeField]
    private GameObject emptySprite;
    [SerializeField]
    private Transform cursorDragParent;

    private float clickMs = 150f / 1000f;
    private float clickStartedTime;
    private bool clickOrDragStarted = false;
    private bool isDragging = false;
    private bool dragStarted = false;

    private Rect mouseDragPos;
    private Vector2 mouseDragStartPos;
    private MouseDragSprites mouseDragSprites;

    private List<GameObject> mouseDragSpriteList;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mouseDragSpriteList = new List<GameObject>();
        for (int i = 0; i < 100; i++)
        {
            GameObject instance = Instantiate(emptySprite, cursorDragParent, true);
            instance.SetActive(false);
            instance.GetComponent<SpriteRenderer>().sortingOrder = 100;
            mouseDragSpriteList.Add(instance);
        }

        mouseDragSprites = new(mouseDragSpriteList, multiSelectCorners);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Vector3 pos = SnapToTilemap(mouseWorldPos);
        transform.position = new Vector3(pos.x, pos.y, 0f);

        if (Input.GetMouseButtonDown(0))
        {
            if (!clickOrDragStarted)
            {
                clickOrDragStarted = true;
                clickStartedTime = Time.time;
                mouseDragStartPos = pos;
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            clickOrDragStarted = false;
            isDragging = false;
            cursorGraphic.SetActive(true);
            mouseDragSprites.Hide();
            dragStarted = false;
        }

        isDragging = clickOrDragStarted && Time.time - clickStartedTime > clickMs;

        if (isDragging)
        {
            if (!dragStarted)
            {
                dragStarted = true;

                // mouseDragPos.yMin = Mathf.Min(mouseDragStartPos.y, mouseWorldPos.y);
                // mouseDragPos.yMax = Mathf.Max(mouseDragStartPos.y, mouseWorldPos.y);
                // mouseDragPos.xMin = Mathf.Min(mouseDragStartPos.x, mouseWorldPos.x);
                // mouseDragPos.xMax = Mathf.Max(mouseDragStartPos.x, mouseWorldPos.x);
                // Debug.Log($"Drag started: ({mouseWorldPos.x:###.##}, {mouseWorldPos.y:###.##}), rect: ({mouseDragPos.xMin:###.##},{mouseDragPos.yMin:###.##},{mouseDragPos.xMax:###.##},{mouseDragPos.yMax:###.##})");
            }

            mouseDragPos.yMin = Mathf.Min(mouseDragStartPos.y, pos.y);
            mouseDragPos.yMax = Mathf.Max(mouseDragStartPos.y, pos.y);
            mouseDragPos.xMin = Mathf.Min(mouseDragStartPos.x, pos.x);
            mouseDragPos.xMax = Mathf.Max(mouseDragStartPos.x, pos.x);
            mouseDragSprites.Update(mouseDragPos);
            mouseDragSprites.Show();

            cursorGraphic.SetActive(false);
        }

    }

    private Vector3 SnapToTilemap(Vector3 pos)
    {
        Vector3 tileMapOffSet = new Vector3(0.5f, 0.5f, 0);
        Vector3 snappedPos = logicalTilemap.LocalToWorld(
            logicalTilemap.CellToLocal(
                logicalTilemap.LocalToCell(
                    logicalTilemap.WorldToLocal(pos)
                )
            )
        ) + tileMapOffSet;

        return snappedPos;
    }

    class MouseDragSprites
    {
        public GameObject TopLeft;
        public GameObject TopRight;
        public GameObject BottomLeft;
        public GameObject BottomRight;
        public List<GameObject> Tops;
        public List<GameObject> Bottoms;
        public List<GameObject> Lefts;
        public List<GameObject> Rights;

        private List<GameObject> spritePool;
        private MultiSelectSprites _spriteConfig;

        public MouseDragSprites(List<GameObject> sprites, MultiSelectSprites spriteConfig)
        {
            List<GameObject> tmp = new(sprites);
            TopLeft = Pop(tmp, 0);
            TopRight = Pop(tmp, 1);
            BottomLeft = Pop(tmp, 2);
            BottomRight = Pop(tmp, 3);

            SetSprite(TopLeft, spriteConfig.topLeft);
            SetSprite(TopRight, spriteConfig.topRight);
            SetSprite(BottomLeft, spriteConfig.bottomLeft);
            SetSprite(BottomRight, spriteConfig.bottomRight);
            _spriteConfig = spriteConfig;

            spritePool = tmp;
        }

        public void Update(Rect mouseDragPos)
        {
            TopLeft.transform.position = new(mouseDragPos.xMin, mouseDragPos.yMax, 0f);
            TopRight.transform.position = new(mouseDragPos.xMax, mouseDragPos.yMax, 0f);
            BottomLeft.transform.position = new(mouseDragPos.xMin, mouseDragPos.yMin, 0f);
            BottomRight.transform.position = new(mouseDragPos.xMax, mouseDragPos.yMin, 0f);

            TopLeft.name = "TopLeft";
            TopRight.name = "TopRight";
            BottomLeft.name = "BottomLeft";
            BottomRight.name = "BottomRight";
        }

        public void Show()
        {
            SetVisibility(true);
        }

        public void SetVisibility(bool visible)
        {
            TopLeft.SetActive(visible);
            TopRight.SetActive(visible);
            BottomLeft.SetActive(visible);
            BottomRight.SetActive(visible);

            // Tops.ForEach(x => x.SetActive(visible));
            // Bottoms.ForEach(x => x.SetActive(visible));
            // Lefts.ForEach(x => x.SetActive(visible));
            // Rights.ForEach(x => x.SetActive(visible));
        }

        public void Hide()
        {
            SetVisibility(false);
        }

        private GameObject Pop(List<GameObject> list, int index)
        {
            if (list.Count <= index)
            {
                Debug.LogError($"CursortTileNap.cs Pop(): List doesn't contain index {index}. List length: {list.Count}");
                return null;
            }

            GameObject obj = list[index];
            list.RemoveAt(index);
            return obj;
        }

        private void SetSprite(GameObject obj, Sprite sprite)
        {
            if (obj.TryGetComponent(out SpriteRenderer renderer))
            {
                renderer.sprite = sprite;
            }
        }
    }

    [Serializable]
    class MultiSelectSprites
    {
        public Sprite topLeft;
        public Sprite topRight;
        public Sprite bottomLeft;
        public Sprite bottomRight;
        public Sprite top;
        public Sprite bottom;
        public Sprite left;
        public Sprite right;
    }
}
