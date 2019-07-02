import { Component, OnInit, Input, Output } from '@angular/core';
import { EntityService } from '../../services/entity.service';
import { EnumConstant } from '../../constants/enum-constant';
import { Guid } from '../../utils/guid';
import { Condition } from 'selenium-webdriver';

@Component({
  selector: 'app-rule',
  templateUrl: './app-rule.component.html',
  styleUrls: ['./app-rule.component.scss']
})
export class AppRuleComponent implements OnInit {

  @Input() @Output() option;

  // 字段下拉框数据
  @Input() fieldData;

  
  @Input()  upperOption;

  constructor(private entityService: EntityService) { }

  // 操作类型
  operateData = EnumConstant.operateTypes;

  // 组合类型
  combineData = EnumConstant.combineTypes;

  ngOnInit() {
    // this.entityService.getDropDownEntityData().subscribe(
    //   result => {
    //     this.entityData = result;
    //   }
    // );
  }

  // getDbContext(condition) {
  //   return this.unique(this.entityData.map(entity => entity.dbContext));
  // }

  // getTableData(condition) {
  //   return this.unique(this.entityData.filter(entity => entity.dbContext == condition.dbContext).map(entity => entity.table));
  // }

  // getFieldData(condition) {
  //   return this.unique(this.entityData.filter(entity => entity.dbContext == condition.dbContext && entity.table == condition.table)
  //     .map(entity => entity.field));
  // }

  // 添加条件
  addCondition(id) {
    this.findRule(this.option, id);
    this.findedRule.childCondition.push({ id: Guid.newGuid(), dbContext: '', table: '', field: '' });
  }

  // 删除条件
  removeCondition(id) {
    this.deleteCondition(this.option, id);
  }

  // 删除组
  removeGroup(id) {
    this.deleteGroup(this.upperOption.childRuleOption, id);
  }

  // 添加条件
  addGroup(option) {
    option.push(
      {
        id: Guid.newGuid(),
        childCondition: [
        ],
        childRuleOption: [
        ]
      }
    );
  }

  // 查找规则
  findedRule;
  findRule(option, id) {
    if (option.id == id) {
      this.findedRule = option;
    } else {
      for (var o of this.option.childRuleOption) {
        this.findRule(o, id);
      }
    }
  }

  // 删除条件
  deleteCondition(option, id) {

    const index = option.childCondition.findIndex(condition => condition.id == id);
    if (index == -1) {
      if (option.childRuleOption) {
        this.deleteCondition(option.childRuleOption, id);
      }
    } else {
      option.childCondition.splice(index, 1);
    }

  }

  // 删除条件组
  deleteGroup(option, id) {
    const index = option.findIndex(o => o.id == id);

    if (index == -1) {
      for (var o of option) {
        if (o.childRuleOption) {
          this.deleteGroup(o.childRuleOption, id);
        }
      }
    } else {
      option.splice(index, 1);
    }
  }

  unique(arr) {
    return Array.from(new Set(arr))
  }

}
