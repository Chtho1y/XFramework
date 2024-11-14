<h1 align="center"><strong>XFramework</strong></h1>

<p align="center">
    <img src="others/XFramework.png" alt="XFramework Logo" width="256" height="256">
</p>

<p align="center">
  <strong>一站式Unity商业化游戏框架解决方案 | The Ultimate Unity Framework Solution</strong>
    <br>
  <a style="text-decoration:none">
    <img src="https://img.shields.io/badge/Unity%20Ver-2021.3++-blue.svg?style=flat-square" alt="Unity Version" />
  </a>
  <a style="text-decoration:none">
    <img src="https://img.shields.io/github/license/Chtho1y/XFramework" alt="License" />
  </a>
  <a style="text-decoration:none">
    <img src="https://img.shields.io/github/last-commit/Chtho1y/XFramework" alt="Last Commit" />
  </a>
  <a style="text-decoration:none">
    <img src="https://img.shields.io/github/issues/Chtho1y/XFramework" alt="Issues" />
  </a>
  <a style="text-decoration:none">
    <img src="https://img.shields.io/github/languages/top/Chtho1y/XFramework" alt="Top Language" />
  </a>
  <a style="text-decoration:none">
    <img src="https://img.shields.io/github/stars/Chtho1y/XFramework?style=social">
  </a>
  <br>
</p>


## Introduction | 简介

XFramework is a robust and beginner-friendly Unity framework that provides a comprehensive solution for cross-platform development. Whether you are an individual developer or part of a team, XFramework offers a clean codebase, clear documentation, and high performance, making it an excellent choice for commercial-grade projects.

XFramework 是一个强大且新手友好的 Unity 框架，提供跨平台开发的综合解决方案。无论您是个人开发者还是团队的一员，XFramework 都提供了清晰的代码库、详尽的文档和高性能，是商业级项目的理想选择。

## Why Choose XFramework? | 为什么选择 XFramework？

1. **Easy to Use | 易于使用**: Get started with XFramework in just 5 minutes. The framework offers a clean and organized codebase, making it easy to understand and extend. You can quickly remove or replace modules you don't need, thanks to its high cohesion and low coupling design.
   
   只需 5 分钟即可开始使用 XFramework。框架提供了整洁有序的代码库，易于理解和扩展。由于其高内聚低耦合的设计，您可以快速移除或替换不需要的模块。
   
2. **Commercial-Grade Performance | 商业级性能**: Leveraging XLua \ HybridCLR for hot updates, XFramework is optimized for mobile platforms and has been validated in high-DAU commercial games. It features XLua \ HybridCLR for hot updates, the best Luban configuration tables, and the YooAsset resource framework, ensuring efficient resource management and memory usage.
   
   支持利用 XLua \ HybridCLR 进行热更新，XFramework 针对移动平台进行了优化，并在高 DAU 商业游戏中得到验证。具有高效的资源管理和内存使用。

3. **Cross-Platform Support | 跨平台支持**: XFramework supports multiple platforms, including Steam, WeChat Minigame, and AppStore. The framework has already been used in projects available on these platforms.
   
   XFramework 支持多个平台，包括 Steam、微信小游戏和 AppStore。该框架已被用于这些平台上的游戏和工业项目。

4. **Modular Design | 模块化设计**: From asset bundles to UI components, XFramework's modular architecture allows you to pick and choose the features you need for your project, enhancing flexibility and scalability. This framework supports ECS (Entity Component System) for large-scale simulations, allowing thousands of players to be displayed on the same screen.
   
   从资源包到 UI 组件，XFramework 的模块化架构允许您根据项目需求选择所需功能，提高了灵活性和可扩展性。该框架支持 ECS 万人同屏功能，适用于大规模工业仿真或游戏 GPU 动画模拟。

## Quick Start Guide | 快速入门指南

To get a quick overview of how to run XFramework on various platforms, in Unity Package Manager, Add Package from git URL:
```
https://github.com/Chtho1y/XFramework.git
```
要快速了解如何在各个平台上运行 XFramework, 在 Unity 包管理器中通过 git URL 添加包。
 <img src="https://github.com/user-attachments/assets/82faad3b-f08d-41c6-a927-f6d5647b7abf" width="600"/>
 
Or add the following to your project's `manifest.json` file in the `Packages` folder:
或者在项目的 `Packages` 文件夹中的 `manifest.json` 文件中添加以下内容：
```json
{
  "dependencies":
  {
    "com.chtho1y.XFramework": "https://github.com/Chtho1y/XFramework.git"
  }
}
```

