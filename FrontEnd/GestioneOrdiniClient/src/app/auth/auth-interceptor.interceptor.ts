import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor
} from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {

  constructor() {}

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    const accessData = localStorage.getItem('accessData'); // Ottieni i dati di accesso
    let authToken = '';

    if (accessData) {
      const parsedData = JSON.parse(accessData); // Parsea i dati
      authToken = parsedData.token; // Estrai il token
    }

    if (authToken) {
      const authReq = req.clone({
        headers: req.headers.set('Authorization', `Bearer ${authToken}`) // Aggiungi l'intestazione di autorizzazione
      });
      return next.handle(authReq); // Invia la richiesta modificata
    }

    return next.handle(req); // Invia la richiesta originale se non c'è token
  }
}
