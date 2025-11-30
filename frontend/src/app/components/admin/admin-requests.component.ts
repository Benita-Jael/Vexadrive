import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup } from '@angular/forms';
import { ServiceRequestService } from '../../services/service-request/service-request.service';
import { ServiceRequestDetailsDTO } from '../../models/service-request/service-request-details.dto';
import { ServiceStatus } from '../../models/service-request/service-status.enum';

@Component({
  selector: 'app-admin-requests',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './admin-requests.component.html',
  styleUrls: ['./admin-requests.component.css']
})
export class AdminRequestsComponent implements OnInit {
  requests: any[] = [];
  ongoingRequests: any[] = [];
  completedRequests: any[] = [];
  loading = false;
  selectedRequest: any | null = null;
  showUpdateForm = false;
  updateForm: FormGroup | undefined;
  uploading = false;
  selectedFile: File | null = null;
  errorMessage: string | null = null;
  successMessage: string | null = null;
  showBillConfirm = false;
  billConfirmRequest: any = null;
  uploadedRequestIds: Set<number> = new Set(); // Track which requests have bills uploaded
  statuses = Object.values(ServiceStatus).filter(v => typeof v === 'number') as unknown as number[];
  searchForm!: FormGroup;
  showSearch = false;

  constructor(private srService: ServiceRequestService, private fb: FormBuilder) {}

  ngOnInit() {
    this.searchForm = this.fb.group({
      vehicleType: [''],
      plateNumber: [''],
      customerEmail: ['']
    });
    this.loadAll();
  }

  loadAll() {
    this.loading = true;
    this.srService.getAllRequests().subscribe({
      next: (data) => {
        this.requests = data;
        // Track which requests already have bills
        this.uploadedRequestIds.clear();
        this.requests.forEach(r => {
          if (r.bill) {
            this.uploadedRequestIds.add(r.serviceRequestId);
          }
        });
        this.separateRequests();
        this.loading = false;
      },
      error: (err) => {
        console.error('Failed to load admin requests', err);
        this.loading = false;
      }
    });
  }

  separateRequests() {
    this.ongoingRequests = this.requests.filter(r => r.status !== 'ServiceCompleted');
    this.completedRequests = this.requests.filter(r => r.status === 'ServiceCompleted');
  }

  search() {
    this.loading = true;
    const { vehicleType, plateNumber, customerEmail } = this.searchForm.value;
    this.srService.searchRequests(vehicleType || undefined, plateNumber || undefined, customerEmail || undefined).subscribe({
      next: (data) => {
        this.requests = data;
        this.separateRequests();
        this.loading = false;
      },
      error: (err) => {
        console.error('Search failed', err);
        this.loading = false;
      }
    });
  }

  resetSearch() {
    this.searchForm.reset();
    this.loadAll();
    this.showSearch = false;
  }

  toggleSearch() {
    this.showSearch = !this.showSearch;
  }


  onFileSelected(event: Event, request?: any) {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      this.selectedFile = input.files[0];
      if (request) {
        this.selectedRequest = request;
      }
      // Show confirmation dialog
      this.showBillConfirm = true;
      this.billConfirmRequest = request;
    }
  }


  confirmUploadBill() {
    const targetRequest = this.billConfirmRequest || this.selectedRequest;
    if (!targetRequest || !this.selectedFile) {
      this.errorMessage = 'Please select a request and a bill file before uploading.';
      this.showBillConfirm = false;
      return;
    }
    this.uploading = true;
    this.srService.uploadBill(targetRequest.serviceRequestId, this.selectedFile).subscribe({
      next: (res: any) => {
        this.uploading = false;
        this.showBillConfirm = false;
        // Mark request as having a bill uploaded (freeze the input)
        this.uploadedRequestIds.add(targetRequest.serviceRequestId);
        // prefer message returned by backend
        if (res && typeof res === 'object' && (res.message || res.msg)) {
          this.successMessage = res.message || res.msg;
        } else if (typeof res === 'string' && res.length > 0) {
          this.successMessage = res;
        } else {
          this.successMessage = 'Bill uploaded successfully';
        }
        // Clear selected file and request
        this.selectedFile = null;
        this.selectedRequest = null;
        this.billConfirmRequest = null;
        // reload requests to pick up bill info
        setTimeout(() => this.loadAll(), 1000);
      },
      error: (err: any) => {
        this.uploading = false;
        this.showBillConfirm = false;
        const payload = err.error;
        if (payload) this.errorMessage = payload.message || (typeof payload === 'string' ? payload : JSON.stringify(payload));
        else this.errorMessage = 'Uploading bill failed. Try again.';
      }
    });
  }

  cancelUploadBill() {
    this.showBillConfirm = false;
    this.selectedFile = null;
    this.selectedRequest = null;
    this.billConfirmRequest = null;
  }

  changeStatus(r: any, status: ServiceStatus) {
    this.srService.updateStatus(r.serviceRequestId, status).subscribe({
      next: (updated) => {
        this.loadAll();
      },
      error: (err) => console.error('Status update failed', err)
    });
  }

  updateEtd(r: any) {
    const etd = prompt('Set ETD (YYYY-MM-DD):', r.estimatedDeliveryDateOnly ?? (r.estimatedDeliveryDate ?? ''));
    if (!etd) return;
    this.srService.updateEtd(r.serviceRequestId, etd).subscribe({
      next: () => this.loadAll(),
      error: (err: any) => console.error('ETD update failed', err)
    });
  }

  getStatusBadgeClass(status: any): string {
    const statusStr = typeof status === 'string' ? status : status.toString();
    switch (statusStr.toLowerCase()) {
      case 'requestcreated':
        return 'bg-info';
      case 'serviceinprogress':
        return 'bg-warning';
      case 'servicecompleted':
        return 'bg-success';
      default:
        return 'bg-secondary';
    }
  }

  deleteRequest(request: any) {
    if (!confirm(`Delete request #${request.serviceRequestId}? This action cannot be undone.`)) return;
    this.srService.deleteRequest(request.serviceRequestId).subscribe({
      next: () => {
        this.successMessage = 'Request deleted';
        this.loadAll();
      },
      error: (err: any) => {
        this.errorMessage = err.error?.message || 'Failed to delete request';
      }
    });
  }
}