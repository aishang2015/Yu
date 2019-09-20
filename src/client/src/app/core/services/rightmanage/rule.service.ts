import { Injectable, Injector } from '@angular/core';
import { BaseService } from '../base.service';
import { HttpClient } from '@angular/common/http';
import { UriConstant } from '../../constants/uri-constant';

@Injectable({
  providedIn: 'root'
})
export class RuleService extends BaseService {

  // 构造函数
  constructor(private injector: Injector,
    private http: HttpClient) { super(injector); }


  // 查看所有规则组
  getAllRuleGroup() {
    let uri = UriConstant.RuleGroupUri;
    return this.SafeRequest(this.http.get(uri));
  }

  // 查看规则组内容
  getRuleDetail(id) {
    let uri = UriConstant.RuleDetailUri + `?id=${id}`;
    return this.SafeRequest(this.http.get(uri));
  }

  // 删除规则组
  deleteRuleGroup(id) {
    let uri = UriConstant.RuleGroupUri + `?id=${id}`;
    return this.SafeRequest(this.http.delete(uri));
  }

  // 添加修改规则组
  addOrModifyRule(rules, ruleConditions, ruleGroup) {
    let uri = UriConstant.RuleDetailUri;
    return this.SafeRequest(this.http.put(uri, { rules: rules, ruleConditions: ruleConditions, ruleGroup: ruleGroup }));
  }
}
