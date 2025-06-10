import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, UrlTree, Router } from '@angular/router';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { AccountService } from '../../account/account.service'

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {
  constructor(private accountService: AccountService, private router: Router) { }

  canActivate(
    next: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<boolean | UrlTree> {
    return this.accountService.currentUser$.pipe(
      map(auth => {debugger;
        if (auth) {
          // User is authenticated, allow access
          return true;
        } else {
          // User is not authenticated, redirect to login
          return this.router.createUrlTree(['account/login'], {
            queryParams: { returnUrl: state.url }
          });
        }
      })
    );
  }
}
