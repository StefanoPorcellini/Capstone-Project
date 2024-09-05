import { iAuthData } from './../../models/auth-data';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, map, Observable, tap } from 'rxjs';
import { iUser } from '../../models/user';
import { Router } from '@angular/router';
import { iAuthResponse } from '../../models/auth-response';
import { environment } from '../../../environments/environment.development';
@Injectable({
  providedIn: 'root'
})
export class AuthService {

  authSubject = new BehaviorSubject<null | iUser>(null);

  user$ = this.authSubject.asObservable();
  syncIsLoggedIn: boolean = false;

  isLoggedIn$ = this.user$.pipe(
    map(user => !!user),
    tap(user => this.syncIsLoggedIn = user)
  );

  constructor(private http: HttpClient, private router: Router) {
    this.restoreUser();
  }

  create(newUser: Partial<iUser>): Observable<iAuthResponse> {
    return this.http.post<iAuthResponse>(environment.createUrl, newUser);
  }

  login(authData: iAuthData): Observable<iAuthResponse> {
    return this.http.post<iAuthResponse>(environment.loginUrl, authData)
      .pipe(tap(data => {
        this.authSubject.next(data.user);
        localStorage.setItem('accessData', JSON.stringify(data));
        this.autoLogout(new Date(data.expires)); // Passa la data di scadenza dalla risposta
      }));
  }

  logout(): void {
    this.authSubject.next(null);
    localStorage.removeItem('accessData');
    this.router.navigate(['/auth/login']);
  }

  // Aggiornato per usare direttamente la data di scadenza dalla risposta
  autoLogout(expirationDate: Date): void {
    const expMs = expirationDate.getTime() - new Date().getTime();
    if (expMs <= 0) {
      this.logout(); // Logout immediato se già scaduto
    } else {
      setTimeout(() => {
        this.logout();
      }, expMs);
    }
  }

  getAccessData(): iAuthResponse | null {
    const accessDataJson = localStorage.getItem('accessData');
    if (!accessDataJson) return null;
    const accessData: iAuthResponse = JSON.parse(accessDataJson);
    return accessData;
  }

  restoreUser(): void {
    const accessData = this.getAccessData();
    if (!accessData) return;

    const expirationDate = new Date(accessData.expires);
    if (expirationDate <= new Date()) {
      this.logout(); // Logout se il token è scaduto
    } else {
      this.authSubject.next(accessData.user);
      this.autoLogout(expirationDate); // Imposta l'auto logout
    }
  }

}
