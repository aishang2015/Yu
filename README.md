# YU

基于aspnetcore 和angular8 开发，在asp net core identity权限框架基础上开发的权限管理系统。

### 一些截图

![img](https://github.com/aishang2015/Yu/blob/master/screenshots/1.png)

![img](https://github.com/aishang2015/Yu/blob/master/screenshots/2.png)

![img](https://github.com/aishang2015/Yu/blob/master/screenshots/2.png)

### 实现功能

- 权限管理
- 简单工作流
- to do。。

### 重要记录

- 2019/11/12 api升级到net core 3.0 （使用了一些预览版的包）
- 2019/12/06 升级到net core 3.1 

### 技术框架

- api
  - 核心框架：AspNetCore 3.0
  - 持久层框架：Entity Framework 3.0
  - 缓存：MemroyCache
  - 日志：Nlog.Web.AspNetCore 
  - 对象映射：AutoMapper
  - 模型验证：FluentValidation.AspNetCore 
  - Expression序列化和反序列化：Serialize.Linq 
  - API文档：Swashbuckle.AspNetCore 
  - 作业调度：Quartz.Net 
  - PostgreSQL支持：Npgsql.EntityFrameworkCore.PostgreSQL 
  - MySQL支持：Pomelo.EntityFrameworkCore.MySql
  - SQLite支持：Microsoft.EntityFrameworkCore.Sqlite 
  - Redis：StackExchange.Redis 
  - MongoDB：MongoDB.Driver 
  - MQTT：MQTTNet.AspNetCore 
  - SignalR: Microsoft.AspNet.SignalR 
- web
  - 核心框架：angular 
  - UI框架：ng-zorro-antd 
  - jwt：angular-jwt
  - 流程图：jsplumb
  - 缩放：panzoom 
  - aspnet/signalr

### 开发环境

- SQL Server Express LocalDb(SqlLocalDb)
- Node 10.4.0 & NPM 6.1.0
- .NET Core 2.2 sdk
- Angular CLI 8.1.3
- Windows 10

### 开发工具

- visual studio 2019
- visual code

### 安装运行

**api端**

用vs打开，f5运行。

**web端**

```js
$ npm install 
$ ng serve --open
```

### 程序结构

```
src
└───api  api工程
	├───Yu.Api  接口层，只有controller
	├───Yu.Model 模型层，所有的验证数据映射放在这里
	├───Yu.Service 业务处理层，所有的业务处理放在这里
	└───Yu.Data 实体层，包含实体定义，仓储实现等
		├───Configurations 实体配置
		├───Entities 实体定义
		├───Infrasturctures 基础设施
		├───Redis Redis处理类
		└───Repositories 仓储类
	└───Yu.Core 核心层，netcore功能和第三方功能的封装
		├───AutoMapper automapper配置
		├───Captcha 验证码功能
		├───Constants 常量定义
		├───Cors 跨域配置
		├───Expressions 表达式操作
		├───Extensions 一些基本类的功能扩展
		├───FileManage 文件操作
		├───Jwt jwt配置
		├───MQTT MQTTNet扩展
		├───Mvc controller基类和认证过滤器
		├───Quartznet 
		├───SignalR SignalR配置
		├───Swagger
		├───Utils 
		├───Validators fluentvalidtion配置
		└───WebSockets WebSocket配置
└───client  web工程
	└───src  
		└───app  
			├───account 账户模块，包含登录和用户个人信息查看组件
			├───dashboard 公告栏模块
			├───right-manage 权限管理模块
			├───home 根基础页面组件
			├───workflow 工作流模块
			└───core 共享功能模块
				├───Interceptors  http拦截器
				├───components 共享组件
				├───constants 常量
				├───directives 指令
				├───pipes 管道
				├───services 服务
				├───utils 帮助类
				└───validators 表单验证器
```

