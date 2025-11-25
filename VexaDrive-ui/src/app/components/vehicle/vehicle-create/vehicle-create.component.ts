import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, NgForm } from '@angular/forms';
import { VehicleService } from '../../../services/vehicle/VehicleService';
import { VehicleCreateDto } from '../../../models/vehicle/VehicleCreateDto';
import { OwnerListDto } from '../../../models/owners/OwnerListDto';
 
@Component({
  selector: 'app-vehicle-create',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './vehicle-create.component.html',
  styleUrls: ['./vehicle-create.component.css']
})
export class VehicleCreateComponent {
 
  @Input() owners: OwnerListDto[] = [];
  @Output() vehicleCreated = new EventEmitter<void>();
  @Output() formClosed = new EventEmitter<void>();
 
  loading = false;
 
  vehicleModel: VehicleCreateDto = {
    model: '',
    numberPlate: '',
    type: '',
    color: '',
    ownerId: 0
  };
 
  constructor(private vehicleService: VehicleService) {}
 
  onCreateSubmit(form: NgForm) {
    if (!form || form.invalid) return;
 
    this.loading = true;
    this.vehicleService.createVehicle(this.vehicleModel).subscribe({
      next: () => {
        alert('Vehicle created successfully!');
        this.vehicleCreated.emit();
        this.resetForm();
        this.formClosed.emit();
        this.loading = false;
      },
      error: (err) => {
        console.error(err);
        alert('Vehicle creation failed.');
        this.loading = false;
      }
    });
  }
 
  onCancel() {
    this.resetForm();
    this.formClosed.emit();
  }
 
  private resetForm() {
    this.vehicleModel = {
      model: '',
      numberPlate: '',
      type: '',
      color: '',
      ownerId: 0
    };
    this.loading = false;
  }
}