export interface VehicleCreateDto {
  model: string;
  numberPlate?: string;
  type: string;   // e.g., Car, Bike
  color: string;
  ownerId: number;
}
