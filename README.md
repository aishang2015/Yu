# YU

基于aspnetcore 2.2 和angular8 开发，在asp net core identity权限框架基础上添加了组织，界面以及数据管理的可扩展的权限管理系统。

### 示例

测试地址: [http://140.143.189.32:8081] 测试账号管理员：admin1-10 普通用户：user1-10，密码均为P@ssword1

*展示程序使用的是sample分支，在master基础上添加了展示数据和定时刷新数据库的功能。sample分支版本落后于master。*

### 技术框架

- api
  - 核心框架：AspNetCore 2.2
  - 持久层框架：Entity Framework 2.2
  - 缓存：MemroyCache
  - 日志：Nlog.Web.AspNetCore 4.8.2
  - 对象映射：AutoMapper 8.1.0
  - 模型验证：FluentValidation.AspNetCore 8.4.0
  - Expression序列化和反序列化：Serialize.Linq 1.8.1 
  - API文档：Swashbuckle.AspNetCore 4.0.1
  - 作业调度：Quartz.Net 3.0.7
  - PostgreSQL支持：Npgsql.EntityFrameworkCore.PostgreSQL 2.2.4
  - MySQL支持：Pomelo.EntityFrameworkCore.MySql 2.2.0
  - SQLite支持：Microsoft.EntityFrameworkCore.Sqlite 2.2.6
  - Redis：StackExchange.Redis 2.0.601
- web
  - 核心框架：angular 8.1.3
  - UI框架：ng-zorro-antd 8.1.2
  - jwt：angular-jwt 2.1.0

### 开发环境

- SQL Server Express LocalDb(SqlLocalDb)
- Node 10.4.0 & NPM 6.1.0
- .NET Core 2.2 sdk
- Angular CLI 8.1.3
- Windows 10

### 开发工具

- visual studio 2017
- visual code

### 安装运行

web端和api端分开开发。示例程序服务器中使用了sql server 2012。本地开发环境使用了vs2017自带的SQL Server Express LocalDb(SqlLocalDb)，可以根据需要自行修改appsettings.json的配置，数据会在数据库自动创建时自动插入。

**api端**

确保安装了net core2.2 sdk，使用vs2017打开，vs会自动还原所有包，f5直接运行。

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
	├───Yu.Data 实体层，包含实体定义，仓储实现等
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
		├───Mvc controller基类和认证过滤器
		├───Quartznet 
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
			└───core 共享功能模块
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

