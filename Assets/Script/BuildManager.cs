using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BuildManager : MonoBehaviour
{

    public TurretData laserTurretData;
    public TurretData missileTurretData;
    public TurretData standardTurretData;

    //表示当前选择要建造的炮台数据
    private TurretData selectedTurretData;

    //表示当前选择的炮台
    private MapCube selectedMapCube;

    public Text moneyText;

    public Animator moneyAnimator;

    private int money = 1000;

    public GameObject upgradeCanvas;

    private Animator upgradeCanvasAnimator;

    public Button buttonUpgrade;


    void ChangeMoney(int change) {
        money += change;
        moneyText.text = "¥" + money;
    }

     void Start()
    {
        upgradeCanvasAnimator = upgradeCanvas.GetComponent<Animator>();
    }

    void Update()
    {
        if ( Input.GetMouseButtonDown(0)) {
            if (EventSystem.current.IsPointerOverGameObject()==false) {
                //建造炮台
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                bool isCollider = Physics.Raycast(ray, out hit, 1000, LayerMask.GetMask("MapCube"));

                if (isCollider) {

                    MapCube mapCube = hit.collider.GetComponent<MapCube>();//得到点击的mapCube

                    if (selectedTurretData != null && mapCube.turretGo == null)
                    {
                        //可以创建
                        if (money > selectedTurretData.cost)
                        {
                            ChangeMoney(-selectedTurretData.cost);
                            mapCube.BuildTurret(selectedTurretData);
                        }
                        else {
                            //TODU 提示钱不够
                            moneyAnimator.SetTrigger("Flicker");
                        }
                    }
                    else if(mapCube.turretGo != null)
                    {
                        //TODO 升级处理
                        ShowUpgradeUI(mapCube.transform.position, mapCube.isUpgraded);
                        //if (mapCube.isUpgraded)
                        // {
                        //    ShowUpgradeUI(mapCube.transform.position, true);
                        // }
                        // else {
                        //     ShowUpgradeUI(mapCube.transform.position, false);
                        // }

                        if (mapCube == selectedMapCube && upgradeCanvas.activeInHierarchy)
                        {
                            HideUpgradeUI();
                        }
                        else {
                            ShowUpgradeUI(mapCube.transform.position, mapCube.isUpgraded);
                        }
                        selectedMapCube = mapCube;
                    }
                }
            }
        }
    }

    public void OnLaserSelected(bool isOn) {

        if (isOn) {
            selectedTurretData = laserTurretData;
        }
    }

    public void OnMissileSelected(bool isOn){
        if (isOn) {
            selectedTurretData = missileTurretData;
        }

    }

    public void OnStandardSelected(bool isOn)
    {
        if (isOn) {
            selectedTurretData = standardTurretData;
        }
    }

    void ShowUpgradeUI(Vector3 pos, bool isDisableUpgrade = false) {
        upgradeCanvas.SetActive(true);
        upgradeCanvas.transform.position = pos;
        buttonUpgrade.interactable = !isDisableUpgrade;
    }


    IEnumerator HideUpgradeUI()
    {
        upgradeCanvasAnimator.SetTrigger("Hide");
        yield return new WaitForSeconds(0.8f);
        upgradeCanvas.SetActive(false);
    }


    public void OnUpgradeButtonDown() {

        if (money >= selectedMapCube.turretData.costUpgraded)
        {
            ChangeMoney(-selectedMapCube.turretData.costUpgraded);
            selectedMapCube.UpgradeTurret();
        }
        else {
            moneyAnimator.SetTrigger("Flicker");
        }
       
        StartCoroutine(HideUpgradeUI());
    }

    public void OnDestroyButtonDown()
    {
        selectedMapCube.DestroyTurret();
        StartCoroutine(HideUpgradeUI());
    }
}
