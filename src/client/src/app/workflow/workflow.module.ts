import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CoreModule } from '../core/core.module';
import { FlowComponent } from './flow/flow.component';
import { RouterModule } from '@angular/router';
import { WfdefinitionComponent } from './wfdefinition/wfdefinition.component';
import { WfformComponent } from './wfform/wfform.component';
import { EditorModule } from '@tinymce/tinymce-angular';


@NgModule({
  declarations: [
    FlowComponent,
    WfdefinitionComponent,
    WfformComponent,
  ],
  imports: [
    CommonModule,
    CoreModule,
    EditorModule,
    RouterModule.forChild([
      { path: 'definition', component: WfdefinitionComponent },
      { path: 'flow/:id', component: FlowComponent },
      { path: 'wfform', component: WfformComponent },
    ])
  ],
})
export class WorkflowModule { }
