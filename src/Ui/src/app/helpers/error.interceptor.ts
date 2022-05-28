import {Injectable} from '@angular/core';
import {HttpRequest, HttpHandler, HttpEvent, HttpInterceptor} from '@angular/common/http';
import {catchError, Observable, throwError} from 'rxjs';
import {LoginService} from '../services/login.service';
import {Router} from "@angular/router";

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {
  constructor(private router: Router,
              private loginService: LoginService) {
  }

  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    return next.handle(request).pipe(catchError(err => {
      if (err.error.errorMessage == 'Oturum süresi dolmuştur. Kullanıcı girişi yapınız.') {
        this.loginService.logout();
        this.router.navigate(['/login']);
      }
      return throwError(err);
    }));
  }
}
