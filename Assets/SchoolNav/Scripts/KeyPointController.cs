﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Newtonsoft.Json;
using easyar;

namespace SchoolNav
{
    public class KeyPointController : MonoBehaviour
    {
        /// <summary>
        /// 跟踪图片
        /// </summary>
        public Transform image;
        /// <summary>
        /// 游戏控制
        /// </summary>
        private GameController game;
        /// <summary>
        /// 返回界面
        /// </summary>
        public GameObject uiBack;
        /// <summary>
        /// 主界面
        /// </summary>
        public GameObject uiMain;
        /// <summary>
        /// 被选中的游戏对象
        /// </summary>
        private Transform selected;
        /// <summary>
        /// 添加按钮
        /// </summary>
        public Button btnAdd;
        /// <summary>
        /// 提示信息文本
        /// </summary>
        public Text textInfo;
        /// <summary>
        /// 关键点名称输入框
        /// </summary>
        public InputField inputField;
        /// <summary>
        /// 关键点类型下拉列表
        /// </summary>
        public Dropdown dropdown;
        /// <summary>
        /// 滚动视图容器
        /// </summary>
        public Transform svContent;
        /// <summary>
        /// 按钮预制件
        /// </summary>
        public SelectButton prefab;
        /// <summary>
        /// 删除按钮
        /// </summary>
        public Button btnDelete;
        public ARSession session;
        /// <summary>
        /// 稀疏空间地图框架
        /// </summary>
        public SparseSpatialMapWorkerFrameFilter mapWorker;
        /// <summary>
        /// 稀疏空间地图
        /// </summary>
        public SparseSpatialMapController map;
        /// <summary>
        /// 平面图像跟踪器
        /// </summary>
        public ImageTrackerFrameFilter imageTracker;

