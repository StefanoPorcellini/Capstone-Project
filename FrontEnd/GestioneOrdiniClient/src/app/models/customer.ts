export interface iCustomer {
  id?: number;          // Facoltativo, presente solo quando il cliente è già esistente
  customerType: 'private' | 'company';
  name: string;
  address?: string;      // Facoltativo
  email?: string;        // Facoltativo
  tel?: string;          // Facoltativo
  cf?: string;           // Necessario solo per i privati
  partitaIVA?: string;   // Necessario solo per le aziende
  ragioneSociale?: string; // Necessario solo per le aziende
}
