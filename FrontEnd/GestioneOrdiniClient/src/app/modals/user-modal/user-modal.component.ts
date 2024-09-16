import { Component, OnInit } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment.development';

interface User {
roleName: any;
  id: number;
  username: string;
  role: string;
}

@Component({
  selector: 'app-user-modal',
  templateUrl: './user-modal.component.html',
})
export class UserModal implements OnInit {
  users: User[] = [];

  constructor(public activeModal: NgbActiveModal, private http: HttpClient) {}

  ngOnInit() {
    this.loadUsers(); // Carica gli utenti all'apertura del modale
  }

  loadUsers() {
    this.http.get<User[]>(`${environment.baseUrl}/${environment.userapi.all}`).subscribe(
      data => {
        this.users = data;
        console.log('Utenti caricati:', this.users); // Log per verificare i dati
      },
      error => {
        console.error('Errore nel caricamento degli utenti:', error); // Log per errori
      }
    );
  }

  editUser(user: User) {
    // Logica per modificare l'utente
  }

  deleteUser(userId: number) {
    // Logica per eliminare l'utente
  }
}
