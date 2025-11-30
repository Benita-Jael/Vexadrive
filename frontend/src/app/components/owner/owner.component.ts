import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { OwnerService } from '../../services/owner/OwnerService';
import { OwnerListDto } from '../../models/owners/OwnerListDto';
import { OwnerDetailsDto } from '../../models/owners/OwnerDetailsDto';

import { OwnerCreateComponent } from './owner-create/owner-create.component';
import { OwnerEditComponent } from './owner-edit/owner-edit.component';
import { OwnerSearchComponent } from './owner-search/owner-search.component';
import { OwnerListComponent } from './owner-list/owner-list.component';

@Component({
  selector: 'app-owner',
  standalone: true,
  imports: [
    CommonModule,
    OwnerCreateComponent,
    OwnerEditComponent,
    OwnerSearchComponent,
    OwnerListComponent
  ],
  templateUrl: './owner.component.html',
  styleUrls: ['./owner.component.css']
})
export class OwnerComponent {
  owners: OwnerListDto[] = [];
  selectedOwner: OwnerDetailsDto | null = null;

  showCreateForm = false;
  showEditForm = false;
  showSearchForm = false;
  loading = false;

  constructor(private ownerService: OwnerService) {
    this.loadOwners();
  }

  loadOwners() {
    this.loading = true;
    this.ownerService.getAllOwners().subscribe({
      next: (data) => {
        this.owners = data;
        this.loading = false;
      },
      error: (err) => {
        console.error(err);
        this.loading = false;
      }
    });
  }

  onCreateOwner() {
    this.showCreateForm = true;
  }

  onToggleSearch() {
    this.showSearchForm = !this.showSearchForm;
  }

  onEditOwner(owner: OwnerListDto) {
    this.ownerService.getOwnerById(owner.ownerId).subscribe({
      next: (data: OwnerDetailsDto) => {
        this.selectedOwner = data;
        this.showEditForm = true;
      },
      error: (err) => console.error('Error fetching owner details:', err)
    });
  }

  onOwnerCreated() {
    this.showCreateForm = false;
    this.loadOwners();
  }

  onOwnerUpdated() {
    this.showEditForm = false;
    this.loadOwners();
  }

  onSearchSubmitted(searchParams: any) {
    this.loading = true;
    this.ownerService.searchOwners(
      searchParams.id,
      searchParams.name,
      searchParams.contact,
      searchParams.email
    ).subscribe({
      next: (data) => {
        this.owners = data;
        this.loading = false;
      },
      error: (err) => {
        console.error(err);
        this.loading = false;
      }
    });
  }

  onDeleteOwner(ownerId: string) {
    if (!confirm('Are you sure you want to delete this owner?')) return;
    this.loading = true;
    this.ownerService.deleteOwner(ownerId).subscribe({
      next: () => {
        alert('Owner deleted successfully!');
        this.loadOwners();
      },
      error: (err) => {
        console.error(err);
        alert('Failed to delete owner.');
        this.loading = false;
      }
    });
  }
}
