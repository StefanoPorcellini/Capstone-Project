import { Component, OnInit } from '@angular/core';
import { AuthService } from '../Service/auth-service.service';
import { Router } from '@angular/router';
import { NgForm } from '@angular/forms';
import Swal from 'sweetalert2';


@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss'
})
export class LoginComponent implements OnInit {

  constructor(
    private authSvc: AuthService,
    private router: Router
  ) {}

  ngOnInit(): void {}

  login(form: NgForm) {
    // Chiamata al servizio di autenticazione con gestione asincrona del risultato
    this.authSvc.login(form.value).subscribe(
      (response) => {
        // Se il login ha successo
        Swal.fire({
          title: 'Login effettuato con successo!',
          icon: 'success',
        });

        // Reindirizza alla dashboard dopo un breve ritardo
        setTimeout(() => {
          this.router.navigate(['/dashboard']);
        }, 500);
      },
      (error) => {
        // Se il login fallisce
        Swal.fire({
          title: 'Username o Password errati!',
          text: 'Controlla i dati',
          icon: 'error',
        });
        console.error('Errore durante il login:', error);
      }
    );
  }

}
