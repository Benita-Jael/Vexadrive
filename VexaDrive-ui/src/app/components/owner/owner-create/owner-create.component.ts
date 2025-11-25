import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, NgForm } from '@angular/forms';
import { OwnerService } from '../../../services/owner/OwnerService';
import { OwnerCreateDto } from '../../../models/owners/OwnerCreateDto';
 
@Component({
  selector: 'app-owner-create',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './owner-create.component.html',
  styleUrls: ['./owner-create.component.css']
})
export class OwnerCreateComponent {
  @Input() showCreateForm = false;
  @Input() loading = false;
 
  @Output() ownerCreated = new EventEmitter<void>();
  @Output() formClosed = new EventEmitter<void>();
 
  ownerModel: OwnerCreateDto = {
    firstName: '',
    lastName: '',
    contactNumber: '',
    email: ''
  };
 
  constructor(private ownerService: OwnerService) {}
 
  onSubmit(form: NgForm) {
    if (!form || form.invalid) return;
 
    this.loading = true;
    this.ownerService.createOwner(this.ownerModel).subscribe({
      next: () => {
        alert('Owner created successfully!');
        this.loading = false;
        this.ownerCreated.emit();
        this.resetForm();
        this.formClosed.emit();
      },
      error: (err) => {
        console.error(err);
        alert('Failed to create owner.');
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
      firstName: '',
      lastName: '',
      contactNumber: '',
      email: ''
    };
    this.loading = false;
  }
}