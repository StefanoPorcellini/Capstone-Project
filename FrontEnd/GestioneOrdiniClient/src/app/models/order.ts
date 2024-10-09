import { iCustomer } from './customer';
export interface iOrder {
  id?: number;               // ID dell'ordine (opzionale per la creazione)
  CustomerId?: number;            // ID del cliente
  description: string;           // Descrizione dell'ordine (opzionale)
  date: string;                  // Data dell'ordine (opzionale, formato date-time)
  maxDeliveryDate?: string;       // Data massima di consegna (opzionale)
  totalAmount?: number;            // Totale dell'ordine
  operatorName?: string;          // Nome dell'operatore (opzionale)
  statusId?: number;              // ID dello stato dell'ordine (opzionale)
  statusName?: string;            // Nome dello stato dell'ordine (opzionale)
  // items: iItem[];                 // Lista degli articoli dell'ordine (richiesta)
  customer?: iCustomer
}

export interface iItem {
  id?: number;                    // ID dell'articolo (opzionale)
  workTypeId: number;             // ID del tipo di lavoro
  workDescription: string;         // Descrizione
  // file?: File | null;             // File allegato all'item (opzionale)
  quantity: number;                // Quantità dell'articolo
  price: number;                   // Prezzo dell'articolo
  laserStdId?: number;
  plotterStdId?: number;
  base?: number;
  height?: number;
  pricePerSqMeter?: number;
  fileName?: string;
  filePath?:string;
}

export interface iStatus {
  statusId: number;               // ID dello stato
  statusName: string;             // Nome dello stato
}

export interface iWorkType {
  id: number;                     // ID del tipo di lavoro
  name: string;                   // Nome del tipo di lavoro
}

export interface OrderWithItemsDto {
  Order: iOrder;             // Usando l'interfaccia iOrder
  ItemsWithFiles: ItemWithFileDto[];   // Lista degli articoli con file allegati
}

export interface ItemWithFileDto extends iItem {
  file?: File | null; // Aggiungi la proprietà file qui
}

