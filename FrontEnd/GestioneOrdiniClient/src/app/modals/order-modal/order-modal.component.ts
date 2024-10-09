import { Component, Input, OnInit, ChangeDetectorRef } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { iOrder, iItem, iStatus, iWorkType, OrderWithItemsDto, ItemWithFileDto } from '../../models/order';
import { iCustomer } from '../../models/customer';
import { OrderService } from '../../services/order.service';
import Swal from 'sweetalert2';
import { NgForm } from '@angular/forms';
import { ModalService } from '../../services/modal.service';
import { iLaserStandard, iPlotterStandard } from '../../models/price-list';


@Component({
  selector: 'app-order-modal',
  templateUrl: './order-modal.component.html',
  styleUrls: ['./order-modal.component.scss']
})
export class OrderModal implements OnInit {
  @Input() selectedDate!: Date;


  newOrder: iOrder = {
    description: '',
    date: new Date().toISOString().slice(0, 19).replace('T', ' '),
    maxDeliveryDate: '',
    totalAmount: 0,
    // items: []
  };

  workType: iWorkType[] = [];

  newItem: Partial<ItemWithFileDto> = {
    workTypeId: this.workType[0]?.id, // Assegna un valore di default se workType è disponibile
    workDescription: '',
    file: null,
    quantity: 1,
    price: 0
  };
  selectedCustomer: iCustomer | undefined;
  customerPhone: string | undefined;
  statuses: iStatus[] = [];
  customers: iCustomer[] = [];
  laserStandard: iLaserStandard[] = [];
  isLaserStandardSelected: boolean = false;
  laserStandardId: number | undefined;
  plotterStandard: iPlotterStandard[] = [];
  isPlotterSelected: boolean = false;
  plotterStandardId: number | undefined;
  // selectedWorkTypeId: number | undefined;
  newOrderWithItem: OrderWithItemsDto = {
    Order: this.newOrder,
    ItemsWithFiles: []
  }


  // Nuove variabili per gestire gli articoli personalizzati
  isLaserCustom: boolean = false;
  isPlotterCustom: boolean = false;

