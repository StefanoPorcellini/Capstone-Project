import { Component, OnInit } from '@angular/core';
import { AuthService } from '../Service/auth-service.service';
import { Router } from '@angular/router';
import { NgForm } from '@angular/forms';
import Swal from 'sweetalert2';
import { catchError } from 'rxjs/operators';
import { of } from 'rxjs';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit {

  constructor(
    private authSvc: AuthService,
    private router: Router
  ) {}

  ngOnInit(): void {}

    login(form: NgForm) {
      this.authSvc.login(form.value).pipe(
        catchError(error => {
          Swal.fire({
            title: 'Username o Password errati!',
            text: 'Controlla i dati',
            icon: 'error',
          });
          console.error(error);
          return of(null); // Restituisce un observable vuoto per continuare il flusso
        })
      ).subscribe(response => {
        if (response) {
          Swal.fire({
            title: 'Login effettuato con successo!',
            icon: 'success',
          });
          setTimeout(() => {
            this.router.navigate(['/dashboard']); // Naviga verso la dashboard
          }, 500);
        }
      });
    }


  }
