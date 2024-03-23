## Yu Minecraft Launcher

Yu Minecraft Launcher (YMCL) 是 [@DaiYu](https://daiyu.fun/) 使用 [.NET 8.0](https://dotnet.microsoft.com/zh-cn/download/dotnet/8.0) 、Windows Presentation Foundation（WPF） 开发的 Minecraft 启动器。YMCL 以 GPL-3.0 协议开放源代码，可以修改且分发，但不得删除原作品的标记，修改后的作品必须也采用 GPL 协议进行开源。

## 更新日志

### 1.0.0.20240324.0_Beta

- 启动游戏
  - 显示游戏输出信息

- 不同 `.minecraft` 文件夹之间切换
- 版本设置
  - 启用与禁用 `Mod`
  - 版本文件夹快捷方式
    - 版本文件夹 `\`
    - 存档文件夹 `\saves`
    - Mod文件夹 `\mods`
    - 资源包文件夹 `\resourcepacks`
    - 光影包文件夹 `\shaderpacks`

  - 游戏启动全局与局部设置
    - Java
    - 最大运行内存
    - 版本隔离
    - 游戏窗口大小（仅全局）
    - 自动进入服务器（仅局部）

- 账户登录/添加与切换
  - Offline
  - Microsoft
  - [Authlib-Injector](https://github.com/yushijinhun/authlib-injector)

- 保存皮肤文件（*.png）

- 自动安装游戏

  - 支持项
    - Vanilla
    - Fabric
    - Quilt
  - 自定义下载线程（1 - 512）
  - 下载源
    - Mojang
    - [BmclApi](https://bmclapi2.bangbang93.com/)

- 多语言支持

  - 简体中文
  - 繁體中文
  - English
  - 日本語
  - Русский язык

- 启动器个性化

  - 自定义主界面（xaml）
    - 本地文件
    - 网络文件
  - 显示主题
    - 跟随系统
    - 浅色模式
    - 深色模式


  - 自定义主题色

- 音乐播放器

  - 本体
    - 音乐搜索（网易云Api）
    - 音乐歌词（仅网络音乐）
    - 音乐播放
    - 下载音乐（*.mp3）
    - 播放本地音频文件
  - 桌面歌词
    - 自定义文本对齐方式（左对齐|居中对齐|右对齐）
    - 自定义文本大小

  

## 使用组件

### [2018k](http://2018k.cn)

>一个简单, 快速的软件更新平台

## 开源项目使用表

- **[Costura.Fody](https://github.com/Fody/Costura)**
- **[MinecraftLaunch](https://github.com/Blessing-Studio/MinecraftLaunch)**
- **[iNKORE.UI.WPF.Modern](https://github.com/iNKORE-Public/UI.WPF.Modern)**
- **[Newtonsoft.Json](https://www.newtonsoft.com/json)**
- **[Panuon.WPF.UI](https://github.com/Panuon/Panuon.WPF.UI)**
- **[UpdateD](https://www.nuget.org/packages/UpdateD)**
