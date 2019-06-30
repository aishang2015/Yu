import { Component, OnInit } from '@angular/core';
import { RuleGroup } from '../models/rule-group';
import { Guid } from 'src/app/core/utils/guid';
import { RuleService } from 'src/app/core/services/rule.service';
import { Rule } from '../models/rule';
import { Condition } from '../models/condition';
import { NzMessageService, NzModalService } from 'ng-zorro-antd';

@Component({
  selector: 'app-rule-manage',
  templateUrl: './rule-manage.component.html',
  styleUrls: ['./rule-manage.component.scss']
})
export class RuleManageComponent implements OnInit {

  // 规则组数组
  ruleGroups: RuleGroup[] = [];

  // 规则详情
  ruleOptions = [
    // {
    //   id: Guid.newGuid(),
    //   childCondition: [
    //     { id: Guid.newGuid(), dbContext: '', table: '', field: '', operateType: '', value: '' }
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
    // },
    // {
    //   id: Guid.newGuid(),
    //   childCondition: [
    //   ],
    //   childRuleOption: [
    //   ]
    // },
  ];

  // 规则
  rules: Rule[] = [];

  // 条件
  ruleConditions: Condition[] = [];

  // 规则组
  ruleGroup: RuleGroup = { id: Guid.newGuid(), name: '' };

  // 编辑模式
  isEditMode = false;

  constructor(private _ruleService: RuleService,
    private _messageService: NzMessageService,
    private _modalService: NzModalService) { }

  ngOnInit(): void {
    this.initRuleGroups();
  }

  // 保存规则组
  saveRule() {

    if (!this.ruleGroup.name) {
      this._messageService.error("请输入规则组名称！");
      return;
    }

    if (this.ruleOptions.length == 0) {
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
    for (var option of this.ruleOptions) {
      rs.push({ id: option.id, upRuleId: '', ruleGroupId: this.ruleGroup.id, combineType: option.combineType });
      this.makeRule(option.childRuleOption, option.id, rs);
    }

    // 做成条件
    for (var option of this.ruleOptions) {
      this.makeCondition(option, option.id, rConditions);
    }

    // 发送数据到服务器
    this._ruleService.addOrModifyRule(rs, rConditions, rGroup).subscribe(
      result => {
        this._messageService.success("操作成功");
        this.isEditMode = false;
        this.initRuleGroups();
      }
    );

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
    this.ruleOptions = [];
    this.ruleGroup = { id: Guid.newGuid(), name: '' };
  }

  // 编辑规则组
  edit(group) {
    this.isEditMode = true;
    this.rules = [];
    this.ruleConditions = [];
    this.ruleGroup = new RuleGroup();
    this.ruleOptions = [];
    this._ruleService.getRuleDetail(group.id).subscribe(
      result => {
        this.rules = result.rules;
        this.ruleConditions = result.ruleConditions;
        this.ruleGroup = result.ruleGroup;
        this.initOption();
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

  // 初始化数据结构
  private initOption() {

    // 构造规则
    for (var rule of this.rules) {

      // 第一级规则
      if (!rule.upRuleId) {
        this.ruleOptions.push({ id: rule.id, childRuleOption: [], childCondition: [], combineType: rule.combineType });
        this.pushRule(rule.id);
      }
    }


    // 构造条件
    for (var condition of this.ruleConditions) {

      var ruleOption = this.findRuleById(condition.ruleId);

      ruleOption.childCondition.push({
        id: Guid.newGuid(), dbContext: condition.dbContext, table: condition.table,
        field: condition.field, operateType: condition.operateType, value: condition.value
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
    for (var option of options) {
      if (option.childCondition.length == 0 || !option.combineType) {
        this.checkRuleResult = false;
      }
      this.checkRule(option.childRuleOption);
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
    for (var option of options) {
      for (var condition of option.childCondition) {
        if (!condition.dbContext || !condition.table || !condition.field || !condition.operateType || !condition.value) {
          this.checkConditionResult = false;
        }
      }
      this.checkCondition(option.childRuleOption);
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
    option.forEach(o => {

      if (o.id == id) {
        this.findedRule = o;
      } else {
        if (o.childRuleOption) {
          this.findRule(o.childRuleOption, id);
        }
      }
    });
  }

  // 根据id查找规则对应的节点
  private findRuleById(id) {
    this.findRule(this.ruleOptions, id);
    return this.findedRule;
  }

}