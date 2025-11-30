import { Component, EventEmitter, Input, OnDestroy, OnInit, Output, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, NgForm } from '@angular/forms';
import { Subject, Subscription, debounceTime } from 'rxjs';

@Component({
  selector: 'app-owner-search',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './owner-search.component.html',
  styleUrls: ['./owner-search.component.css']
})
export class OwnerSearchComponent implements OnInit, OnDestroy {
  @ViewChild('searchForm') searchForm!: NgForm;
 
  @Input() showSearchForm = false;
  @Input() loading = false;
 
  @Output() searchSubmitted = new EventEmitter<{
    id?: number;
    name?: string;
    contact?: string;
    email?: string;
  }>();
 
  @Output() formClosed = new EventEmitter<void>();
 
  searchModel = {
    id: undefined as number | undefined,
    name: '',
    contact: '',
    email: ''
  };
 
  private searchSubject = new Subject<void>();
  private sub?: Subscription;
 
  ngOnInit() {
    this.sub = this.searchSubject.pipe(debounceTime(300))
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
    if (this.searchForm) this.searchForm.resetForm(this.searchModel);
    this.formClosed.emit();
  }
 
  onClose() {
    this.onCancel();
  }
 
  onClear() {
    this.resetFormModel();
    if (this.searchForm) this.searchForm.resetForm(this.searchModel);
  }
 
  private resetFormModel() {
    this.searchModel = {
      id: undefined,
      name: '',
      contact: '',
      email: ''
    };
  }
}