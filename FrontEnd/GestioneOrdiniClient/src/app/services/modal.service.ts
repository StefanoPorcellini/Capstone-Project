import { Injectable, EventEmitter } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { UserModal } from '../modals/user-modal/user-modal.component';
import { ClientModal } from '../modals/client-modal/client-modal.component';
import { PriceList } from '../modals/price-list/price-list.component';
import { OrderModal } from '../modals/order-modal/order-modal.component';
import { SignalRService } from '../services/signal-r.service'; // Importa il servizio SignalR

@Injectable({
  providedIn: 'root'
})
export class ModalService {
  private openModals: number = 0; // Conteggio dei modali aperti
  public modalClosed: EventEmitter<void> = new EventEmitter(); // Evento per la chiusura del modale

  constructor(
    private modalService: NgbModal,
    private signalRService: SignalRService // Inietta il servizio SignalR
  ) {}

  isModalOpen(): boolean {
    return this.openModals > 0;
  }

  openUserModal(): Promise<any> {
    const modalRef = this.modalService.open(UserModal);
    return modalRef.result;
  }

  openClientModal(): Promise<any> {
    this.openModals++;
    const modalRef = this.modalService.open(ClientModal, { size: 'xl', scrollable: true });
    return modalRef.result.finally(() => {
      this.openModals--;
      this.modalClosed.emit(); // Emitti evento alla chiusura del modale
    });
  }

  openPriceListModal(): Promise<any> {
    const modalRef = this.modalService.open(PriceList, { size: 'xl', scrollable: true });
    return modalRef.result.finally(() => this.modalClosed.emit()); // Emitti evento alla chiusura del modale
  }

  openNewOrderModal(): Promise<any> {
    this.openModals++;
    const modalRef = this.modalService.open(OrderModal, { size: 'xl', scrollable: true });
    return modalRef.result.finally(() => {
      this.openModals--;
      this.modalClosed.emit(); // Emitti evento alla chiusura del modale
    });
  }
}
