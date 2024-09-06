import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor
} from '@angular/common/http';
import { Observable } from 'rxjs';
import { AuthService } from './Service/auth-service.service';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {

  constructor(private authSvc:AuthService) {}

  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    const data = this.authSvc.getAccessData();
    if (data?.accessToken){
      request = request.clone({
        setHeaders:  {
          Authorization: `Bearer ${data.accessToken}`
        }
      })
    }
    return next.handle(request);
  }
}
