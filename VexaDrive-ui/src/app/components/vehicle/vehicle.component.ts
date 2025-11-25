import { Component, OnInit } from '@angular/core';
import { VehicleService } from '../../services/vehicle/VehicleService';
import { VehicleListDto } from '../../models/vehicle/VehicleListDto';
import { VehicleDetailsDto } from '../../models/vehicle/VehicleDetailsDto';
import { OwnerListDto } from '../../models/owners/OwnerListDto';
import { CommonModule } from '@angular/common';   // << REQUIRED
import { VehicleCreateComponent } from './vehicle-create/vehicle-create.component';
import { VehicleEditComponent } from './vehicle-edit/vehicle-edit.component';
import { VehicleSearchComponent } from './vehicle-search/vehicle-search.component';
import { VehicleListComponent } from './vehicle-list/vehicle-list.component';

import { FormsModule } from '@angular/forms';     // if using ngModel
 
@Component({

  selector: 'app-vehicle',

  standalone: true,

  templateUrl: './vehicle.component.html',

  styleUrls: ['./vehicle.component.css'],

  imports: [

    CommonModule,

    FormsModule,

    VehicleCreateComponent,

    VehicleEditComponent,

    VehicleSearchComponent,
  
    VehicleListComponent

  ]

})
 
export class VehicleComponent implements OnInit {
 
  vehicles: VehicleListDto[] = [];
  owners: OwnerListDto[] = [];
 
  selectedVehicle: VehicleDetailsDto | null = null;
 
  showCreateForm = false;
  showEditForm = false;
  showSearchForm = false;
 
  loading = false;
 
  constructor(private vehicleService: VehicleService) {}
 
  ngOnInit() {
    this.fetchOwners();
    this.fetchVehicles();
  }
 
  // Fetch all vehicles
  fetchVehicles() {
    this.loading = true;
    this.vehicleService.getAllVehicles().subscribe({
      next: (data) => {
        this.vehicles = data;
        this.loading = false;
      },
      error: (err) => {
        console.error('Error fetching vehicles:', err);
        this.loading = false;
      }
    });
  }
 
  // Fetch all owners (for dropdowns)
  fetchOwners() {
    this.vehicleService.getOwners().subscribe({
      next: (data) => {
        this.owners = data;
      },
      error: (err) => console.error('Error fetching owners:', err)
    });
  }
 
  // Create vehicle
  onCreateVehicle() {
    this.showCreateForm = true;
  }
 
  // Event from Create child
  onVehicleCreated() {
    this.showCreateForm = false;
    this.fetchVehicles();
  }
 
  // Event from Edit child
  onVehicleUpdated() {
    this.showEditForm = false;
    this.selectedVehicle = null;
    this.fetchVehicles();
  }
 
  // Event from Search child
  onSearchSubmitted(searchParams: any) {
    this.loading = true;
    this.vehicleService.searchVehicles(
      searchParams.vehicleId,
      searchParams.model,
      searchParams.numberPlate,
      searchParams.type,
      searchParams.ownerId
    ).subscribe({
      next: (data) => {
        this.vehicles = data;
        this.loading = false;
      },
      error: (err) => {
        console.error('Error searching vehicles:', err);
        this.loading = false;
      }
    });
  }
 
  // Edit button clicked in list
  onEditVehicle(vehicle: VehicleListDto) {
    this.vehicleService.getVehicleById(vehicle.vehicleId).subscribe({
      next: (data) => {
        this.selectedVehicle = data;
        this.showEditForm = true;
      },
      error: (err) => console.error('Error fetching vehicle details:', err)
    });
  }
 
  // Delete button clicked in list
  onDeleteVehicle(vehicleId: number) {
    this.loading = true;
    this.vehicleService.deleteVehicle(vehicleId).subscribe({
      next: () => {
        alert('Vehicle deleted successfully!');
        this.fetchVehicles();
      },
      error: (err) => {
        console.error('Error deleting vehicle:', err);
        alert('Vehicle delete failed.');
        this.loading = false;
      }
    });
  }
 
  // Toggle search form
  onToggleSearch() {
    this.showSearchForm = !this.showSearchForm;
  }
}