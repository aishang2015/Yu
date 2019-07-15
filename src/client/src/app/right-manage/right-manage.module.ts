import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CoreModule } from '../core/core.module';
import { RouterModule } from '@angular/router';
import { UserManageComponent } from './user-manage/user-manage.component';
import { RoleManageComponent } from './role-manage/role-manage.component';
import { MenuManageComponent } from './menu-manage/menu-manage.component';
import { GroupManageComponent } from './group-manage/group-manage.component';
import { ApiManageComponent } from './api-manage/api-manage.component';
import { RuleManageComponent } from './rule-manage/rule-manage.component';
import { EntityManageComponent } from './entity-manage/entity-manage.component';
import { SubPathGuard } from '../core/services/sub-path-guard.service';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

@NgModule({
  declarations: [
    UserManageComponent,
    RoleManageComponent,
    MenuManageComponent,
    GroupManageComponent,
    ApiManageComponent,
    RuleManageComponent,
    EntityManageComponent,
  ],
  imports: [
    CommonModule,
    CoreModule,
    RouterModule.forChild([
      { path: 'user', component: UserManageComponent, canActivate: [SubPathGuard] },
      { path: 'role', component: RoleManageComponent, canActivate: [SubPathGuard] },
      { path: 'menu', component: MenuManageComponent, canActivate: [SubPathGuard] },
      { path: 'group', component: GroupManageComponent, canActivate: [SubPathGuard] },
      { path: 'api', component: ApiManageComponent, canActivate: [SubPathGuard] },
      { path: 'rule', component: RuleManageComponent, canActivate: [SubPathGuard] },
      { path: 'entity', component: EntityManageComponent, canActivate: [SubPathGuard] },
    ])
  ]
})
export class RightManageModule { }
