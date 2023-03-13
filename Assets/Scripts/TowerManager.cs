using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class TowerManager : Singleton<TowerManager> {
    public TowerButton towerButtonPressed { get; set; }
    private SpriteRenderer spriteRenderer;  //Setting image to our tower
    private List<Tower> TowerList = new List<Tower>();
    private List<Collider2D> BuildList = new List<Collider2D>();
    private Collider2D buildTile;

    [SerializeField]
    private GameObject TowerButtonPanel;

    [SerializeField]
    private GameObject ArrowSalePanel;
    [SerializeField]
    private GameObject GlaiveSalePanel;
    [SerializeField]
    private GameObject HammerSalePanel;
    [SerializeField]
    private GameObject FireballSalePanel;

    private GameObject SalePanel;

    // Use this for initialization
    void Start () {
        spriteRenderer = GetComponent<SpriteRenderer>();
        buildTile = GetComponent<Collider2D>();
        spriteRenderer.enabled = false;
	}

    // Update is called once per frame
    GameObject towerPanel;
    GameObject towerObject;
    RaycastHit2D _hit;
    Vector3 towerPos;
    void Update () {
		if (Input.GetMouseButtonDown(0))
        {
            Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            /* Ray Cast involves intersecting a ray with the object in an environment.
             * The ray cast tells you what objects in the environment the ray runs into.
             * and may return additional information as well, such as intersection point
             */
            //Finding the worldPoint of where we click, from Vector2.zero (which is buttom left corner)
            RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);

            //Check to see if mouse press location is on buildSites
            if (towerPanel != null)
                Destroy(towerPanel);
            Debug.Log("tag : " + hit.collider.tag);
           
            if (hit.collider.tag == "buildSite")
            {
                towerPanel = Instantiate(TowerButtonPanel);
                towerPanel.transform.position = hit.transform.position;
                towerPos = towerPanel.transform.position;
                _hit = hit;
            }
        }

        //When we have a sprite enabled, have it follow the mouse (I.E - Placing a Tower)
        /*if (spriteRenderer.enabled)
        {
            followMouse();
        }*/
    }

    public void OnBuildTower(TowerButton towerSelected)
    {
        if(towerSelected.TowerPrice <= GameManager.Instance.TotalMoney)
        {
            buildTile = _hit.collider;
            buildTile.tag = "buildSiteFull";     //This prevents us from stacking towers ontop of each other.
            buildTile.name = towerSelected.Name;
            RegisterBuildSite(buildTile);
            placeTower(towerSelected);
        }
    }

    public void RegisterBuildSite(Collider2D buildTag)
    {
        BuildList.Add(buildTag);
    }

    //public void UnRegisterBuildSite(Coll)

    public void RegisterTower(Tower tower)
    {
        TowerList.Add(tower);
    }

    public void RenameTagsBuildSites()
    {
        foreach(Collider2D buildTag in BuildList)
        {
            buildTag.tag = "buildSite";
        }
        BuildList.Clear();
    }

    public void DestroyAllTower()
    {
        foreach(Tower tower in TowerList)
        {
            Destroy(tower.gameObject);
        }
        TowerList.Clear();
    }
    //Place new tower on the mouse click location
    public void placeTower(TowerButton towerButton)
    {
        //If the pointer is not over the Tower Button GameObject && the tower button has been pressed
        //Created new tower at the click location
        if (!EventSystem.current.IsPointerOverGameObject() && towerButton != null)
        {
            Tower newTower = Instantiate(towerButton.TowerObject);
            newTower.transform.position = towerPos;
            buyTower(towerButton.TowerPrice);
            GameManager.Instance.AudioSource.PlayOneShot(SoundManager.Instance.TowerBuilt);
            RegisterTower(newTower);
            disableDragSprite();
        }
    }
    public void buyTower(int price)
    {
        GameManager.Instance.SubtractMoney(price);
    }

    public void saleTower(int price)
    {
        if (price != 0)
        {
            Destroy(_hit.collider.gameObject);
            GameManager.Instance.AddMoney(price);
        }
    }

    public void followMouse()
    {
        transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector2(transform.position.x, transform.position.y);
    }

    public void enableDragSprite(Sprite sprite)
    {
        spriteRenderer.enabled = true;
        spriteRenderer.sprite = sprite; //Set sprite to the one we passed in the parameter
        spriteRenderer.sortingOrder = 0;
    }
    public void disableDragSprite()
    {
        spriteRenderer.enabled = false;
    }
}
