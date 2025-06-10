import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NgbModule} from  '@ng-bootstrap/ng-bootstrap';
import { RouterModule } from '@angular/router';
import { AgGridModule } from 'ag-grid-angular';
import { NgSelectModule } from '@ng-select/ng-select';
import { FormGridComponent } from '../shared/form-grid/form-grid.component'
import { MainControlSidebarComponent } from '../shared/main-control-sidebar/main-control-sidebar.component';
import { MainFooterComponent } from '../shared/main-footer/main-footer.component'
import { MainLayoutComponent } from '../shared/main-layout/main-layout.component'
import { MainNavbarComponent } from '../shared/main-navbar/main-navbar.component';
import { MainSidebarComponent } from '../shared/main-sidebar/main-sidebar.component';
import { TextInputComponent } from './text-input/text-input.component';
@NgModule({
  declarations: [
   // BreadcrumbComponent, 
   FormGridComponent, TextInputComponent,
   MainControlSidebarComponent,
   MainFooterComponent,MainLayoutComponent,MainNavbarComponent,MainSidebarComponent
  ],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterModule,
    NgbModule,
    FormsModule,
       // PerfectScrollbarModule,
    NgSelectModule,    
    AgGridModule,
    // AutocompleteComponent,TextInputComponent,  TextInputNormalComponent, FormGridComponent,
    // OptionButtonsInputComponent,VerticalNavigationComponent,
    // VerticalSidebarComponent,
    // HorizontalSidebarComponent,
    // BreadcrumbComponent
  ],  
  exports: [
   TextInputComponent,    
    //  BsDropdownModule,
 //   DropdownInputComponent,
 //  TextInputNormalComponent,
 //   OptionButtonsInputComponent,
       FormGridComponent,MainControlSidebarComponent,MainFooterComponent,
    NgbModule,MainFooterComponent,MainLayoutComponent,MainNavbarComponent,MainSidebarComponent
     // PerfectScrollbarModule,
    // VerticalNavigationComponent,     
  //   HorizontalSidebarComponent,
   //  BreadcrumbComponent,
     
  ]
})
export class SharedModule { }
