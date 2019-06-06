import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CoreModule } from '../core/core.module';
import { Router, RouterModule } from '@angular/router';
import { UserManageComponent } from './user-manage/user-manage.component';
import { RoleManageComponent } from './role-manage/role-manage.component';
import { LoginGuard } from '../core/services/login-guard.service';
import { MenuManageComponent } from './menu-manage/menu-manage.component';
import { GroupManageComponent } from './group-manage/group-manage.component';
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { AuthHeaderInterceptor } from '../core/Interceptors/AuthHeaderInterceptor';

@NgModule({
  declarations: [
    UserManageComponent,
    RoleManageComponent,
    MenuManageComponent,
    GroupManageComponent,
  ],
  imports: [
    CommonModule,
    CoreModule,
    RouterModule.forChild([
      { path: 'user', component: UserManageComponent, canActivate: [LoginGuard] },
      { path: 'role', component: RoleManageComponent, canActivate: [LoginGuard] },
      { path: 'menu', component: MenuManageComponent, canActivate: [LoginGuard] },
      { path: 'group', component: GroupManageComponent, canActivate: [LoginGuard] }
    ])
  ]
})
export class RightManageModule { }