  constructor(
    public activeModal: NgbActiveModal,
    private orderService: OrderService,
    private modalService: ModalService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit() {
    this.loadCustomers();
    this.loadStatuses();
    this.loadWorkType();
    this.loadLaserStd();
    this.loadPlotterStd();
    if (this.workType.length > 0) {this.newItem.workTypeId = this.workType[0].id;}
  }

  loadCustomers() {
    this.orderService.getCustomers().subscribe(
      (data: iCustomer[]) => {
        this.customers = data;
        console.log("Clienti caricati: ", this.customers); // Aggiungi questo log
      },
      () => Swal.fire({
        icon: 'error',
        title: 'Errore!',
        text: 'Si è verificato un errore nel caricamento dei clienti.'
      })
    );
  }

  loadStatuses() {
    this.orderService.getStatuses().subscribe(
      (data: iStatus[]) => {
        this.statuses = data;
      },
      () => Swal.fire({
        icon: 'error',
        title: 'Errore!',
        text: 'Si è verificato un errore nel caricamento degli status.'
      })
    );
  }

  loadWorkType() {
    this.orderService.getWorkType().subscribe(
      (data: iWorkType[]) => {
        this.workType = data;
      },
      () => Swal.fire({
        icon: 'error',
        title: 'Errore!',
        text: 'Si è verificato un errore nel caricamento dei tipi di lavoro.'
      })
    );
  }

  loadLaserStd() {
    this.orderService.getLaserStandard().subscribe(
      (data: iLaserStandard[]) => {
        this.laserStandard = data;
      },
      () => Swal.fire({
        icon: 'error',
        title: 'Errore!',
        text: 'Si è verificato un errore nel caricamento degli standard laser.'
      })
    );
  }

  loadPlotterStd() {
    this.orderService.getPlotterStandard().subscribe(
      (data: iPlotterStandard[]) => {
        this.plotterStandard = data;
      },
      () => Swal.fire({
        icon: 'error',
        title: 'Errore!',
        text: 'Si è verificato un errore nel caricamento degli standard plotter.'
      })
    );
  }

  onCustomerSelect() {
    console.log("Funzione onCustomerSelect chiamata con customerId: ", this.newOrder.CustomerId);
    this.selectedCustomer = this.customers.find(customer => customer.id === Number(this.newOrder.CustomerId));
    this.customerPhone = this.selectedCustomer?.tel;
    this.newOrder.customer = this.selectedCustomer; // Assign the selected customer to newOrder
    console.log("Cliente selezionato: ", this.selectedCustomer);
    console.log("id cliente: ", this.selectedCustomer?.id?.valueOf());

  }

  onFileSelected(event: any) {
    const file = event.target.files[0];
    if (file) {
      this.newItem.file = file;
    }
  }

  addItem() {
    if (this.validateNewItem()) {
      console.log('Selected workTypeId:', this.newItem.workTypeId); // Logga workTypeId per debug

      if (this.isLaserStandardSelected && this.laserStandardId) {
        const selectedLaser = this.laserStandard.find(laser => laser.id === this.laserStandardId);
        if (selectedLaser) {
          this.newItem.price = selectedLaser.price;
        }
      }

      const item: ItemWithFileDto = {
        workTypeId: this.newItem.workTypeId as number, // Usa direttamente newItem.workTypeId
        workDescription: this.newItem.workDescription as string,
        file: this.newItem.file,
        quantity: this.newItem.quantity as number,
        price: this.newItem.price as number,
        laserStdId: this.newItem.laserStdId,
        plotterStdId: this.newItem.plotterStdId,
      };

      this.newOrderWithItem.ItemsWithFiles?.push(item);
      this.resetNewItem();
      console.log('item aggiunto: ', item);
    } else {
      Swal.fire({
        icon: 'warning',
        title: 'Attenzione!',
        text: 'Compila tutti i campi per aggiungere un articolo.'
      });
    }
  }

  resetNewItem() {
    //Assegna un valore di default a workTypeId se workType è disponibile
    if (this.workType.length > 0) {
      this.newItem.workTypeId = this.workType[0].id;
    } else {
      this.newItem.workTypeId = undefined;
    }
    this.newItem = {
      workTypeId: undefined,
      workDescription: '',
      file: null,
      quantity: 1,
      price: 0
    };

    // Resetta i campi relativi ai tipi di lavoro
    this.laserStandardId = undefined;
    this.plotterStandardId = undefined;
    this.isLaserStandardSelected = false;
    this.isPlotterSelected = false;
    this.isLaserCustom = false; // Resetta per laser personalizzato
    this.isPlotterCustom = false; // Resetta per plotter personalizzato
  }

  removeItem(item: iItem) {
    Swal.fire({
      title: 'Sei sicuro di voler eliminare questo articolo?',
      text: 'Non potrai recuperarlo dopo l\'eliminazione!',
      icon: 'warning',
      showCancelButton: true,
      confirmButtonColor: '#d33',
      cancelButtonColor: '#3085d6',
      confirmButtonText: 'Elimina',
      cancelButtonText: 'Annulla'
    }).then((result) => {
      if (result.isConfirmed) {
        const index = this.newOrderWithItem.ItemsWithFiles?.indexOf(item);
        if (index !== -1 && this.newOrderWithItem.ItemsWithFiles) {
          this.newOrderWithItem.ItemsWithFiles.splice(index!, 1);
        }
      }
    });
  }

  onSubmit(form: NgForm) {
    console.log("Form valido:", form.valid);
    console.log("Form valori:", form.value);
    console.log("Form controlli:", form.controls);
    console.log("New Order:", this.newOrder);
    console.log('Work Type ID:', this.newItem.workTypeId);
    console.log('newOrderWithItem: ', this.newOrderWithItem);

    if (form.valid && this.newOrderWithItem.ItemsWithFiles.length > 0) {

        this.orderService.createOrder(this.newOrderWithItem).subscribe(
            (response) => {
                Swal.fire({
                    icon: 'success',
                    title: 'Ordine Creato',
                    text: 'Il nuovo ordine è stato creato con successo.'
                });
                this.activeModal.close();
            },
            (error) => {
                let errorMessage = error.error?.message || 'Si è verificato un errore imprevisto.';
                if (error.status === 400) {
                    errorMessage = "Dati non validi. Controlla i campi del form.";
                }
                Swal.fire({
                    icon: 'error',
                    title: 'Errore nella creazione dell\'ordine',
                    text: errorMessage
                });
            }
        );
    } else {
        Swal.fire({
            icon: 'error',
            title: 'Errore!',
            text: 'Per favore, riempi tutti i campi e aggiungi almeno un articolo.'
        });
    }
}

  openClientModal(event: MouseEvent) {
    event.preventDefault();
    this.modalService.openClientModal();
  }

  validateNewItem(): boolean {
    if (!this.newItem.workTypeId || !this.newItem.workDescription || this.newItem.quantity! <= 0) {
      console.error("Invalid workTypeId or other fields missing");
      Swal.fire({
        icon: 'warning',
        title: 'Attenzione!',
        text: 'Compila tutti i campi obbligatori per l\'articolo.'
      });
      return false;
    }

    if (this.isLaserStandardSelected && !this.laserStandardId) {
      Swal.fire({
        icon: 'warning',
        title: 'Attenzione!',
        text: 'Seleziona uno standard laser.'
      });
      return false;
    }

    if (this.isPlotterSelected && !this.plotterStandardId) {
      Swal.fire({
        icon: 'warning',
        title: 'Attenzione!',
        text: 'Seleziona uno standard plotter.'
      });
      return false;
    }

    if (!this.newItem.price || this.newItem.price <= 0) {
      Swal.fire({
        icon: 'warning',
        title: 'Attenzione!',
        text: 'Inserisci un prezzo valido.'
      });
      return false;
    }

    return true;
  }

  // Metodi per gestire la selezione di articoli personalizzati
  resetLaserFields() {
    if (!this.isLaserCustom) {
      this.newItem.workDescription = ''; // Reset della descrizione
      this.newItem.quantity = 1; // Reset della quantità
    }
  }

  resetPlotterFields() {
    if (!this.isPlotterCustom) {
      this.newItem.workDescription = ''; // Reset della descrizione
      this.newItem.quantity = 1; // Reset della quantità
    }
  }

  getWorkTypeName(workTypeId: number | undefined): string {
    const workType = this.workType.find(wt => wt.id === workTypeId);
    return workType ? workType.name : 'Tipo di lavoro sconosciuto';
  }

  onLaserStandardSelect(event: Event) {
    const selectElement = event.target as HTMLSelectElement;
    const selectedValue = Number(selectElement.value);

    // Verifica se l'ID selezionato esiste
    const selectedLaser = this.laserStandard.find(laser => laser.id === selectedValue);
    if (selectedLaser) {
        this.laserStandardId = selectedLaser.id;
        this.newItem.laserStdId = selectedLaser.id;
        this.newItem.workDescription = selectedLaser.description;
        // Potresti anche impostare qui il prezzo se lo desideri
        this.newItem.price = selectedLaser.price; // Assicurati che questo campo sia presente
    }
}

onPlotterStandardSelect(event: Event) {
  const selectElement = event.target as HTMLSelectElement; // Cast dell'event.target
  const selectedValue = Number(selectElement.value); // Converti il valore in un numero

  // Verifica se selectedValue è un numero valido prima di confrontarlo
  const selectedPlotter = this.plotterStandard.find(plotter => plotter.id === selectedValue);

  if (selectedPlotter) {
    // Aggiorna il plotterStdId con il valore selezionato
    this.plotterStandardId = selectedPlotter.id;
    this.newItem.plotterStdId = selectedPlotter.id;
    // Aggiorna la descrizione dell'articolo
    this.newItem.workDescription = selectedPlotter.description;
    this.newItem.price = selectedPlotter.price
  }
}
}
