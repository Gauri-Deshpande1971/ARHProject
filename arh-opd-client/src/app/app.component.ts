import { Component } from '@angular/core';
import { Observable } from 'rxjs';
import { IUser } from './components/shared/models/models';
import { AccountService } from './components/account/account.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
 export class AppComponent {
   title = 'digi-lib-client';

currentUser$: Observable<IUser|null>;
  
  user:any;
  isLoggedIn = false;
 // user_id?:string='';
  constructor(private accountService: AccountService, private router: Router) {
    this.currentUser$ = this.accountService.currentUser$;
   
  }
  
  ngOnInit(): void {    
    this.accountService.currentUser$.subscribe((user: IUser | null) => {
      if (!user) {
        this.router.navigate(['/account/login']); // <<< Redirect if no user
      }
       else
         if (this.router.url !== '/account/login') 
          this.router.navigate(['/dashboard']); 
    });
    const currentUser = sessionStorage.getItem('currentUser');
    if (currentUser) {
    this.user = JSON.parse(currentUser);  
     this.loadCurrentUser(); 
    }
   
  }

  loadCurrentUser() {   
    this.currentUser$ = this.accountService.currentUser$;
    
    const user_id=this.user.user.userid;
    const roleid=this.user.user.roleid;
    sessionStorage.setItem('roleid',roleid);
    const token = localStorage.getItem('token');
    
    if (token) {
      if(user_id)
      this.accountService.loadCurrentUser(user_id,token).subscribe(
        (user) => {
          this.isLoggedIn = true;
          console.log('User loaded successfully', user);
        },
        (error) => {
          console.error('Error occurred:', error.message);
          this.isLoggedIn = false;
          // Redirect to login page in case of error (or if no token)
          this.router.navigateByUrl('/account/login');
        }
      );
    } else {
      this.isLoggedIn = false;
      console.log('No token found, user is not logged in.');
      // Optionally, redirect to login page if token is missing
      this.router.navigateByUrl('/account/login');
    }
  }
  logout() {
    this.accountService.logout();
  }

}
