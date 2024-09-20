export interface iOrder {
  id: number;
  creationDate: Date;
  deliveryDate: Date;
  statusId: number;
  item: iItem;
}

export interface iItem {
  id: number;
  fileName?: string;
  filePath?: string;
  quantity?: number;
  workType: string;
  description: string;
}

export interface iOrderStatus {
  id: number;
  name: string;
}

export interface OrderWithDto {
  order: iOrder;
  file?: File;
}
