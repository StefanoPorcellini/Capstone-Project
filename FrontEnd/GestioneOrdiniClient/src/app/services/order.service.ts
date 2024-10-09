import { iOrder, iStatus, iWorkType, OrderWithItemsDto } from './../models/order';
import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { environment } from '../../environments/environment.development';
import { Observable, BehaviorSubject, tap, map, catchError } from 'rxjs';
import * as signalR from '@microsoft/signalr';
import { th } from 'date-fns/locale';
import { iCustomer } from '../models/customer';
import { iLaserPriceList, iLaserStandard, iPlotterStandard } from '../models/price-list';
import Swal from 'sweetalert2';

@Injectable({
  providedIn: 'root'
})
export class OrderService {

  private baseUrl: string = `${environment.baseUrl}/`;
  private p: string = 'Order';
  private orders$: BehaviorSubject<iOrder[]> = new BehaviorSubject<iOrder[]>([]);

  laserPriceListUrl = environment.baseUrl + '/' + environment.standardApi.getAll('LaserPriceList');
  laserStandardUrl = environment.baseUrl + '/' + environment.standardApi.getAll('LaserStandards');
  plotterStandardUrl = environment.baseUrl + '/' + environment.standardApi.getAll('PlotterStandards');

  constructor(private http: HttpClient) {}

  getOrders(): Observable<iOrder[]> {
    return this.http.get<iOrder[]>(`${this.baseUrl}${environment.standardApi.getAll(this.p)}`);
  }

  // Metodo per ottenere gli ordini aggiornati in tempo reale
  getRealTimeOrders(): Observable<iOrder[]> {
    return this.orders$.asObservable();
  }

  // Metodo per la creazione di un nuovo ordine
  createOrder(orderWithItemsDto: OrderWithItemsDto): Observable<any> {
    const formData: FormData = new FormData();

    // Campi dell'ordine
    formData.append('CustomerId', String(orderWithItemsDto.Order.CustomerId));
    formData.append('Description', orderWithItemsDto.Order.description);
    formData.append('Date', orderWithItemsDto.Order.date);
    if (orderWithItemsDto.Order.maxDeliveryDate) {
        formData.append('MaxDeliveryDate', orderWithItemsDto.Order.maxDeliveryDate);
    }
    if (orderWithItemsDto.Order.operatorName) {
        formData.append('OperatorName', orderWithItemsDto.Order.operatorName);
    }
    if (orderWithItemsDto.Order.statusId) {
        formData.append('StatusId', String(orderWithItemsDto.Order.statusId));
    }

    // Campi degli articoli
    orderWithItemsDto.ItemsWithFiles.forEach((item, index) => {
        formData.append(`ItemsWithFiles[${index}].WorkTypeId`, String(item.workTypeId));
        formData.append(`ItemsWithFiles[${index}].WorkDescription`, item.workDescription);
        if (item.file) {
            formData.append(`ItemsWithFiles[${index}].File`, item.file, item.file.name);
        }
        if (item.laserStdId) {
            formData.append(`ItemsWithFiles[${index}].LaserStandardId`, String(item.laserStdId));
        }
        if (item.plotterStdId) {
            formData.append(`ItemsWithFiles[${index}].PlotterStandardId`, String(item.plotterStdId));
        }
        formData.append(`ItemsWithFiles[${index}].Quantity`, String(item.quantity));
        formData.append(`ItemsWithFiles[${index}].Price`, String(item.price));
        if(item.base){ formData.append(`ItemsWithFiles[${index}].Base`, String(item.base)); }
        if(item.height){ formData.append(`ItemsWithFiles[${index}].Height`, String(item.height)); }
        if(item.pricePerSqMeter){ formData.append(`ItemsWithFiles[${index}].PricePerSqMeter`, String(item.pricePerSqMeter)); }
    });

    return this.http.post<OrderWithItemsDto>(`${this.baseUrl}${environment.standardApi.create(this.p)}`, formData)
        .pipe(
            catchError((error: HttpErrorResponse) => {
                console.error('Errore durante la creazione dell\'ordine:', error);
                Swal.fire({
                    icon: 'error',
                    title: 'Errore nella creazione dell\'ordine',
                    text: error.error?.message || 'Si è verificato un errore imprevisto.'
                });
                throw error;
            })
        );
  }

