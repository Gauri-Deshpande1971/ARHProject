import { HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { NavigationExtras, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { Observable, throwError } from 'rxjs';
import { catchError, delay } from 'rxjs/operators';
import { AccountService } from 'src/app/account/account.service';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {

    constructor(private router: Router, private toastr: ToastrService, 
            private accountService: AccountService) {}

    intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        return next.handle(req).pipe(
            catchError(error => {
                if (error) {
                    if (error.status === 400) {
                        if (error.error.errors) {
                            throw error.error;
                        } else {
                          //  this.toastr.error(error.error.message, error.error.statusCode);
                        }
                    }
                    if (error.status === 401) {
                        if (error.error.message !== null && error.error.message !== '')
                            this.toastr.error(error.error.message, error.error.statusCode);
                        //  this.router.navigateByUrl('/account/login');
                        this.logout();
                    }

                    if (error.status === 404) {
                        this.router.navigateByUrl('/not-found');
                    }
                    if (error.status === 500) {
                        const navigationExtras: NavigationExtras = { state: {error: error.error}};
                        this.router.navigateByUrl('/server-error', navigationExtras);
                    }
                }

                return throwError(error);
            })
        );
    }

    logout() {
        this.accountService.logout();
      }
    
}