## Project Structure | 项目结构
* [Project Structure](others/Structure.md)
```
XFramework
├── Editor
│ ├── AssetBundleBuilder
│ │ ├── AssetBundleBuilder
│ │ ├── AssetBundleBuilderOptions
│ │ ├── AssetBundleBuilderWindow
│ │ ├── Res2BundleBuilder
│ ├── UIBuilder
│ │ ├── ComponentType
│ │ ├── LuaCodeGenerator
│ │ ├── UIBuilderWindow
│ ├── EmmyLuaService
│ ├── GameEditor
│ ├── MissingScriptsAndEventsFinder
│ ├── ProtoTool
├── Engine
│ ├── Bundle
│ │ ├── AssetBundleUpdater
│ │ ├── BundleInfo
│ │ ├── BundleManager
│ │ ├── CryptoTool
│ │ ├── PathProtocol
│ │ ├── ResourceMode
│ │ ├── VersionInfo
│ ├── Lua
│ │ ├── LuaLifecycle
│ │ ├── XLuaSimulator
│ ├── Pool
│ │ ├── GameObjectPool
│ │ ├── GameObjectPoolManager
│ │ ├── ObjectPool
│ ├── Utils
│ │ ├── AnimationEventHandler
│ │ ├── CollisionBehaviour
│ │ ├── GameUtil
│ │ ├── HttpUtil
│ │ ├── LogPrinter
│ │ ├── MonoSingleton
│ │ ├── ParticleScaler
│ │ ├── Singleton
│ │ ├── StringUtil
│ │ ├── TimeUtil
│ │ ├── TriggerBehaviour
│ │ ├── TweenUtil
│ │ ├── UniTaskUtil
│ │ ├── UniWebViewUtil
│ │ ├── GameManager
│ │ ├── UnityEventHelper
├── Main
│ ├── BundleServerConfig
│ ├── GameLoaderProgress
│ ├── GameRoot
├── UI
│ ├── DragButton
│ ├── EmptyGraphic
│ ├── Irregular
│ ├── ScreenFit
│ ├── TextEx
│ ├── Transition
│ ├── UIEvent
│ ├── UIUtil
│ ├── XButton
│ ├── XButtonGroup
│ ├── XComponent
│ ├── XDragButton
│ ├── XDropdown
│ ├── XImage
│ ├── XInputField
│ ├── XRawImage
│ ├── XRectTransform
│ ├── XScrollBar
│ ├── XScrollRect
│ ├── XSlider
│ ├── XText
│ ├── XToggle
│ ├── XTransform
```

## Recommended Third-Party Plugins | 推荐的第三方插件

To ensure the best experience with XFramework, we recommend using the following third-party plugins. Please purchase and import them as needed:
为了确保使用 XFramework 的最佳体验，我们推荐使用以下第三方插件。请根据需要购买并导入：

* [DoTween](https://assetstore.unity.com/packages/tools/animation/dotween-hotween-v2-27676)
* [InGameDebugConsole](https://assetstore.unity.com/packages/tools/gui/in-game-debug-console-68068)

## Open Source Projects We Recommend | 推荐的开源项目

* [UniTask](https://github.com/Cysharp/UniTask)

## Community and Support | 社区和支持

Join our community to discuss and get support for XFramework:
加入我们的社区，讨论并获取 XFramework 的支持：
* 联系QQ: 939526265
* [QQ Group: 574763941](https://qm.qq.com/cgi-bin/qm/qr?_wv=1027&k=C_grV7Zwbegcjlk79wDdvkh8PtRKPkDU&authKey=pwnX5CZ%2FWmWD4D5tRFbHyOy6WHXJ99L%2B%2BCzZH%2B33lH9Qx1Z5AtbVEZXIhEwYqFHq&noverify=0&group_code=574763941)
* [个人站](https://www.aiqwq.com/)

## Buy Me a Milk Tea | 请我喝杯奶茶
<strong> If XFramework has helped you, consider [buying me a milk tea](others/Donate/Donate.md). Your support will enable us to improve and develop faster.
如果 XFramework 对您有帮助，考虑[请我喝杯奶茶](others/Donate/Donate.md)。您的支持将帮助我更好更快地改进和发展。</strong>

## Stargazers
[![Stargazers repo roster for @Chtho1y/XFramework](https://reporoster.com/stars/Chtho1y/XFramework)](https://github.com/Chtho1y/XFramework/stargazers)

<!-- ## Star History

[![Star History Chart](https://api.star-history.com/svg?repos=Chtho1y/XFramework&type=Date)](https://star-history.com/#Chtho1y/XFramework&Date) -->
