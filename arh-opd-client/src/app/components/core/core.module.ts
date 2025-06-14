import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NavBarComponent } from './nav-bar/nav-bar.component';
import { RouterModule } from '@angular/router';
import { TestErrorComponent } from './test-error/test-error.component';
import { NotFoundComponent } from './not-found/not-found.component';
import { ServerErrorComponent } from './server-error/server-error.component';
import { ToastrModule } from 'ngx-toastr';
//import { SectionHeaderComponent } from './section-header/section-header.component';
//import { BreadcrumbModule } from 'xng-breadcrumb';
import { SharedModule } from '../shared/shared.module';
import { DownloadApkComponent } from './download-apk/download-apk.component';


@NgModule({
   declarations: [
     NavBarComponent
  //   TestErrorComponent,
  //   NotFoundComponent,
  //   ServerErrorComponent,
  // //  SectionHeaderComponent,
  //   DownloadApkComponent
   ],
  imports: [
    CommonModule,
    RouterModule,
    //  BreadcrumbModule,
    SharedModule,
    ToastrModule.forRoot( {
      positionClass: 'toast-bottom-right',
      preventDuplicates: true
    })
  ],
  exports: [
   NavBarComponent
   // SectionHeaderComponent
  ]
})
export class CoreModule { }
