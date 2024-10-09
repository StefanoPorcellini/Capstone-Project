import { OrderModal } from './../../modals/order-modal/order-modal.component';
import Swal from 'sweetalert2';
import { AuthService } from '../../auth/Service/auth-service.service';
import { iUserAuth } from './../../models/user';
import { Component } from '@angular/core';
import { UserModal } from '../../modals/user-modal/user-modal.component'; // Assicurati di importare il tuo UserModal
import { ClientModal } from '../../modals/client-modal/client-modal.component';
import { PriceList } from '../../modals/price-list/price-list.component';
import { ModalService } from '../../services/modal.service';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.scss'
})
export class NavbarComponent {

  constructor(private authSvc: AuthService, private modalService: ModalService) {}

  userAuth: iUserAuth | null = null;
  userRole: string = ''

  ngOnInit() {
    const accessDataString = localStorage.getItem('accessData');
    if (accessDataString) {
      const accessData = JSON.parse(accessDataString);
      this.userAuth = accessData.userAuth;
      this.userRole = accessData.userAuth.role;
      console.log('Utente loggato: ', this.userAuth);
      console.log('Ruolo Utente: ', this.userAuth);
    }

  }
  logout() {
    // Mostra il SweetAlert di conferma
    Swal.fire({
      title: 'Sei sicuro?',
      text: 'Vuoi veramente effettuare il logout?',
      icon: 'warning',
      showCancelButton: true,
      confirmButtonColor: '#3085d6',
      cancelButtonColor: '#d33',
      confirmButtonText: 'Sì, esci!',
      cancelButtonText: 'Annulla'
    }).then((result) => {
      if (result.isConfirmed) {
        // Effettua il logout se l'utente conferma
        this.authSvc.logout();
        Swal.fire(
          'Disconnesso!',
          'Hai effettuato il logout con successo.',
          'success'
        );
      }
    });
  }

  openUserModal() {
    this.modalService.openUserModal();
  }

  openClientModal(){
    this.modalService.openClientModal();
  }

  openPriceListModal(){
    this.modalService.openPriceListModal();
  }

  openNewOrder() {
    this.modalService.openNewOrderModal();
  }
}
