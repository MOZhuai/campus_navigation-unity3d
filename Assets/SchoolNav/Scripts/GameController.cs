﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System.Net;
using System;

namespace SchoolNav
{
    /// <summary>
    /// 游戏控制器
    /// </summary>
    public class GameController : MonoBehaviour
    {
        private static GameController instance = null;
        /// <summary>
        /// 输入的地图名称
        /// </summary>
        public string inputName;
        /// <summary>
        /// 动态添加物体保存地址
        /// </summary>
        private static readonly string pathDynamicObject = "/dynamicobject.txt";
        /// <summary>
        /// 关键点保存地址
        /// </summary>
        private static readonly string pathKeyPoint = "/keypoint.txt";
        /// <summary>
        /// 路径保存地址
        /// </summary>
        private static readonly string pathRoad = "/road.txt";

        void Awake()
        {
            //实现单实例
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (this != instance)
            {
                Destroy(gameObject);
                return;
            }
        }
        /// <summary>
        /// 获取稀疏空间地图名称
        /// </summary>
        /// <returns>地图名称</returns>
        public string GetMapName()
        {
            return PlayerPrefs.GetString("MapName", "");
        }
        /// <summary>
        /// 保存稀疏空间地图名称
        /// </summary>
        /// <param name="mapName">地图名称</param>
        public void SaveMapName(string mapName)
        {
            PlayerPrefs.SetString("MapName", mapName);
        }
        /// <summary>
        /// 获取稀疏空间地图ID
        /// </summary>
        /// <returns>地图ID</returns>
        public string GetMapID()
        {
            return PlayerPrefs.GetString("MapID", "");
        }
        /// <summary>
        /// 保存稀疏空间地图ID
        /// </summary>
        /// <param name="mapID">稀疏空间地图ID</param>
        public void SaveMapID(string mapID)
        {
            PlayerPrefs.SetString("MapID", mapID);
        }
        /// <summary>
        /// 删除地图
        /// </summary>
        public void DelMap()
        {
            PlayerPrefs.DeleteKey("MapID");
            PlayerPrefs.DeleteKey("MapName");
        }
        /// <summary>
        /// 获取请求IP地址
        /// </summary>
        public string GetHttpIP()
        {
            return PlayerPrefs.GetString("HttpIP", "");
        }
        /// <summary>
        /// 保存请求IP地址
        /// </summary>
        public void SaveHttpIP(string httpIP)
        {
            PlayerPrefs.SetString("HttpIP", httpIP);
        }
        /// <summary>
        /// 返回调试用菜单
        /// </summary>
        public void BackDbgMenu()
        {
            SceneManager.LoadScene("DbgMenu");
        }
        /// <summary>
        /// 返回菜单
        /// </summary>
        public void BackMenu(){
            SceneManager.LoadScene("Menu");
        }
        /// <summary>
        /// 保存路径
        /// </summary>
        /// <param name="stringArray">JSON字符串数组</param>
        // public void SaveRoad(string[] stringArray)
        // {
        //     SaveStringArray(stringArray, Application.persistentDataPath + pathRoad);
        // }
        /// <summary>
        /// 加载路径
        /// </summary>
        /// <returns>JSON字符串列表</returns>
        public List<string> LoadRoad()
        {
            return LoadStringList(Application.persistentDataPath + pathRoad);
        }
        /// <summary>
        /// 保存关键点
        /// </summary>
        /// <param name="stringArray">JSON字符串数组</param>
        // public void SaveKeyPoint(string[] stringArray)
        // {
        //     SaveStringArray(stringArray, Application.persistentDataPath + pathKeyPoint);
        // }
        /// <summary>
        /// 加载关键点
        /// </summary>
        /// <returns>JSON字符串列表</returns>
        public List<string> LoadKeyPoint()
        {
            return LoadStringList(Application.persistentDataPath + pathKeyPoint);
        }
        /// <summary>
        /// 保存动态添加物体信息
        /// </summary>
        /// <param name="stringArray">JSON字符串数组</param>
        public void SaveDynamicObject(string[] stringArray)
        {
			Debug.Log(Application.persistentDataPath + pathDynamicObject);
            SaveStringArray(stringArray, Application.persistentDataPath + pathDynamicObject);
        }
        /// <summary>
        /// 读取动态添加物体信息
        /// </summary>
        /// <returns>物体JSON字符串列表</returns>
        public List<string> LoadDynamicObject()
        {
            return LoadStringList(Application.persistentDataPath + pathDynamicObject);
        }
        /// <summary>
        /// 保存字符串数组
        /// </summary>
        /// <param name="stringArray">字符串数组</param>
        /// <param name="path">保存路径</param>
        private void SaveStringArray(string[] stringArray, string path)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(path))
                {
                    foreach (var s in stringArray)
                    {
                        writer.WriteLine(s);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Log(ex.Message);
            }
        }
        /// <summary>
        /// 读取文本信息
        /// </summary>
        /// <param name="path">文本路径</param>
        /// <returns>字符串列表</returns>
        private List<string> LoadStringList(string path)
        {
            List<string> list = new List<string>();
            try
            {
                using (StreamReader reader = new StreamReader(path))
                {
                    while (!reader.EndOfStream)
                    {
                        list.Add(reader.ReadLine());
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Log(ex.Message);
            }
            return list;
        }
        
        /// <summary>
        /// 调用后台接口
        /// </summary>
        /// <param name="url">请求路径</param>
        /// <param name="method">请求方法</param>
        public string httpApi(string url, string method){
            var request = HttpWebRequest.Create(url) as HttpWebRequest;
            request.Method = method;
            // request.ContentType = "application/json; charset=utf-8";
            // request.Timeout = 3000000;
            // request.ContentLength = 0;
            string res;
            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {
                using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                {
                    res = sr.ReadToEnd();
                    Console.WriteLine(res);
                }
            }
            return res;
        }
    }
}

