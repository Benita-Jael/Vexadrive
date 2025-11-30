export interface VehicleDetailsDto {
  vehicleId: number;
  model?: string;
  numberPlate?: string;
  type?: string;
  color?: string;
  // Matches backend: CustomerUserId is IdentityUser.Id
  customerUserId?: string;
  customerEmail?: string;
}
