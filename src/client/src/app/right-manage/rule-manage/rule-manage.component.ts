import { Component, OnInit } from '@angular/core';
import { RuleGroup } from '../models/rule-group';
import { Guid } from 'src/app/core/utils/guid';
import { RuleService } from 'src/app/core/services/rule.service';
import { Rule } from '../models/rule';
import { Condition } from '../models/condition';
import { NzMessageService, NzModalService } from 'ng-zorro-antd';
import { EntityService } from 'src/app/core/services/entity.service';

@Component({
  selector: 'app-rule-manage',
  templateUrl: './rule-manage.component.html',
  styleUrls: ['./rule-manage.component.scss']
})
export class RuleManageComponent implements OnInit {

  // 规则组数组
  ruleGroups: RuleGroup[] = [];

  // 规则详情
  ruleOptions: any = {
    //   id: Guid.newGuid(),
    //   childCondition: [
    //     { id: Guid.newGuid(), field: '', operateType: '', value: '' }
    //   ],
    //   childRuleOption: [
    //     {
    //       id: Guid.newGuid(),
    //       childCondition: [
    //       ],
    //       childRuleOption: [
    //       ]
    //     }
    //   ],
    //   combineType:'1'
  };

  // 规则
  rules: Rule[] = [];

  // 条件
  ruleConditions: Condition[] = [];

  // 规则组
  ruleGroup: RuleGroup = { id: Guid.newGuid(), name: '', dbContext: null, entity: null };

  // 表选择框数据
  entityData = [];

  // 编辑模式
  isEditMode = false;

  constructor(private _ruleService: RuleService,
    private _messageService: NzMessageService,
    private _modalService: NzModalService,
    private _entityService: EntityService) { }

  ngOnInit(): void {
    this.initRuleGroups();
    this.initEntityData();
  }

  // 保存规则组
  saveRule() {

    if (!this.ruleGroup.name) {
      this._messageService.error("请输入规则组名称！");
      return;
    }

    if (this.ruleOptions.childCondition.length == 0) {
      this._messageService.error("请输入内容！");
      return;
    }

    if (!this.isRuleValid()) {
      this._messageService.error("请确认所有规则内容的完整！");
      return;
    }

    if (!this.isConditionValid()) {
      this._messageService.error("请确认所有条件的完整！");
      return;
    }

    let rs: Rule[] = [];
    let rConditions: Condition[] = [];
    let rGroup: RuleGroup = new RuleGroup();

    Object.assign(rGroup, this.ruleGroup);

    // 做成规则
    rs.push({ id: this.ruleOptions.id, upRuleId: '', ruleGroupId: this.ruleGroup.id, combineType: this.ruleOptions.combineType });
    this.makeRule(this.ruleOptions.childRuleOption, this.ruleOptions.id, rs);

    // 做成条件
    this.makeCondition(this.ruleOptions, this.ruleOptions.id, rConditions);

    // 发送数据到服务器
    this._ruleService.addOrModifyRule(rs, rConditions, rGroup).subscribe(
      result => {
        this._messageService.success("操作成功");
        this.isEditMode = false;
        this.initRuleGroups();
      }
    );

  }

  // 取消
  cancel(){
    this.isEditMode = false;
  }

  // 生成规则
  private makeRule(options, id, rs) {
    for (var option of options) {
      rs.push({ id: option.id, upRuleId: id, ruleGroupId: this.ruleGroup.id, combineType: option.combineType });
      this.makeRule(option.childRuleOption, option.id, rs);
    }
  }

  // 生成条件
  private makeCondition(option, id, cs) {
    option.childCondition.forEach(condition => {
      cs.push({
        id: condition.id, ruleId: id, ruleGroupId: this.ruleGroup.id, dbContext: condition.dbContext,
        table: condition.table, field: condition.field, operateType: condition.operateType, value: condition.value
      });
    });

    for (var option of option.childRuleOption) {
      this.makeCondition(option, option.id, cs);
    }

  }

  // 添加规则组
  addRuleGroup() {
    this.isEditMode = true;
    this.ruleOptions = { id: Guid.newGuid(), childCondition: [], childRuleOption: [], combineType: '0' };
    this.ruleGroup = { id: Guid.newGuid(), name: '', dbContext: null, entity: null };
  }

