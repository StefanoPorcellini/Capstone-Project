import { Role } from './../../models/role';
import { Component, OnInit } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment.development';
import { iUserAuth } from '../../models/user'; // Importa i modelli esistenti

@Component({
  selector: 'app-user-modal',
  templateUrl: './user-modal.component.html',
})
export class UserModal implements OnInit {
  users: iUserAuth[] = [];
  selectedUser: iUserAuth | null = null; // Per tenere traccia dell'utente selezionato per la modifica
  newUsername: string = ''; // Variabile per il nuovo username
  newPassword: string = ''; // Variabile per la password
  newRoleName: string = ''; // Variabile per il nome del ruolo
  roles: Role[] = []; // Array per i ruoli
  newRoleId: number | null = null; // Variabile per il nuovo ruolo (ID)

  constructor(public activeModal: NgbActiveModal, private http: HttpClient) {}

  ngOnInit() {
    this.loadUsers(); // Carica gli utenti all'apertura del modale
    this.loadRoles(); // Carica i ruoli
  }

  loadUsers() {
    this.http.get<iUserAuth[]>(`${environment.baseUrl}/${environment.userapi.all}`).subscribe(
      data => {
        this.users = data;
        console.log('Utenti caricati:', this.users);
      },
      error => {
        console.error('Errore nel caricamento degli utenti:', error);
      }
    );
  }

  loadRoles() {
    this.http.get<Role[]>(`${environment.baseUrl}/${environment.userapi.roles}`).subscribe(
      data => {
        this.roles = data;
        console.log('Ruoli caricati:', this.roles);
      },
      error => {
        console.error('Errore nel caricamento dei ruoli:', error);
      }
    );
  }

  createUser() {
    const newUser = {
      username: this.newUsername,
      password: this.newPassword,
      roleName: this.newRoleName
    };
    this.http.post<iUserAuth>(`${environment.baseUrl}/${environment.userapi.create}`, newUser).subscribe(
      data => {
        this.users.push(data); // Aggiungi il nuovo utente alla lista
        this.resetForm(); // Resetta il modulo
        console.log('Utente creato:', data);
      },
      error => {
        console.error('Errore nella creazione dell\'utente:', error);
      }
    );
  }

  editUser(user: iUserAuth) {
    this.selectedUser = user; // Imposta l'utente selezionato per la modifica
    this.newUsername = user.username; // Precompila il campo username
    this.newRoleId = user.roleId; // Precompila il campo ruolo (modifica in base al tuo modello)
  }

  updateUser() {
    if (this.selectedUser) {
      const updatedUser = {
        username: this.newUsername,
        password: this.newPassword, // Se necessario, includi la password
        roleName: this.newRoleName
      };
      this.http.put<iUserAuth>(`${environment.baseUrl}/${environment.userapi.update(this.selectedUser.id)}`, updatedUser).subscribe(
        data => {
          const index = this.users.findIndex(u => u.id === data.id);
          if (index !== -1) {
            this.users[index] = data; // Aggiorna l'utente nella lista
          }
          this.resetForm(); // Resetta il modulo
          console.log('Utente aggiornato:', data);
        },
        error => {
          console.error('Errore nell\'aggiornamento dell\'utente:', error);
        }
      );
    }
  }

  deleteUser(userId: number) {
    this.http.delete(`${environment.baseUrl}/${environment.userapi.delete(userId)}`).subscribe(
      () => {
        this.users = this.users.filter(user => user.id !== userId); // Rimuovi l'utente dalla lista
        console.log('Utente eliminato:', userId);
      },
      error => {
        console.error('Errore nell\'eliminazione dell\'utente:', error);
      }
    );
  }

  resetForm() {
    this.selectedUser = null; // Resetta l'utente selezionato
    this.newUsername = ''; // Resetta il campo username
    this.newPassword = ''; // Resetta il campo password
    this.newRoleName = ''; // Resetta il campo ruolo
  }
}
