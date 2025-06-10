import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { MainLayoutComponent } from './components/shared/main-layout/main-layout.component';
import { DashboardComponent } from './components/views/dashboard/dashboard.component';
import { AuthGuard } from './components/core/guards/auth.guard';
import { AppComponent } from './app.component';

export const routes: Routes = [
   { path: '', component: AppComponent }, 
   {
    path: '',
    redirectTo: 'account/login',
    pathMatch: 'full',
  },

  // Lazy load account module (which includes LoginComponent)
  {
    path: 'account',
    loadChildren: () =>
      import('./components/account/account.module').then(
        (mod) => mod.AccountModule
      ),
    data: { breadcrumb: { skip: true } },
  },

  //
   {
    path: '',
    redirectTo: 'dashboard', // Redirect to dashboard when empty route is accessed
    pathMatch: 'full',
  },
  {
    path: '',
    component: MainLayoutComponent,
    canActivate: [AuthGuard],
    children: [
      {
        path: 'dashboard',
        component: DashboardComponent,
      },
      //  { path: 'account',
      //   loadChildren: () => import('./components/account/account.module').then(mod=>mod.AccountModule),
      //  data: {breadcrumb: {skip: true}}},
       { path: 'masters',
        loadChildren: () => import('./components/masters/masters.module').then(mod => mod.MastersModule), 
        data: {breadcrumb: {skip: true}}},
 
      // ,
      // {
      //   path: 'Stock/VieworStock',
      //   component: ViewstockComponent,
      //   canActivate: [AuthGuard],
      // }
      // ,
      // {
      //   path: 'Stock/AddandUpdateStock',
      //   component: StockentryComponent,
      //   canActivate: [AuthGuard],
      // },
      // {
      //   path: 'order/OrderEntry',
      //   component: OrderentryComponent,
      //   canActivate: [AuthGuard],
      // }
      ]
    }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
