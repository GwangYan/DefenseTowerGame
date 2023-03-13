using UnityEngine;

public class TowerButton : MonoBehaviour {
    [SerializeField]
    private Tower towerObject;
    [SerializeField]
    private Sprite dragSprite;
    [SerializeField]
    private int towerPrice;
    [SerializeField]
    private string name;

    public Tower TowerObject
    {
        get { return towerObject; }
    }

    public Sprite DragSprite
    {
        get { return dragSprite; }
    }

    public int TowerPrice
    {
        get { return towerPrice; }
    }

    public string Name
    {
        get { return name; }
    }

    void OnMouseDown()
    {
        Debug.Log("OnMouseButton");
        TowerManager.Instance.OnBuildTower(this);
    }
}
