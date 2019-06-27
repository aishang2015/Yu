import { Component, OnInit } from '@angular/core';
import { RuleGroup } from '../models/rule-group';
import { Guid } from 'src/app/core/utils/guid';

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
    //   ]
    // },
    // {
    //   id: Guid.newGuid(),
    //   childCondition: [
    //   ],
    //   childRuleOption: [
    //   ]
    // },
  ]

  ngOnInit(): void {
  }

  // 编辑规则组
  edit(data) { }

  // 保存规则组
  saveRule() {
    console.log(this.ruleOptions);
  }

  // 添加规则组
  addRuleGroup() {
    this.ruleOptions = [];
  }

}