        void Start()
        {
            game = FindObjectOfType<GameController>();
            Load();
            btnAdd.interactable = false;
            btnDelete.interactable = false;
            Close();
            imageTracker.enabled = false;
            LoadMap();
        }
        void Update()
        {
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                if (Input.GetMouseButtonDown(0)
                && !EventSystem.current.IsPointerOverGameObject())
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    TouchedObject(ray);
                }
            }
            else
            {
                if (Input.touchCount == 1
                && Input.touches[0].phase == TouchPhase.Began
                && !EventSystem.current.IsPointerOverGameObject(Input.touches[0].fingerId))
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.touches[0].position);
                    TouchedObject(ray);
                }
            }
        }
        /// <summary>
        /// 本地化地图
        /// </summary>
        private void LoadMap()
        {
            //设置地图
            map.MapManagerSource.ID = game.GetMapID();
            map.MapManagerSource.Name = game.GetMapName();
            //地图获取反馈
            map.MapLoad += (map, status, error) =>
            {
                if (status)
                {
                    textInfo.text = "地图加载成功。";
                }
                else
                {
                    textInfo.text = "地图加载失败：" + error;
                }
            };
            //定位成功事件
            map.MapLocalized += () =>
            {
                textInfo.text = "稀疏空间定位成功。";
                imageTracker.enabled = true;
            };
            //停止定位事件
            map.MapStopLocalize += () =>
            {
                textInfo.text = "停止稀疏空间定位。";
                imageTracker.enabled = false;
            };
            textInfo.text = "开始本地化稀疏空间。";
            mapWorker.Localizer.startLocalization();    //本地化地图
        }
        /// <summary>
        /// 点击物体
        /// </summary>
        /// <param name="ray"></param>
        private void TouchedObject(Ray ray)
        {
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                uiBack.SetActive(false);
                uiMain.SetActive(true);
                var tf = new GameObject().transform;
                tf.position = hit.transform.position;
                tf.parent = map.transform;
                selected = tf;
                btnAdd.interactable = true;
            }
        }
        /// <summary>
        /// 返回
        /// </summary>
        public void Back()
        {
            if (game)
            {
                game.BackMenu();
            }
        }
        /// <summary>
        /// 关闭主界面
        /// </summary>
        public void Close()
        {
            uiMain.SetActive(false);
            uiBack.SetActive(true);
        }
        /// <summary>
        /// 添加关键点
        /// </summary>
        public void Add()
        {
            if (!string.IsNullOrEmpty(inputField.text) && selected != null)
            {
                SelectButton btn = Instantiate(prefab, svContent);

                btn.keyPoint.name = inputField.text;
                btn.keyPoint.position = selected.localPosition;
                btn.keyPoint.pointType = dropdown.value;

                // 保存关键点到mysql-keypoint
                // 调用接口http://localhost:8080/mobileapp/keyPoint/add
                // 参数?pname=str5&x=7&y=4&z=9&ptype=0&pbuilding=1
                string url_bid = "http://" + game.GetHttpIP() + ":8080/mobileapp/building/getBySSMap/" + game.GetMapID();
                string url = "http://" + game.GetHttpIP() + ":8080/mobileapp/keyPoint/add?" +
                        "pname=" + btn.keyPoint.name + "&" +
                        "x=" + btn.keyPoint.position.x + "&" +
                        "y=" + btn.keyPoint.position.y + "&" +
                        "z=" + btn.keyPoint.position.z + "&" +
                        "ptype=" + btn.keyPoint.pointType + "&" +
                        "pbuilding=" + game.httpApi(url_bid, "Get");
                game.httpApi(url, "Post");

                btn.GetComponentInChildren<Text>().text = inputField.text;
                inputField.text = "";
                selected = null;
                textInfo.text = "添加完成。";
                btnAdd.interactable = false;
            }
        }
        /// <summary>
        /// 保存关键点
        /// </summary>
        // public void Save()
        // {
        //     string[] jsons = new string[svContent.childCount];
        //     for (int i = 0; i < svContent.childCount; i++)
        //     {
        //         jsons[i] = JsonUtility.ToJson(svContent.GetChild(i).GetComponent<SelectButton>().keyPoint);
        //     }
        //     if (game)
        //     {
        //         game.SaveKeyPoint(jsons);
        //         textInfo.text = "保存完成。";
        //     }
        // }
        /// <summary>
        /// 加载关键点
        /// </summary>
        private void Load()
        {
            if (game)
            {
                // 从mysql中加载keypoint
                string url = "http://" + game.GetHttpIP() + ":8080/mobileapp/keyPoint/getBySSMap/" + game.GetMapID();
                string res = game.httpApi(url, "Get");
                List<KeyPoint> kps = JsonConvert.DeserializeObject<List<KeyPoint>>(res);
                foreach(KeyPoint kp in kps){
                    SelectButton btn = Instantiate(prefab, svContent);
                    btn.keyPoint = kp;
                    btn.GetComponentInChildren<Text>().text = btn.keyPoint.name;
                }
            }
        }
        /// <summary>
        /// 关键点按钮点击
        /// </summary>
        /// <param name="btn">按钮</param>
        public void SelectButtonClicked(Transform btn)
        {
            selected = btn;
            textInfo.text = btn.GetComponentInChildren<Text>().text;
            btnDelete.interactable = true;
            btnAdd.interactable = false;
        }
        /// <summary>
        /// 删除关键点
        /// </summary>
        public void Delete()
        {
            // 删除关键点
            // 调用接口http://localhost:8080/mobileapp/keyPoint/delete
            // 参数?pname=test1&pbuilding=1
            string url_bid = "http://" + game.GetHttpIP() + ":8080/mobileapp/building/getBySSMap/" + game.GetMapID();
            string url = "http://" + game.GetHttpIP() + ":8080/mobileapp/keyPoint/delete?" + 
                "pname=" + textInfo.text + "&" +
                "pbuilding=" + game.httpApi(url_bid, "Get");
            game.httpApi(url, "Post");

            Destroy(selected.gameObject);
            textInfo.text = "删除完成。";
            btnDelete.interactable = false;
        }
    }
}
