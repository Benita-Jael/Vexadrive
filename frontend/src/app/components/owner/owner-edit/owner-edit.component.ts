import { Component, Input, Output, EventEmitter, OnChanges, SimpleChanges, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, NgForm } from '@angular/forms';
import { OwnerService } from '../../../services/owner/OwnerService';
import { OwnerDetailsDto } from '../../../models/owners/OwnerDetailsDto';
import { OwnerUpdateDto } from '../../../models/owners/OwnerUpdateDto';
 
@Component({
  selector: 'app-owner-edit',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './owner-edit.component.html',
  styleUrls: ['./owner-edit.component.css']
})
export class OwnerEditComponent implements OnChanges, OnInit {
  @Input() showEditForm = false;
  @Input() selectedOwner: OwnerDetailsDto | null = null;
 
  @Output() ownerUpdated = new EventEmitter<void>();
  @Output() formClosed = new EventEmitter<void>();
 
  loading = false;
 
  ownerModel: OwnerUpdateDto = {
    ownerId: '',
    firstName: '',
    lastName: '',
    contactNumber: '',
    email: ''
  };
 
  constructor(private ownerService: OwnerService) {}
 
  ngOnInit() {
    // Can add any initialization if needed
  }
 
  ngOnChanges(changes: SimpleChanges) {
    if (this.selectedOwner && this.showEditForm) {
      this.ownerModel = {
          ownerId: this.selectedOwner.ownerId,
        firstName: this.selectedOwner.firstName || '',
        lastName: this.selectedOwner.lastName || '',
        contactNumber: this.selectedOwner.contactNumber || '',
        email: this.selectedOwner.email || ''
      };
    }
  }
 
  onEditSubmit(form: NgForm) {
    if (!form || form.invalid || !this.selectedOwner) return;
 
    this.loading = true;
    this.ownerService.updateOwner(this.ownerModel).subscribe({
      next: () => {
        alert('Owner updated successfully!');
        this.loading = false;
        this.showEditForm = false;
        this.ownerUpdated.emit();
        this.resetForm();
        this.formClosed.emit();
      },
      error: (err) => {
        console.error(err);
        alert('Failed to update owner.');
        this.loading = false;
      }
    });
  }
 
  onCancel() {
    this.resetForm();
    this.formClosed.emit();
  }
 
  private resetForm() {
    this.ownerModel = {
      ownerId: '',
      firstName: '',
      lastName: '',
      contactNumber: '',
      email: ''
    };
    this.loading = false;
  }
}