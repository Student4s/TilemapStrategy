using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Placer : MonoBehaviour
{
    public List<Placable> placedThings;

    private TilemapHolder2 grid;
    private Preview placablePreview;

    [SerializeField] private Camera mainCamera;

    private void Awake()
    {
        placedThings = new List<Placable>();
        mainCamera = Camera.main;
    }

    private TilemapHolder2 GetGrid()
    {
        if (grid == null)
        {
            grid = GetComponent<TilemapHolder2>();
        }

        return grid;
    }

    private void Update()
    {
        if (placablePreview == null)
        {
            return;
        }

        if (Input.GetMouseButtonDown(1))
        {
            Destroy(placablePreview.gameObject);
            placablePreview = null;
            return;
        }
        else if (Input.GetKeyDown(KeyCode.F1))
        {
            InstantiatePlacable();
        }

        if (Input.GetMouseButton(0))
        {
            Vector3 mouse = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            Vector2Int gridPos = GetGrid().GetGridPosHere(mouse);

            Vector2 cellCenter;
            if (GetGrid().IsAreaBounded(gridPos.x, gridPos.y, Vector2Int.one))
            {
                cellCenter = GetGrid().GetGridCellPosition(gridPos);
            }
            else
            {
                cellCenter = mouse;
            }

            Debug.Log(cellCenter);
            placablePreview.SetCurrentMousePosition(cellCenter, gridPos,
                () => GetGrid().IsBuildAvailable(gridPos, placablePreview));
        }
    }

    public void ShowPlacablePreview(Preview preview)
    {
        if (placablePreview != null)
        {
            Destroy(placablePreview.gameObject);
        }

        var cameraPos = Camera.main.transform.position;
        var instPreviewPos = new Vector2(cameraPos.x, cameraPos.y);

        placablePreview = Instantiate(preview, instPreviewPos, Quaternion.identity);

        Vector2Int gridPos = GetGrid().GetGridPosHere(placablePreview.transform.position);

        if (GetGrid().IsAreaBounded(gridPos.x, gridPos.y, Vector2Int.one))
        {
            placablePreview.SetSpawnPosition(gridPos);
            placablePreview.SetBuildAvailable(GetGrid().IsBuildAvailable(gridPos, placablePreview));
        }
        else
        {
            placablePreview.SetBuildAvailable(false);
        }
    }

    private void InstantiatePlacable()
    {
        if (placablePreview != null && placablePreview.IsBuildAvailable())
        {
            Placable placableInstance = placablePreview.InstantiateHere();

            placedThings.Add(placableInstance);
            OccupyCells(placableInstance.GridPlace);

            Destroy(placablePreview.gameObject);

            if (placablePreview != null)
            {
                placablePreview = null;
            }
        }
    }

    private void OccupyCells(GridPlace place)
    {
        GetGrid().SetGridPlaceStatus(place, true);
    }
}