import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ReactiveFormsModule, FormBuilder, Validators, FormGroup } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { ServiceRequestService } from '../../../services/service-request/service-request.service';
import { ServiceRequestDetailsDTO } from '../../../models/service-request/service-request-details.dto';
import { environment } from '../../../../environments/environment';

@Component({
  selector: 'app-admin-requests',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule],
  templateUrl: './admin-requests.component.html',
  styleUrls: ['./admin-requests.component.css']
})
export class AdminRequestsComponent implements OnInit {
  allRequests: ServiceRequestDetailsDTO[] = [];
  filteredRequests: ServiceRequestDetailsDTO[] = [];
  loading = false;
  selectedRequest: ServiceRequestDetailsDTO | null = null;
  showUpdateForm = false;
  updateForm!: FormGroup;
  uploading = false;
  selectedFile: File | null = null;
  errorMessage: string | null = null;
  successMessage: string | null = null;

  filterStatus = 'all';
  searchText = '';

  constructor(
    private srService: ServiceRequestService,
    private fb: FormBuilder
  ) {}

  ngOnInit() {
    this.updateForm = this.fb.group({
      status: ['', Validators.required],
      estimatedDeliveryDate: ['']
    });

    this.loadRequests();
  }

  loadRequests() {
    this.loading = true;
    this.srService.getAllRequests().subscribe({
      next: (data: ServiceRequestDetailsDTO[]) => {
        this.allRequests = data;
        this.applyFilters();
        this.loading = false;
      },
      error: (err: any) => {
        console.error('Failed to load requests', err);
        this.loading = false;
        this.errorMessage = 'Failed to load requests';
      }
    });
  }

  applyFilters() {
    this.filteredRequests = this.allRequests.filter(req => {
      const matchesStatus = this.filterStatus === 'all' || 
        (req.status && req.status.toLowerCase() === this.filterStatus.toLowerCase());
      const matchesSearch = !this.searchText || 
        req.vehicleModel?.toLowerCase().includes(this.searchText.toLowerCase()) ||
        req.problemDescription?.toLowerCase().includes(this.searchText.toLowerCase());
      return matchesStatus && matchesSearch;
    });
  }

  selectRequest(request: ServiceRequestDetailsDTO) {
    this.selectedRequest = request;
    this.showUpdateForm = true;
    this.errorMessage = null;
    this.successMessage = null;
    this.updateForm.reset({
      status: request.status || '',
      // prefer date-only string from backend if provided
      estimatedDeliveryDate: request.estimatedDeliveryDateOnly ? request.estimatedDeliveryDateOnly : (request.estimatedDeliveryDate ? new Date(request.estimatedDeliveryDate).toISOString().split('T')[0] : '')
    });
  }

  closeUpdate() {
    this.showUpdateForm = false;
    this.selectedRequest = null;
    this.selectedFile = null;
  }

  submitUpdate() {
    if (!this.selectedRequest || this.updateForm.invalid) {
      this.errorMessage = 'Please fill in all required fields';
      return;
    }

    const { status, estimatedDeliveryDate } = this.updateForm.value;
    
    // Update status if changed
    if (status && status !== this.selectedRequest.status) {
      this.srService.updateStatus(this.selectedRequest.serviceRequestId, status).subscribe({
        next: () => {
          this.successMessage = 'Status updated successfully';
          setTimeout(() => this.loadRequests(), 1000);
        },
        error: (err: any) => {
          this.errorMessage = err.error?.message || 'Failed to update status';
        }
      });
    }

    // Update ETD if provided
    if (estimatedDeliveryDate) {
      const etdDate = estimatedDeliveryDate; // expected as yyyy-MM-dd from input
      this.srService.updateEtd(this.selectedRequest.serviceRequestId, etdDate).subscribe({
        next: (updated: ServiceRequestDetailsDTO) => {
          this.successMessage = 'ETD updated successfully';
          // update local list with returned value
          this.replaceRequestInLists(updated);
        },
        error: (err: any) => {
          this.errorMessage = err.error?.message || 'Failed to update ETD';
        }
      });
    }
  }

