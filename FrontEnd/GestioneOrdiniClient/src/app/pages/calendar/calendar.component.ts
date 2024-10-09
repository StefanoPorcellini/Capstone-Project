import { OrderService } from './../../services/order.service';
import { SignalRService } from './../../services/signal-r.service'; // Importa SignalRService
import { Component, LOCALE_ID, Inject, ViewChild, TemplateRef, OnInit } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { CalendarEvent, CalendarEventTimesChangedEvent, CalendarView, DAYS_OF_WEEK } from 'angular-calendar';
import { startOfDay, endOfDay, isSameMonth, isSameDay } from 'date-fns'; // Importa le funzioni di date-fns
import { toZonedTime } from 'date-fns-tz'; // Importa date-fns-tz
import { iOrder } from '../../models/order';
import { ModalService } from '../../services/modal.service';
import Swal from 'sweetalert2';

const colors: any = {
  red: {
    primary: '#ad2121',
    secondary: '#FAE3E3',
  },
  blue: {
    primary: '#1e90ff',
    secondary: '#D1E8FF',
  },
  yellow: {
    primary: '#e3bc08',
    secondary: '#FDF1BA',
  },
};

@Component({
  selector: 'app-calendar',
  templateUrl: './calendar.component.html',
  styleUrls: ['./calendar.component.scss']
})
export class CalendarComponent implements OnInit {

  @ViewChild('modalContent', { static: true }) modalContent: TemplateRef<any> | undefined;

  CalendarView = CalendarView;

  // Set the default view to 'Week'
  view: CalendarView = CalendarView.Week;
  viewDate: Date = new Date();
  weekStartsOn: number = DAYS_OF_WEEK.MONDAY;
  weekendDays: number[] = [DAYS_OF_WEEK.SATURDAY, DAYS_OF_WEEK.SUNDAY];

  modalData: {
    action: string;
    event: CalendarEvent;
  } | undefined;

  events: CalendarEvent[] = [];
  activeDayIsOpen: boolean = true;

  constructor(
    @Inject(LOCALE_ID) public locale: string,
    private modal: NgbModal,
    private orderService: OrderService,
    private signalRService: SignalRService, // Inietta SignalRService
    private modalService: ModalService // Inietta il ModalService
  ) {}

  ngOnInit(): void {
    this.fetchEvents();
    this.signalRService.startConnection(); // Avvia la connessione SignalR
    this.listenForRealTimeUpdates(); // Ascolta gli aggiornamenti in tempo reale
    // Ascolta l'evento di chiusura del modale
    this.modalService.modalClosed.subscribe(() => {
      this.fetchEvents(); // Aggiorna gli eventi quando un modale viene chiuso
    });
  }

  // Metodo per ottenere gli ordini dal backend
  fetchEvents(): void {
    this.orderService.getOrders().subscribe((orders: iOrder[]) => {
      console.log('Ordini ricevuti:', orders); // Aggiungi questo log
      this.mapOrdersToEvents(orders);
    });
  }

  // Mappa gli ordini sugli eventi del calendario
  private mapOrdersToEvents(orders: iOrder[]): void {
    const timeZone = 'Europe/Rome'; // Specifica il fuso orario
    this.events = []; // Inizializza la lista di eventi

    this.events = orders.map(order => {
      const startZoned = toZonedTime(new Date(order.date), timeZone); // Converte la data in locale

      // Aggiungi un log per il controllo della data di inizio
      console.log('Data di inizio per l\'ordine:', startZoned);

      const endZoned = order.maxDeliveryDate
        ? toZonedTime(new Date(order.maxDeliveryDate), timeZone) // Usa maxDeliveryDate se disponibile
        : new Date(startZoned.getTime() + 2 * 60 * 60 * 1000); // Imposta una durata predefinita (es. 2 ore)
        const isWeekday = startZoned.getDay() >= 1 && startZoned.getDay() <= 5; // 1=Monday, 5=Friday


      // Aggiungi un log per il controllo della data di fine
      console.log('Data di fine per l\'ordine:', endZoned);

      return {
        start: startZoned,
        end: endZoned,
        title: `${order.description || 'Ordine senza descrizione'}`,
        color: colors.blue,
        allDay: false, // Imposta allDay su false
        id: order.id
      };
    });

    // Log per controllare gli eventi finali
    console.log('Eventi mappati:', this.events);
  }

