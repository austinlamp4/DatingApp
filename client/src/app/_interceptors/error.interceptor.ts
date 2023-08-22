import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor,
  HttpErrorResponse
} from '@angular/common/http';
import { Observable, catchError } from 'rxjs';
import { NavigationExtras, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {

  constructor(private router: Router, private toastr: ToastrService) {} //Inject router to redirect a user if we need, inject toastr in case we need to let the user know they did something

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    return next.handle(request).pipe(
      catchError((error: HttpErrorResponse) => {
        if (error) {
          switch (error.status) {
            case 400:
              if (error.error.errors) { //If validation error, we receive an error object with an 'errors' object inside of it
                const modelStateErrors = [];
                for (const key in error.error.errors) {
                  if (error.error.errors[key]) { //Building up an array of the errors we get from a validation error and storing it inside an array
                    modelStateErrors.push(error.error.errors[key])
                  }
                }
                throw modelStateErrors.flat();
              } else { //If a normal 400 error
                this.toastr.error(error.error, error.status.toString());
              }
              break;
            case 401:
              this.toastr.error('Unauthorized', error.status.toString());
              break;
            case 404:
              this.router.navigateByUrl('/not-found'); //Redirect the user to a not found page
              break;
            case 500:
              const navigationExtras: NavigationExtras = { state: { error: error.error } };
              this.router.navigateByUrl('/server-error', navigationExtras);
              break;
            default:
              this.toastr.error('Something unexpected occured!');
              console.log(error);
              break;
          }
        }
        throw error;
      })
    )
  }
}
