import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { OwnerListDto } from '../../../models/owners/OwnerListDto';
import { OwnerService } from '../../../services/owner/OwnerService';

@Component({
  selector: 'app-owner-list',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './owner-list.component.html',
  styleUrls: ['./owner-list.component.css']
})
export class OwnerListComponent {
  @Input() owners: OwnerListDto[] = [];
  @Input() loading: boolean = false;
  @Input() error: string | null = null;

  @Output() editOwner = new EventEmitter<OwnerListDto>();
  @Output() deleteOwner = new EventEmitter<string>();

  constructor(private ownerService: OwnerService) {}

  onEditOwner(owner: OwnerListDto): void {
    this.editOwner.emit(owner);
  }

  onDeleteOwner(owner: OwnerListDto): void {
    if (confirm(`Are you sure you want to delete "${owner.fullName || owner.email}"?`)) {
      this.ownerService.deleteOwner(owner.ownerId).subscribe({
        next: () => {
          this.deleteOwner.emit(owner.ownerId); // ✅ emit only the ID (string)
          alert('Owner deleted successfully!');
        },
        error: (err: any) => {
          console.error('Error deleting owner:', err);
          alert('Failed to delete owner');
        }
      });
    }
  }
}
