using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using UnityEngine.UI;


namespace SchoolNav
{
    public class RoadController : MonoBehaviour
    {
        /// <summary>
        /// 游戏控制
        /// </summary>
        private GameController game;
        /// <summary>
        /// 起点下拉列表
        /// </summary>
        public Dropdown dpdStart;
        /// <summary>
        /// 终点下拉列表
        /// </summary>
        public Dropdown dpdEnd;
        /// <summary>
        /// 提示文本
        /// </summary>
        public Text textInfo;
        /// <summary>
        /// 滚动视图内容框
        /// </summary>
        public Transform svContent;
        /// <summary>
        /// 按钮预制件
        /// </summary>
        public SelectButton prefab;
        /// <summary>
        /// 关键点列表
        /// </summary>
        private List<KeyPoint> keyPoints;
        /// <summary>
        /// 选中对象
        /// </summary>
        private Transform selected;
        /// <summary>
        /// 删除按钮
        /// </summary>
        public Button btnDelete;

        void Start()
        {
            game = FindObjectOfType<GameController>();
            keyPoints = new List<KeyPoint>();
            BindDropdown();
            btnDelete.interactable = false;
            Load();
        }
        /// <summary>
        /// 返回调试用菜单
        /// </summary>
        public void BackDbgMenu()
        {
            if (game)
            {
                game.BackDbgMenu();
            }
        }
        /// <summary>
        /// 返回实际菜单
        /// </summary>
        public void BackMenu()
        {
            if (game)
            {
                game.BackMenu();
            }
        }
        /// <summary>
        /// 绑定下拉列表
        /// </summary>
        private void BindDropdown()
        {
            if (game)
            {
                // var list = game.LoadKeyPoint();
                // 从mysql中加载keypoint
                string url = "http://" + game.GetHttpIP() + ":8080/mobileapp/keyPoint/getBySSMap/" + game.GetMapID();
                string res = game.httpApi(url, "Get");
                List<KeyPoint> kps = JsonConvert.DeserializeObject<List<KeyPoint>>(res);
                foreach (KeyPoint kp in kps)
                {
                    dpdStart.options.Add(new Dropdown.OptionData(kp.name));
                    dpdEnd.options.Add(new Dropdown.OptionData(kp.name));
                    dpdStart.captionText.text = dpdStart.options[0].text;
                    dpdEnd.captionText.text = dpdEnd.options[0].text;
                    keyPoints.Add(kp);
                }
            }
        }
        /// <summary>
        /// 添加路径
        /// </summary>
        public void Add()
        {
            SelectButton btn = Instantiate(prefab, svContent);
            btn.road.startName = dpdStart.captionText.text;
            btn.road.endName = dpdEnd.captionText.text;
            btn.road.startPosition = GetPositionByName(btn.road.startName);
            btn.road.endPosition = GetPositionByName(btn.road.endName);
            // 添加路径
            // 调用接口http://localhost:8080/mobileapp/road/unityAdd/
            // 参数f820baa2-7edc-4f8b-a890-9ff2bbc94168?startName=test3&endName=addt4
            string url = "http://" + game.GetHttpIP() + ":8080/mobileapp/road/unityAdd/" + 
                game.GetMapID() + "?" +
                "startName=" + dpdStart.captionText.text + "&" +
                "endName=" + dpdEnd.captionText.text;
            game.httpApi(url, "Post");
            btn.GetComponentInChildren<Text>().text = btn.road.startName + "<===>" + btn.road.endName;
            textInfo.text = "添加完成。";
        }
        /// <summary>
        /// 根据关键点名称获取坐标
        /// </summary>
        /// <param name="pName">关键点名称</param>
        /// <returns>坐标</returns>
        private Vector3 GetPositionByName(string pName)
        {
            foreach (var kp in keyPoints)
            {
                if (kp.name == pName)
                {
                    return kp.position;
                }
            }
            return Vector3.zero;
        }
        /// <summary>
        /// 保存路径
        /// </summary>
        // public void Save()
        // {
        //     string[] jsons = new string[svContent.childCount];
        //     for (int i = 0; i < svContent.childCount; i++)
        //     {
        //         jsons[i] = JsonUtility.ToJson(svContent.GetChild(i).GetComponent<SelectButton>().road);
        //     }
        //     if (game)
        //     {
        //         game.SaveRoad(jsons);
        //         textInfo.text = "保存完成。";
        //     }
        // }
        /// <summary>
        /// 按钮点击响应
        /// </summary>
        /// <param name="btn">按钮</param>
        public void SelectButtonClicked(Transform btn)
        {
            selected = btn;
            textInfo.text = btn.GetComponentInChildren<Text>().text;
            btnDelete.interactable = true;
        }
        /// <summary>
        /// 删除路径
        /// </summary>
        public void Delete()
        {
            // 删除road
            // 调用http://localhost:8080/mobileapp/road/delete/
            // 参数f820baa2-7edc-4f8b-a890-9ff2bbc94168/test1<===>test2
            string url = "http://" + game.GetHttpIP() + ":8080/mobileapp/road/delete/" + 
                game.GetMapID() + "/" + textInfo.text;
            game.httpApi(url, "Post");

            Destroy(selected.gameObject);
            textInfo.text = "删除完成。";
            btnDelete.interactable = false;
        }
        /// <summary>
        /// 加载路径
        /// </summary>
        private void Load()
        {
            if (game)
            {
                // 获取road
                // 调用http://localhost:8080/mobileapp/road/unityGetByBName/
                // 参数test_qs_1
                string url = "http://" + game.GetHttpIP() + ":8080/mobileapp/road/unityGetByBName/" + game.GetMapName();
                string res = game.httpApi(url, "Get");
                List<Road> rs = JsonConvert.DeserializeObject<List<Road>>(res);
                foreach(Road r in rs){
                    var btn = Instantiate(prefab, svContent);
                    btn.road = r;
                    btn.GetComponentInChildren<Text>().text = btn.road.startName + "<===>" + btn.road.endName;
                }

                // var list = game.LoadRoad();
                // foreach (var item in list)
                // {
                //     var btn = Instantiate(prefab, svContent);
                //     btn.road = JsonUtility.FromJson<Road>(item);
                //     btn.GetComponentInChildren<Text>().text = btn.road.startName + "<===>" + btn.road.endName;
                // }
            }
        }
    }
}

