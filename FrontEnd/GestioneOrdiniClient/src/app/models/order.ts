export interface iOrder {
  orderId?: number;                      // ID dell'ordine (opzionale per la creazione)
  customerId?: number;                   // ID del cliente
  customerName: string;                  // Nome del cliente (obbligatorio)
  customerAddress?: string;              // Indirizzo del cliente
  customerEmail?: string;                // Email del cliente
  customerTel: string;                   // Telefono del cliente (obbligatorio)
  customerOrders?: any[];                // Altri ordini del cliente (opzionale)

  description?: string;                  // Descrizione dell'ordine (opzionale)
  date?: string;                         // Data dell'ordine (opzionale, formato date-time)
  maxDeliveryDate?: string;              // Data massima di consegna (opzionale, formato date-time)
  totalAmount?: number;                  // Totale dell'ordine (opzionale)

  operatorName?: string;                 // Nome dell'operatore (opzionale)
  statusId?: number;                     // ID dello stato dell'ordine (opzionale)
  statusName?: string;                   // Nome dello stato dell'ordine (opzionale)
  statusOrders?: any[];                  // Ordini correlati allo stato (opzionale)

  items?: any[];                         // Lista degli articoli dell'ordine (opzionale)

  file?: File | null;                    // File allegato all'ordine (opzionale, formato binario)

}

export interface iStatus{
  statusId?: number
  statusName: string
}
