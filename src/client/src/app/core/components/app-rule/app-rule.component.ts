import { Component, OnInit, Input, Output } from '@angular/core';
import { EntityService } from '../../services/entity.service';
import { EnumConstant } from '../../constants/enum-constant';
import { Guid } from '../../utils/guid';

@Component({
  selector: 'app-rule',
  templateUrl: './app-rule.component.html',
  styleUrls: ['./app-rule.component.scss']
})
export class AppRuleComponent implements OnInit {

  @Input() @Output() option;

  constructor(private entityService: EntityService) { }

  // 实体数据
  entityData = [];

  // 数据库下拉框数据
  dbContextData = null;

  // 表下拉框数据
  tableData = null;

  // 字段下拉框数据
  fieldData = null;

  // 操作类型
  operateData = EnumConstant.operateTypes;

  // 组合类型
  combineData = EnumConstant.combineTypes;

  ngOnInit() {
    this.entityService.getDropDownEntityData().subscribe(
      result => {
        this.entityData = result;
      }
    );
  }

  // 初始化db选择数据
  initSelectedData(condition) {
    condition.dbContextData = this.unique(this.entityData.map(entity => entity.dbContext));
  }

  // 初始化表选择数据
  dbContextSelectedDataChange(condition) {
    condition.tableData = this.unique(this.entityData.filter(entity => entity.dbContext == condition.dbContext).map(entity => entity.table));
  }

  // 初始化字段选择数据
  tableSelectDataChange(condition) {
    condition.fieldData = this.unique(this.entityData.filter(entity => entity.dbContext == condition.dbContext && entity.table == condition.table)
      .map(entity => entity.field));
  }

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
    this.deleteGroup(this.option, id);
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

  // 删除条件
  deleteCondition(option, id) {

    for (var o of option) {
      const index = o.childCondition.findIndex(condition => condition.id == id);
      console.log(index);
      if (index == -1) {
        if (o.childRuleOption) {
          this.deleteCondition(o.childRuleOption, id);
        }
      } else {
        o.childCondition.splice(index, 1);
        break;
      }
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
