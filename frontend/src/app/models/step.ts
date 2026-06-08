import {Paint} from "./paint";

export interface Step {
  id?: number;
  where: string;
  colour: string;
  paintingTechnique: string;
  paintId?: number;
  paint?: Paint;
}
