import { Component, OnInit } from '@angular/core';
import { WorkFlowTypeService } from 'src/app/core/services/workflow/workflowtype.service';
import { WorkFlowDefineService } from 'src/app/core/services/workflow/workflowdefine.service';
import { WorkFlowDefine } from '../../models/workflowDefine';
import { WorkflowType } from '../../models/workflowType';

@Component({
  selector: 'app-workflow-selected',
  templateUrl: './workflow-selected.component.html',
  styleUrls: ['./workflow-selected.component.scss']
})
export class WorkflowSelectedComponent implements OnInit {

  // 选中
  checked = new WorkFlowDefine();

  // 工作流定义
  workflowDefines: WorkFlowDefine[] = [];

  // 工作流类型
  workflowTypes: WorkflowType[] = [];

  constructor(private _workflowTypeService: WorkFlowTypeService,
    private _workflowDefineService: WorkFlowDefineService) { }

  ngOnInit() {
    this._workflowDefineService.getAll().subscribe(result => {
      this.workflowDefines = result;
    });
    this._workflowTypeService.get().subscribe(result => {
      this.workflowTypes = result;
    })
  }

  getWorkFlowDefines(type) {
    return this.workflowDefines.filter(wfd => wfd.typeId == type.id);
  }

  check(define) {
    this.checked = define;
  }





}
