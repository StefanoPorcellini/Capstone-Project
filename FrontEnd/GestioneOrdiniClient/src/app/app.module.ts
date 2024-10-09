import { LOCALE_ID, NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { adapterFactory } from 'angular-calendar/date-adapters/date-fns';
import { registerLocaleData } from '@angular/common';
import localeIt from '@angular/common/locales/it';
import { UserModal } from './modals/user-modal/user-modal.component';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { DashboardComponent } from './pages/dashboard/dashboard.component';
import { HTTP_INTERCEPTORS, HttpClientModule } from '@angular/common/http';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { AuthInterceptor } from './auth/auth-interceptor.interceptor';
import { NavbarComponent } from './navbar/navbar/navbar.component';
import {
  CalendarModule,
  DateAdapter,
  CalendarDateFormatter,
} from 'angular-calendar';
import { CalendarComponent } from './pages/calendar/calendar.component';
import { Time24Pipe } from './pipe/time24.pipe';
import { FormsModule, NgForm } from '@angular/forms';
import { ClientModal } from './modals/client-modal/client-modal.component';
import { PriceList } from './modals/price-list/price-list.component';
import { CustomDateFormatter } from './tools/CustomDateFormatter';
import { OrderModal } from './modals/order-modal/order-modal.component';

registerLocaleData(localeIt);

@NgModule({
  declarations: [
    AppComponent,
    DashboardComponent,
    NavbarComponent,
    CalendarComponent,
    Time24Pipe,
    UserModal,
    ClientModal,
    PriceList,
    OrderModal
  ],
  imports: [
    BrowserModule,
    FormsModule,
    AppRoutingModule,
    HttpClientModule,
    NgbModule,
    CalendarModule.forRoot(
      {
        provide: DateAdapter,
        useFactory: adapterFactory,
      },
      {
        dateFormatter: {
          provide: CalendarDateFormatter,
          useClass: CustomDateFormatter, // Usa la nuova classe CustomDateFormatter
        },
      }
    ),
  ],
  providers: [
    { provide: HTTP_INTERCEPTORS, useClass: AuthInterceptor, multi: true },
    { provide: LOCALE_ID, useValue: 'it-IT' }
  ],
  bootstrap: [AppComponent]
})
export class AppModule {}
