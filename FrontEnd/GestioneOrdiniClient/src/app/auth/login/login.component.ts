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
    private authSvc:AuthService,
    private router:Router
  ){}

  ngOnInit(): void {}

  login(form: NgForm){
    try {
      this.authSvc.login(form.value).subscribe();
      Swal.fire({
        title: 'Registrazione completata con successo!',
        text: 'Ora puoi effettuare il login',
        icon: 'success',
      });
          setTimeout(() => {
          this.router.navigate(['../../pages/dashboard/dashboard.component.html']);
      }, 500);

  } catch (error) {
      Swal.fire({
        title: 'Username o Password errati!',
        text: 'Controlla i dati',
        icon: 'error',
      });
    console.error(error);
      this.router.navigate(['/login']);
  }
  }

}
