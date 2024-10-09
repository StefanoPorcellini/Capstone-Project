import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { environment } from '../../../environments/environment.development';
import { iLaserPriceList, iLaserStandard, iPlotterStandard } from '../../models/price-list';
import Swal from 'sweetalert2';

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

  loadLaserPriceList() {
    this.http.get<iLaserPriceList[]>(this.laserPriceListUrl).subscribe(data => {
      this.laserPriceList = data;
    });
  }

  loadLaserStandard() {
    this.http.get<iLaserStandard[]>(this.laserStandardUrl).subscribe(data => {
      this.laserStandard = data;
    });
  }

  loadPlotterStandard() {
    this.http.get<iPlotterStandard[]>(this.plotterStandardUrl).subscribe(data => {
      this.plotterStandard = data;
    });
  }

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

  editItem(listType: string, id: number, updatedItem: any) {
    Swal.fire({
      title: 'Confermi la modifica?',
      icon: 'warning',
      showCancelButton: true,
      confirmButtonText: 'Sì, modifica',
      cancelButtonText: 'No, annulla'
    }).then((result) => {
      if (result.isConfirmed) {
        const url = this.getUpdateUrl(listType, id);
        this.http.put(url, updatedItem).subscribe({
          next: (res) => {
            Swal.fire('Modifica riuscita', '', 'success');
            this.refreshList(listType);
          },
          error: (err) => {
            Swal.fire('Errore', 'La modifica non è riuscita', 'error');
          }
        });
      }
    });
  }

  deleteItem(listType: string, id: number) {
    Swal.fire({
      title: 'Sei sicuro?',
      text: "Questa operazione non può essere annullata!",
      icon: 'warning',
      showCancelButton: true,
      confirmButtonText: 'Sì, elimina',
      cancelButtonText: 'No, annulla'
    }).then((result) => {
      if (result.isConfirmed) {
        const url = this.getDeleteUrl(listType, id);
        this.http.delete(url).subscribe({
          next: (res) => {
            Swal.fire('Eliminato!', 'L\'elemento è stato eliminato.', 'success');
            this.refreshList(listType);
          },
          error: (err) => {
            Swal.fire('Errore', 'L\'elemento non è stato eliminato', 'error');
          }
        });
      }
    });
  }

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
