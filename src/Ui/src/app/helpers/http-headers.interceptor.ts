import {Injectable} from '@angular/core';
import {HttpRequest, HttpHandler, HttpEvent, HttpInterceptor} from '@angular/common/http';
import {Observable} from 'rxjs';
import {LoginService} from '../services/login.service';

@Injectable()
export class HttpHeadersInterceptor implements HttpInterceptor {
  constructor(private loginService: LoginService) {
  }

  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    if (this.loginService.getUserLoginInfo()) {
      request = request.clone({headers: request.headers.set('UserId', this.loginService.getUserId())});
    }
    return next.handle(request);
  }
}
