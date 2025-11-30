import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators, FormGroup } from '@angular/forms';
import { ServiceRequestService } from '../../services/service-request/service-request.service';
import { VehicleService } from '../../services/vehicle/VehicleService';
import { interval, Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { ServiceRequestCreateDTO } from '../../models/service-request/service-request-create.dto';
import { ServiceRequestDetailsDTO } from '../../models/service-request/service-request-details.dto';
import { VehicleCreateDto } from '../../models/vehicle/VehicleCreateDto';

interface Vehicle {
  vehicleId: number;
  model: string;
  numberPlate: string;
  type: string;
  color: string;
}

@Component({
  selector: 'app-service-requests',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './service-requests.component.html',
  styleUrls: ['./service-requests.component.css']
})
export class ServiceRequestsComponent implements OnInit, OnDestroy {
  private destroy$ = new Subject<void>();
  requests: ServiceRequestDetailsDTO[] = [];
  vehicles: Vehicle[] = [];
  loading = false;
  creating = false;
  submitting = false;
  addingVehicle = false;
  addingVehicleLoading = false;
  errorMessage: string | null = null;
  successMessage: string | null = null;
  vehicleTypes = ['Car', 'Scooty', 'Bike', 'SUV', 'Jeep', 'E-Car', 'E-Scooty', 'Other'];

  form!: FormGroup;
  vehicleForm!: FormGroup;

  constructor(private fb: FormBuilder, private srService: ServiceRequestService, private vehicleService: VehicleService) {}

  ngOnInit() {
    this.form = this.fb.group({
      vehicleId: [null, Validators.required],
      problemDescription: ['', [Validators.required, Validators.minLength(10)]],
      serviceAddress: ['', Validators.required],
      serviceDate: [null, Validators.required]
    });

    this.vehicleForm = this.fb.group({
      model: ['', [Validators.required, Validators.minLength(3)]],
      numberPlate: ['', [Validators.required, Validators.minLength(5)]],
      type: ['', Validators.required],
      otherType: [''],
      color: ['', Validators.required]
    });

    this.loadVehicles();
    this.loadRequests();
    // Poll for updates (status changes) every 10 seconds
    interval(10000).pipe(takeUntil(this.destroy$)).subscribe(() => this.loadRequests());
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  loadVehicles() {
    this.srService.getCustomerVehicles().subscribe({
      next: (data: Vehicle[]) => {
        this.vehicles = data;
      },
      error: (err: any) => {
        console.error('Failed to load vehicles', err);
        this.errorMessage = 'Failed to load your vehicles';
      }
    });
  }

  loadRequests() {
    this.loading = true;
    this.srService.getCustomerRequests().subscribe({
      next: (data: ServiceRequestDetailsDTO[]) => {
        // Requests already sorted by latest first from backend
        this.requests = data;
        this.loading = false;
      },
      error: (err: any) => {
        console.error('Failed to load requests', err);
        this.loading = false;
        this.errorMessage = 'Failed to load your requests';
      }
    });
  }

  toggleCreate() {
    this.creating = !this.creating;
    this.errorMessage = null;
    if (!this.creating) {
      this.form.reset();
    }
  }

  toggleAddVehicle() {
    this.addingVehicle = !this.addingVehicle;
    if (!this.addingVehicle) {
      this.vehicleForm.reset();
    }
  }

  submitAddVehicle() {
    if (this.vehicleForm.invalid) {
      this.errorMessage = 'Please fill in all vehicle fields';
      return;
    }

    this.addingVehicleLoading = true;

    // Handle "Other" type
    let formValue = { ...this.vehicleForm.value };
    if (formValue.type === 'Other') {
      if (!formValue.otherType || formValue.otherType.trim() === '') {
        this.errorMessage = 'Please specify the vehicle type';
        this.addingVehicleLoading = false;
        return;
      }
      formValue.type = formValue.otherType;
    }

    const vehicleDto: VehicleCreateDto = formValue;

    this.vehicleService.createVehicle(vehicleDto).subscribe({
      next: (res: any) => {
        // Backend returns a simple success message. Refresh vehicles list and auto-select the newly added vehicle by numberPlate.
        this.vehicleService.getAllVehicles().subscribe({
          next: (data: any[]) => {
            this.vehicles = data.map(v => ({
              vehicleId: v.vehicleId,
              model: v.model,
              numberPlate: v.numberPlate,
              type: v.type,
              color: v.color
            } as Vehicle));

            const newPlate = formValue.numberPlate?.toString().trim().toLowerCase();
            const added = this.vehicles.find(v => v.numberPlate?.toString().trim().toLowerCase() === newPlate);
            if (added) {
              this.form.patchValue({ vehicleId: added.vehicleId });
            }
            this.addingVehicle = false;
            this.vehicleForm.reset();
            this.addingVehicleLoading = false;
            this.errorMessage = null;
            // Show backend message when available
            if (typeof res === 'string' && res.length > 0) {
              this.successMessage = res;
              setTimeout(() => (this.successMessage = null), 3000);
            } else if (res?.message) {
              this.successMessage = res.message;
              setTimeout(() => (this.successMessage = null), 3000);
            }
          },
          error: (refreshErr: any) => {
            console.error('Failed to refresh vehicles after add', refreshErr);
            // Still close the add UI but show a brief message
            this.addingVehicle = false;
            this.vehicleForm.reset();
            this.addingVehicleLoading = false;
            this.errorMessage = 'Vehicle added, but failed to refresh list. Please reload the page.';
          }
        });
      },
      error: (err: any) => {
        console.error('Add vehicle failed', err);
        this.addingVehicleLoading = false;
        // backend might return a plain text message or an object
        const payload = err.error;
        if (payload) {
          this.errorMessage = payload.message || (typeof payload === 'string' ? payload : JSON.stringify(payload));
        } else {
          this.errorMessage = 'Vehicle creation failed';
        }
      }
    });
  }

  getVehicleDisplay(vehicleId: number): string {
    const vehicle = this.vehicles.find(v => v.vehicleId === vehicleId);
    if (vehicle) {
      return `${vehicle.model} (${vehicle.numberPlate})`;
    }
    return 'Unknown Vehicle';
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
      case 'readyforpickup':
        return 'bg-primary';
      default:
        return 'bg-secondary';
    }
  }

  formatStatus(status: any): string {
    const statusStr = typeof status === 'string' ? status : status.toString();
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

  submitCreate() {
    if (this.form.invalid) {
      this.errorMessage = 'Please fill in all required fields';
      return;
    }

    this.submitting = true;
    const dto: ServiceRequestCreateDTO = { 
      ...this.form.value,
      serviceDate: new Date(this.form.value.serviceDate).toISOString()
    } as ServiceRequestCreateDTO;

    this.srService.createRequest(dto).subscribe({
      next: (res: ServiceRequestDetailsDTO) => {
        this.loadRequests();
        this.creating = false;
        this.form.reset();
        this.submitting = false;
        this.errorMessage = null;
        this.successMessage = 'Request submitted successfully';
        setTimeout(() => this.successMessage = null, 3000);
      },
      error: (err: any) => {
        console.error('Create request failed', err);
        this.submitting = false;
        this.errorMessage = err.error?.message || err.error?.Message || 'Failed to create service request. Please try again.';
      }
    });
  }

  downloadBill(requestId: number) {
    this.srService.downloadBill(requestId).subscribe({
      next: (blob: Blob) => {
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = `bill_${requestId}.pdf`;
        document.body.appendChild(a);
        a.click();
        a.remove();
        window.URL.revokeObjectURL(url);
      },
      error: (err) => {
        console.error('Failed to download bill', err);
        this.errorMessage = 'Failed to download bill';
      }
    });
  }
}
