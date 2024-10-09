export interface iLaserPriceList {
  id: number;
  minQuantity: number;
  maxQuantity: number;
  unitPrice: number;
  description: string;
}

export interface iLaserStandard {
  id: number;
  description: string;
  price: number;
}

export interface iPlotterStandard {
  id: number;
  description: string;
  price: number;
}
