export interface iLaserPriceList {
  id: number;
  minQuantity: number;
  maxQuantity: number;
  unitPrice: number;
  description: string;
  laserItemId: number
}

export interface iLaserStandard {
  id: number;
  descripiton: string;
  price: number;
}

export interface iPlotterStandard {
  id: number;
  descripiton: string;
  price: number;
}
