using UnityEngine;
using UnityEngine.UI;
using easyar;
using System;

namespace SchoolNav
{
    public class MapController : MonoBehaviour
    {
        /// <summary>
        /// 游戏控制
        /// </summary>
        private GameController game;
        public Button btnSave;
        /// <summary>
        /// 显示文本
        /// </summary>
        public Text text;
        public ARSession session;
        /// <summary>
        /// 稀疏空间工作框架
        /// </summary>
        public SparseSpatialMapWorkerFrameFilter mapWorker;
        /// <summary>
        /// 稀疏空间地图
        /// </summary>
        public SparseSpatialMapController map;
		
void Start()
{
    game = FindObjectOfType<GameController>();
    btnSave.interactable = false;
    session.WorldRootController.TrackingStatusChanged += OnTrackingStatusChanged;
    if (session.WorldRootController.TrackingStatus == MotionTrackingStatus.Tracking)
    {
        btnSave.interactable = true;
    }
    else
    {
        btnSave.interactable = false;
    }
}
/// <summary>
/// 跟踪状态事件
/// </summary>
/// <param name="status"></param>
private void OnTrackingStatusChanged(MotionTrackingStatus status)
{
    if (status == MotionTrackingStatus.Tracking)
    {
        btnSave.interactable = true;
        text.text = "进入跟踪状态。";
    }
    else
    {
        btnSave.interactable = false;
        text.text = "跟踪状态异常";
    }
}
/// <summary>
/// 保存地图
/// 需要将地图的ID和Name保存到数据库，因为EasyAR官方没有提供稀疏空间地图访问的接口
/// </summary>
public void SaveMap()
{
    btnSave.interactable = false;
    //地图保存结果反馈
    mapWorker.BuilderMapController.MapHost += (mapInfo, isSuccess, error) =>
    {
        if (isSuccess)
        {
            game.SaveMapID(mapInfo.ID);
            game.SaveMapName(mapInfo.Name);
            text.text = "地图保存成功。";
            // 保存地图信息到mysql-building
            // 调用接口http://localhost:8080/mobileapp/building/add
            // 参数?bid=3&bname=3&x=744&y=57
            string url = "http://" + game.GetHttpIP() + ":8080/mobileapp/building/add?" +
                "bname=" + mapInfo.Name + "&" +
                "ssmap=" + mapInfo.ID;
            game.httpApi(url, "Post");
        }
        else
        {
            text.text = "地图保存出错：" + error;
            btnSave.interactable = true;
        }
    };
    try
    {
        //保存地图到EasyAR云端
        mapWorker.BuilderMapController.Host(game.inputName, null);
        text.text = "开始保存地图，请稍等。";
    }
    catch (Exception ex)
    {
        text.text = "保存出错：" + ex.Message;
        btnSave.interactable = true;
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
    }
}