  // 编辑规则组
  edit(group) {
    this.rules = [];
    this.ruleConditions = [];
    this.ruleGroup = new RuleGroup();
    this._ruleService.getRuleDetail(group.id).subscribe(
      result => {
        this.rules = result.rules;
        this.ruleConditions = result.ruleConditions;
        this.ruleGroup = result.ruleGroup;
        this.initOption();
        this.isEditMode = true;
      }
    );
  }

  // 删除规则组
  delete(group) {
    this._modalService.confirm(
      {
        nzContent: "是否删除当前规则组",
        nzOnOk: () => {
          this._ruleService.deleteRuleGroup(group.id).subscribe(
            result => {
              this._messageService.success("删除成功");
              this.initRuleGroups();
            }
          )
        }
      }
    )
  }


  // 初始化实体数据数据库表字段
  private initEntityData() {
    this._entityService.getDropDownEntityData().subscribe(
      result => {
        this.entityData = result;
      }
    );
  }

  // 取得数据下拉框数据
  getDbContextData() {
    return this.unique(this.entityData.map(entity => entity.dbContext));
  }

  // 取得表下拉框数据
  getTableData() {
    return this.unique(this.entityData.filter(entity => entity.dbContext == this.ruleGroup.dbContext).map(entity => entity.table));
  }

  // 取得字段下拉框数据
  getFieldData() {
    return this.unique(this.entityData.filter(entity => entity.dbContext == this.ruleGroup.dbContext && entity.table == this.ruleGroup.entity)
      .map(entity => entity.field));
  }

  // 表数据发生变化
  tableDataChange() {
    this.resetRuleConditionField(this.ruleOptions);
  }

  // 重置规则条件字段值
  resetRuleConditionField(options) {
    for (var option of options) {
      for (var condition of option.childCondition) {
        condition.field = null;
      }
      this.resetRuleConditionField(option.childRuleOption);
    }
  }

  // 去重
  private unique(arr) {
    return Array.from(new Set(arr))
  }


  // 初始化数据结构
  private initOption() {

    // 构造规则
    for (var rule of this.rules) {

      // 第一级规则
      if (!rule.upRuleId) {
        this.ruleOptions = { id: rule.id, childRuleOption: [], childCondition: [], combineType: rule.combineType };
        this.pushRule(rule.id);
      }
    }


    // 构造条件
    for (var condition of this.ruleConditions) {

      var ruleOption = this.findRuleById(condition.ruleId);

      ruleOption.childCondition.push({
        id: Guid.newGuid(), field: condition.field, operateType: condition.operateType, value: condition.value
      });
    }

  }

  // 初始化规则组
  private initRuleGroups() {
    this._ruleService.getAllRuleGroup().subscribe(
      result => {
        this.ruleGroups = result;
      }
    )
  }

  // 检查是否所有规则包含条件
  private checkRuleResult: boolean = true;
  private checkRule(options) {
    if (options.childCondition.length == 0 || !options.combineType) {
      this.checkRuleResult = false;
    }
    if (options.childRuleOption) {
      for (var ruleOption of options.childRuleOption) {
        this.checkRule(ruleOption);
      }
    }
  }
  private isRuleValid() {
    this.checkRule(this.ruleOptions);
    var result = this.checkRuleResult;
    this.checkRuleResult = true;
    return result;
  }

  // 检查条件的内容是否完整
  private checkConditionResult: boolean = true;
  private checkCondition(options) {

    for (var condition of options.childCondition) {
      if (!condition.field || !condition.operateType || !condition.value) {
        this.checkConditionResult = false;
      }
    }

    if (options.childRuleOption) {
      options.childRuleOption.forEach(o => {
        this.checkCondition(o);
      });
    }
  }
  private isConditionValid() {
    this.checkCondition(this.ruleOptions);
    var result = this.checkConditionResult;
    this.checkConditionResult = true;
    return result;
  }

  // 构造规则
  pushRule(id) {
    var ruleOption = this.findRuleById(id);
    var childs = this.rules.filter(rule => rule.upRuleId == id);
    childs.forEach(element => {
      ruleOption.childRuleOption.push({ id: element.id, childRuleOption: [], childCondition: [], combineType: element.combineType });
      this.pushRule(element.id);
    });
  }

  // 查找规则
  private findedRule;
  private findRule(option, id) {
    if (option.id == id) {
      this.findedRule = option;
    } else {
      if (option.childRuleOption) {
        option.childRuleOption.forEach(o => {
          this.findRule(o, id);
        });
      }
    }
  }

  // 根据id查找规则对应的节点
  private findRuleById(id) {
    this.findRule(this.ruleOptions, id);
    return this.findedRule;
  }

}