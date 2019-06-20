import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CoreModule } from '../core/core.module';
import { RouterModule } from '@angular/router';
import { UserManageComponent } from './user-manage/user-manage.component';
import { RoleManageComponent } from './role-manage/role-manage.component';
import { MenuManageComponent } from './menu-manage/menu-manage.component';
import { GroupManageComponent } from './group-manage/group-manage.component';
import { ApiManageComponent } from './api-manage/api-manage.component';

@NgModule({
  declarations: [
    UserManageComponent,
    RoleManageComponent,
    MenuManageComponent,
    GroupManageComponent,
    ApiManageComponent
  ],
  imports: [
    CommonModule,
    CoreModule,
    RouterModule.forChild([
      { path: 'user', component: UserManageComponent },
      { path: 'role', component: RoleManageComponent },
      { path: 'menu', component: MenuManageComponent },
      { path: 'group', component: GroupManageComponent },
      { path: 'api', component: ApiManageComponent }
    ])
  ]
})
export class RightManageModule { }
