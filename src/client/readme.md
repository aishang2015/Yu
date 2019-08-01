### 使用方法

1. 添加实体
   - 在项目任意位置创建类，只需要继承BaseEntity。然后就可以在全局通过IRepository<TEntity,Tkey>来访问仓储类了。
   - 只需在你的实体类上标注BelongToAttribute来决定它属于哪个数据库。标记完后，框架会自动生成dbset。实现的对应的IEntityTypeConfiguration也会自动应用到数据库中。
   - 通过在实体类上添加DataRuleManageAttribute，就可以让该实体类的定义保存到实体管理表中。然后就可以通过实体管理表来制定数据规则了。
2. 所有Service类自动注入，需要定义接口为IXXXXService，实现类为XXXXService即可。
3. Model层的验证器文件，数据映射配置文件根据需求创建即可框架会自动扫描并加入配置。
4. 一些系统功能的实现都放到了core层，分文件夹进行管理，可以根据需求进行修改。

