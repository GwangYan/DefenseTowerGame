using UnityEngine;

public class TowerSaleButton : MonoBehaviour
{
    [SerializeField]
    private int towerSalePrice;
    [SerializeField]
    private string towerName;
    public int TowerSalePrice
    {
        get { return towerSalePrice; }
    }

    public string TowerName
    {
        get { return towerName;  }
    }

    void OnMouseDown()
    {
        TowerManager.Instance.saleTower(this.TowerSalePrice);
    }
}