  // Metodo per modificare un ordine esistente
  updateOrder(orderWithItemsDto: OrderWithItemsDto, orderId: number): Observable<OrderWithItemsDto> {
    const formData: FormData = new FormData();

    // Campi dell'ordine
    formData.append('CustomerId', String(orderWithItemsDto.Order.CustomerId));
    formData.append('Description', orderWithItemsDto.Order.description);
    formData.append('Date', orderWithItemsDto.Order.date);
    if (orderWithItemsDto.Order.maxDeliveryDate) {
        formData.append('MaxDeliveryDate', orderWithItemsDto.Order.maxDeliveryDate);
    }
    if (orderWithItemsDto.Order.operatorName) {
        formData.append('OperatorName', orderWithItemsDto.Order.operatorName);
    }
    if (orderWithItemsDto.Order.statusId) {
        formData.append('StatusId', String(orderWithItemsDto.Order.statusId));
    }

    // Campi degli articoli
    orderWithItemsDto.ItemsWithFiles.forEach((item, index) => {
        formData.append(`ItemsWithFiles[${index}].WorkTypeId`, String(item.workTypeId));
        formData.append(`ItemsWithFiles[${index}].WorkDescription`, item.workDescription);
        if (item.file) {
            formData.append(`ItemsWithFiles[${index}].File`, item.file, item.file.name);
        }
        if (item.laserStdId) {
            formData.append(`ItemsWithFiles[${index}].LaserStandardId`, String(item.laserStdId));
        }
        if (item.plotterStdId) {
            formData.append(`ItemsWithFiles[${index}].PlotterStandardId`, String(item.plotterStdId));
        }
        formData.append(`ItemsWithFiles[${index}].Quantity`, String(item.quantity));
        formData.append(`ItemsWithFiles[${index}].Price`, String(item.price));
        if(item.base){ formData.append(`ItemsWithFiles[${index}].Base`, String(item.base)); }
        if(item.height){ formData.append(`ItemsWithFiles[${index}].Height`, String(item.height)); }
        if(item.pricePerSqMeter){ formData.append(`ItemsWithFiles[${index}].PricePerSqMeter`, String(item.pricePerSqMeter)); }
    });

    return this.http.put<OrderWithItemsDto>(`${this.baseUrl}${environment.standardApi.update(this.p, orderId)}`, formData)
        .pipe(
            catchError((error: HttpErrorResponse) => {
                console.error('Errore durante l\'aggiornamento dell\'ordine:', error);
                Swal.fire({
                    icon: 'error',
                    title: 'Errore nell\'aggiornamento dell\'ordine',
                    text: error.error?.message || 'Si è verificato un errore imprevisto.'
                });
                throw error;
            })
        );
  }
  // Metodo per ottenere un ordine specifico per ID
  getOrderById(orderId: number): Observable<iOrder> {
    const url = `${this.baseUrl}${environment.standardApi.getById(this.p, orderId)}`; // Costruisci l'URL per la richiesta
    return this.http.get<iOrder>(url); // Restituisci l'osservabile della risposta
  }

  // Metodo per eliminare un ordine
  deleteOrder(orderId: number): Observable<any> {
    return this.http.delete(`${this.baseUrl}${environment.standardApi.delete(this.p, orderId)}`)
        .pipe(
            catchError((error: HttpErrorResponse) => {
                console.error('Errore durante l\'eliminazione dell\'ordine:', error);
                Swal.fire({
                    icon: 'error',
                    title: 'Errore nell\'eliminazione dell\'ordine',
                    text: error.error?.message || 'Si è verificato un errore imprevisto.'
                });
                throw error;
            })
        );
  }

  getCustomers(): Observable<iCustomer[]> {
    return this.http.get<iCustomer[]>(`${this.baseUrl}${environment.standardApi.getAll('Customer')}`);
  }

  getStatuses(): Observable<iStatus[]> {
    return this.http.get<iStatus[]>(`${this.baseUrl}Order/statuses`);
  }

  getWorkType(): Observable<iWorkType[]> {
    return this.http.get<iWorkType[]>(`${this.baseUrl}Order/workType`);
  }

  getLaserPriceList(): Observable<iLaserPriceList[]> {
    return this.http.get<iLaserPriceList[]>(this.laserPriceListUrl);
  }

  getLaserStandard(): Observable<iLaserStandard[]> {
    return this.http.get<iLaserStandard[]>(this.laserStandardUrl);
  }

  getPlotterStandard(): Observable<iPlotterStandard[]> {
    return this.http.get<iPlotterStandard[]>(this.plotterStandardUrl);
  }
}
