import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { iCustomer } from '../../models/customer';
import { environment } from '../../../environments/environment.development';
import { SignalRService } from '../../services/signal-r.service'; // Assicurati di avere il percorso corretto

@Component({
  selector: 'app-client-modal',
  templateUrl: './client-modal.component.html',
  styleUrls: ['./client-modal.component.scss']
})
export class ClientModal implements OnInit {

  url: string = 'Customer';

  customers: iCustomer[] = [];
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

  showCreateCustomerForm: boolean = false;

  constructor(
    public activeModal: NgbActiveModal,
    private http: HttpClient,
    private signalRService: SignalRService // Inietta il servizio SignalR
  ) {}

  ngOnInit() {
    this.loadCustomers();
    this.startSignalRConnection();
  }

  loadCustomers() {
    this.http.get(`${environment.baseUrl}/${environment.standardApi.getAll(this.url)}`).subscribe((response: any) => {
      console.log('clienti:', response); // Verifica i dati ricevuti
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

    this.http.post(`${environment.baseUrl}/${environment.standardApi.create(this.url)}`, customerData).subscribe(() => {
      this.loadCustomers();
      this.resetForm();
    });
  }

  updateCustomer(customer: iCustomer) {
    this.http.put(`${environment.baseUrl}/${environment.standardApi.update(this.url, customer.id!)}`, customer).subscribe(() => {
      this.loadCustomers();
    });
  }

  deleteCustomer(id: number) {
    this.http.delete(`${environment.baseUrl}/${environment.standardApi.delete(this.url, id)}`).subscribe(() => {
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

  toggleCreateCustomerForm() {
    this.showCreateCustomerForm = !this.showCreateCustomerForm;
  }

  private startSignalRConnection(): void {
    this.signalRService.startConnection(); // Avvia la connessione SignalR
    this.signalRService.addCustomerUpdateListener((message: string) => {
      console.log('Update received from SignalR: ', message);
      this.loadCustomers(); // Ricarica i clienti quando ricevi un aggiornamento
    });
  }
}
