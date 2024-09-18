import { HttpClient } from '@angular/common/http';
import { Component } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { iCustomer } from '../../models/customer';
import { environment } from '../../../environments/environment.development';


@Component({
  selector: 'app-client-modal',
  templateUrl: './client-modal.component.html',
  styleUrl: './client-modal.component.scss'
})
export class ClientModal {

  customers: iCustomer[] = [];   // Utilizzo dell'interfaccia Customer
  newCustomer: iCustomer = {
    customerType: 'private',
    name: '',
    address: '',
    email: '',
    tel: '',
    cf: '',
    partitaIVA: '',
    ragioneSociale: ''
  };

  constructor(public activeModal: NgbActiveModal, private http: HttpClient) {}

  ngOnInit() {
    this.loadCustomers();
  }

  loadCustomers() {
    this.http.get(`${environment.baseUrl}/${environment.customerapi.getAll}`).subscribe((response: any) => {
      console.log('clienti:',response); // Verifica i dati ricevuti
      this.customers = response;
    });
  }

  createCustomer() {
    const customerData = { ...this.newCustomer };
    if (this.newCustomer.customerType === 'private') {
      delete customerData.partitaIVA;
      delete customerData.ragioneSociale;
    } else {
      delete customerData.cf;
    }

    this.http.post(`${environment.baseUrl}/${environment.customerapi.create}`, customerData).subscribe(() => {
      this.loadCustomers();
      this.resetForm();
    });
  }

  updateCustomer(customer: any) {
    this.http.put(`${environment.baseUrl}/${environment.customerapi.update(customer.id)}`, customer).subscribe(() => {
      this.loadCustomers();
    });
  }

  deleteCustomer(id: number) {
    this.http.delete(`${environment.baseUrl}/${environment.customerapi.delate(id)}`).subscribe(() => {
      this.loadCustomers();
    });
  }

  resetForm() {
    this.newCustomer = {
      customerType: 'private',
      name: '',
      address: '',
      email: '',
      tel: '',
      cf: '',
      partitaIVA: '',
      ragioneSociale: ''
    };
  }
}
