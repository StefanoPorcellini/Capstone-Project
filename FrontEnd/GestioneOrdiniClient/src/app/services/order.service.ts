import { iOrder } from './../models/order';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment.development';
import { Observable, BehaviorSubject } from 'rxjs';
import * as signalR from '@microsoft/signalr';
import { th } from 'date-fns/locale';

@Injectable({
  providedIn: 'root'
})
export class OrderService {

  private baseUrl: string = `${environment.baseUrl}/`;
  private p: string = 'Order';
  private hubConnection: signalR.HubConnection | undefined;
  private orders$: BehaviorSubject<iOrder[]> = new BehaviorSubject<iOrder[]>([]);

  constructor(private http: HttpClient) {
    this.startConnection();
    this.addSignalRListener();
  }

  getOrders(): Observable<iOrder[]> {
    return this.http.get<iOrder[]>(`${this.baseUrl}/${environment.standardApi.getAll(this.p)}`);
  }

  // Metodo per ottenere gli ordini aggiornati in tempo reale
  getRealTimeOrders(): Observable<iOrder[]> {
    return this.orders$.asObservable();
  }

  // Connessione SignalR
  private startConnection(): void {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(`${this.baseUrl}/orderHub`)  // Assicurati che l'URL sia corretto
      .build();

    this.hubConnection
      .start()
      .catch(err => console.error('Error while starting SignalR connection: ' + err));
  }

  private addSignalRListener(): void {
    this.hubConnection?.on('OrderUpdated', (orders: iOrder[]) => {
      this.orders$.next(orders);
    });
  }

   // Metodo per la creazione di un nuovo ordine
   createOrder(order: iOrder, file: File | null): Observable<iOrder> {
    const formData: FormData = new FormData();

    // Aggiunge tutti i campi dell'ordine
    formData.append('orderId', order.orderId?.toString() || ''); // Se esiste l'ID lo aggiunge, altrimenti campo vuoto
    formData.append('customerId', order.customerId?.toString() || '');
    formData.append('customerName', order.customerName);
    formData.append('customerAddress', order.customerAddress || '');
    formData.append('customerEmail', order.customerEmail || '');
    formData.append('customerTel', order.customerTel);
    formData.append('description', order.description || '');
    formData.append('date', order.date || '');
    formData.append('maxDeliveryDate', order.maxDeliveryDate || '');
    formData.append('totalAmount', order.totalAmount?.toString() || '');
    formData.append('operatorName', order.operatorName || '');
    formData.append('statusId', order.statusId?.toString() || '');
    formData.append('statusName', order.statusName || '');

    // Aggiunge i file, se esiste
    if (file) {
      formData.append('file', file, file.name);
    }

    return this.http.post<iOrder>(`${this.baseUrl}/${environment.standardApi.create(this.p)}`, formData);
  }
}
