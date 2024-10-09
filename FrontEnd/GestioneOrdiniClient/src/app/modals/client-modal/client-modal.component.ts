import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { iCustomer } from '../../models/customer';
import { environment } from '../../../environments/environment.development';
import { SignalRService } from '../../services/signal-r.service';
import Swal from 'sweetalert2'; // Importa SweetAlert

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

  showForm: boolean = false;

  constructor(
    public activeModal: NgbActiveModal,
    private http: HttpClient,
    private signalRService: SignalRService
  ) {}

  ngOnInit() {
    this.loadCustomers();
    this.startSignalRConnection();
  }

  toggleForm() {
    this.showForm = !this.showForm;
    this.resetForm();
  }

  loadCustomers() {
    this.http.get(`${environment.baseUrl}/${environment.standardApi.getAll(this.url)}`).subscribe((response: any) => {
      console.log('clienti:', response);
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
    Swal.fire({
      title: 'Confermi la modifica?',
      icon: 'warning',
      showCancelButton: true,
      confirmButtonText: 'Sì, modifica',
      cancelButtonText: 'No, annulla'
    }).then((result) => {
      if (result.isConfirmed) {
        this.http.put(`${environment.baseUrl}/${environment.standardApi.update(this.url, customer.id!)}`, customer).subscribe(() => {
          Swal.fire('Modifica riuscita', '', 'success');
          this.loadCustomers();
        }, (error) => {
          Swal.fire('Errore', 'La modifica non è riuscita', 'error');
        });
      }
    });
  }

  deleteCustomer(id: number) {
    Swal.fire({
      title: 'Sei sicuro?',
      text: "Questa operazione non può essere annullata!",
      icon: 'warning',
      showCancelButton: true,
      confirmButtonText: 'Sì, elimina',
      cancelButtonText: 'No, annulla'
    }).then((result) => {
      if (result.isConfirmed) {
        this.http.delete(`${environment.baseUrl}/${environment.standardApi.delete(this.url, id)}`).subscribe(() => {
          Swal.fire('Eliminato!', 'Il cliente è stato eliminato.', 'success');
          this.loadCustomers();
        }, (error) => {
          Swal.fire('Errore', 'Il cliente non è stato eliminato', 'error');
        });
      }
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
    this.showForm = !this.showForm;
  }

  private startSignalRConnection(): void {
    this.signalRService.startConnection();
    this.signalRService.addCustomerUpdateListener((message: string) => {
      console.log('Update received from SignalR: ', message);
      this.loadCustomers();
    });
  }
}
