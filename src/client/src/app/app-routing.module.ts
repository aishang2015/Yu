import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { LoginGuard } from './core/services/login-guard.service';

const routes: Routes = [
  { path: 'dashboard', loadChildren: './dashboard/dashboard.module#DashboardModule', canActivate: [LoginGuard] },
  { path: 'login', loadChildren: './account/account.module#AccountModule', canActivate: [LoginGuard] },
  { path: 'right', loadChildren: './right-manage/right-manage.module#RightManageModule', canActivate: [LoginGuard] },
  { path: '', pathMatch: 'full', redirectTo: 'login' },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
