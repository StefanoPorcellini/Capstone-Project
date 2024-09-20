import { OrderModal } from './../../modals/order-modal/order-modal.component';
import Swal from 'sweetalert2';
import { AuthService } from '../../auth/Service/auth-service.service';
import { iUserAuth } from './../../models/user';
import { Component } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { UserModal } from '../../modals/user-modal/user-modal.component'; // Assicurati di importare il tuo UserModal
import { ClientModal } from '../../modals/client-modal/client-modal.component';
import { PriceList } from '../../modals/price-list/price-list.component';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.scss'
})
export class NavbarComponent {

  constructor(private authSvc: AuthService, private modalService: NgbModal) {}

  userAuth: iUserAuth | null = null;

  ngOnInit() {
    const accessDataString = localStorage.getItem('accessData');
    if (accessDataString) {
      const accessData = JSON.parse(accessDataString);
      this.userAuth = accessData.userAuth;
      console.log('Utente loggato: ', this.userAuth);
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
    const userModalRef = this.modalService.open(UserModal); // Aggiunto l'opzione 'centered'
  }

  openClientModal(){
    const clentModalRef = this.modalService.open(ClientModal,{size: 'xl', scrollable: true})
  }

  openPriceListModal(){
    const clentModalRef = this.modalService.open(PriceList,{ size: 'xl', scrollable: true})
  }

  openNewOrder() {
    const orderModalRef = this.modalService.open(OrderModal,{size: 'xl', scrollable: true})
  }
}
