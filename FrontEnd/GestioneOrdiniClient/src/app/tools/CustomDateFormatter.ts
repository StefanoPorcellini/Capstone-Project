import { Injectable } from '@angular/core';
import { CalendarNativeDateFormatter, DateFormatterParams } from 'angular-calendar';
import { format } from 'date-fns';
import { it } from 'date-fns/locale';

@Injectable()
export class CustomDateFormatter extends CalendarNativeDateFormatter {

  // Funzione di utilità per mettere la prima lettera maiuscola
  private capitalizeFirstLetter(value: string): string {
    return value.charAt(0).toUpperCase() + value.slice(1);
  }

  // Override per la formattazione della vista mensile (nomi dei giorni in italiano con maiuscola)
  public override monthViewColumnHeader({ date, locale }: DateFormatterParams): string {
    const dayName = format(date, 'EEEE', { locale: it });
    return this.capitalizeFirstLetter(dayName);  // Metti l'iniziale maiuscola
  }

  // Override per la formattazione del titolo della vista mensile (nome del mese in italiano con maiuscola)
  public override monthViewTitle({ date, locale }: DateFormatterParams): string {
    const monthTitle = format(date, 'MMMM yyyy', { locale: it });
    return this.capitalizeFirstLetter(monthTitle);  // Metti l'iniziale maiuscola
  }

  // Override per la visualizzazione delle ore nella vista settimanale
  public override weekViewHour({ date, locale }: DateFormatterParams): string {
    return new Intl.DateTimeFormat(locale || 'it-IT', {
      hour: 'numeric',
      minute: 'numeric',
    }).format(date);
  }

  // Formattazione per la visualizzazione settimanale (titolo della settimana)
  public override weekViewTitle({ date, locale }: DateFormatterParams): string {
    const startWeek = format(date, 'dd/MM', { locale: it });
    const endWeek = format(new Date(date.getFullYear(), date.getMonth(), date.getDate() + 6), 'dd/MM yyyy', { locale: it });
    return `Settimana ${startWeek} - ${endWeek}`;  // Titolo per l'intervallo della settimana
  }

  // Formattazione per la visualizzazione giornaliera (titolo del giorno in italiano)
  public override dayViewTitle({ date, locale }: DateFormatterParams): string {
    const dayTitle = format(date, 'EEEE, d MMMM yyyy', { locale: it });
    return this.capitalizeFirstLetter(dayTitle);  // Metti l'iniziale maiuscola
  }
}
