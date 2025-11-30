export interface BillDetailsDto {
  billId: number;
  serviceRequestId: number;
  fileName: string;
  contentType: string;
  storagePath: string;
  uploadedAt: string;
}
