import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, NgForm } from '@angular/forms';
import { VehicleService } from '../../../services/vehicle/VehicleService';
import { VehicleUpdateDto } from '../../../models/vehicle/VehicleUpdateDto';
import { VehicleDetailsDto } from '../../../models/vehicle/VehicleDetailsDto';
import { OwnerListDto } from '../../../models/owners/OwnerListDto';

@Component({
  selector: 'app-vehicle-edit',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './vehicle-edit.component.html',
  styleUrls: ['./vehicle-edit.component.css']
})
export class VehicleEditComponent {

  // ✅ Accept the full object instead of just an ID
  @Input() selectedVehicle!: VehicleDetailsDto | null;
  @Input() owners: OwnerListDto[] = [];

  @Output() vehicleUpdated = new EventEmitter<void>();
  @Output() formClosed = new EventEmitter<void>();

  loading = false;

  vehicleModel: VehicleUpdateDto = {
    vehicleId: 0,
    model: '',
    numberPlate: '',
    type: '',
    color: '',
    ownerId: ''
  };

  constructor(private vehicleService: VehicleService) {}

  ngOnInit() {
    // ✅ If parent passes the full object, we can just assign it
    if (this.selectedVehicle) {
      this.vehicleModel = {
        vehicleId: this.selectedVehicle.vehicleId,
        model: this.selectedVehicle.model ?? '',
        numberPlate: this.selectedVehicle.numberPlate ?? '',
        type: this.selectedVehicle.type ?? '',
        color: this.selectedVehicle.color ?? '',
        // backend uses CustomerUserId (string)
        ownerId: (this.selectedVehicle as any).customerUserId ?? ''
      };
    }
  }

  onUpdateSubmit(form: NgForm) {
    if (!form || form.invalid) return;

    this.loading = true;
    this.vehicleService.updateVehicle(this.vehicleModel).subscribe({
      next: () => {
        alert('Vehicle updated successfully!');
        this.vehicleUpdated.emit();
        this.loading = false;
      },
      error: (err) => {
        console.error(err);
        alert('Update failed.');
        this.loading = false;
      }
    });
  }

  onCancel() {
    this.formClosed.emit();
  }
}
