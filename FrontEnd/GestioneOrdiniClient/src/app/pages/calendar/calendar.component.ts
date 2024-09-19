import { Component, LOCALE_ID, Inject, ViewChild, TemplateRef, OnInit } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { CalendarEvent, CalendarEventTimesChangedEvent, CalendarView, DAYS_OF_WEEK } from 'angular-calendar';
import { subDays, startOfDay, addDays, isSameMonth, isSameDay, endOfDay, format } from 'date-fns';
import { it } from 'date-fns/locale';
import { HttpClient } from '@angular/common/http'; // Importa HttpClient
import { environment } from '../../../environments/environment.development'; // Importa l'environment

// Formatta una data come test per assicurarti che funzioni in italiano
format(new Date(), 'P', { locale: it });

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

  url: string = environment.standardApi.getAll(environment.baseUrl + '/Order'); // URL API per gli ordini

  @ViewChild('modalContent', { static: true }) modalContent: TemplateRef<any> | undefined;

  CalendarView = CalendarView;

  view: CalendarView = CalendarView.Month;
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
    private http: HttpClient  // Inietta HttpClient
  ) {}

  ngOnInit(): void {
    this.fetchEvents();
  }

  fetchEvents(): void {
    this.http.get<any[]>(this.url).subscribe((data) => {
      this.events = data.map(event => ({
        start: new Date(event.startDate), // Assicurati che il formato della data sia corretto
        end: new Date(event.endDate),
        title: event.title,
        color: colors.blue,
      }));
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

  deleteEvent(eventToDelete: CalendarEvent): void {
    this.events = this.events.filter((event) => event !== eventToDelete);
  }

  setView(view: CalendarView): void {
    this.view = view;
  }

  closeOpenMonthViewDay(): void {
    this.activeDayIsOpen = false;
  }
}
