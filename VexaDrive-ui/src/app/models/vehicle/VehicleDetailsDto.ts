export interface VehicleDetailsDto {
  vehicleId: number;
  model?: string;
  numberPlate?: string;
  type?: string;
  color?: string;
  ownerId: number;
  ownerFullName?: string;
  ownerContact?: string;
  ownerEmail?: string;
}
