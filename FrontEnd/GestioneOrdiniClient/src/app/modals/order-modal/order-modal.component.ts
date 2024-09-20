import { Component, Input } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { environment } from '../../../environments/environment.development';
import { iOrder, iStatus } from '../../models/order';
import { iCustomer } from '../../models/customer';
import { OrderService } from '../../services/order.service';

@Component({
  selector: 'app-order-modal',
  templateUrl: './order-modal.component.html',
  styleUrls: ['./order-modal.component.scss']
})
export class OrderModal {
  @Input() selectedDate!: Date;
  newOrder: iOrder = {} as iOrder;
  statuses: iStatus[] = [];
  orders: iOrder[] = [];
  customers: iCustomer[] = [];
  file: File | null = null;


  constructor(
    public activeModal: NgbActiveModal, private orderService: OrderService
  ) {}

  onFileSelected(event: any) {
    this.file = event.target.files[0];
  }

  createOrder() {
    this.orderService.createOrder(this.newOrder, this.file).subscribe(
      (response) => {
        console.log('Order created successfully:', response);
        // Aggiungi eventuale logica di successo qui
      },
      (error) => {
        console.error('Error creating order:', error);
        // Gestisci l'errore
      }
    );
  }


}
