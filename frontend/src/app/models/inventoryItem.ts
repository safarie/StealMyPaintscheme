import {Paint} from "./paint";

export interface InventoryItem {
  id?: number;
  quantity: number;
  paintId: number;
  paint?: Paint;
  userId?: number;
}
