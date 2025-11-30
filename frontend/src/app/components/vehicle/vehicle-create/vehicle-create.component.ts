import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, NgForm } from '@angular/forms';
import { VehicleService } from '../../../services/vehicle/VehicleService';
import { VehicleCreateDto } from '../../../models/vehicle/VehicleCreateDto';
 
@Component({
  selector: 'app-vehicle-create',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './vehicle-create.component.html',
  styleUrls: ['./vehicle-create.component.css']
})
export class VehicleCreateComponent {
 
  @Output() vehicleCreated = new EventEmitter<void>();
  @Output() formClosed = new EventEmitter<void>();
 
  loading = false;
 
  vehicleModel: VehicleCreateDto = {
    model: '',
    numberPlate: '',
    type: '',
    color: ''
  };
 
  constructor(private vehicleService: VehicleService) {}
 
  onCreateSubmit(form: NgForm) {
    if (!form || form.invalid) return;
 
    this.loading = true;
    this.vehicleService.createVehicle(this.vehicleModel).subscribe({
      next: () => {
        alert('Vehicle added successfully!');
        this.vehicleCreated.emit();
        this.resetForm();
        this.formClosed.emit();
        this.loading = false;
      },
      error: (err) => {
        console.error(err);
        alert('Vehicle creation failed: ' + (err.error?.message || err.error?.Message || err.message));
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
      color: ''
    };
    this.loading = false;
  }
}