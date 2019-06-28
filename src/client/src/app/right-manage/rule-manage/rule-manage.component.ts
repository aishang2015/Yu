import { Component, OnInit } from '@angular/core';
import { RuleGroup } from '../models/rule-group';
import { Guid } from 'src/app/core/utils/guid';
import { RuleService } from 'src/app/core/services/rule.service';
import { Rule } from '../models/rule';
import { Condition } from '../models/condition';
import { NzMessageService } from 'ng-zorro-antd';

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
    //   combineType
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
  ruleGroup: RuleGroup;

  constructor(private _ruleService: RuleService, private _messageService: NzMessageService) { }

  ngOnInit(): void {
  }

  // 保存规则组
  saveRule() {

    if (!this.isRuleValid()) {
      this._messageService.error("请确认是否所有规则都包含有条件！");
      return;
    }

    if (!this.isConditionValid()) {
      this._messageService.error("请确认是否所有条件的完整！");
      return;
    }

    let rs: Rule[] = [];
    let rConditions: Condition[] = [];
    let rGroup: RuleGroup;

    Object.assign(rGroup, this.ruleGroup);

    for (var option of this.ruleOptions) {
      rs.push({ id: option.id, upRuleId: '', ruleGroupId: this.ruleGroup.id, combineType: option.combineType });
      this.makeRule(option.childRuleOption, option.id, rs);
    }

  }

  // 生成结果
  private makeRule(option, id, rs) {
    for (var option of this.ruleOptions) {
      rs.push({ id: option.id, upRuleId: id, ruleGroupId: this.ruleGroup.id, combineType: option.combineType });
      this.makeRule(option.childRuleOption, option.id, rs);
    }
  }

  // 添加规则组
  addRuleGroup() {
    this.ruleOptions = [];
    this.ruleGroup = { id: Guid.newGuid(), name: '' };
  }

  // 编辑规则组
  edit(group) {
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

      ruleOption.push({
        id: Guid.newGuid(), dbContext: condition.dbContext, table: condition.table,
        field: condition.field, operateType: condition.operateType, value: condition.value
      });
    }

  }

  // 检查是否所有规则包含条件
  private checkRuleResult: boolean = true;
  private checkRule(options) {
    for (var option of options) {
      if (option.childCondition.length == 0) {
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
      this.checkRule(option.childRuleOption);
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