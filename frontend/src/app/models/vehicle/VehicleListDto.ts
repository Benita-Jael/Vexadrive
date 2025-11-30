export interface VehicleListDto {
  vehicleId: number;
  model?: string;
  numberPlate?: string;
  type?: string;
  color?: string;
  // Identity user id (string) — make optional to accept both list and detail shapes
  ownerId?: string;
}
