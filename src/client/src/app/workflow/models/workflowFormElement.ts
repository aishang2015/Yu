export class WorkFlowFormElement {
    id?:string;          // id

    name:string;        // 名称

    elementId: string;  // domid

    defineId:string;    // 工作流定义ID
    type: string;       // 类型
    width: number;      // 宽度

    options:string;     // 选项  选择器用

    line: number;       // 行数 文本域用
}