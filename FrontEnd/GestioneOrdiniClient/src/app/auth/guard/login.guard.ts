import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, CanActivateChild, GuardResult, MaybeAsync, Router, RouterStateSnapshot } from '@angular/router';
import { AuthService } from '../Service/auth-service.service';
import { map, Observable, take } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class LoginGuard implements CanActivate, CanActivateChild {

  constructor(private authSvc: AuthService, private router: Router) {}

  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot): Observable<boolean> {
      return this.authSvc.isLoggedIn$.pipe(
        take(1),
        map(isLoggedIn => {
          console.log('LoginGuard isLoggedIn:', isLoggedIn); // Debug
          if (isLoggedIn) {
            // Se l'utente è autenticato, reindirizzalo alla dashboard
            this.router.navigate(['/dashboard']);
            return false; // Impedisce l'accesso alla pagina di login
          }
          return true; // Permette l'accesso alla pagina di login
        })
      );
    }
  canActivateChild(
    childRoute: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<boolean> {
    return this.canActivate(childRoute, state);
  }

}
