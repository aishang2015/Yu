import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CoreModule } from '../core/core.module';
import { FlowComponent } from './flow/flow.component';
import { RouterModule } from '@angular/router';
import { WfdefinitionComponent } from './wfdefinition/wfdefinition.component';


@NgModule({
  declarations: [
    FlowComponent,
    WfdefinitionComponent
  ],
  imports: [
    CommonModule,
    CoreModule,
    RouterModule.forChild([
      { path: 'definition', component: WfdefinitionComponent },
      { path: 'flow/:id', component: FlowComponent },
    ])
  ]
})
export class WorkflowModule { }
