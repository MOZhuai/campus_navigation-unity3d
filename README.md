# 校园考场导航系统的设计和实现(Unity3D部分源码以及项目说明)
本项目是主要由 Unity3d 和 Android 设计的导航应用，使用的框架包括EasyAR、SSM和百度地图等。
该部分为 Unity3d 的源代码。
### 项目视频介绍(to be released)
[![校园考场导航系统](https://i.ytimg.com/vi/xnkWCshrG84/maxresdefault.jpg)](https://www.youtube.com/watch?v=xnkWCshrG84 "校园考场导航系统")

### 项目简介
本系统选取SSM技术框架，采用MVC设计模式，构建基于web的系统架构。基于开源的Android系统，借助百度地图API实现室外二维、三维的导航功能，采用Unity3D + EasyAR 4.0框架实现教学楼内的实景导航功能，从而开发能够帮助外来考生快速寻找考点的考场导航系统。

**关键词**：考场导航；SSM框架；百度地图API；EasyAR；实景导航

### 其他部分源码
|模块名以及源码链接|描述|
|:--:|:--:|
|[后台管理系统JAVA](https://github.com/MOZhuai/campus_navigation-back_end)|Spring+SpringMVC+MyBatis，包含地图、考试人员、考试科目(时间、科目名)等信息的数据库操作接口|
|[前端管理页面源码HTML](https://github.com/MOZhuai/campus_navigation-front_end)|数据库内各项管理信息的展示，包含后端接口的调用|
|[地图管理人员端Android](https://github.com/MOZhuai/campus_navigation-examinee)|用于采集空间地图信息，保存地图信息在EasyAR云端地图库，在数据库内保存途经点、路径等信息|

### 项目整体框架
|<img width="360" alt="image" src="https://github.com/MOZhuai/campus_navigation-unity3d/assets/35127072/98d569e1-496d-439a-a1e7-b8cbca7c1bfb">|<img width="360" alt="image" src="https://github.com/MOZhuai/campus_navigation-unity3d/assets/35127072/c7166896-f55b-40b7-b079-f2f07adab0c3">|
|:--:|:--:|
| **功能框架** | **系统架构** |

### 考试时间表和步行导航模块
|<img width="175" alt="image" src="https://github.com/MOZhuai/campus_navigation-unity3d/assets/35127072/6d096e75-ea2d-47c7-a915-7598c87afd2c">|<img width="175" alt="image" src="https://github.com/MOZhuai/campus_navigation-unity3d/assets/35127072/87c788e9-e600-452f-81cc-9e69c1fb7245">|<img width="175" alt="image" src="https://github.com/MOZhuai/campus_navigation-unity3d/assets/35127072/102165c9-cd55-4b03-8eb4-86b37f8ce480">|<img width="175" alt="image" src="https://github.com/MOZhuai/campus_navigation-unity3d/assets/35127072/1dc89c00-3ab3-4f95-924c-8b901277dbe7">|
|:--:|:--:|:--:|:--:|
| **登录界面** | **考试时间表展示界面** | **考生位置<br>考场位置信息展示界面** | **步行导航过程界面** |

### 实景导航模块
|<img width="175" alt="image" src="https://github.com/MOZhuai/campus_navigation-unity3d/assets/35127072/a9d86af8-b349-441d-a394-7b0ae76fbdb1">|<img width="175" alt="image" src="https://github.com/MOZhuai/campus_navigation-unity3d/assets/35127072/68d764a9-8b61-4950-b02e-c50c56b2e102">|<img width="175" alt="image" src="https://github.com/MOZhuai/campus_navigation-unity3d/assets/35127072/95789020-083d-4fe2-a9a6-ed2c34e32ac9">|<img width="175" alt="image" src="https://github.com/MOZhuai/campus_navigation-unity3d/assets/35127072/ef94bb7b-6f8b-435d-830b-61ca0d8fa480">|
|:--:|:--:|:--:|:--:|
| **空间地图识别成功界面** | **实景导航过程界面1** | **实景导航过程界面2** | **实景导航到达界面** |

### 地图录入模块(地图管理人员端)
|<img width="175" alt="image" src="https://github.com/MOZhuai/campus_navigation-unity3d/assets/35127072/71e9ad89-ba53-474d-9bf2-3ee8f8972de6">|<img width="175" alt="image" src="https://github.com/MOZhuai/campus_navigation-unity3d/assets/35127072/2af78104-be74-4d0d-8f32-d9801b2713d3">|<img width="175" alt="image" src="https://github.com/MOZhuai/campus_navigation-unity3d/assets/35127072/6a8eaccb-2a5d-4707-87b0-cad84a1da82d">|<img width="175" alt="image" src="https://github.com/MOZhuai/campus_navigation-unity3d/assets/35127072/684ff6d9-ff0d-4015-9db2-0007f7348300">|<img width="175" alt="image" src="https://github.com/MOZhuai/campus_navigation-unity3d/assets/35127072/c60a78b7-d556-41e2-a8e9-5fd0875c94f1">|
|:--:|:--:|:--:|:--:|:--:|
| **地图添加** | **采集空间特征点** | **保存采集途经点** | **保存目的地点** | **保存路径** |

### 前端管理页面展示(部分)
|<img width="800" alt="image" src="https://github.com/MOZhuai/campus_navigation-unity3d/assets/35127072/6286180c-bc48-472e-8c8d-27fcd930431b">|
|:--:|
| **考试科目界面** |

|<img width="800" alt="image" src="https://github.com/MOZhuai/campus_navigation-unity3d/assets/35127072/728c9c28-e4ae-463d-964f-2d97a77c8119">|
|:--:|
| **考试科目管理** |


|<img width="800" alt="image" src="https://github.com/MOZhuai/campus_navigation-unity3d/assets/35127072/c564bab1-cd03-4704-9541-a8129cde407a">|
|:--:|
| **地图管理** |

