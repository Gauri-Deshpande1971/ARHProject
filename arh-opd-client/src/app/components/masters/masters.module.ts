import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedModule } from '../shared/shared.module';
import { MastersRoutingModule } from './masters-routing.module';
import { StreamComponent } from './stream/stream.component';
import { FormsModule,ReactiveFormsModule } from '@angular/forms';
import { CategoryComponent } from './category/category.component';
import { BookComponent } from './book/book.component';
import { ClassesComponent } from './classes/classes.component';
import { SubjectsComponent } from './subjects/subjects.component';
import { ContenttypesComponent } from './contenttypes/contenttypes.component';
import {ContentComponent} from './content/content.component'
@NgModule({
  declarations: [StreamComponent,ContentComponent, CategoryComponent, BookComponent, ClassesComponent, SubjectsComponent,ContenttypesComponent],
  imports: [
    CommonModule,    
    MastersRoutingModule,
    SharedModule,
    FormsModule,
    ReactiveFormsModule
  ]
})
export class MastersModule { }
