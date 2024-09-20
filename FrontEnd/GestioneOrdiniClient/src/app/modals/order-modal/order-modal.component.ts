import { Component, Input } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { environment } from '../../../environments/environment.development';
import { iOrder, iOrderStatus } from '../../models/order';

@Component({
  selector: 'app-order-modal',
  templateUrl: './order-modal.component.html',
  styleUrls: ['./order-modal.component.scss']
})
export class OrderModalComponent {
  @Input() selectedDate!: Date;
  order: iOrder = {} as iOrder;
  statuses: iOrderStatus[] = [];
  orders: iOrder[] = [];

  constructor(
    public activeModal: NgbActiveModal,
  ) {}

  ngOnInit(): void {
    // this.loadStatuses();
    // this.loadOrders();
  }

  // loadStatuses() {
  //   this.orderService.getAllOrderStatuses().subscribe(statuses => {
  //     this.statuses = statuses;
  //   });
  // }

  // loadOrders() {
  //   this.orderService.getAllOrders().subscribe(orders => {
  //     this.orders = orders.filter(order =>
  //       new Date(order.creationDate) <= this.selectedDate &&
  //       new Date(order.deliveryDate) >= this.selectedDate
  //     );
  //   });
  // }

  // addOrder() {
  //   const formData = new FormData();
  //   formData.append('order', JSON.stringify(this.order));
  //   if (this.order.file) {
  //     formData.append('file', this.order.file);
  //   }

  //   this.orderService.createOrder(formData).subscribe(() => {
  //     this.activeModal.close();
  //   });
  // }
}