  // Ascolta gli aggiornamenti in tempo reale da SignalR
  private listenForRealTimeUpdates(): void {
    this.signalRService.addCustomerUpdateListener((message: string) => {
      try {
        const orders: iOrder[] = JSON.parse(message); // Parsing della stringa in un array di ordini
        this.mapOrdersToEvents(orders); // Aggiorna gli eventi con i nuovi ordini
      } catch (error) {
        console.error('Errore nel parsing del messaggio SignalR', error);
      }
    });
  }

  dayClicked({ date, events }: { date: Date; events: CalendarEvent[] }): void {
    if (isSameMonth(date, this.viewDate)) {
      if (
        (isSameDay(this.viewDate, date) && this.activeDayIsOpen === true) ||
        events.length === 0
      ) {
        this.activeDayIsOpen = false;
      } else {
        this.activeDayIsOpen = true;
      }
      this.viewDate = date;
    }
  }

  eventTimesChanged({
    event,
    newStart,
    newEnd,
  }: CalendarEventTimesChangedEvent): void {
    this.events = this.events.map((iEvent) => {
      if (iEvent === event) {
        return {
          ...event,
          start: newStart,
          end: newEnd,
        };
      }
      return iEvent;
    });
    this.handleEvent('Dropped or resized', event);
  }

  handleEvent(action: string, event: CalendarEvent): void {
    this.modalData = { event, action };
    this.modal.open(this.modalContent, { size: 'lg' });
  }

  addEvent(): void {
    this.events = [
      ...this.events,
      {
        title: 'New event',
        start: startOfDay(new Date()),
        end: endOfDay(new Date()),
        color: colors.red,
        draggable: true,
        resizable: {
          beforeStart: true,
          afterEnd: true,
        },
      },
    ];
  }

  // Funzione per eliminare un evento
  deleteEvent(eventToDelete: CalendarEvent): void {
    const orderId = this.extractOrderId(eventToDelete);
    console.log('ID ordine da eliminare:', orderId); // Log per vedere l'ID

    if (orderId) {
      Swal.fire({
        title: 'Sei sicuro di voler eliminare questo ordine?',
        showCancelButton: true,
        confirmButtonText: 'Sì, elimina',
        cancelButtonText: 'Annulla'
      }).then((result) => {
        if (result.isConfirmed) {
          this.orderService.deleteOrder(parseInt(orderId)).subscribe(() => {
            this.events = this.events.filter((event) => event !== eventToDelete);
            Swal.fire('Eliminato!', 'L\'ordine è stato eliminato.', 'success');
          }, (error) => {
            console.error('Errore durante l\'eliminazione dell\'ordine:', error);
            Swal.fire('Errore!', 'Non è stato possibile eliminare l\'ordine.', 'error');
          });
        }
      });
    } else {
      console.error('ID ordine non valido:', orderId);
      Swal.fire('Errore!', 'Non è stato possibile trovare l\'ID dell\'ordine.', 'error');
    }
  }

  // Funzione per modificare un evento
  editEvent(eventToEdit: CalendarEvent): void {
    const orderId = this.extractOrderId(eventToEdit); // Funzione per estrarre l'ID dell'ordine
    if (orderId) {
      // Qui dovresti aprire un modale per modificare l'ordine
      // Per esempio, potresti avere un metodo `openEditModal` per gestire l'apertura del modale
      this.openEditModal(orderId); // Passa l'ID dell'ordine al modale
    }
  }

  // Metodo per estrarre l'ID dell'ordine dall'evento
  private extractOrderId(event: CalendarEvent): string | null {
    // Usa la proprietà personalizzata 'id' per ottenere l'ID dell'ordine
    return event.id ? event.id.toString() : null; // Restituisci l'ID se esiste
  }


  private openEditModal(orderId: string): void {
    // Logica per aprire il modale di modifica dell'ordine
    // Potresti avere un altro servizio per ottenere i dettagli dell'ordine da modificare
    this.orderService.getOrderById(parseInt(orderId)).subscribe(order => {
      // Qui apri il modale con i dettagli dell'ordine
      // E.g., this.modal.open(EditOrderComponent, { data: order });
    });
  }

  setView(view: CalendarView): void {
    this.view = view;
  }

  closeOpenMonthViewDay(): void {
    this.activeDayIsOpen = false;
  }
}
