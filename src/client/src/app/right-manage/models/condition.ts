export class Condition {
    id: string; // id
    ruleId: string; // 所属规则
    ruleGroupId: string; // 所属规则组
    dbContext: string; // 数据库类型
    table: string; // 实体
    field: string; // 字段
    operateType: number; // 操作类型
    value: string; // 对应值
}