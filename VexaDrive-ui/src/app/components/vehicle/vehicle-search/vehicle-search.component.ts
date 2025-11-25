import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, OnDestroy, OnInit, Output, ViewChild } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { debounceTime, Subject, Subscription } from 'rxjs';
import { OwnerListDto } from '../../../models/owners/OwnerListDto';
import { VehicleService } from '../../../services/vehicle/VehicleService';
 
@Component({
  selector: 'app-vehicle-search',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './vehicle-search.component.html',
  styleUrls: ['./vehicle-search.component.css']
})
export class VehicleSearchComponent implements OnInit, OnDestroy {
 
  @ViewChild('searchForm') searchForm!: NgForm;
 
  @Input() owners: OwnerListDto[] = [];
  @Input() showSearchForm: boolean = false;
  @Input() loading: boolean = false;
 
  @Output() searchSubmitted = new EventEmitter<{
    vehicleId?: number;
    model?: string;
    numberPlate?: string;
    type?: string;
    color?: string;
    ownerId?: number;
  }>();
 
  @Output() formClosed = new EventEmitter<void>();
 
  searchModel = {
    vehicleId: undefined as number | undefined,
    model: '',
    numberPlate: '',
    type: '',
    color: '',
    ownerId: undefined as number | undefined
  };
 
  private searchSubject = new Subject<void>();
  private sub?: Subscription;
 
  constructor(private vehicleService: VehicleService) {}
 
  ngOnInit() {
    this.sub = this.searchSubject
      .pipe(debounceTime(300))
      .subscribe(() => this.onSearchSubmit());
  }
 
  ngOnDestroy() {
    this.sub?.unsubscribe();
    this.searchSubject.complete();
  }
 
  onSearchSubmit() {
    this.searchSubmitted.emit({ ...this.searchModel });
  }
 
  onCancel() {
    this.resetFormModel();
    if (this.searchForm) {
      this.searchForm.resetForm(this.searchModel);
    }
    this.formClosed.emit();
  }
 
  onClose() {
    this.onCancel();
  }
 
  onClear() {
    this.resetFormModel();
    if (this.searchForm)
      this.searchForm.resetForm(this.searchModel);
  }
 
  private resetFormModel() {
    this.searchModel = {
      vehicleId: undefined,
      model: '',
      numberPlate: '',
      type: '',
      color: '',
      ownerId: undefined
    };
  }
}