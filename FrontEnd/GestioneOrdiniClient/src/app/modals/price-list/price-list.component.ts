import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { environment } from '../../../environments/environment.development';
import { iLaserPriceList, iLaserStandard, iPlotterStandard } from '../../models/price-list';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';



@Component({
  selector: 'app-price-list',
  templateUrl: './price-list.component.html',
  styleUrls: ['./price-list.component.scss']
})
export class PriceList implements OnInit {
  laserPriceListUrl = environment.baseUrl + '/' + environment.standardApi.getAll('LaserPriceList');
  laserStandardUrl = environment.baseUrl + '/' + environment.standardApi.getAll('LaserStandards');
  plotterStandardUrl = environment.baseUrl + '/' + environment.standardApi.getAll('PlotterStandards');

  laserPriceList: iLaserPriceList[] = [];
  laserStandard: iLaserStandard[] = [];
  plotterStandard: iPlotterStandard[] = [];
  selectedList: string = 'laserPriceList'; // Valore iniziale

  constructor(public activeModal: NgbActiveModal, private http: HttpClient) {}

  ngOnInit() {
    this.loadLaserPriceList();
  }

  // Carica i dati del listino prezzi laser
  loadLaserPriceList() {
    this.http.get<iLaserPriceList[]>(this.laserPriceListUrl).subscribe(data => {
      this.laserPriceList = data;
    });
  }

  // Carica i dati del listino prezzi laser standard
  loadLaserStandard() {
    this.http.get<iLaserStandard[]>(this.laserStandardUrl).subscribe(data => {
      this.laserStandard = data;
    });
  }

  // Carica i dati del listino prezzi plotter
  loadPlotterStandard() {
    this.http.get<iPlotterStandard[]>(this.plotterStandardUrl).subscribe(data => {
      this.plotterStandard = data;
    });
  }

  // Gestione del cambio di lista (listino prezzi selezionato)
  onListChange(listType: string) {
    this.selectedList = listType;
    switch (listType) {
      case 'laserPriceList':
        this.loadLaserPriceList();
        break;
      case 'laserStandard':
        this.loadLaserStandard();
        break;
      case 'plotterStandard':
        this.loadPlotterStandard();
        break;
    }
  }

  // Metodo per modificare un elemento
  editItem(listType: string, id: number, updatedItem: any) {
    const url = this.getUpdateUrl(listType, id);
    this.http.put(url, updatedItem).subscribe({
      next: (res) => {
        console.log('Update successful', res);
        this.refreshList(listType);
      },
      error: (err) => {
        console.error('Update failed', err);
      }
    });
  }

  // Metodo per cancellare un elemento
  deleteItem(listType: string, id: number) {
    const url = this.getDeleteUrl(listType, id);
    this.http.delete(url).subscribe({
      next: (res) => {
        console.log('Delete successful', res);
        this.refreshList(listType);
      },
      error: (err) => {
        console.error('Delete failed', err);
      }
    });
  }

  // Metodo per ottenere l'URL per l'update
  getUpdateUrl(listType: string, id: number): string {
    switch (listType) {
      case 'laserPriceList':
        return environment.baseUrl + '/' + environment.standardApi.update('LaserPriceList', id);
      case 'laserStandard':
        return environment.baseUrl + '/' + environment.standardApi.update('LaserStandard', id);
      case 'plotterStandard':
        return environment.baseUrl + '/' + environment.standardApi.update('PlotterStandard', id);
      default:
        throw new Error('Unknown list type');
    }
  }

  // Metodo per ottenere l'URL per la cancellazione
  getDeleteUrl(listType: string, id: number): string {
    switch (listType) {
      case 'laserPriceList':
        return environment.baseUrl + '/' + environment.standardApi.delete('LaserPriceList', id);
      case 'laserStandard':
        return environment.baseUrl + '/' + environment.standardApi.delete('LaserStandard', id);
      case 'plotterStandard':
        return environment.baseUrl + '/' + environment.standardApi.delete('PlotterStandard', id);
      default:
        throw new Error('Unknown list type');
    }
  }

  // Metodo per ricaricare la lista dopo un'operazione di update/delete
  refreshList(listType: string) {
    switch (listType) {
      case 'laserPriceList':
        this.loadLaserPriceList();
        break;
      case 'laserStandard':
        this.loadLaserStandard();
        break;
      case 'plotterStandard':
        this.loadPlotterStandard();
        break;
    }
  }
}
