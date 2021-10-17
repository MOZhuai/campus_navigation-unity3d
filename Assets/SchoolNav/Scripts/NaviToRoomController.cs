﻿using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.AI;
using easyar;

namespace SchoolNav
{
    public class NaviToRoomController : MonoBehaviour
    {
        /// <summary>
        /// 返回界面
        /// </summary>
        public GameObject uiBack;
        /// <summary>
        /// 导航界面
        /// </summary>
        public GameObject uiNav;
        /// <summary>
        /// 游戏控制
        /// </summary>
        private GameController game;
        /// <summary>
        /// 使用者位置
        /// </summary>
        public Transform player;
        /// <summary>
        /// 显示游戏对象数组
        /// </summary>
        public ShowChildObject[] staticObjects;
        /// <summary>
        /// 稀疏空间地图
        /// </summary>
        public Transform ssMap;
        /// <summary>
        /// 盒子预制件
        /// </summary>
        public Transform blueBox;
        /// <summary>
        /// 滚动视图内容游戏对象
        /// </summary>
        public Transform svContent;
        /// <summary>
        /// 目的地预制件
        /// </summary>
        public Transform targetPrefab;
        /// <summary>
        /// 选择按钮预制件
        /// </summary>
        public SelectButton selectButton;
        /// <summary>
        /// 路径预制件
        /// </summary>
        public Transform roadPrefab;
        /// <summary>
        /// 导航网络表面
        /// </summary>
        public NavMeshSurface surface;
        /// <summary>
        /// 导航代理
        /// </summary>
        public NavMeshAgent agent;
        /// <summary>
        /// 导航路径
        /// </summary>
        private NavMeshPath path;
        /// <summary>
        /// 目的地
        /// </summary>
        private Transform target;
        /// <summary>
        /// 线条渲染器
        /// </summary>
        public LineRenderer lineRenderer;
        /// <summary>
        /// 刷新频率
        /// </summary>
        public float refresh;
        /// <summary>
        /// 稀疏空间地图工作框架
        /// </summary>
        public SparseSpatialMapWorkerFrameFilter mapWorker;
        /// <summary>
        /// 稀疏空间地图
        /// </summary>
        public SparseSpatialMapController map;
        /// <summary>
        /// 文本显示
        /// </summary>
        public Text textShow;
        public ARSession session;
        /// <summary>
        /// 导航按钮
        /// </summary>
        public Button btnNav;
        /// <summary>
        /// 导航状态
        /// </summary>
        private NavStatus navStatus;
		
