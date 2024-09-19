import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';

@Injectable({
  providedIn: 'root'
})
export class SignalRService {
  private hubConnection: signalR.HubConnection | null = null;

  constructor() { }

  public startConnection(): void {
    const accessData = localStorage.getItem('accessData');
    let authToken = '';

    if (accessData) {
      const parsedData = JSON.parse(accessData);
      authToken = parsedData.token;
    }

    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl('https://localhost:7147/Hubs/orderHub', {
        accessTokenFactory: () => authToken
      })
      .build();

    this.hubConnection
      .start()
      .then(() => console.log('SignalR connection started'))
      .catch(err => console.log('Error while starting SignalR connection: ' + err));
  }

  public addCustomerUpdateListener(callback: (message: string) => void): void {
    if (this.hubConnection) {
      this.hubConnection.on('ReceiveCustomerUpdate', callback);
    }
  }
}
