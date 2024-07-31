
# 此项目暂停更新

 → https://github.com/DaiYu-233/YMCL.Avalonia

<p align="center">
<img Height="200" Width="200" src="https://github.com/DaiYu-233/YMCL/blob/main/Assets/YMCL-Icon.png?raw=true"/>
</p>
<div align="center">
<h1 align="center">Yu Minecraft Launcher</h1>
</div>
Yu Minecraft Launcher (YMCL) 是使用 [.NET 8.0](https://dotnet.microsoft.com/zh-cn/download/dotnet/8.0) 、Windows Presentation Foundation（WPF） 开发的 Minecraft 启动器。YMCL 以 Apache License 协议开放源代码，软件必须包含原始代码的版权声明和许可声明，以及一个包含许可信息的通知文件。

## UrlScheme

```UrlScheme
#参数内不可包含特殊字符，如包含空格需使用单引号。[参数]必须，<参数>可选。
#在终端中使用：start ymcl://"<参数>"
ymcl://<参数> #参数格式，示例：ymcl://--launch 1.12.2

--launch [version] <minecraftFolder> <serverIP> #启动Minecraft

--import setting [json(UTF-8编码的十六进制字符串)] #导入启动器设置，json示例：796d636c(ymcl)
```
## 开源项目使用

**[Costura.Fody](https://github.com/Fody/Costura)**  **[MinecraftLaunch](https://github.com/Blessing-Studio/MinecraftLaunch)**  **[iNKORE.UI.WPF.Modern](https://github.com/iNKORE-Public/UI.WPF.Modern)**  **[Newtonsoft.Json](https://www.newtonsoft.com/json)**  **[Panuon.WPF.UI](https://github.com/Panuon/Panuon.WPF.UI)**  **[UpdateD](https://www.nuget.org/packages/UpdateD)**
