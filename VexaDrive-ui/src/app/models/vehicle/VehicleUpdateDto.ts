export interface VehicleUpdateDto {
  vehicleId: number;
  model?: string;
  numberPlate?: string;
  type?: string;
  color?: string;
  ownerId?: number;
}
