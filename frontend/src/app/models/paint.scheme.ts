import {Step} from "./step";

export interface PaintScheme {
  id?: number;
  name: string;
  description?: string;
  tags?: string[];
  userId?: number;
  createdAt?: string;
  isStolen?: boolean;
  imageUrl?: string;
  steps: Step[];
}
