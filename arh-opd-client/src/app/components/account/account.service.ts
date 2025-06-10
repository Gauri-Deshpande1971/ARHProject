import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { BehaviorSubject, Observable, of, ReplaySubject, throwError } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { environment } from '../../../environments/environment';
import { IUser, IUserToken } from '../shared/models/models';

@Injectable({
  providedIn: 'root'
})
export class AccountService {
  baseUrl = environment.apiUrl;
    private currentUserSource = new BehaviorSubject<IUser | null>(null);
 // private currentUserSource = new ReplaySubject<IUser>(1);
  currentUser$ = this.currentUserSource.asObservable();

  constructor(private http: HttpClient, private router: Router) { }

  // getCurrentUserValue() {
  //   this.currentUserSource.value;
  // }

  loadCurrentUser(user_id:string,token: string| null): Observable<IUser | null>  {
    if (token === null) {
      this.currentUserSource.next(null);
      return throwError(() => new Error('Token is missing'));
    }

    let headers = new HttpHeaders();
    headers = headers.set('Authorization', `Bearer ${token}`);     
     return this.http.get<IUser>(`${this.baseUrl}User/GetByUserId?user_id=${user_id}`, { headers }).pipe(
      map((user: IUser) => {
        if (user) {
          // If user is found, store the token and update currentUserSource
          localStorage.setItem('token', user.token);
          this.currentUserSource.next(user);
          return user; // Make sure to return `user` which is of type `IUser`
        } else {
          console.warn('No user returned from API.');
          this.currentUserSource.next(null); // In case no user is found
          return null; // Return `null` if no user
        }
      }),
      catchError((error) => {
        console.error('Error occurred while fetching user data', error);
        this.currentUserSource.next(null); // Ensure the current user is cleared on error
        return throwError(() => new Error('Failed to load user.'));
      })
    );
  }

  login(values: any) {     
    return this.http.post<IUser>(this.baseUrl + 'User/Login', values).pipe(
      map((user: IUser) => {
        if (user) {          
           console.log(user);
           localStorage.setItem('token', user.token);
           sessionStorage.setItem('currentUser', JSON.stringify(user));
           this.currentUserSource.next(user);
          return user;
        }
        else {
          return null;
        }
      })
    );
  }
  
  continuelogin(values: any) {
    //  return this.http.post(this.baseUrl + 'account/continuelogin', values).pipe(
    return this.http.post<IUser>(this.baseUrl + 'User/Login', values).pipe(
        map((user: IUser) => {
        if (user) {
          if (user.status === "Success") {
            localStorage.setItem('token', user.token);
            this.currentUserSource.next(user);          
          return user; 
        }
        else
         return null;
      }
        else {
          return null;
        }
      }
    ));
  
  }

  SetNewPassword(values: any) {
    return this.http.post<IUser>(this.baseUrl + 'account/setnewpassword', values).pipe(
      map((user: IUser) => {
        if (user) {
            localStorage.setItem('token', user.token);
            this.currentUserSource.next(user);
            return user;
        }
        else {
          return null;
        }
      })
    );
  }

  register(values: any) {
    return this.http.post(this.baseUrl + 'account/register', values);

    // return this.http.post(this.baseUrl + 'account/register', values).pipe(
    //   map((user: IUser) =>{
    //     if (user) {
    //       localStorage.setItem('token', user.token);
    //       this.currentUserSource.next(user);
    //     }
    //   })
    // );
  }

  logout() {
    this.http.post(this.baseUrl + 'User/logout', null)
    .subscribe(x => {
      localStorage.removeItem('token');
      this.currentUserSource.next(null);
  
      this.router.navigateByUrl('/');
  
    });
  }

  // checkEmailExists(email: string) {
  //   return this.http.get(this.baseUrl + 'account/emailexists?email=' + email);
  // }

  public ChangePassword(data: any) {
    //  return this.http.post(this.baseUrl + 'account/changepassword',data);

    return this.http.post<IUser>(this.baseUrl + 'account/changepassword', data).pipe(
      map((user: IUser) => {
      if (user) {
        if (user.status === "Success") {
          localStorage.removeItem('token');
          localStorage.setItem('token', user.token);
          this.currentUserSource.next(user);
        }
        return user; 
      }
      else {
        return null;
      }
    })
  );
  }

}
