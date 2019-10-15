import { Component, OnInit, Input, NgModule, Compiler, ViewChild, ViewContainerRef } from '@angular/core';
import { WorkFlowDefineService } from 'src/app/core/services/workflow/workflowdefine.service';
import { WorkFlowFormService } from 'src/app/core/services/workflow/workflowform.service';
import { WorkFlowFormElement } from '../../models/workflowFormElement';
import { NgZorroAntdModule } from 'ng-zorro-antd';

@Component({
  selector: 'app-job-edit',
  templateUrl: './job-edit.component.html',
  styleUrls: ['./job-edit.component.scss', '../../../../assets/workflow/style.css']
})
export class JobEditComponent implements OnInit {

  // 工作流定义id
  @Input()
  wfDefineId;

  @ViewChild('htmlContainer', { static: true, read: ViewContainerRef })
  htmlContainer;

  // 数据
  dataModel = '';

  // 表单元素
  workflowFormElements: WorkFlowFormElement[] = [];

  constructor(
    private _compiler: Compiler,
    private _workflowDefineService: WorkFlowDefineService,
    private _workflowFormService: WorkFlowFormService) { }

  ngOnInit() {
    this._workflowFormService.get(this.wfDefineId).subscribe(result => {
      this.dataModel = result.formContent ? result.formContent.editFormHtml : '';
      this.workflowFormElements = result.formElements;
      this.htmlEdit();
    });
  }

  // 编辑html
  htmlEdit() {
    for (let i = 0; i < this.workflowFormElements.length; i++) {
      let element: any = this.workflowFormElements[i];

      // 正则替换真正页面的标签
      let newhtml = this.getEditHtml(element.elementId, element.type, element.width);
      let regex = new RegExp(`<input id="${element.elementId}".*?/>`);
      this.dataModel = this.dataModel.replace(regex, newhtml);
      this.createComponent(this.dataModel);
    }
  }

  // 替换html的内容
  getEditHtml(id, key, width) {
    let style = width ? `style="width:${width}px;"` : '';
    let html = '';
    switch (key) {
      case 'datepicker':
        html = `<nz-date-picker id="${id}" ${style}></nz-date-picker>`;
        break;
      case 'timepicker':
        html = `<nz-time-picker id="${id}" ${style}></nz-time-picker>`;
        break;
      case 'inputnumber':
        html = `<nz-input-number id="${id}" ${style}></nz-input-number>`;
        break;
      case 'input':
        html = `<input id="${id}" nz-input ${style}/>`;
        break;
      case 'textarea':
        html = `<textarea id="${id}" nz-input ${style}></textarea>`;
        break;
      case 'selecter':
        html = `<nz-select id="${id}" ${style}></nz-select>`;
        break;
      case 'upload':
        html = `
                  <button nz-button><i nz-icon nzType="upload"></i><span>点击上传文件</span></button>
                `;
        break;
    }
    return html;
  }

  // 创建动态组件
  createComponent(template) {

    @Component({ template })
    class TemplateComponent { }

    @NgModule({ declarations: [TemplateComponent], imports: [NgZorroAntdModule] })
    class TemplateModule { }

    const mod = this._compiler.compileModuleAndAllComponentsSync(TemplateModule);
    const factory = mod.componentFactories.find((comp) =>
      comp.componentType === TemplateComponent
    );

    this.htmlContainer.clear();
    this.htmlContainer.createComponent(factory);
  }
}
