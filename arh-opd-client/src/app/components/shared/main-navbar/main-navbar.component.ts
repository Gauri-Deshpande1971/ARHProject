import { Component } from '@angular/core';
import {MainLayoutComponent} from '../main-layout/main-layout.component'
import { AccountService } from '../../account/account.service';
import { Observable } from 'rxjs';
import { IUser } from '../models/models';
@Component({
  selector: 'app-main-navbar',
  templateUrl: './main-navbar.component.html',
  styleUrls: ['./main-navbar.component.css']
})
export class MainNavbarComponent {
  user:any;
  username:any;
  currentUser$: Observable<IUser|null>;
  constructor(public app: MainLayoutComponent,private accountService:AccountService) {
     this.currentUser$ = this.accountService.currentUser$;
  }
  ngOnInit(): void {    
         const currentUser = sessionStorage.getItem('currentUser');
    if (currentUser) {
    this.user = JSON.parse(currentUser); 
    this.username=this.user.user.userName;
    }
  }
  logout() {
    this.accountService.logout();
  }
}