        void Start()
        {
            game = FindObjectOfType<GameController>();
            lineRenderer.gameObject.SetActive(false);
            // LoadObjects();
            LoadTarget();
            LoadRoad();
            Close();
            navStatus = NavStatus.wating;
            btnNav.interactable = false;
            LoadMap();
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
                    /////////////这里要改为请微微左右摆动镜头
                    textShow.text = "地图加载成功。";
                }
                else
                {
                    textShow.text = "地图加载失败：" + error;
                }
            };
            //定位成功事件
            map.MapLocalized += () =>
            {
                /////////////这里要改为定位成功，请沿蓝色路线行走
                textShow.text = "稀疏空间定位成功。";
                StartNavi();
            };
            //停止定位事件
            map.MapStopLocalize += () =>
            {
                textShow.text = "停止稀疏空间定位。";
                switch (navStatus)
                {
                    case NavStatus.localized:
                        navStatus = NavStatus.localized;
                        btnNav.interactable = false;
                        Close();
                        break;
                    case NavStatus.navigation:
                        CancelInvoke("DisplayPath");
                        break;
                    default:
                        break;
                }
            };
            textShow.text = "开始本地化稀疏空间。";
            mapWorker.Localizer.startLocalization();    //本地化地图
        }
        
        /// <summary>
        /// 定位成功后开始导航
        /// </summary>
        private void StartNavi(){
            navStatus = NavStatus.navigation;
            lineRenderer.gameObject.SetActive(true);
            BakePath();
            CancelInvoke("DisplayPath");

            var target1 = Instantiate(targetPrefab, ssMap);

            Vector3 pp = JsonConvert.DeserializeObject<Vector3>(PlayerPrefs.GetString("TargetPosition", ""));

            target1.localPosition = pp;
            target1.GetComponent<ShowChildObject>().SetVisible(false);

            target = target1;
            InvokeRepeating("DisplayPath", 0, refresh);
            Close();
        }
        /// <summary>
        /// 设置静态游戏对象显示
        /// </summary>
        private void SetStaticObject()
        {
            foreach (var item in staticObjects)
            {
                item.SetVisible((item.transform.position - player.position).magnitude <= 2);
            }
        }
        /// <summary>
        /// 关闭导航界面
        /// </summary>
        public void Close()
        {
            uiBack.SetActive(true);
            uiNav.SetActive(false);
        }
        /// <summary>
        /// 显示导航界面
        /// </summary>
        public void ShowNav()
        {
            uiBack.SetActive(false);
            uiNav.SetActive(true);
        }
        /// <summary>
        /// 返回菜单
        /// </summary>
        public void Back()
        {
            if (game)
            {
                CancelInvoke("DisplayPath");
                Application.Quit();
            }
        }
        /// <summary>
        /// 加载动态物体
        /// </summary>
        // private void LoadObjects()
        // {
        //     if (game)
        //     {
        //         var list = game.LoadDynamicObject();
        //         foreach (var item in list)
        //         {
        //             var dynamicObject = JsonUtility.FromJson<DynamicObject>(item);
        //             var tf = Instantiate(blueBox, ssMap);
        //             tf.localPosition = dynamicObject.position;
        //             tf.localEulerAngles = dynamicObject.rotation;
        //             tf.localScale = dynamicObject.scale;
        //             var obj = tf.GetComponent<ShowSelfObject>();
        //             obj.SetVisible((tf.position - player.position).magnitude <= 2);
        //         }
        //     }
        // }
        /// <summary>
        /// 加载目的地
        /// </summary>
        private void LoadTarget()
        {
            if (game)
            {
                // 从mysql中加载keypoint
                string url = "http://" + game.GetHttpIP() + ":8080/mobileapp/keyPoint/getBySSMap/" + game.GetMapID();
                string res = game.httpApi(url, "Get");
                List<KeyPoint> kps = JsonConvert.DeserializeObject<List<KeyPoint>>(res);
                foreach(KeyPoint point in kps){
                    if (point.pointType == 0){
                        var target = Instantiate(targetPrefab, ssMap);
                        target.localPosition = point.position;
                        target.GetComponent<ShowChildObject>().SetVisible(false);
                        var btn = Instantiate(selectButton, svContent);
                        btn.GetComponentInChildren<Text>().text = point.name;
                        btn.target = target;
                    }
                }
                // var list = game.LoadKeyPoint();
                // foreach (var item in list)
                // {
                //     KeyPoint point = JsonUtility.FromJson<KeyPoint>(item);

                //     if (point.pointType == 0)
                //     {
                //         var target = Instantiate(targetPrefab, ssMap);
                //         target.localPosition = point.position;
                //         target.GetComponent<ShowChildObject>().SetVisible(false);
                //         var btn = Instantiate(selectButton, svContent);
                //         btn.GetComponentInChildren<Text>().text = point.name;
                //         btn.target = target;
                //     }
                // }
            }
        }
        /// <summary>
        /// 加载路径
        /// </summary>
        private void LoadRoad()
        {
            if (game)
            {    
                // 获取road
                // 调用http://localhost:8080/mobileapp/road/unityGetByBName/
                // 参数test_qs_1
                string url = "http://" + game.GetHttpIP() + ":8080/mobileapp/road/unityGetByBName/" + game.GetMapName();
                string res = game.httpApi(url, "Get");
                List<Road> rs = JsonConvert.DeserializeObject<List<Road>>(res);
                foreach(Road road in rs){
                    var tfRoad = Instantiate(roadPrefab, ssMap.Find("Roads"));

                    tfRoad.localPosition = (road.startPosition + road.endPosition) / 2;
                    tfRoad.LookAt(road.startPosition);
                    tfRoad.localScale = new Vector3(0.02f, 1f, (road.endPosition - road.startPosition).magnitude * 0.1f + 0.2f);
                }

                // var list = game.LoadRoad();
                // foreach (var item in list)
                // {
                //     var road = JsonUtility.FromJson<Road>(item);
                //     var tfRoad = Instantiate(roadPrefab, ssMap.Find("Roads"));

                //     tfRoad.localPosition = (road.startPosition + road.endPosition) / 2;
                //     tfRoad.LookAt(road.startPosition);
                //     tfRoad.localScale = new Vector3(0.02f, 1f, (road.endPosition - road.startPosition).magnitude * 0.1f + 0.2f);
                // }
            }
        }
        /// <summary>
        /// 烘焙路径
        /// </summary>
        private void BakePath()
        {
            agent.enabled = false;
            surface.BuildNavMesh();
            path = new NavMeshPath();
        }
        /// <summary>
        /// 显示导航路径
        /// </summary>
        private void DisplayPath()
        {
            agent.transform.position = player.position;
            agent.enabled = true;
            agent.CalculatePath(target.position, path);
            lineRenderer.positionCount = path.corners.Length;
            lineRenderer.SetPositions(path.corners);
            agent.enabled = false;
        }
        /// <summary>
        /// 选择按钮点击
        /// </summary>
        /// <param name="btn">按钮</param>
        public void SelectButtonClicked(Transform btn)
        {
            navStatus = NavStatus.navigation;
            lineRenderer.gameObject.SetActive(true);
            BakePath();
            CancelInvoke("DisplayPath");
            target = btn.GetComponent<SelectButton>().target;
            InvokeRepeating("DisplayPath", 0, refresh);
            Close();
        }
    }
}

