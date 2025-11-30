import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { VehicleListDto } from '../../../models/vehicle/VehicleListDto';

@Component({
  selector: 'app-vehicle-list',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './vehicle-list.component.html',
  styleUrls: ['./vehicle-list.component.css']
})
export class VehicleListComponent {

  @Input() vehicles: VehicleListDto[] = [];
  @Input() loading: boolean = false;

  // ✅ Emit full VehicleListDto object for edit
  @Output() editVehicle = new EventEmitter<VehicleListDto>();
  @Output() deleteVehicle = new EventEmitter<number>();

  onEdit(vehicle: VehicleListDto) {
    this.editVehicle.emit(vehicle);
  }

  onDelete(vehicleId: number) {
    if (confirm("Are you sure you want to delete this vehicle?")) {
      this.deleteVehicle.emit(vehicleId);
    }
  }
}
