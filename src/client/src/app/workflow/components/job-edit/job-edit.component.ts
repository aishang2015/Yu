import { Component, OnInit, Input, NgModule, Compiler, ViewChild, ViewContainerRef } from '@angular/core';
import { WorkFlowDefineService } from 'src/app/core/services/workflow/workflowdefine.service';
import { WorkFlowFormService } from 'src/app/core/services/workflow/workflowform.service';
import { WorkFlowFormElement } from '../../models/workflowFormElement';
import { NgZorroAntdModule } from 'ng-zorro-antd';
import { WorkFlowInstanceService } from 'src/app/core/services/workflow/workflowinstance.service';
import { FormGroup, ReactiveFormsModule, FormBuilder, FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { WorkflowFlowNodeElement } from '../../models/workflowFlowNodeElement';

@Component({
  selector: 'app-job-edit',
  templateUrl: './job-edit.component.html',
  styleUrls: ['./job-edit.component.scss', '../../../../assets/workflow/style.css']
})
export class JobEditComponent implements OnInit {

  // 工作流定义id
  @Input()
  wfDefineId;

  // 工作流实例Id
  @Input()
  wfInstanceId;

  // html容器
  @ViewChild('htmlContainer', { static: true, read: ViewContainerRef })
  htmlContainer;

  // html数据
  dataModel = '';

  // 表单元素
  workflowFormElements: WorkFlowFormElement[] = [];

  // 工作流节点表单元素状态
  workflowFlowNodeElements: WorkflowFlowNodeElement[] = [];

  // 动态组件对象
  dynamicComponentRef;

  // 值
  values: { [code: string]: object } = {};

  constructor(
    private fb: FormBuilder,
    private _compiler: Compiler,
    private _workflowDefineService: WorkFlowDefineService,
    private _workflowFormService: WorkFlowFormService,
    private _workflowInstanceService: WorkFlowInstanceService) { }

  ngOnInit() {
    this._workflowFormService.get(this.wfDefineId).subscribe(result => {
      this.dataModel = result.formContent ? result.formContent.editFormHtml : '';
      this.workflowFormElements = result.formElements;
      //let group: { [code: string]: [object] } = {};
      //this.workflowFormElements.forEach(wffe => group[wffe.id] = [null]);
      //this.editForm = this.fb.group(group);
      this._workflowInstanceService.getForm(this.wfInstanceId).subscribe(result => {
        this.workflowFormElements.forEach(wffe => {
          let form = result.find(f => f.elementId == wffe.id);
          if (wffe.type == 'timepicker' || wffe.type == 'datepicker') {
            this.values[wffe.elementId] = (form && form.value != '') ? new Date(form.value) : new Date();
          } else {
            this.values[wffe.elementId] = form ? form.value : null;
          }
        });
      });

      this._workflowInstanceService.getFlowNodeElement(this.wfInstanceId).subscribe(
        result => {
          this.workflowFlowNodeElements = result;
          this.htmlEdit();
        }
      );
    });
  }

  // 编辑html
  htmlEdit() {
    for (let i = 0; i < this.workflowFormElements.length; i++) {
      let element: any = this.workflowFormElements[i];

      let nodeElement = this.workflowFlowNodeElements.find(wffne => wffne.formElementId == element.id);

      // 正则替换真正页面的标签
      let newhtml = this.getEditHtml(element.elementId, element.type, element.width, element.line, element.options, nodeElement ? nodeElement.isEditable : true, nodeElement ? nodeElement.isVisible : true);
      //newhtml = '<form nz-form [formGroup]="editForm">'+ newhtml + '</form>';
      let regex = new RegExp(`<input id="${element.elementId}".*?/>`);
      this.dataModel = this.dataModel.replace(regex, newhtml);
    }
    this.createComponent(this.dataModel);
  }

  // 替换html的内容
  getEditHtml(id, key, width, rows, ops, editable, visible) {
    let style = width ? `style="width:${width}px;` : '';
    style = style.concat(visible ? '"' : 'display: none;"');
    let row = `rows="${rows ? rows : 1}"`;
    let options = '';
    ops.split(',').forEach(str => {
      options += ` <nz-option nzValue="${str}" nzLabel="${str}"></nz-option>`;
    })
    let html = '';
    switch (key) {
      case 'datepicker':
        html = `<nz-date-picker [(ngModel)]="values['${id}']" [disabled]="${!editable}" id="${id}" ${style}></nz-date-picker>`;
        break;
      case 'timepicker':
        html = `<nz-time-picker [(ngModel)]="values['${id}']" [disabled]="${!editable}"  id="${id}" ${style}></nz-time-picker>`;
        break;
      case 'inputnumber':
        html = `<nz-input-number [(ngModel)]="values['${id}']" [disabled]="${!editable}"  id="${id}" ${style}></nz-input-number>`;
        break;
      case 'input':
        html = `<input id="${id}" [(ngModel)]="values['${id}']" [disabled]="${!editable}"  nz-input ${style}/>`;
        break;
      case 'textarea':
        html = `<textarea [(ngModel)]="values['${id}']" [disabled]="${!editable}"  id="${id}" ${row} nz-input ${style}></textarea>`;
        break;
      case 'selecter':
        html = `<nz-select [(ngModel)]="values['${id}']" [disabled]="${!editable}"  id="${id}" ${style}>${options}</nz-select>`;
        break;
      case 'upload':
        html = `<button nz-button><i nz-icon nzType="upload"></i><span>点击上传文件</span></button>`;
        break;
    }
    return html;
  }

  // 创建动态组件
  createComponent(template) {

    @Component({ template })
    class TemplateComponent { values: { [code: string]: string } = {}; }

    @NgModule({ declarations: [TemplateComponent], imports: [NgZorroAntdModule, FormsModule] })
    class TemplateModule { }

    const mod = this._compiler.compileModuleAndAllComponentsSync(TemplateModule);
    const factory = mod.componentFactories.find((comp) =>
      comp.componentType === TemplateComponent
    );

    this.htmlContainer.clear();
    let componentRef = this.htmlContainer.createComponent(factory);
    this.dynamicComponentRef = <TemplateComponent>componentRef;

    // 初始化动态组件的表单数据
    this.dynamicComponentRef.instance.values = this.values;
  }

  // 保存内容
  saveContent() {
    let values = this.dynamicComponentRef.instance.values;
    let forms = [];
    for (const element of this.workflowFormElements) {
      let value = values[element.elementId];
      forms.push({
        elementID: element.id,
        value: value
      });
    }
    return this._workflowInstanceService.putForm({
      instanceId: this.wfInstanceId,
      workFlowInstanceForms: forms
    });
  }
}
