export interface ServiceRequestCreateDTO {
  vehicleId: number;
  problemDescription: string;
  serviceAddress: string;
  serviceDate: string; // ISO string
}