  changeStatus(request: ServiceRequestDetailsDTO, newStatus: string) {
    if (!request) return;
    this.srService.updateStatus(request.serviceRequestId, newStatus as any).subscribe({
      next: (updated: ServiceRequestDetailsDTO) => {
        this.successMessage = 'Status updated successfully';
        this.replaceRequestInLists(updated);
      },
      error: (err: any) => {
        this.errorMessage = err.error?.message || 'Failed to update status';
      }
    });
  }

  deleteRequest(request: ServiceRequestDetailsDTO) {
    if (!confirm(`Delete request #${request.serviceRequestId}? This action cannot be undone.`)) return;
    this.srService.deleteRequest(request.serviceRequestId).subscribe({
      next: () => {
        this.successMessage = 'Request deleted';
        // remove from lists
        this.allRequests = this.allRequests.filter(r => r.serviceRequestId !== request.serviceRequestId);
        this.applyFilters();
      },
      error: (err: any) => {
        this.errorMessage = err.error?.message || 'Failed to delete request';
      }
    });
  }

  private replaceRequestInLists(updated: ServiceRequestDetailsDTO) {
    // update in allRequests
    this.allRequests = this.allRequests.map(r => r.serviceRequestId === updated.serviceRequestId ? updated : r);
    this.applyFilters();
    // ensure selectedRequest is up-to-date
    if (this.selectedRequest?.serviceRequestId === updated.serviceRequestId) this.selectedRequest = updated;
  }

  onFileSelected(event: Event) {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      this.selectedFile = input.files[0];
    }
  }

  uploadBill() {
    if (!this.selectedRequest || !this.selectedFile) {
      this.errorMessage = 'Please select a bill file first';
      return;
    }

    const targetRequest = this.selectedRequest;
    if (!targetRequest || !this.selectedFile) {
      this.errorMessage = 'Please select a request and a bill file before uploading.';
      return;
    }

    this.uploading = true;
    this.srService.uploadBill(targetRequest.serviceRequestId, this.selectedFile).subscribe({
      next: (res: any) => {
        this.uploading = false;
        // prefer message returned by backend
        if (res && typeof res === 'object' && (res.message || res.msg)) {
          this.successMessage = res.message || res.msg;
        } else if (typeof res === 'string' && res.length > 0) {
          this.successMessage = res;
        } else {
          this.successMessage = 'Bill uploaded successfully';
        }
        this.selectedFile = null;
        // reload requests to pick up bill info
        setTimeout(() => this.loadRequests(), 1000);
      },
      error: (err: any) => {
        this.uploading = false;
        const payload = err.error;
        if (payload) this.errorMessage = payload.message || (typeof payload === 'string' ? payload : JSON.stringify(payload));
        else this.errorMessage = 'Uploading bill failed. Try again.';
      }
    });
  }

  getStatusBadgeClass(status: any): string {
    const statusStr = typeof status === 'string' ? status : status?.toString() || '';
    switch (statusStr.toLowerCase()) {
      case 'requestcreated':
        return 'bg-info';
      case 'serviceinprogress':
        return 'bg-warning';
      case 'servicecompleted':
        return 'bg-success';
      case 'readyforpickup':
        return 'bg-primary';
      default:
        return 'bg-secondary';
    }
  }

  formatStatus(status: any): string {
    const statusStr = typeof status === 'string' ? status : status?.toString() || '';
    switch (statusStr.toLowerCase()) {
      case 'requestcreated':
        return '📝 Request Created';
      case 'serviceinprogress':
        return '🔧 In Progress';
      case 'servicecompleted':
        return '✓ Completed';
      case 'readyforpickup':
        return '📦 Ready for Pickup';
      default:
        return statusStr;
    }
  }
}
