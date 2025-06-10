import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { StreamComponent } from './stream/stream.component';
import { CategoryComponent } from './category/category.component';
import { BookComponent } from './book/book.component';
import { ClassesComponent } from './classes/classes.component';
import { SubjectsComponent } from './subjects/subjects.component';
import { ContenttypesComponent } from './contenttypes/contenttypes.component';
import { ContentComponent } from './content/content.component';

const routes: Routes = [{path:'stream',component:StreamComponent},
  {path:'category',component:CategoryComponent},
  {path:'book',component:BookComponent},
  { path: 'book/:id', component: BookComponent },
  {path:'classes',component:ClassesComponent},
  {path:'subjects',component:SubjectsComponent},
  {path:'contenttypes',component:ContenttypesComponent},
  {path:'content',component:ContentComponent}
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class MastersRoutingModule { }
