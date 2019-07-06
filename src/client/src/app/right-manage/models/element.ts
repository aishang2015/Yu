export class Element {
    id: string; // 元素Id
    upId: string; // 上级元素Id
    name: string; // 元素名称
    elementType: number; // 元素类型
    identification: string; // 元素唯一标识
    route: string; // 路由
    apis: string[]; // 关联的api
}