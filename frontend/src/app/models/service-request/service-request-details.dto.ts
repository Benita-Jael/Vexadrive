import { ServiceStatus } from './service-status.enum';
import { BillDetailsDto } from '../bill/bill-details.dto';
import { NotificationListDto } from '../notification/notification-list.dto';

export interface ServiceRequestDetailsDTO {
  serviceRequestId: number;
  customerUserId: string;
  vehicleId: number;
  vehicleModel: string;
  problemDescription: string;
  serviceAddress: string;
  serviceDate: string;
  serviceDateOnly?: string;
  status: ServiceStatus;
  createdAt: string;
  createdAtDate?: string;
  updatedAt: string;
  updatedAtDate?: string;
  estimatedDeliveryDate?: string;
  estimatedDeliveryDateOnly?: string;
  ownerName?: string;
  ownerEmail?: string;
  ownerContact?: string;
  bill?: BillDetailsDto;
  notifications: NotificationListDto[];
}
