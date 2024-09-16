import { iAuthData } from './../../models/auth-data';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, map, Observable, tap } from 'rxjs';
import { iUser, iUserAuth } from '../../models/user';
import { Router } from '@angular/router';
import { iAuthResponse } from '../../models/auth-response';
import { environment } from '../../../environments/environment.development';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  private authSubject = new BehaviorSubject<iUserAuth | null>(null);

  user$ = this.authSubject.asObservable();
  syncIsLoggedIn: boolean = false;

  // Utilizzare 'tap' correttamente per aggiornare 'syncIsLoggedIn'
  isLoggedIn$ = this.user$.pipe(
    map(user => !!user), // Trasforma l'utente in un booleano
    tap(isLoggedIn => {
      console.log('isLoggedIn (tap):', isLoggedIn); // Debug per verificare l'aggiornamento
      this.syncIsLoggedIn = isLoggedIn; // Sincronizza lo stato
    })
  );

  constructor(private http: HttpClient, private router: Router) {
    this.user$.subscribe(user => {
      console.log('User emitted:', user); // Debug per verificare l'emissione
    });

    this.restoreUser(); // Prova a ripristinare l'utente dallo storage locale
  }

  // URL per le API utente
  private userUrl = environment.baseUrl;

  // Metodo per creare un nuovo utente
  create(newUser: Partial<iUser>): Observable<iAuthResponse> {
    return this.http.post<iAuthResponse>(`${this.userUrl}/User/create`, newUser);
  }

  // Metodo per effettuare il login
  login(authData: iAuthData): Observable<iAuthResponse> {
    return this.http.post<iAuthResponse>(`${this.userUrl}/User/login`, authData)
      .pipe(tap(data => {
        if (data && data.token) {
          console.log('Login data:', data); // Verifica i dati ricevuti
          const user: iUserAuth =  { // Usa iUserAuth per recuperare i dati dal utente loggato
            id: data.userAuth.id,
            username: data.userAuth.username,
            roleId: data.userAuth.roleId
          };
          this.authSubject.next(user); // Imposta l'utente
          // Verifica che i dati siano serializzati correttamente
          const accessData = {
            ...data,
            expires: new Date(data.expires)
          };
          localStorage.setItem('accessData', JSON.stringify(accessData)); // Salva nel localStorage
          this.autoLogout(new Date(data.expires)); // Imposta il logout automatico
        } else {
          console.error('Invalid login response:', data); // Verifica i dati
        }
      }));
  }

  // Metodo per effettuare il logout
  logout(): void {
    this.authSubject.next(null); // Resetta l'utente autenticato
    localStorage.removeItem('accessData'); // Rimuove i dati di accesso dallo storage locale
    this.router.navigate(['/auth/login']); // Reindirizza alla pagina di login
  }

  // Imposta il logout automatico alla scadenza del token
  autoLogout(expirationDate: Date): void {
    const expMs = expirationDate.getTime() - new Date().getTime();
    if (expMs <= 0) {
      this.logout(); // Logout immediato se il token è già scaduto
    } else {
      setTimeout(() => {
        this.logout(); // Logout alla scadenza del token
      }, expMs);
    }
  }

  // Restituisce i dati di accesso dal localStorage
  getAccessData(): iAuthResponse | null {
    const accessDataJson = localStorage.getItem('accessData');
    if (!accessDataJson) return null;
    return JSON.parse(accessDataJson) as iAuthResponse;
  }

  // Ripristina l'utente dal localStorage se esiste
  restoreUser(): void {
    const accessData = this.getAccessData();
    if (!accessData || !accessData.userAuth.username) {
      console.log('No access data or user found');
      return;
    }

    console.log('Restoring user:', accessData.userAuth.username); // Verifica i dati
    const expirationDate = new Date(accessData.expires);
    if (expirationDate <= new Date()) {
      this.logout(); // Logout se il token è scaduto
    } else {
      const user: iUserAuth =  {
        id: accessData.userAuth.id,
        username: accessData.userAuth.username,
        roleId: accessData.userAuth.roleId
      }
      this.authSubject.next(user); // Ripristina l'utente
      this.autoLogout(expirationDate); // Imposta l'auto logout
    }
  }
}
